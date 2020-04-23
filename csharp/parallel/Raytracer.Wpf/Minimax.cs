//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Raytracer.Wpf
{
    public abstract class Minimax
    {
        /// <summary>
        /// Returns the max depth that Minimax should search to.  A value 
        /// of -1 indicates an uncapped search depth.
        /// </summary>
        public abstract int MaxDepth { get; }

        /// <summary>
        /// Returns the time limit after which Minimax should stop searching.
        /// </summary>
        public abstract TimeSpan TimeLimit { get; }

        /// <summary>
        /// Returns the soft cap for the number of concurrent Tasks that Minimax
        /// should use.
        /// </summary>
        public abstract int DegreeOfParallelism { get; }

        /// <summary>
        /// Returns whether the given state represents a finished game.
        /// Must be thread-safe.
        /// </summary>
        /// <param name="state">The game state to check.</param>
        /// <returns>True if the state represents a finished game,
        /// false otherwise.</returns>
        protected abstract bool TerminalTest(MinimaxSpot[,] state);

        /// <summary>
        /// Returns the value of the given state, where a positive value indicates
        /// an advantage for the light player.
        /// Must be thread-safe.
        /// </summary>
        /// <param name="state">The game state to evaluate.</param>
        /// <returns>A number representing the value of the given state.</returns>
        protected abstract int EvaluateHeuristic(MinimaxSpot[,] state);

        /// <summary>
        /// Returns a collection containing the valid moves for 
        /// the given player on the given game state.
        /// Must be thread-safe.
        /// </summary>
        /// <param name="state">The game state to consider.</param>
        /// <param name="isLightPlayer">The bool indicating which player.</param>
        /// <returns>An enumerable of MinimaxMove, representing the valid moves.</returns>
        protected abstract IEnumerable<MinimaxMove> GetValidMoves(MinimaxSpot[,] state, bool isLightPlayer);

        /// <summary>
        /// Returns the game state that results when the given player plays the given move on the given
        /// state.  If the move is invalid, the new state should be the same as the old state.
        /// Must be thread-safe.
        /// </summary>
        /// <param name="state">The state to play a move on.</param>
        /// <param name="move">The move to play.</param>
        /// <param name="isLightPlayer">The player to play the move.</param>
        /// <returns>A MinimaxSpot matrix that represents the insight state.</returns>
        protected abstract MinimaxSpot[,] GetInsight(MinimaxSpot[,] state, MinimaxMove move, bool isLightPlayer);

        /// <summary>
        /// Should only be called through the public Search method.
        /// </summary>
        /// <param name="state">The game state to consider.</param>
        /// <param name="isLightPlayer">The player to move.</param>
        /// <param name="alpha">The alpha pruning value.</param>
        /// <param name="beta">The beta pruning value.</param>
        /// <param name="depth">The current search depth.</param>
        /// <returns>A MinimaxMove that represents the best move found.</returns>
        /// <remarks>
        /// The initial alpha value should be Int32.MinValue, the initial beta value 
        /// should be Int32.MaxValue, and the initial depth value should be 0.
        /// 
        /// The search will terminate ASAP if the m_ct cancellation token is signaled.
        /// 
        /// This method is thread-safe.
        /// </remarks>
        private MinimaxMove InternalSearch(MinimaxSpot[,] state, bool isLightPlayer, int alpha, int beta, int depth)
        {
            // Stop the search if...
            if (TerminalTest(state) || depth >= _maxDepth || _cancellationToken.IsCancellationRequested)
            {
                _movesConsidered++;
                return new MinimaxMove(EvaluateHeuristic(state));
            }

            // Initialize the best move for this recursive call.
            var bestMove = new MinimaxMove(isLightPlayer ? int.MinValue : int.MaxValue);

            // Get the valid moves for this recursive call.
            var validMoves = GetValidMoves(state, isLightPlayer);

            // If there are valid moves, recurse on each.
            var consideredLocalMoves = false;
            foreach (var move in validMoves)
            {
                consideredLocalMoves = true;

                var currentMove = move;
                currentMove.Value = InternalSearch(GetInsight(state, currentMove, isLightPlayer), !isLightPlayer, alpha, beta, depth + 1).Value;
                if (isLightPlayer)
                {
                    if (currentMove.Value > bestMove.Value)
                    {
                        bestMove = currentMove;
                    }
                    if (bestMove.Value >= beta)
                    {
                        break;
                    }

                    alpha = Math.Max(alpha, bestMove.Value.Value);
                }
                else
                {
                    if (currentMove.Value < bestMove.Value)
                    {
                        bestMove = currentMove;
                    }
                    if (bestMove.Value <= alpha)
                    {
                        break;
                    }

                    beta = Math.Min(beta, bestMove.Value.Value);
                }
            }

            // If there were no valid moves, still calculate the value.
            if (!consideredLocalMoves)
            {
                bestMove.Value = InternalSearch(state, !isLightPlayer, alpha, beta, depth + 1).Value;
            }

            return bestMove;
        }

        /// <summary>
        /// Should only be called through the public Search method.
        /// </summary>
        /// <param name="state">The game state to consider.</param>
        /// <param name="isLightPlayer">The player to move.</param>
        /// <param name="alpha">The alpha pruning value.</param>
        /// <param name="beta">The beta pruning value.</param>
        /// <param name="depth">The current search depth.</param>
        /// <param name="token">The pruning token.</param>
        /// <returns>A MinimaxMove that represents the best move found.</returns>
        /// <remarks>
        /// The initial alpha value should be Int32.MinValue, the initial beta value 
        /// should be Int32.MaxValue, the initial depth value should be 0, and the 
        /// initial token should be a non-settable token.
        /// 
        /// The search will terminate ASAP if the m_ct cancellation token is signaled.
        /// 
        /// This method is thread-safe.
        /// </remarks>
        private MinimaxMove InternalSearchTPL(MinimaxSpot[,] state, bool isLightPlayer, int alpha, int beta, int depth, CancellationToken token)
        {
            // Stop the search if...
            if (TerminalTest(state) || depth >= _maxDepth || _cancellationToken.IsCancellationRequested)
            {
                _movesConsidered++; // NOTE: this is racy and may be lower than the actual count, but it only needs to be an appx
                return new MinimaxMove(EvaluateHeuristic(state));
            }

            // Initialize the best move for this recursive call.
            var bestMove = new MinimaxMove(isLightPlayer ? int.MinValue : int.MaxValue);

            // Get the valid moves for this recursive call.
            var validMoves = GetValidMoves(state, isLightPlayer);
            var consideredLocalMoves = false;
            var workers = new Queue<Task>();
            var bigLock = new object();
            var cts = new CancellationTokenSource();
            foreach (var move in validMoves)
            {
                // SHARED STATE
                //     The local variables (bestMove, alpha, beta) are protected by a lock.
                //     The non-local variables (m_taskCount) are modified using Interlocked
                consideredLocalMoves = true;

                // If the pruning token is signaled, stop this loop.
                if (token.IsCancellationRequested)
                {
                    cts.Cancel();
                    break;
                }

                var currentMove = move;
                if (_taskCount < _degOfParallelism && depth <= _maxDepth - 1)
                {
                    Interlocked.Increment(ref _taskCount);
                    workers.Enqueue(Task.Run(() =>
                    {
                        currentMove.Value = InternalSearchTPL(GetInsight(state, currentMove, isLightPlayer), !isLightPlayer, alpha, beta, depth + 1, cts.Token).Value;
                        lock (bigLock)
                        {
                            if (isLightPlayer)
                            {
                                if (currentMove.Value > bestMove.Value)
                                {
                                    bestMove = currentMove;
                                }
                                if (bestMove.Value >= beta)
                                {
                                    cts.Cancel();
                                }

                                alpha = Math.Max(alpha, bestMove.Value.Value);
                            }
                            else
                            {
                                if (currentMove.Value < bestMove.Value)
                                {
                                    bestMove = currentMove;
                                }
                                if (bestMove.Value <= alpha)
                                {
                                    cts.Cancel();
                                }

                                beta = Math.Min(beta, bestMove.Value.Value);
                            }
                        }
                        Interlocked.Decrement(ref _taskCount);
                    }));
                }
                else
                {
                    bool isPruning = false;
                    currentMove.Value = InternalSearchTPL(GetInsight(state, currentMove, isLightPlayer), !isLightPlayer, alpha, beta, depth + 1, cts.Token).Value;

                    // If there are no tasks, no need to lock.
                    bool lockTaken = false;
                    try
                    {
                        if (workers.Count > 0)
                        {
                            Monitor.Enter(bigLock, ref lockTaken);
                        }
                        if (isLightPlayer)
                        {
                            if (currentMove.Value > bestMove.Value)
                            {
                                bestMove = currentMove;
                            }
                            if (bestMove.Value >= beta)
                            {
                                isPruning = true;
                            }

                            alpha = Math.Max(alpha, bestMove.Value.Value);
                        }
                        else
                        {
                            if (currentMove.Value < bestMove.Value)
                            {
                                bestMove = currentMove;
                            }
                            if (bestMove.Value <= alpha)
                            {
                                isPruning = true;
                            }

                            beta = Math.Min(beta, bestMove.Value.Value);
                        }
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            Monitor.Exit(bigLock);
                        }
                    }

                    if (isPruning)
                    {
                        cts.Cancel();
                        break;
                    }
                }
            }

            Task.WaitAll(workers.ToArray());

            // If there were no valid moves, still calculate the value.
            if (!consideredLocalMoves)
            {
                bestMove.Value =
                    InternalSearchTPL(state, !isLightPlayer, alpha, beta, depth + 1, token).Value;
            }

            return bestMove;
        }

        /// <summary>
        /// Returns the best move resulting from a Minimax, alpha-beta pruning search on the given state.
        /// </summary>
        /// <param name="state">The state to consider.</param>
        /// <param name="isLightPlayer">The player to move.</param>
        /// <param name="inParallel">A boolean indicating whether to use the parallel algorithm.</param>
        /// <returns>A MinimaxMove that represents the best move found.</returns>
        /// <remarks>
        /// This method will only return a MinimaxMove(-1...) if there are no valid moves.
        /// </remarks>
        public MinimaxMove Search(MinimaxSpot[,] state, bool isLightPlayer, bool inParallel)
        {
            // Initialize a bunch of state.
            _maxDepth = MaxDepth == -1 ? int.MaxValue : MaxDepth;
            _degOfParallelism = DegreeOfParallelism;
            _timeLimit = TimeLimit;
            _taskCount = 0;
            _movesConsidered = 0;
            var curCts = _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            var aiMove = new MinimaxMove(-1, -1, null);

            // Start the timeout timer.  Done using a dedicated thread to minimize delay 
            // in cancellation due to lack of threads in the pool to run the callback.
            var timeoutTask = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(_timeLimit);
                curCts.Cancel();
            }, TaskCreationOptions.LongRunning);

            // Do the search
            aiMove = inParallel
                ? InternalSearchTPL(state, isLightPlayer, int.MinValue, int.MaxValue, 0, CancellationToken.None)
                : InternalSearch(state, isLightPlayer, int.MinValue, int.MaxValue, 0);

            // Make sure that MinimaxMove(-1...) is only returned if there are no valid moves, because
            // InternalSearch* may return MinimaxMove(-1...) if none of the valid moves beats Int32.Min/Max.
            if (aiMove.Row == -1)
            {
                foreach (var move in GetValidMoves(state, isLightPlayer))
                {
                    aiMove = move;
                    aiMove.Value = isLightPlayer ? int.MinValue : int.MaxValue;
                    break;
                }
            }

            return aiMove;
        }

        /// <summary>
        /// Cancel the ongoing operation, if there is one.
        /// </summary>
        public void Cancel()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Returns the number of moves considered by the most recent Search.
        /// </summary>
        public int MovesConsidered => _movesConsidered;

        private int _maxDepth, _degOfParallelism;
        private TimeSpan _timeLimit;
        private int _taskCount;
        private volatile int _movesConsidered;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
    }

    /// <summary>
    /// An enum that represents the state of a board game spot.
    /// </summary>
    public enum MinimaxSpot
    {
        Empty = 0,
        Dark = -1,
        Light = 1
    }

    /// <summary>
    /// A struct that represents a board game move. The value field
    /// should only be manipulated by Minimax.
    /// </summary>
    public struct MinimaxMove
    {
        public int Row, Col;
        public int? Value;

        public MinimaxMove(int row, int col)
        {
            Row = row;
            Col = col;
            Value = null;
        }

        public MinimaxMove(int? value)
        {
            Row = Col = -1;
            Value = value;
        }

        public MinimaxMove(int row, int col, int? value)
        {
            Row = row;
            Col = col;
            Value = value;
        }
    }
}
