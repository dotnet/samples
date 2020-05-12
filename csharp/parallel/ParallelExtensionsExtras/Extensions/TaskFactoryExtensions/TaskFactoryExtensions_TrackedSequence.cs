//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Linq;

namespace System.Threading.Tasks
{
    public static partial class TaskFactoryExtensions
    {
        /// <summary>Asynchronously executes a sequence of tasks, maintaining a list of all tasks processed.</summary>
        /// <param name="factory">The TaskFactory to use to create the task.</param>
        /// <param name="functions">
        /// The functions that generate the tasks through which to iterate sequentially.
        /// Iteration will cease if a task faults.
        /// </param>
        /// <returns>A Task that will return the list of tracked tasks iterated.</returns>
        public static Task<IList<Task>> TrackedSequence(this TaskFactory factory, params Func<Task>[] functions)
        {
            var tcs = new TaskCompletionSource<IList<Task>>();
            factory.Iterate(TrackedSequenceInternal(functions, tcs).Cast<object>());
            return tcs.Task;
        }

        /// <summary>Creates the enumerable to iterate through with Iterate.</summary>
        /// <param name="functions">
        /// The functions that generate the tasks through which to iterate sequentially.
        /// Iteration will cease if a task faults.
        /// </param>
        /// <param name="tcs">The TaskCompletionSource to resolve with the asynchronous results.</param>
        /// <returns>The enumerable through which to iterate.</returns>
        private static IEnumerable<Task> TrackedSequenceInternal(
            IEnumerable<Func<Task>> functions, TaskCompletionSource<IList<Task>> tcs)
        {
            // Store a list of all tasks iterated through.  This will be provided
            // to the resulting task when we're done.
            var tasks = new List<Task>();

            // Run seqeuentially through all of the provided functions.
            foreach (var func in functions)
            {
                // Get the next task.  If we get an exception while trying to do so,
                // an invalid function was provided.  Fault the TCS and break out.
                Task nextTask = null;
                try { nextTask = func(); } catch (Exception exc) { tcs.TrySetException(exc); }
                if (nextTask == null) yield break;

                // Store the task that was generated and yield it from the sequence.  If the task
                // faults, break out of the loop so that no more tasks are processed.
                tasks.Add(nextTask);
                yield return nextTask;
                if (nextTask.IsFaulted) break;
            }

            // We're done.  Transfer all tasks we iterated through.
            tcs.TrySetResult(tasks);
        }
    }
}
