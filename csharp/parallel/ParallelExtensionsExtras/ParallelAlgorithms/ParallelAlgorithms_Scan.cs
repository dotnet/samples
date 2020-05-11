//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Threading.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Computes a parallel prefix scan over the source enumerable using the specified function.</summary>
        /// <typeparam name="T">The type of the data in the source.</typeparam>
        /// <param name="source">The source data over which a prefix scan should be computed.</param>
        /// <param name="function">The function to use for the scan.</param>
        /// <returns>The results of the scan operation.</returns>
        /// <remarks>
        /// For very small functions, such as additions, an implementation targeted
        /// at the relevant type and operation will perform significantly better than
        /// this generalized implementation.
        /// </remarks>
        public static T[] Scan<T>(IEnumerable<T> source, Func<T, T, T> function) =>
            Scan(source, function, loadBalance: false);

        /// <summary>Computes a parallel prefix scan over the source enumerable using the specified function.</summary>
        /// <typeparam name="T">The type of the data in the source.</typeparam>
        /// <param name="source">The source data over which a prefix scan should be computed.</param>
        /// <param name="function">The function to use for the scan.</param>
        /// <param name="loadBalance">Whether to load-balance during process.</param>
        /// <returns>The results of the scan operation.</returns>
        /// <remarks>
        /// For very small functions, such as additions, an implementation targeted
        /// at the relevant type and operation will perform significantly better than
        /// this generalized implementation.
        /// </remarks>
        public static T[] Scan<T>(IEnumerable<T> source, Func<T, T, T> function, bool loadBalance)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException(nameof(source));

            // Create output copy
            var output = source.ToArray();

            // Do the prefix scan in-place on the copy and return the results
            ScanInPlace(output, function, loadBalance);
            return output;
        }

        /// <summary>Computes a parallel prefix scan in-place on an array using the specified function.</summary>
        /// <typeparam name="T">The type of the data in the source.</typeparam>
        /// <param name="data">The data over which a prefix scan should be computed. Upon exit, stores the results.</param>
        /// <param name="function">The function to use for the scan.</param>
        /// <returns>The results of the scan operation.</returns>
        /// <remarks>
        /// For very small functions, such as additions, an implementation targeted
        /// at the relevant type and operation will perform significantly better than
        /// this generalized implementation.
        /// </remarks>
        public static void ScanInPlace<T>(T[] data, Func<T, T, T> function) =>
            ScanInPlace(data, function, loadBalance: false);

        /// <summary>Computes a parallel prefix scan in-place on an array using the specified function.</summary>
        /// <typeparam name="T">The type of the data in the source.</typeparam>
        /// <param name="data">The data over which a prefix scan should be computed. Upon exit, stores the results.</param>
        /// <param name="function">The function to use for the scan.</param>
        /// <param name="loadBalance">Whether to load-balance during process.</param>
        /// <returns>The results of the scan operation.</returns>
        /// <remarks>
        /// For very small functions, such as additions, an implementation targeted
        /// at the relevant type and operation will perform significantly better than
        /// this generalized implementation.
        /// </remarks>
        public static void ScanInPlace<T>(T[] data, Func<T, T, T> function, bool loadBalance)
        {
            // Validate arguments
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (function == null) throw new ArgumentNullException(nameof(function));

            // Do the prefix scan in-place and return the results.  This implementation
            // of parallel prefix scan ends up executing the function twice as many
            // times as the sequential implementation.  Thus, only if we have more than 2 cores
            // will the parallel version have a chance of running faster than sequential.
            if (Environment.ProcessorCount <= 2)
            {
                InclusiveScanInPlaceSerial(data, function, 0, data.Length, 1);
            }
            else if (loadBalance)
            {
                InclusiveScanInPlaceWithLoadBalancingParallel(data, function, 0, data.Length, 1);
            }
            else // parallel, non-loadbalance
            {
                InclusiveScanInPlaceParallel(data, function);
            }
        }

        /// <summary>Computes a sequential prefix scan over the array using the specified function.</summary>
        /// <typeparam name="T">The type of the data in the array.</typeparam>
        /// <param name="arr">The data, which will be overwritten with the computed prefix scan.</param>
        /// <param name="function">The function to use for the scan.</param>
        /// <param name="arrStart">The start of the data in arr over which the scan is being computed.</param>
        /// <param name="arrLength">The length of the data in arr over which the scan is being computed.</param>
        /// <param name="skip">The inclusive distance between elements over which the scan is being computed.</param>
        /// <remarks>No parameter validation is performed.</remarks>
        private static void InclusiveScanInPlaceSerial<T>(T[] arr, Func<T, T, T> function, int arrStart, int arrLength, int skip)
        {
            for (int i = arrStart; i + skip < arrLength; i += skip)
            {
                arr[i + skip] = function(arr[i], arr[i + skip]);
            }
        }

        /// <summary>Computes a sequential exclusive prefix scan over the array using the specified function.</summary>
        /// <param name="arr">The data, which will be overwritten with the computed prefix scan.</param>
        /// <param name="function">The function to use for the scan.</param>
        /// <param name="lowerBoundInclusive">The inclusive lower bound of the array at which to start the scan.</param>
        /// <param name="upperBoundExclusive">The exclusive upper bound of the array at which to end the scan.</param>
        public static void ExclusiveScanInPlaceSerial<T>(T[] arr, Func<T, T, T> function, int lowerBoundInclusive, int upperBoundExclusive)
        {
            T total = arr[lowerBoundInclusive];
            arr[lowerBoundInclusive] = default;
            for (int i = lowerBoundInclusive + 1; i < upperBoundExclusive; i++)
            {
                T prevTotal = total;
                total = function(total, arr[i]);
                arr[i] = prevTotal;
            }
        }

        /// <summary>Computes a parallel prefix scan over the array using the specified function.</summary>
        /// <typeparam name="T">The type of the data in the array.</typeparam>
        /// <param name="arr">The data, which will be overwritten with the computed prefix scan.</param>
        /// <param name="function">The function to use for the scan.</param>
        /// <param name="arrStart">The start of the data in arr over which the scan is being computed.</param>
        /// <param name="arrLength">The length of the data in arr over which the scan is being computed.</param>
        /// <param name="skip">The inclusive distance between elements over which the scan is being computed.</param>
        /// <remarks>No parameter validation is performed.</remarks>
        private static void InclusiveScanInPlaceWithLoadBalancingParallel<T>(T[] arr, Func<T, T, T> function,
            int arrStart, int arrLength, int skip)
        {
            // If the length is 0 or 1, just return a copy of the original array.
            if (arrLength <= 1) return;
            int halfInputLength = arrLength / 2;

            // Pairwise combine. Use static partitioning, as the function
            // is likely to be very small.
            Parallel.For(0, halfInputLength, i =>
            {
                int loc = arrStart + i * 2 * skip;
                arr[loc + skip] = function(arr[loc], arr[loc + skip]);
            });

            // Recursively prefix scan the pairwise computations.
            InclusiveScanInPlaceWithLoadBalancingParallel(arr, function, arrStart + skip, halfInputLength, skip * 2);

            // Generate output. As before, use static partitioning.
            Parallel.For(0, arrLength % 2 == 0 ? halfInputLength - 1 : halfInputLength, i =>
            {
                int loc = arrStart + i * 2 * skip + skip;
                arr[loc + skip] = function(arr[loc], arr[loc + skip]);
            });
        }

        /// <summary>Computes a parallel inclusive prefix scan over the array using the specified function.</summary>
        public static void InclusiveScanInPlaceParallel<T>(T[] arr, Func<T, T, T> function)
        {
            int procCount = Environment.ProcessorCount;
            T[] intermediatePartials = new T[procCount];
            using var phaseBarrier =
                new Barrier(procCount,
                    _ => ExclusiveScanInPlaceSerial(
                        intermediatePartials, function, 0, intermediatePartials.Length));

            // Compute the size of each range
            int rangeSize = arr.Length / procCount;
            int nextRangeStart = 0;

            // Create, store, and wait on all of the tasks
            var tasks = new Task[procCount];
            for (int i = 0; i < procCount; i++, nextRangeStart += rangeSize)
            {
                // Get the range for each task, then start it
                int rangeNum = i;
                int lowerRangeInclusive = nextRangeStart;
                int upperRangeExclusive = i < procCount - 1 ? nextRangeStart + rangeSize : arr.Length;
                tasks[rangeNum] = Task.Factory.StartNew(() =>
                {
                    // Phase 1: Prefix scan assigned range, and copy upper bound to intermediate partials
                    InclusiveScanInPlaceSerial(arr, function, lowerRangeInclusive, upperRangeExclusive, 1);
                    intermediatePartials[rangeNum] = arr[upperRangeExclusive - 1];

                    // Phase 2: One thread only should prefix scan the intermediaries... done implicitly by the barrier
                    phaseBarrier.SignalAndWait();

                    // Phase 3: Incorporate partials
                    if (rangeNum != 0)
                    {
                        for (int j = lowerRangeInclusive; j < upperRangeExclusive; j++)
                        {
                            arr[j] = function(intermediatePartials[rangeNum], arr[j]);
                        }
                    }
                });
            }

            // Wait for all of the tasks to complete
            Task.WaitAll(tasks);
        }
    }
}
