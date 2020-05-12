//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Linq
{
    /// <summary>Extension methods for Parallel LINQ.</summary>
    public static class ParallelLinqExtensions
    {
        /// <summary>Takes the top elements as if they were sorted.</summary>
        /// <typeparam name="TSource">Specifies the type of the elements.</typeparam>
        /// <typeparam name="TKey">Specifies the type of the keys used to compare elements.</typeparam>
        /// <param name="source">The source elements.</param>
        /// <param name="keySelector">A function used to extract a key from each element.</param>
        /// <param name="count">The number of elements to take.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> TakeTop<TSource, TKey>(this ParallelQuery<TSource> source,
            Func<TSource, TKey> keySelector,
            int count)
        {
            // We want to sort in descending order, so we need the opposite of the default comparer
            var comparer = new DescendingDefaultComparer<TKey>();

            // Aggregate, using a sorted list per thread to keep track of the best N elements,
            // then merge those at the end.
            return source.Aggregate(
                () => new SortedTopN<TKey, TSource>(count, comparer),

                (accum, item) =>
                {
                    accum.Add(keySelector(item), item);
                    return accum;
                },

                (accum1, accum2) =>
                {
                    foreach (var item in accum2) accum1.Add(item);
                    return accum1;
                },

                (accum) => accum.Values);
        }

        /// <summary>A comparer that comparers using the inverse of the default comparer.</summary>
        /// <typeparam name="T">Specifies the type being compared.</typeparam>
        private class DescendingDefaultComparer<T> : IComparer<T>
        {
            private static readonly Comparer<T> _defaultComparer = Comparer<T>.Default;
            public int Compare(T x, T y) => _defaultComparer.Compare(y, x);
        }

        /// <summary>Implements a map-reduce operation.</summary>
        /// <typeparam name="TSource">Specifies the type of the source elements.</typeparam>
        /// <typeparam name="TMapped">Specifies the type of the mapped elements.</typeparam>
        /// <typeparam name="TKey">Specifies the type of the element keys.</typeparam>
        /// <typeparam name="TResult">Specifies the type of the results.</typeparam>
        /// <param name="source">The source elements.</param>
        /// <param name="map">A function used to get the target data from a source element.</param>
        /// <param name="keySelector">A function used to get a key from the target data.</param>
        /// <param name="reduce">A function used to reduce a group of elements.</param>
        /// <returns>The result elements of the reductions.</returns>
        public static ParallelQuery<TResult> MapReduce<TSource, TMapped, TKey, TResult>(
            this ParallelQuery<TSource> source,
            Func<TSource, TMapped> map,
            Func<TMapped, TKey> keySelector,
            Func<IGrouping<TKey, TMapped>, TResult> reduce) => source.
                Select(map).
                GroupBy(keySelector).
                Select(reduce);

        /// <summary>Implements a map-reduce operation.</summary>
        /// <typeparam name="TSource">Specifies the type of the source elements.</typeparam>
        /// <typeparam name="TMapped">Specifies the type of the mapped elements.</typeparam>
        /// <typeparam name="TKey">Specifies the type of the element keys.</typeparam>
        /// <typeparam name="TResult">Specifies the type of the results.</typeparam>
        /// <param name="source">The source elements.</param>
        /// <param name="map">A function used to get an enumerable of target data from a source element.</param>
        /// <param name="keySelector">A function used to get a key from target data.</param>
        /// <param name="reduce">A function used to reduce a group of elements to an enumerable of results.</param>
        /// <returns>The result elements of the reductions.</returns>
        public static ParallelQuery<TResult> MapReduce<TSource, TMapped, TKey, TResult>(
            this ParallelQuery<TSource> source,
            Func<TSource, IEnumerable<TMapped>> map,
            Func<TMapped, TKey> keySelector,
            Func<IGrouping<TKey, TMapped>, IEnumerable<TResult>> reduce) => source.
                SelectMany(map).
                GroupBy(keySelector).
                SelectMany(reduce);

        /// <summary>Runs the query and outputs its results into the target collection.</summary>
        /// <typeparam name="TSource">Specifies the type of elements output from the query.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="target">The target collection.</param>
        public static void OutputToProducerConsumerCollection<TSource>(
            this ParallelQuery<TSource> source,
            IProducerConsumerCollection<TSource> target)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            // Store all results into the collection
            source.ForAll(item => target.TryAdd(item));
        }

        /// <summary>This is the method to opt into Parallel LINQ.</summary>
        /// <typeparam name="TSource">Specifies the type of elements provided to the query.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="parallelOptions">The options to use for query processing.</param>
        /// <returns>The source as a ParallelQuery to bind to ParallelEnumerable extension methods.</returns>
        public static ParallelQuery<TSource> AsParallel<TSource>(
            this IEnumerable<TSource> source,
            ParallelLinqOptions parallelOptions)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            // Validate unsupported options
            if (parallelOptions.TaskScheduler != null && parallelOptions.TaskScheduler != TaskScheduler.Default)
            {
                throw new ArgumentException("Parallel LINQ only supports the default TaskScheduler.");
            }

            // First convert to PLINQ
            var result = source.AsParallel();

            // Then apply all of the options as requested...
            if (parallelOptions.Ordered)
            {
                result = result.AsOrdered();
            }
            if (parallelOptions.CancellationToken.CanBeCanceled)
            {
                result = result.WithCancellation(parallelOptions.CancellationToken);
            }
            if (parallelOptions.MaxDegreeOfParallelism >= 1)
            {
                result = result.WithDegreeOfParallelism(parallelOptions.MaxDegreeOfParallelism);
            }
            if (parallelOptions.ExecutionMode != ParallelExecutionMode.Default)
            {
                result = result.WithExecutionMode(parallelOptions.ExecutionMode);
            }
            if (parallelOptions.MergeOptions != ParallelMergeOptions.Default)
            {
                result = result.WithMergeOptions(parallelOptions.MergeOptions);
            }
            return result;
        }
    }
}
