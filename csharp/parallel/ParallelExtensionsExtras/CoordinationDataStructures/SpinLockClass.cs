//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace System.Threading
{
    /// <summary>Provides a simple, reference type wrapper for SpinLock.</summary>
    public class SpinLockClass
    {
        private SpinLock _spinLock; // NOTE: must *not* be readonly due to SpinLock being a mutable struct

        /// <summary>Initializes an instance of the SpinLockClass class.</summary>
        public SpinLockClass() => _spinLock = new SpinLock();

        /// <summary>Initializes an instance of the SpinLockClass class.</summary>
        /// <param name="enableThreadOwnerTracking">
        /// Controls whether the SpinLockClass should track
        /// thread-ownership fo the lock.
        /// </param>
        public SpinLockClass(bool enableThreadOwnerTracking) => _spinLock = new SpinLock(enableThreadOwnerTracking);

        /// <summary>Runs the specified delegate under the lock.</summary>
        /// <param name="runUnderLock">The delegate to be executed while holding the lock.</param>
        public void Execute(Action runUnderLock)
        {
            bool lockTaken = false;
            try
            {
                Enter(ref lockTaken);
                runUnderLock();
            }
            finally
            {
                if (lockTaken) Exit();
            }
        }

        /// <summary>Enters the lock.</summary>
        /// <param name="lockTaken">
        /// Upon exit of the Enter method, specifies whether the lock was acquired. 
        /// The variable passed by reference must be initialized to false.
        /// </param>
        public void Enter(ref bool lockTaken) => _spinLock.Enter(ref lockTaken);

        /// <summary>Exits the SpinLock.</summary>
        public void Exit() => _spinLock.Exit();

        /// <summary>Exits the SpinLock.</summary>
        /// <param name="useMemoryBarrier">
        /// A Boolean value that indicates whether a memory fence should be issued in
        /// order to immediately publish the exit operation to other threads.
        /// </param>
        public void Exit(bool useMemoryBarrier) => _spinLock.Exit(useMemoryBarrier);
    }
}
