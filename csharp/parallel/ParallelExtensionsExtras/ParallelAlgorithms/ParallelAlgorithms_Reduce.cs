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
        /// <summary>Reduces the input data using the specified aggregation operation.</summary>
        /// <typeparam name="T">Specifies the type of data being aggregated.</typeparam>
        /// <param name="input">The input data to be reduced.</param>
        /// <param name="seed">The seed to use to initialize the operation; this seed may be used multiple times.</param>
        /// <param name="associativeCommutativeOperation">The reduction operation.</param>
        /// <returns>The reduced value.</returns>
        public static T Reduce<T>(
            IList<T> input, T seed,
            Func<T, T, T> associativeCommutativeOperation) =>
            Reduce(input, s_defaultParallelOptions, seed, associativeCommutativeOperation);

        /// <summary>Reduces the input data using the specified aggregation operation.</summary>
        /// <typeparam name="T">Specifies the type of data being aggregated.</typeparam>
        /// <param name="input">The input data to be reduced.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="seed">The seed to use to initialize the operation; this seed may be used multiple times.</param>
        /// <param name="associativeCommutativeOperation">The reduction operation.</param>
        /// <returns>The reduced value.</returns>
        public static T Reduce<T>(
            IList<T> input, ParallelOptions parallelOptions,
            T seed, Func<T, T, T> associativeCommutativeOperation)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Reduce(0, input.Count, parallelOptions, i => input[i], seed, associativeCommutativeOperation);
        }

        /// <summary>Reduces the input range using the specified aggregation operation.</summary>
        /// <typeparam name="T">Specifies the type of data being aggregated.</typeparam>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="mapOperation">The function used to retrieve the data to be reduced for a given index.</param>
        /// <param name="seed">The seed to use to initialize the operation; this seed may be used multiple times.</param>
        /// <param name="associativeCommutativeOperation">The reduction operation.</param>
        /// <returns>The reduced value.</returns>
        public static T Reduce<T>(
            int fromInclusive, int toExclusive,
            Func<int, T> mapOperation, T seed, Func<T, T, T> associativeCommutativeOperation) =>
            Reduce(fromInclusive, toExclusive, s_defaultParallelOptions, mapOperation, seed, associativeCommutativeOperation);

        /// <summary>Reduces the input range using the specified aggregation operation.</summary>
        /// <typeparam name="T">Specifies the type of data being aggregated.</typeparam>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="mapOperation">The function used to retrieve the data to be reduced for a given index.</param>
        /// <param name="seed">The seed to use to initialize the operation; this seed may be used multiple times.</param>
        /// <param name="associativeCommutativeOperation">The reduction operation.</param>
        /// <returns>The reduced value.</returns>
        public static T Reduce<T>(
            int fromInclusive, int toExclusive, ParallelOptions parallelOptions,
            Func<int, T> mapOperation, T seed, Func<T, T, T> associativeCommutativeOperation)
        {
            if (parallelOptions == null) throw new ArgumentNullException(nameof(parallelOptions));
            if (mapOperation == null) throw new ArgumentNullException(nameof(mapOperation));
            if (associativeCommutativeOperation == null) throw new ArgumentNullException(nameof(associativeCommutativeOperation));
            if (toExclusive < fromInclusive) throw new ArgumentOutOfRangeException(nameof(toExclusive));

            object obj = new object(); // used as a monitor for the final reduction
            T result = seed; // accumulator for final reduction

            // Reduce in parallel
            Parallel.For(fromInclusive, toExclusive, parallelOptions,
                // Initialize each thread with the user-specified seed
                () => seed,
                // Map the current index to a value and aggregate that value into the local reduction
                (i, loop, localResult) => associativeCommutativeOperation(mapOperation(i), localResult),
                // Combine all of the local reductions
                localResult => { lock (obj) result = associativeCommutativeOperation(localResult, result); });

            // Return the final result
            return result;
        }
    }
}
