//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Raytracer.Wpf
{
    class ReversiGame : Minimax
    {
        public ReversiGame(int numRows, int numCols)
        {
            _numRows = numRows;
            _numCols = numCols;
            Board = new MinimaxSpot[numRows, numCols];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Board[i, j] = MinimaxSpot.Empty;
                }
            }

            Board[3, 3] = MinimaxSpot.Light;
            Board[4, 4] = MinimaxSpot.Light;
            Board[4, 3] = MinimaxSpot.Dark;
            Board[3, 4] = MinimaxSpot.Dark;

            IsLightMove = false;
        }

        public void SetMinimaxKnobs(int maxDepth, TimeSpan timeLimit, int degOfParallelism)
        {
            _depth = maxDepth;
            _time = timeLimit;
            _degressOfParallelism = degOfParallelism;
        }

        /// <summary>
        /// Returns whether a move on the given row and column on the given state by the given player
        /// is valid.
        /// </summary>
        /// <param name="state">The state to consider.</param>
        /// <param name="isLightPlayer">The player ot move.</param>
        /// <param name="row">The move row.</param>
        /// <param name="col">The move column.</param>
        /// <returns></returns>
        private bool IsValidMove(MinimaxSpot[,] state, bool isLightPlayer, int row, int col)
        {
            if (state[row, col] != MinimaxSpot.Empty)
            {
                return false;
            }

            var you = isLightPlayer ? MinimaxSpot.Light : MinimaxSpot.Dark;
            var enemy = isLightPlayer ? MinimaxSpot.Dark : MinimaxSpot.Light;

            // Check above.
            if (row + 1 < _numRows && state[row + 1, col] == enemy)
            {
                for (int r = row + 2; r < _numRows; r++)
                {
                    if (state[r, col] == you)
                    {
                        return true;
                    }
                    if (state[r, col] == MinimaxSpot.Empty)
                    {
                        break;
                    }
                }
            }

            // Check below.
            if (row - 1 >= 0 && state[row - 1, col] == enemy)
            {
                for (int r = row - 2; r >= 0; r--)
                {
                    if (state[r, col] == you)
                    {
                        return true;
                    }
                    if (state[r, col] == MinimaxSpot.Empty)
                    {
                        break;
                    }
                }
            }

            // Check right.
            if (col + 1 < _numCols && state[row, col + 1] == enemy)
            {
                for (int c = col + 2; c < _numCols; c++)
                {
                    if (state[row, c] == you)
                    {
                        return true;
                    }
                    if (state[row, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }
                }
            }

            // Check left.
            if (col - 1 >= 0 && state[row, col - 1] == enemy)
            {
                for (int c = col - 2; c >= 0; c--)
                {
                    if (state[row, c] == you)
                    {
                        return true;
                    }
                    if (state[row, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }
                }
            }

            // Check above-right
            if (row + 1 < _numRows && col + 1 < _numCols && state[row + 1, col + 1] == enemy)
            {
                for (int r = row + 2, c = col + 2; r < _numRows && c < _numCols; r++, c++)
                {
                    if (state[r, c] == you)
                    {
                        return true;
                    }
                    if (state[r, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }
                }
            }

            // Check above-left
            if (row + 1 < _numRows && col - 1 >= 0 && state[row + 1, col - 1] == enemy)
            {
                for (int r = row + 2, c = col - 2; r < _numRows && c >= 0; r++, c--)
                {
                    if (state[r, c] == you)
                    {
                        return true;
                    }
                    if (state[r, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }
                }
            }

            // Check below-right
            if (row - 1 >= 0 && col + 1 < _numCols && state[row - 1, col + 1] == enemy)
            {
                for (int r = row - 2, c = col + 2; r >= 0 && c < _numCols; r--, c++)
                {
                    if (state[r, c] == you)
                    {
                        return true;
                    }
                    if (state[r, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }
                }
            }

            // Check below-left
            if (row - 1 >= 0 && col - 1 >= 0 && state[row - 1, col - 1] == enemy)
            {
                for (int r = row - 2, c = col - 2; r >= 0 && c >= 0; r--, c--)
                {
                    if (state[r, c] == you)
                    {
                        return true;
                    }
                    if (state[r, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the valid moves for the state maintained by this ReversiGame
        /// (delegates to another overload).
        /// </summary>
        /// <returns>The valid moves.</returns>
        public IEnumerable<MinimaxMove> GetValidMoves() => GetValidMoves(Board, IsLightMove);

        /// <summary>
        /// Makes a move on the given row and column.
        /// </summary>
        /// <param name="row">The move row</param>
        /// <param name="col">The move column</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public bool MakeMove(int row, int col)
        {
            if (!IsValidMove(Board, IsLightMove, row, col))
            {
                return false;
            }

            var you = IsLightMove ? MinimaxSpot.Light : MinimaxSpot.Dark;
            var enemy = IsLightMove ? MinimaxSpot.Dark : MinimaxSpot.Light;
            var backup = new MinimaxSpot[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    backup[i, j] = Board[i, j];
                }
            }

            Board[row, col] = you;

            // Conquer above.
            if (row + 1 < _numRows && Board[row + 1, col] == enemy)
            {
                bool b = false;
                for (int r = row + 2; r < _numRows; r++)
                {
                    if (Board[r, col] == MinimaxSpot.Empty)
                    {
                        break;
                    }

                    if (Board[r, col] == you)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    for (int r = row + 1; Board[r, col] != you; r++)
                    {
                        Board[r, col] = you;
                    }
                }
            }

            // Conquer below.
            if (row - 1 >= 0 && Board[row - 1, col] == enemy)
            {
                bool b = false;
                for (int r = row - 2; r >= 0; r--)
                {
                    if (Board[r, col] == MinimaxSpot.Empty)
                    {
                        break;
                    }

                    if (Board[r, col] == you)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    for (int r = row - 1; Board[r, col] != you; r--)
                    {
                        Board[r, col] = you;
                    }
                }
            }

            // Conquer right.
            if (col + 1 < _numCols && Board[row, col + 1] == enemy)
            {
                bool b = false;
                for (int c = col + 2; c < _numCols; c++)
                {
                    if (Board[row, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }

                    if (Board[row, c] == you)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    for (int c = col + 1; Board[row, c] != you; c++)
                    {
                        Board[row, c] = you;
                    }
                }
            }

            // Conquer left.
            if (col - 1 >= 0 && Board[row, col - 1] == enemy)
            {
                bool b = false;
                for (int c = col - 2; c >= 0; c--)
                {
                    if (Board[row, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }

                    if (Board[row, c] == you)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    for (int c = col - 1; Board[row, c] != you; c--)
                    {
                        Board[row, c] = you;
                    }
                }
            }

            // Conquer above-right
            if (row + 1 < _numRows && col + 1 < _numCols && Board[row + 1, col + 1] == enemy)
            {
                bool b = false;
                for (int r = row + 2, c = col + 2; r < _numRows && c < _numCols; r++, c++)
                {
                    if (Board[r, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }

                    if (Board[r, c] == you)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    for (int r = row + 1, c = col + 1; Board[r, c] != you; r++, c++)
                    {
                        Board[r, c] = you;
                    }
                }
            }

            // Conquer above-left
            if (row + 1 < _numRows && col - 1 >= 0 && Board[row + 1, col - 1] == enemy)
            {
                bool b = false;
                for (int r = row + 2, c = col - 2; r < _numRows && c >= 0; r++, c--)
                {
                    if (Board[r, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }

                    if (Board[r, c] == you)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    for (int r = row + 1, c = col - 1; Board[r, c] != you; r++, c--)
                    {
                        Board[r, c] = you;
                    }
                }
            }

            // Conquer below-right
            if (row - 1 >= 0 && col + 1 < _numCols && Board[row - 1, col + 1] == enemy)
            {
                bool b = false;
                for (int r = row - 2, c = col + 2; r >= 0 && c < _numCols; r--, c++)
                {
                    if (Board[r, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }

                    if (Board[r, c] == you)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    for (int r = row - 1, c = col + 1; Board[r, c] != you; r--, c++)
                    {
                        Board[r, c] = you;
                    }
                }
            }

            // Conquer below-left
            if (row - 1 >= 0 && col - 1 >= 0 && Board[row - 1, col - 1] == enemy)
            {
                bool b = false;
                for (int r = row - 2, c = col - 2; r >= 0 && c >= 0; r--, c--)
                {
                    if (Board[r, c] == MinimaxSpot.Empty)
                    {
                        break;
                    }

                    if (Board[r, c] == you)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    for (int r = row - 1, c = col - 1; Board[r, c] != you; r--, c--)
                    {
                        Board[r, c] = you;
                    }
                }
            }

            IsLightMove = !IsLightMove;

            return true;
        }

        /// <summary>
        /// Passes on the current player's move.
        /// </summary>
        public void PassMove() => IsLightMove = !IsLightMove;

        /// <summary>
        /// Returns the game result.
        /// </summary>
        /// <returns>The game result</returns>
        public ReversiGameResult GetGameResult()
        {
            var gr = new ReversiGameResult();
            for (int i = 0; i < _numRows; i++)
            {
                for (int j = 0; j < _numCols; j++)
                {
                    if (Board[i, j] == MinimaxSpot.Light)
                    {
                        gr.NumLightPieces++;
                    }

                    if (Board[i, j] == MinimaxSpot.Dark)
                    {
                        gr.NumDarkPieces++;
                    }
                }
            }

            if (TerminalTest(Board))
            {
                if (gr.NumLightPieces > gr.NumDarkPieces)
                {
                    gr.GameState = ReversiGameState.LightWon;
                }
                else if (gr.NumLightPieces < gr.NumDarkPieces)
                {
                    gr.GameState = ReversiGameState.DarkWon;
                }
                else
                {
                    gr.GameState = ReversiGameState.Draw;
                }
            }
            else
            {
                gr.GameState = ReversiGameState.Ongoing;
            }

            return gr;
        }

        public MinimaxMove GetAIMove(bool inParallel) => Search(Board, IsLightMove, inParallel);

        public override int MaxDepth => _depth;

        public override TimeSpan TimeLimit => _time;

        public override int DegreeOfParallelism => _degressOfParallelism;

        protected override bool TerminalTest(MinimaxSpot[,] state)
        {
            // Can either player move?
            if (GetValidMoves(state, true).Count() == 0 && GetValidMoves(state, false).Count() == 0)
            {
                return true;
            }

            return false;
        }

        protected override int EvaluateHeuristic(MinimaxSpot[,] state)
        {
            int boardValue = 0;

            // +1 for light pieces, -1 for dark pieces
            for (int i = 0; i < _numRows; i++)
            {
                for (int j = 0; j < _numCols; j++)
                {
                    if (state[i, j] == MinimaxSpot.Light)
                    {
                        boardValue++;
                    }
                    if (state[i, j] == MinimaxSpot.Dark)
                    {
                        boardValue--;
                    }
                }
            }

            // +-X for corner pieces
            int cornerPieceValue = 13;
            if (state[0, 0] == MinimaxSpot.Light)
            {
                boardValue += cornerPieceValue;
            }
            if (state[0, 0] == MinimaxSpot.Dark)
            {
                boardValue -= cornerPieceValue;
            }
            if (state[0, 7] == MinimaxSpot.Light)
            {
                boardValue += cornerPieceValue;
            }
            if (state[0, 7] == MinimaxSpot.Dark)
            {
                boardValue -= cornerPieceValue;
            }
            if (state[7, 0] == MinimaxSpot.Light)
            {
                boardValue += cornerPieceValue;
            }
            if (state[7, 0] == MinimaxSpot.Dark)
            {
                boardValue -= cornerPieceValue;
            }
            if (state[7, 7] == MinimaxSpot.Light)
            {
                boardValue += cornerPieceValue;
            }
            if (state[7, 7] == MinimaxSpot.Dark)
            {
                boardValue -= cornerPieceValue;
            }

            // +-X for edge pieces
            int edgePieceValue = 9;
            for (int i = 0; i < _numRows; i++)
            {
                if (state[i, 0] == MinimaxSpot.Light)
                {
                    boardValue += edgePieceValue;
                }
                if (state[i, 0] == MinimaxSpot.Dark)
                {
                    boardValue -= edgePieceValue;
                }
                if (state[i, _numCols - 1] == MinimaxSpot.Light)
                {
                    boardValue += edgePieceValue;
                }
                if (state[i, _numCols - 1] == MinimaxSpot.Dark)
                {
                    boardValue -= edgePieceValue;
                }
            }
            for (int i = 0; i < _numCols; i++)
            {
                if (state[0, i] == MinimaxSpot.Light)
                {
                    boardValue += edgePieceValue;
                }
                if (state[0, 0] == MinimaxSpot.Dark)
                {
                    boardValue -= edgePieceValue;
                }
                if (state[_numRows - 1, i] == MinimaxSpot.Light)
                {
                    boardValue += edgePieceValue;
                }
                if (state[_numRows - 1, i] == MinimaxSpot.Dark)
                {
                    boardValue -= edgePieceValue;
                }
            }

            return boardValue;
        }

        protected override IEnumerable<MinimaxMove> GetValidMoves(MinimaxSpot[,] state, bool isLightPlayer)
        {
            for (int i = 0; i < _numRows; i++)
            {
                for (int j = 0; j < _numCols; j++)
                {
                    if (IsValidMove(state, isLightPlayer, i, j))
                    {
                        yield return new MinimaxMove(i, j);
                    }
                }
            }
        }

        protected override MinimaxSpot[,] GetInsight(MinimaxSpot[,] state, MinimaxMove move, bool isLightPlayer)
        {
            var insightState = new MinimaxSpot[_numRows, _numCols];
            for (int i = 0; i < _numRows; i++)
            {
                for (int j = 0; j < _numCols; j++)
                {
                    insightState[i, j] = state[i, j];
                }
            }

            insightState[move.Row, move.Col] = isLightPlayer ? MinimaxSpot.Light : MinimaxSpot.Dark;
            return insightState;
        }

        private void ReadDump()
        {
            var sr = new StreamReader("./dump.txt");

            string s = sr.ReadLine();
            int r = 7;
            while (s != null)
            {
                char[] line = s.ToCharArray();
                for (int c = 0; c < 8; c++)
                {
                    if (line[c] == 'w')
                    {
                        Board[r, c] = MinimaxSpot.Light;
                    }

                    if (line[c] == 'b')
                    {
                        Board[r, c] = MinimaxSpot.Dark;
                    }
                }
                r--;
                s = sr.ReadLine();
            }
            sr.Close();
        }

        public void Dump(string msg)
        {
            var sw = new StreamWriter("./dump.txt");
            for (int i = 7; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j] == MinimaxSpot.Light)
                    {
                        sw.Write('w');
                    }
                    else if (Board[i, j] == MinimaxSpot.Dark)
                    {
                        sw.Write('b');
                    }
                    else
                    {
                        sw.Write('-');
                    }
                }
                sw.WriteLine();
            }
            sw.WriteLine(msg);
            sw.Close();
        }

        private readonly int _numRows;
        private readonly int _numCols;
        private int _depth, _degressOfParallelism;
        private TimeSpan _time;

        public MinimaxSpot[,] Board { get; }

        public bool IsLightMove { get; private set; }
    }

    public struct ReversiGameResult
    {
        public ReversiGameState GameState;
        public int NumLightPieces, NumDarkPieces;
    }

    public enum ReversiGameState
    {
        Ongoing = 42,
        LightWon = 1,
        DarkWon = -1,
        Draw = 0
    }
}
