//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Executes a function for each element in a source, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="source">The input elements to be processed.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeForEach<TSource, TResult>(
            IEnumerable<TSource> source, Func<TSource, TResult> body) =>
            SpeculativeForEach(source, s_defaultParallelOptions, body);

        /// <summary>Executes a function for each element in a source, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="source">The input elements to be processed.</param>
        /// <param name="options">The options to use for processing the loop.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeForEach<TSource, TResult>(
            IEnumerable<TSource> source, ParallelOptions options, Func<TSource, TResult> body)
        {
            // Validate parameters; the Parallel.ForEach we delegate to will validate the rest
            if (body == null) throw new ArgumentNullException(nameof(body));

            // Store one result.  We box it if it's a value type to avoid torn writes and enable
            // CompareExchange even for value types.
            object result = null;

            // Run all bodies in parallel, stopping as soon as one has completed.
            Parallel.ForEach(source, options, (item, loopState) =>
            {
                // Run an iteration.  When it completes, store (box) 
                // the result, and cancel the rest
                Interlocked.CompareExchange(ref result, (object)body(item), null);
                loopState.Stop();
            });

            // Return the computed result
            return (TResult)result;
        }
    }
}
