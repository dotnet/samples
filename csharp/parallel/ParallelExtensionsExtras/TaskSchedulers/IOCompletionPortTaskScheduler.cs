//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Threading.Tasks.Schedulers
{
    /// <summary>Provides a TaskScheduler that uses an I/O completion port for concurrency control.</summary>
    public sealed class IOCompletionPortTaskScheduler : TaskScheduler, IDisposable
    {
        /// <summary>The queue of tasks to be scheduled.</summary>
        private readonly ConcurrentQueue<Task> _tasks;
        /// <summary>The I/O completion port to use for concurrency control.</summary>
        private readonly IOCompletionPort _iocp;
        /// <summary>Whether the current thread is a scheduler thread.</summary>
        private readonly ThreadLocal<bool> _schedulerThread;
        /// <summary>Event used to wait for all threads to shutdown.</summary>
        private readonly CountdownEvent _remainingThreadsToShutdown;

        /// <summary>Initializes the IOCompletionPortTaskScheduler.</summary>
        /// <param name="maxConcurrencyLevel">The maximum number of threads in the scheduler to be executing concurrently.</param>
        /// <param name="numAvailableThreads">The number of threads to have available in the scheduler for executing tasks.</param>
        public IOCompletionPortTaskScheduler(int maxConcurrencyLevel, int numAvailableThreads)
        {
            // Validate arguments
            if (maxConcurrencyLevel < 1) throw new ArgumentNullException(nameof(maxConcurrencyLevel));
            if (numAvailableThreads < 1) throw new ArgumentNullException(nameof(numAvailableThreads));

            _tasks = new ConcurrentQueue<Task>();
            _iocp = new IOCompletionPort(maxConcurrencyLevel);
            _schedulerThread = new ThreadLocal<bool>();
            _remainingThreadsToShutdown = new CountdownEvent(numAvailableThreads);

            // Create and start the threads
            for (int i = 0; i < numAvailableThreads; i++)
            {
                new Thread(() =>
                {
                    try
                    {
                        // Note that this is a scheduler thread.  Used for inlining checks.
                        _schedulerThread.Value = true;

                        // Continually wait on the I/O completion port until 
                        // there's a work item, then process it.
                        while (_iocp.WaitOne())
                        {
                            if (_tasks.TryDequeue(out Task next)) TryExecuteTask(next);
                        }
                    }
                    finally { _remainingThreadsToShutdown.Signal(); }
                })
                { IsBackground = true }.Start();
            }
        }

        /// <summary>Dispose of the scheduler.</summary>
        public void Dispose()
        {
            // Close the I/O completion port.  This will cause any threads blocked
            // waiting for items to wake up.
            _iocp.Dispose();

            // Wait for all threads to shutdown.  This could cause deadlock
            // if the current thread is calling Dispose or is part of such a cycle.
            _remainingThreadsToShutdown.Wait();
            _remainingThreadsToShutdown.Dispose();

            // Clean up remaining state
            _schedulerThread.Dispose();
        }

        /// <summary>Gets a list of all tasks scheduled to this scheduler.</summary>
        /// <returns>An enumerable of all scheduled tasks.</returns>
        protected override IEnumerable<Task> GetScheduledTasks() => _tasks.ToArray();

        /// <summary>Queues a task to this scheduler for execution.</summary>
        /// <param name="task">The task to be executed.</param>
        protected override void QueueTask(Task task)
        {
            // Store the task and let the I/O completion port know that more work has arrived.
            _tasks.Enqueue(task);
            _iocp.NotifyOne();
        }

        /// <summary>Try to execute a task on the current thread.</summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="taskWasPreviouslyQueued">Whether the task was previously queued to this scheduler.</param>
        /// <returns>Whether the task was executed.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) =>
            // Only inline from scheduler threads.  This is to ensure concurrency control 
            // is able to handle inlining as well.
            _schedulerThread.Value && TryExecuteTask(task);

        /// <summary>Provides a simple managed wrapper for an I/O completion port.</summary>
        private sealed class IOCompletionPort : IDisposable
        {
            /// <summary>Infinite timeout value to use for GetQueuedCompletedStatus.</summary>
            private const uint INFINITE_TIMEOUT = unchecked((uint)Timeout.Infinite);
            /// <summary>An invalid file handle value.</summary>
            private static readonly IntPtr s_invalidFileHandle = unchecked((IntPtr)(-1));
            /// <summary>An invalid I/O completion port handle value.</summary>
            private static readonly IntPtr s_invalidIOCompletionPortHandle = IntPtr.Zero;

            /// <summary>The I/O completion porth handle.</summary>
            private readonly SafeFileHandle _handle;

            /// <summary>Initializes the I/O completion port.</summary>
            /// <param name="maxConcurrencyLevel">The maximum concurrency level allowed by the I/O completion port.</param>
            public IOCompletionPort(int maxConcurrencyLevel)
            {
                // Validate the argument and create the port.
                if (maxConcurrencyLevel < 1) throw new ArgumentOutOfRangeException(nameof(maxConcurrencyLevel));
                _handle = CreateIoCompletionPort(s_invalidFileHandle, s_invalidIOCompletionPortHandle, UIntPtr.Zero, (uint)maxConcurrencyLevel);
            }

            /// <summary>Clean up.</summary>
            public void Dispose() => _handle.Dispose();

            /// <summary>Notify that I/O completion port that new work is available.</summary>
            public void NotifyOne()
            {
                if (!PostQueuedCompletionStatus(_handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
                    throw new Win32Exception();
            }

            /// <summary>Waits for an item on the I/O completion port.</summary>
            /// <returns>true if an item was available; false if the completion port closed before an item could be retrieved.</returns>
            public bool WaitOne()
            {
                // Wait for an item to be posted.
                // DangerousGetHandle is used so that the safe handle can be closed even while blocked in the call to GetQueuedCompletionStatus.
                if (!GetQueuedCompletionStatus(_handle.DangerousGetHandle(), out _, out _, out _, INFINITE_TIMEOUT))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    if (errorCode == 735 /*ERROR_ABANDONED_WAIT_0*/ || errorCode == 6 /*INVALID_HANDLE*/)
                        return false;
                    else
                        throw new Win32Exception(errorCode);
                }
                return true;
            }

            /// <summary>
            /// Creates an input/output (I/O) completion port and associates it with a specified file handle, 
            /// or creates an I/O completion port that is not yet associated with a file handle, allowing association at a later time.
            /// </summary>
            /// <param name="fileHandle">An open file handle or INVALID_HANDLE_VALUE.</param>
            /// <param name="existingCompletionPort">A handle to an existing I/O completion port or NULL.</param>
            /// <param name="completionKey">The per-handle user-defined completion key that is included in every I/O completion packet for the specified file handle.</param>
            /// <param name="numberOfConcurrentThreads">The maximum number of threads that the operating system can allow to concurrently process I/O completion packets for the I/O completion port.</param>
            /// <returns>If the function succeeds, the return value is the handle to an I/O completion port.  If the function fails, the return value is NULL.</returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern SafeFileHandle CreateIoCompletionPort(
                  IntPtr fileHandle, IntPtr existingCompletionPort, UIntPtr completionKey, uint numberOfConcurrentThreads);

            /// <summary>Attempts to dequeue an I/O completion packet from the specified I/O completion port.</summary>
            /// <param name="completionPort">A handle to the completion port.</param>
            /// <param name="lpNumberOfBytes">A pointer to a variable that receives the number of bytes transferred during an I/O operation that has completed.</param>
            /// <param name="lpCompletionKey">A pointer to a variable that receives the completion key value associated with the file handle whose I/O operation has completed.</param>
            /// <param name="lpOverlapped">A pointer to a variable that receives the address of the OVERLAPPED structure that was specified when the completed I/O operation was started.</param>
            /// <param name="dwMilliseconds">The number of milliseconds that the caller is willing to wait for a completion packet to appear at the completion port. </param>
            /// <returns>Returns nonzero (TRUE) if successful or zero (FALSE) otherwise.</returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool GetQueuedCompletionStatus(
                IntPtr completionPort, out uint lpNumberOfBytes, out IntPtr lpCompletionKey, out IntPtr lpOverlapped, uint dwMilliseconds);

            /// <summary>Posts an I/O completion packet to an I/O completion port.</summary>
            /// <param name="completionPort">A handle to the completion port.</param>
            /// <param name="dwNumberOfBytesTransferred">The value to be returned through the lpNumberOfBytesTransferred parameter of the GetQueuedCompletionStatus function.</param>
            /// <param name="dwCompletionKey">The value to be returned through the lpCompletionKey parameter of the GetQueuedCompletionStatus function.</param>
            /// <param name="lpOverlapped">The value to be returned through the lpOverlapped parameter of the GetQueuedCompletionStatus function.</param>
            /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool PostQueuedCompletionStatus(
                SafeFileHandle completionPort, IntPtr dwNumberOfBytesTransferred, IntPtr dwCompletionKey, IntPtr lpOverlapped);
        }
    }
}
