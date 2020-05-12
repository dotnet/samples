//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Diagnostics;
using System.Threading.Tasks;

namespace System.Threading.Async
{
    /// <summary>Provides an asynchronous barrier.</summary>
    [DebuggerDisplay("ParticipantCount={ParticipantCount}, RemainingCount={RemainingCount}")]
    public sealed class AsyncBarrier
    {
        /// <summary>The number of participants in the barrier.</summary>
        private readonly int _participantCount;
        /// <summary>The task used to signal completion of the current round.</summary>
        private TaskCompletionSource<object> _currentSignalTask;
        /// <summary>The number of participants remaining to arrive for this round.</summary>
        private int _remainingParticipants;

        /// <summary>Initializes the BarrierAsync with the specified number of participants.</summary>
        /// <param name="participantCount">The number of participants in the barrier.</param>
        public AsyncBarrier(int participantCount)
        {
            if (participantCount <= 0) throw new ArgumentOutOfRangeException("participantCount");
            _participantCount = participantCount;

            _remainingParticipants = participantCount;
            _currentSignalTask = new TaskCompletionSource<object>();
        }

        /// <summary>Gets the participant count.</summary>
        public int ParticipantCount => _participantCount;
        /// <summary>Gets the number of participants still not yet arrived in this round.</summary>
        public int RemainingCount => _remainingParticipants;

        /// <summary>Signals that a participant has arrived.</summary>
        /// <returns>A Task that will be signaled when the current round completes.</returns>
        public Task SignalAndWait()
        {
            var curCts = _currentSignalTask;
#pragma warning disable 420
            if (Interlocked.Decrement(ref _remainingParticipants) == 0)
#pragma warning restore 420
            {
                _remainingParticipants = _participantCount;
                _currentSignalTask = new TaskCompletionSource<object>();
                curCts.SetResult(null);
            }
            return curCts.Task;
        }
    }
}
