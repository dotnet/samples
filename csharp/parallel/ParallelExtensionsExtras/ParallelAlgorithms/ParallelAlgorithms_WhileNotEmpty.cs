//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Processes data in parallel, allowing the processing function to add more data to be processed.</summary>
        /// <typeparam name="T">Specifies the type of data being processed.</typeparam>
        /// <param name="initialValues">The initial set of data to be processed.</param>
        /// <param name="body">The operation to execute for each value.</param>
        public static void WhileNotEmpty<T>(IEnumerable<T> initialValues, Action<T, Action<T>> body) => WhileNotEmpty(s_defaultParallelOptions, initialValues, body);

        /// <summary>Processes data in parallel, allowing the processing function to add more data to be processed.</summary>
        /// <typeparam name="T">Specifies the type of data being processed.</typeparam>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="initialValues">The initial set of data to be processed.</param>
        /// <param name="body">The operation to execute for each value.</param>
        public static void WhileNotEmpty<T>(
            ParallelOptions parallelOptions,
            IEnumerable<T> initialValues,
            Action<T, Action<T>> body)
        {
            // Validate arguments
            if (parallelOptions == null) throw new ArgumentNullException(nameof(parallelOptions));
            if (initialValues == null) throw new ArgumentNullException(nameof(initialValues));
            if (body == null) throw new ArgumentNullException(nameof(body));

            // Create two lists to alternate between as source and destination.
            var lists = new[] { new ConcurrentStack<T>(initialValues), new ConcurrentStack<T>() };

            // Iterate until no more items to be processed
            for (int i = 0; ; i++)
            {
                // Determine which list is the source and which is the destination
                int fromIndex = i % 2;
                var from = lists[fromIndex];
                var to = lists[fromIndex ^ 1];

                // If the source is empty, we're done
                if (from.IsEmpty) break;

                // Otherwise, process all source items in parallel, adding any new items into the destination
                Action<T> adder = newItem => to.Push(newItem);
                Parallel.ForEach(from, parallelOptions, e => body(e, adder));

                // Clear out the source as it's now been fully processed
                from.Clear();
            }
        }
    }
}
