//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace System.Threading.Async
{
    /// <summary>Provides for asynchronous exclusive and concurrent execution support.</summary>
    [DebuggerDisplay("WaitingConcurrent={WaitingConcurrent}, WaitingExclusive={WaitingExclusive}, CurrentReaders={CurrentConcurrent}, Exclusive={CurrentlyExclusive}")]
    public sealed class AsyncReaderWriter
    {
        /// <summary>The lock that protects all shared state in this instance.</summary>
        private readonly object _lock = new object();
        /// <summary>The queue of concurrent readers waiting to execute.</summary>
        private readonly Queue<Task> _waitingConcurrent = new Queue<Task>();
        /// <summary>The queue of exclusive writers waiting to execute.</summary>
        private readonly Queue<Task> _waitingExclusive = new Queue<Task>();
        /// <summary>The number of concurrent readers currently executing.</summary>
        private int _currentConcurrent = 0;
        /// <summary>The number of exclusive writers currently executing.</summary>
        private bool _currentlyExclusive = false;
        /// <summary>The non-generic factory to use for task creation.</summary>
        private readonly TaskFactory _factory;

        /// <summary>Initializes the ReaderWriterAsync.</summary>
        public AsyncReaderWriter() => _factory = Task.Factory;

        /// <summary>Initializes the ReaderWriterAsync with the specified TaskFactory for us in creating all tasks.</summary>
        /// <param name="factory">The TaskFactory to use to create all tasks.</param>
        public AsyncReaderWriter(TaskFactory factory) =>
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        /// <summary>Gets the number of exclusive operations currently queued.</summary>
        public int WaitingExclusive { get { lock (_lock) return _waitingExclusive.Count; } }
        /// <summary>Gets the number of concurrent operations currently queued.</summary>
        public int WaitingConcurrent { get { lock (_lock) return _waitingConcurrent.Count; } }
        /// <summary>Gets the number of concurrent operations currently executing.</summary>
        public int CurrentConcurrent { get { lock (_lock) return _currentConcurrent; } }
        /// <summary>Gets whether an exclusive operation is currently executing.</summary>
        public bool CurrentlyExclusive { get { lock (_lock) return _currentlyExclusive; } }

        /// <summary>Queues an exclusive writer action to the ReaderWriterAsync.</summary>
        /// <param name="action">The action to be executed exclusively.</param>
        /// <returns>A Task that represents the execution of the provided action.</returns>
        public Task QueueExclusiveWriter(Action action)
        {
            // Create the task.  This Task will be started by the coordination primitive
            // when it's safe to do so, e.g. when there are no other tasks associated
            // with this async primitive executing.
            var task = _factory.Create(state =>
            {
                // Run the user-provided action
                try { ((Action)state)(); }
                // Ensure that we clean up when we're done
                finally { FinishExclusiveWriter(); }
            }, action);

            // Now that we've created the task, we need to do something with it, either queueing it or scheduling it immediately
            lock (_lock)
            {
                // If there's already a task running, or if there are any other exclusive tasks that need to run,
                // queue it.  Otherwise, no one else is running or wants to run, so schedule it now.
                if (_currentlyExclusive || _currentConcurrent > 0 || _waitingExclusive.Count > 0) _waitingExclusive.Enqueue(task);
                else RunExclusive_RequiresLock(task);
            }

            // Return the created task for the caller to track.
            return task;
        }

        /// <summary>Queues an exclusive writer function to the ReaderWriterAsync.</summary>
        /// <param name="function">The function to be executed exclusively.</param>
        /// <returns>A Task that represents the execution of the provided function.</returns>
        public Task<TResult> QueueExclusiveWriter<TResult>(Func<TResult> function)
        {
            // Create the task.  This Task will be started by the coordination primitive
            // when it's safe to do so, e.g. when there are no other tasks associated
            // with this async primitive executing.
            var task = _factory.Create(state =>
            {
                // Run the user-provided function
                try { return ((Func<TResult>)state)(); }
                // Ensure that we clean up when we're done
                finally { FinishExclusiveWriter(); }
            }, function);

            // Now that we've created the task, we need to do something with it, either queueing it or scheduling it immediately
            lock (_lock)
            {
                // If there's already a task running, or if there are any other exclusive tasks that need to run,
                // queue it.  Otherwise, no one else is running or wants to run, so schedule it now.
                if (_currentlyExclusive || _currentConcurrent > 0 || _waitingExclusive.Count > 0) _waitingExclusive.Enqueue(task);
                else RunExclusive_RequiresLock(task);
            }

            // Return the created task for the caller to track.
            return task;
        }

        /// <summary>Queues a concurrent reader action to the ReaderWriterAsync.</summary>
        /// <param name="action">The action to be executed concurrently.</param>
        /// <returns>A Task that represents the execution of the provided action.</returns>
        public Task QueueConcurrentReader(Action action)
        {
            // Create the task.  This Task will be started by the coordination primitive
            // when it's safe to do so, e.g. when there are no exclusive tasks running
            // or waiting to run.
            Task task = _factory.Create(state =>
            {
                // Run the user-provided action
                try { ((Action)state)(); }
                // Ensure that we clean up when we're done
                finally { FinishConcurrentReader(); }
            }, action);

            // Now that we've created the task, we need to do something with it, either queueing it or scheduling it immediately
            lock (_lock)
            {
                // If there are any exclusive tasks running or waiting, queue the concurrent task
                if (_currentlyExclusive || _waitingExclusive.Count > 0) _waitingConcurrent.Enqueue(task);
                // Otherwise schedule it immediately
                else RunConcurrent_RequiresLock(task);
            }

            // Return the task to the caller.
            return task;
        }

        /// <summary>Queues a concurrent reader function to the ReaderWriterAsync.</summary>
        /// <param name="function">The function to be executed concurrently.</param>
        /// <returns>A Task that represents the execution of the provided function.</returns>
        public Task<TResult> QueueConcurrentReader<TResult>(Func<TResult> function)
        {
            // Create the task.  This Task will be started by the coordination primitive
            // when it's safe to do so, e.g. when there are no exclusive tasks running
            // or waiting to run.
            var task = _factory.Create(state =>
            {
                // Run the user-provided function
                try { return ((Func<TResult>)state)(); }
                // Ensure that we clean up when we're done
                finally { FinishConcurrentReader(); }
            }, function);

            // Now that we've created the task, we need to do something with it, either queueing it or scheduling it immediately
            lock (_lock)
            {
                // If there are any exclusive tasks running or waiting, queue the concurrent task
                if (_currentlyExclusive || _waitingExclusive.Count > 0) _waitingConcurrent.Enqueue(task);
                // Otherwise schedule it immediately
                else RunConcurrent_RequiresLock(task);
            }

            // Return the task to the caller.
            return task;
        }

        /// <summary>Starts the specified exclusive task.</summary>
        /// <param name="exclusive">The exclusive task to be started.</param>
        /// <remarks>This must only be executed while holding the instance's lock.</remarks>
        private void RunExclusive_RequiresLock(Task exclusive)
        {
            _currentlyExclusive = true;
            exclusive.Start(_factory.GetTargetScheduler());
        }

        /// <summary>Starts the specified concurrent task.</summary>
        /// <param name="concurrent">The exclusive task to be started.</param>
        /// <remarks>This must only be executed while holding the instance's lock.</remarks>
        private void RunConcurrent_RequiresLock(Task concurrent)
        {
            _currentConcurrent++;
            concurrent.Start(_factory.GetTargetScheduler());
        }

        /// <summary>Starts all queued concurrent tasks.</summary>
        /// <remarks>This must only be executed while holding the instance's lock.</remarks>
        private void RunConcurrent_RequiresLock()
        {
            while (_waitingConcurrent.Count > 0) RunConcurrent_RequiresLock(_waitingConcurrent.Dequeue());
        }

        /// <summary>Completes the processing of a concurrent reader.</summary>
        private void FinishConcurrentReader()
        {
            lock (_lock)
            {
                // Update the tracking count of the number of concurrently executing tasks
                _currentConcurrent--;

                // If we've now hit zero tasks running concurrently and there are any waiting writers, run one of them
                if (_currentConcurrent == 0 && _waitingExclusive.Count > 0) RunExclusive_RequiresLock(_waitingExclusive.Dequeue());

                // Otherwise, if there are no waiting writers but there are waiting readers for some reason (they should
                // have started when they were added by the user), run all concurrent tasks waiting.
                else if (_waitingExclusive.Count == 0 && _waitingConcurrent.Count > 0) RunConcurrent_RequiresLock();
            }
        }

        /// <summary>Completes the processing of an exclusive writer.</summary>
        private void FinishExclusiveWriter()
        {
            lock (_lock)
            {
                // We're no longer executing exclusively, though this might get reversed shortly
                _currentlyExclusive = false;

                // If there are any more waiting exclusive tasks, run the next one in line
                if (_waitingExclusive.Count > 0) RunExclusive_RequiresLock(_waitingExclusive.Dequeue());

                // Otherwise, if there are any waiting concurrent tasks, run them all
                else if (_waitingConcurrent.Count > 0) RunConcurrent_RequiresLock();
            }
        }
    }
}
