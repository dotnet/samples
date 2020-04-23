//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace System.Threading.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        #region Int32, No Options
        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            int fromInclusive, int toExclusive,
            Action<int, int> body) => ForRange(fromInclusive, toExclusive, s_defaultParallelOptions, body);

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            int fromInclusive, int toExclusive,
            Action<int, int, ParallelLoopState> body) => ForRange(fromInclusive, toExclusive, s_defaultParallelOptions, body);

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="localInit">The function delegate that returns the initial state of the local data for each thread.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <param name="localFinally">The delegate that performs a final action on the local state of each thread.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange<TLocal>(
            int fromInclusive, int toExclusive,
            Func<TLocal> localInit,
            Func<int, int, ParallelLoopState, TLocal, TLocal> body,
            Action<TLocal> localFinally) => ForRange(fromInclusive, toExclusive, s_defaultParallelOptions,
                localInit, body, localFinally);
        #endregion

        #region Int64, No Options
        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            long fromInclusive, long toExclusive,
            Action<long, long> body) =>
            ForRange(fromInclusive, toExclusive, s_defaultParallelOptions, body);

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            long fromInclusive, long toExclusive,
            Action<long, long, ParallelLoopState> body) =>
            ForRange(fromInclusive, toExclusive, s_defaultParallelOptions, body);

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="localInit">The function delegate that returns the initial state of the local data for each thread.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <param name="localFinally">The delegate that performs a final action on the local state of each thread.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange<TLocal>(
            long fromInclusive, long toExclusive,
            Func<TLocal> localInit,
            Func<long, long, ParallelLoopState, TLocal, TLocal> body,
            Action<TLocal> localFinally) =>
            ForRange(fromInclusive, toExclusive, s_defaultParallelOptions, localInit, body, localFinally);
        #endregion

        #region Int32, Parallel Options
        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            int fromInclusive, int toExclusive,
            ParallelOptions parallelOptions,
            Action<int, int> body)
        {
            if (parallelOptions == null) throw new ArgumentNullException(nameof(parallelOptions));
            if (body == null) throw new ArgumentNullException(nameof(body));

            return Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, range =>
            {
                body(range.Item1, range.Item2);
            });
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            int fromInclusive, int toExclusive,
            ParallelOptions parallelOptions,
            Action<int, int, ParallelLoopState> body)
        {
            if (parallelOptions == null) throw new ArgumentNullException(nameof(parallelOptions));
            if (body == null) throw new ArgumentNullException(nameof(body));

            return Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, (range, loopState) =>
            {
                body(range.Item1, range.Item2, loopState);
            });
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="localInit">The function delegate that returns the initial state of the local data for each thread.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <param name="localFinally">The delegate that performs a final action on the local state of each thread.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange<TLocal>(
            int fromInclusive, int toExclusive,
            ParallelOptions parallelOptions,
            Func<TLocal> localInit,
            Func<int, int, ParallelLoopState, TLocal, TLocal> body,
            Action<TLocal> localFinally)
        {
            if (parallelOptions == null) throw new ArgumentNullException(nameof(parallelOptions));
            if (localInit == null) throw new ArgumentNullException(nameof(localInit));
            if (body == null) throw new ArgumentNullException(nameof(body));
            if (localFinally == null) throw new ArgumentNullException(nameof(localFinally));

            return Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, localInit, (range, loopState, x) =>
            {
                return body(range.Item1, range.Item2, loopState, x);
            }, localFinally);
        }
        #endregion

        #region Int64, Parallel Options
        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            long fromInclusive, long toExclusive,
            ParallelOptions parallelOptions,
            Action<long, long> body)
        {
            if (parallelOptions == null) throw new ArgumentNullException(nameof(parallelOptions));
            if (body == null) throw new ArgumentNullException(nameof(body));

            return Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, range =>
            {
                body(range.Item1, range.Item2);
            });
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange(
            long fromInclusive, long toExclusive,
            ParallelOptions parallelOptions,
            Action<long, long, ParallelLoopState> body)
        {
            if (parallelOptions == null) throw new ArgumentNullException(nameof(parallelOptions));
            if (body == null) throw new ArgumentNullException(nameof(body));

            return Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, (range, loopState) =>
            {
                body(range.Item1, range.Item2, loopState);
            });
        }

        /// <summary>Executes a for loop over ranges in which iterations may run in parallel. </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="localInit">The function delegate that returns the initial state of the local data for each thread.</param>
        /// <param name="body">The delegate that is invoked once per range.</param>
        /// <param name="localFinally">The delegate that performs a final action on the local state of each thread.</param>
        /// <returns>A ParallelLoopResult structure that contains information on what portion of the loop completed.</returns>
        public static ParallelLoopResult ForRange<TLocal>(
            long fromInclusive, long toExclusive,
            ParallelOptions parallelOptions,
            Func<TLocal> localInit,
            Func<long, long, ParallelLoopState, TLocal, TLocal> body,
            Action<TLocal> localFinally)
        {
            if (parallelOptions == null) throw new ArgumentNullException(nameof(parallelOptions));
            if (localInit == null) throw new ArgumentNullException(nameof(localInit));
            if (body == null) throw new ArgumentNullException(nameof(body));
            if (localFinally == null) throw new ArgumentNullException(nameof(localFinally));

            return Parallel.ForEach(Partitioner.Create(fromInclusive, toExclusive), parallelOptions, localInit, (range, loopState, x) =>
            {
                return body(range.Item1, range.Item2, loopState, x);
            }, localFinally);
        }
        #endregion
    }
}
