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
        /// <summary>Executes a map operation, converting an input list into an output list, in parallel.</summary>
        /// <typeparam name="TInput">Specifies the type of the input data.</typeparam>
        /// <typeparam name="TOutput">Specifies the type of the output data.</typeparam>
        /// <param name="input">The input list to be mapped used the transform function.</param>
        /// <param name="transform">The transform function to use to map the input data to the output data.</param>
        /// <returns>The output data, transformed using the transform function.</returns>
        public static TOutput[] Map<TInput, TOutput>(IList<TInput> input, Func<TInput, TOutput> transform) =>
            Map(input, s_defaultParallelOptions, transform);

        /// <summary>Executes a map operation, converting an input list into an output list, in parallel.</summary>
        /// <typeparam name="TInput">Specifies the type of the input data.</typeparam>
        /// <typeparam name="TOutput">Specifies the type of the output data.</typeparam>
        /// <param name="input">The input list to be mapped used the transform function.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="transform">The transform function to use to map the input data to the output data.</param>
        /// <returns>The output data, transformed using the transform function.</returns>
        public static TOutput[] Map<TInput, TOutput>(IList<TInput> input, ParallelOptions parallelOptions, Func<TInput, TOutput> transform)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (parallelOptions == null) throw new ArgumentNullException(nameof(parallelOptions));
            if (transform == null) throw new ArgumentNullException(nameof(transform));

            var output = new TOutput[input.Count];
            Parallel.For(0, input.Count, parallelOptions, i => output[i] = transform(input[i]));
            return output;
        }
    }
}
