//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;

namespace System.Threading.Tasks
{
    public static partial class TaskFactoryExtensions
    {
        #region No Object State Overloads
        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return Iterate(factory, source, null, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the iteration.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source,
            CancellationToken cancellationToken)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return Iterate(factory, source, null, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source,
            TaskCreationOptions creationOptions)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return Iterate(factory, source, null, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <param name="scheduler">The scheduler to which tasks will be scheduled.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source,
            TaskScheduler scheduler)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return Iterate(factory, source, null, factory.CancellationToken, factory.CreationOptions, scheduler);
        }

        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the iteration.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which tasks will be scheduled.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source,
            CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler) => Iterate(factory, source, null, cancellationToken, creationOptions, scheduler);
        #endregion

        #region Object State Overloads and Full Implementation
        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <param name="state">The asynchronous state for the returned Task.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source, object state)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return Iterate(factory, source, state, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <param name="state">The asynchronous state for the returned Task.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the iteration.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source, object state,
            CancellationToken cancellationToken)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return Iterate(factory, source, state, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <param name="state">The asynchronous state for the returned Task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source, object state,
            TaskCreationOptions creationOptions)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return Iterate(factory, source, state, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <param name="state">The asynchronous state for the returned Task.</param>
        /// <param name="scheduler">The scheduler to which tasks will be scheduled.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source, object state,
            TaskScheduler scheduler)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return Iterate(factory, source, state, factory.CancellationToken, factory.CreationOptions, scheduler);
        }

        /// <summary>Asynchronously iterates through an enumerable of tasks.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="source">The enumerable containing the tasks to be iterated through.</param>
        /// <param name="state">The asynchronous state for the returned Task.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the iteration.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which tasks will be scheduled.</param>
        /// <returns>A Task that represents the complete asynchronous operation.</returns>
        public static Task Iterate(
            this TaskFactory factory,
            IEnumerable<object> source, object state,
            CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            // Validate/update parameters
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            // Get an enumerator from the enumerable
            var enumerator = source.GetEnumerator();
            if (enumerator == null) throw new InvalidOperationException("Invalid enumerable - GetEnumerator returned null");

            // Create the task to be returned to the caller.  And ensure
            // that when everything is done, the enumerator is cleaned up.
            var trs = new TaskCompletionSource<object>(state, creationOptions);
            trs.Task.ContinueWith(_ => enumerator.Dispose(), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            // This will be called every time more work can be done.
            Action<Task> recursiveBody = null;
            recursiveBody = antecedent =>
            {
                try
                {
                    // If we should continue iterating and there's more to iterate
                    // over, create a continuation to continue processing.  We only
                    // want to continue processing once the current Task (as yielded
                    // from the enumerator) is complete.
                    if (enumerator.MoveNext())
                    {
                        var nextItem = enumerator.Current;

                        // If we got a Task, continue from it to continue iterating
                        if (nextItem is Task nextTask)
                        {
                            nextTask.IgnoreExceptions(); // TODO: Is this a good idea?
                            nextTask.ContinueWith(recursiveBody).IgnoreExceptions();
                        }
                        // If we got a scheduler, continue iterating under the new scheduler,
                        // enabling hopping between contexts.
                        else if (nextItem is TaskScheduler)
                        {
                            Task.Factory.StartNew(() => recursiveBody(null), CancellationToken.None, TaskCreationOptions.None, (TaskScheduler)nextItem).IgnoreExceptions();
                        }
                        else trs.TrySetException(new InvalidOperationException("Task or TaskScheduler object expected in Iterate"));
                    }

                    // Otherwise, we're done!
                    else trs.TrySetResult(null);
                }
                // If MoveNext throws an exception, propagate that to the user,
                // either as cancellation or as a fault
                catch (Exception exc)
                {
                    if (exc is OperationCanceledException oce && oce.CancellationToken == cancellationToken)
                    {
                        trs.TrySetCanceled();
                    }
                    else trs.TrySetException(exc);
                }
            };

            // Get things started by launching the first task
            factory.StartNew(() => recursiveBody(null), CancellationToken.None, TaskCreationOptions.None, scheduler).IgnoreExceptions();

            // Return the representative task to the user
            return trs.Task;
        }
        #endregion
    }
}
