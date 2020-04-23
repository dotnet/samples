//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Threading.Tasks
{
    /// <summary>Asynchronously invokes a handler for every posted item.</summary>
    /// <typeparam name="T">Specifies the type of data processed by the instance.</typeparam>
    public sealed class AsyncCall<T> : MarshalByRefObject
    {
        /// <summary>
        /// A queue that stores the posted data.  Also serves as the syncObj for protected instance state.
        /// A ConcurrentQueue is used to enable lock-free dequeues while running with a single consumer task.
        /// </summary>
        private readonly ConcurrentQueue<T> _queue;
        /// <summary>The delegate to invoke for every element.</summary>
        private readonly Delegate _handler;
        /// <summary>The maximum number of items that should be processed by an individual task.</summary>
        private readonly int _maxItemsPerTask;
        /// <summary>The TaskFactory to use to launch new tasks.</summary>
        private readonly TaskFactory _tf;
        /// <summary>The options to use for parallel processing of data.</summary>
        private readonly ParallelOptions _parallelOptions;
        /// <summary>Whether a processing task has been scheduled.</summary>
        private int _processingCount;

        /// <summary>Initializes the AsyncCall with an action to execute for each element.</summary>
        /// <param name="actionHandler">The action to run for every posted item.</param>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism to use.  If not specified, 1 is used for serial execution.</param>
        /// <param name="scheduler">The scheduler to use.  If null, the default scheduler is used.</param>
        /// <param name="maxItemsPerTask">The maximum number of items to be processed per task.  If not specified, Int32.MaxValue is used.</param>
        public AsyncCall(Action<T> actionHandler, int maxDegreeOfParallelism = 1, int maxItemsPerTask = int.MaxValue, TaskScheduler scheduler = null) :
            this(maxDegreeOfParallelism, maxItemsPerTask, scheduler) => _handler = actionHandler ?? throw new ArgumentNullException("handler");

        /// <summary>
        /// Initializes the AsyncCall with a function to execute for each element.  The function returns an Task 
        /// that represents the asynchronous completion of that element's processing.
        /// </summary>
        /// <param name="functionHandler">The function to run for every posted item.</param>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism to use.  If not specified, 1 is used for serial execution.</param>
        /// <param name="scheduler">The scheduler to use.  If null, the default scheduler is used.</param>
        public AsyncCall(Func<T, Task> functionHandler, int maxDegreeOfParallelism = 1, TaskScheduler scheduler = null) :
            this(maxDegreeOfParallelism, 1, scheduler) => _handler = functionHandler ?? throw new ArgumentNullException("handler");

        /// <summary>General initialization of the AsyncCall.  Another constructor must initialize the delegate.</summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism to use.  If not specified, 1 is used for serial execution.</param>
        /// <param name="maxItemsPerTask">The maximum number of items to be processed per task.  If not specified, Int32.MaxValue is used.</param>
        /// <param name="scheduler">The scheduler to use.  If null, the default scheduler is used.</param>
        private AsyncCall(int maxDegreeOfParallelism = 1, int maxItemsPerTask = int.MaxValue, TaskScheduler scheduler = null)
        {
            // Validate arguments
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism));
            if (maxItemsPerTask < 1) throw new ArgumentOutOfRangeException(nameof(maxItemsPerTask));
            if (scheduler == null) scheduler = TaskScheduler.Default;

            // Configure the instance
            _queue = new ConcurrentQueue<T>();
            _maxItemsPerTask = maxItemsPerTask;
            _tf = new TaskFactory(scheduler);
            if (maxDegreeOfParallelism != 1)
            {
                _parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism, TaskScheduler = scheduler };
            }
        }

        /// <summary>Post an item for processing.</summary>
        /// <param name="item">The item to be processed.</param>
        public void Post(T item)
        {
            lock (_queue)
            {
                // Add the item to the internal queue
                _queue.Enqueue(item);

                // Check to see whether the right number of tasks have been scheduled.
                // If they haven't, schedule one for this new piece of data.
                if (_handler is Action<T>)
                {
                    if (_processingCount == 0)
                    {
                        _processingCount = 1;
                        _tf.StartNew(ProcessItemsActionTaskBody);
                    }
                }
                else if (_handler is Func<T, Task>)
                {
                    if (_processingCount == 0 ||  // is anyone at all currently processing?
                        _parallelOptions != null && _processingCount < _parallelOptions.MaxDegreeOfParallelism && // are enough workers currently processing?
                        !_queue.IsEmpty) // and, as an optimization, double check to make sure the item hasn't already been picked up by another worker
                    {
                        _processingCount++;
                        _tf.StartNew(ProcessItemFunctionTaskBody, null);
                    }
                }
                else Debug.Fail("_handler is an invalid delegate type");
            }
        }

        /// <summary>Gets an enumerable that yields the items to be processed at this time.</summary>
        /// <returns>An enumerable of items.</returns>
        private IEnumerable<T> GetItemsToProcess()
        {
            // Yield the next elements to be processed until either there are no more elements
            // or we've reached the maximum number of elements that an individual task should process.
            int processedCount = 0;
            while (processedCount < _maxItemsPerTask && _queue.TryDequeue(out T nextItem))
            {
                yield return nextItem;
                processedCount++;
            }
        }

        /// <summary>Used as the body of an action task to process items in the queue.</summary>
        private void ProcessItemsActionTaskBody()
        {
            try
            {
                // Get the handler
                Action<T> handler = (Action<T>)_handler;

                // Process up to _maxItemsPerTask items, either serially or in parallel
                // based on the provided maxDegreeOfParallelism (which determines
                // whether a ParallelOptions is instantiated).
                if (_parallelOptions == null)
                    foreach (var item in GetItemsToProcess()) handler(item);
                else
                    Parallel.ForEach(GetItemsToProcess(), _parallelOptions, handler);
            }
            finally
            {
                lock (_queue)
                {
                    // If there are still items in the queue, schedule another task to continue processing.
                    // Otherwise, note that we're no longer processing.
                    if (!_queue.IsEmpty) _tf.StartNew(ProcessItemsActionTaskBody, TaskCreationOptions.PreferFairness);
                    else _processingCount = 0;
                }
            }
        }

        /// <summary>Used as the body of a function task to process items in the queue.</summary>
        private void ProcessItemFunctionTaskBody(object ignored)
        {
            bool anotherTaskQueued = false;
            try
            {
                // Get the handler
                Func<T, Task> handler = (Func<T, Task>)_handler;

                // Get the next item from the queue to process
                if (_queue.TryDequeue(out T nextItem))
                {
                    // Run the handler and get the follow-on task.
                    // If we got a follow-on task, run this process again when the task completes.
                    // If we didn't, just start another task to keep going now.
                    var task = handler(nextItem);
                    if (task != null) task.ContinueWith(ProcessItemFunctionTaskBody, _tf.Scheduler);
                    else _tf.StartNew(ProcessItemFunctionTaskBody, null);

                    // We've queued a task to continue processing, which means that logically
                    // we're still maintaining the same level of parallelism.
                    anotherTaskQueued = true;
                }
            }
            finally
            {
                // If we didn't queue up another task to continue processing (either
                // because an exception occurred, or we failed to grab an item from the queue)
                if (!anotherTaskQueued)
                {
                    lock (_queue)
                    {
                        // Verify that there's still nothing in the queue, now under the same
                        // lock that the queuer needs to take in order to increment the processing count
                        // and launch a new processor.
                        if (!_queue.IsEmpty) _tf.StartNew(ProcessItemFunctionTaskBody, null);
                        else _processingCount--;
                    }
                }
            }
        }
    }

    /// <summary>Provides static factory methods for creating AsyncCall(Of T) instances.</summary>
    public static class AsyncCall
    {
        /// <summary>Initializes the AsyncCall with an action to execute for each element.</summary>
        /// <param name="actionHandler">The action to run for every posted item.</param>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism to use.  If not specified, 1 is used for serial execution.</param>
        /// <param name="scheduler">The scheduler to use.  If null, the default scheduler is used.</param>
        /// <param name="maxItemsPerTask">The maximum number of items to be processed per task.  If not specified, Int32.MaxValue is used.</param>
        public static AsyncCall<T> Create<T>(
            Action<T> actionHandler,
            int maxDegreeOfParallelism = 1,
            int maxItemsPerTask = int.MaxValue,
            TaskScheduler scheduler = null) =>
            new AsyncCall<T>(actionHandler, maxDegreeOfParallelism, maxItemsPerTask, scheduler);

        /// <summary>
        /// Initializes the AsyncCall with a function to execute for each element.  The function returns an Task 
        /// that represents the asynchronous completion of that element's processing.
        /// </summary>
        /// <param name="functionHandler">The function to run for every posted item.</param>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism to use.  If not specified, 1 is used for serial execution.</param>
        /// <param name="maxItemsPerTask">The maximum number of items to be processed per task.  If not specified, Int32.MaxValue is used.</param>
        /// <param name="scheduler">The scheduler to use.  If null, the default scheduler is used.</param>
        public static AsyncCall<T> Create<T>(
            Func<T, Task> functionHandler,
            int maxDegreeOfParallelism = 1,
            TaskScheduler scheduler = null) =>
            new AsyncCall<T>(functionHandler, maxDegreeOfParallelism, scheduler);
    }
}
