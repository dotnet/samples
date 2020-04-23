//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Threading.Tasks;

namespace System.Threading.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Executes a function for each value in a range, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="fromInclusive">The start of the range, inclusive.</param>
        /// <param name="toExclusive">The end of the range, exclusive.</param>
        /// <param name="options">The options to use for processing the loop.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeFor<TResult>(
            int fromInclusive, int toExclusive, Func<int, TResult> body) =>
            SpeculativeFor(fromInclusive, toExclusive, s_defaultParallelOptions, body);

        /// <summary>Executes a function for each value in a range, returning the first result achieved and ceasing execution.</summary>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="fromInclusive">The start of the range, inclusive.</param>
        /// <param name="toExclusive">The end of the range, exclusive.</param>
        /// <param name="options">The options to use for processing the loop.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeFor<TResult>(
            int fromInclusive, int toExclusive, ParallelOptions options, Func<int, TResult> body)
        {
            // Validate parameters; the Parallel.For we delegate to will validate the rest
            if (body == null) throw new ArgumentNullException(nameof(body));

            // Store one result.  We box it if it's a value type to avoid torn writes and enable
            // CompareExchange even for value types.
            object result = null;

            // Run all bodies in parallel, stopping as soon as one has completed.
            Parallel.For(fromInclusive, toExclusive, options, (i, loopState) =>
            {
                // Run an iteration.  When it completes, store (box) 
                // the result, and cancel the rest
                Interlocked.CompareExchange(ref result, (object)body(i), null);
                loopState.Stop();
            });

            // Return the computed result
            return (TResult)result;
        }
    }
}
