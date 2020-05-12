//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Collections.Concurrent.Partitioners
{
    /// <summary>Partitions a data source one item at a time.</summary>
    public static class SingleItemPartitioner
    {
        /// <summary>Creates a partitioner for an enumerable that partitions it one item at a time.</summary>
        /// <typeparam name="T">Specifies the type of data contained in the enumerable.</typeparam>
        /// <param name="source">The source enumerable to be partitioned.</param>
        /// <returns>The partitioner.</returns>
        public static OrderablePartitioner<T> Create<T>(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            else if (source is IList<T>) return new SingleItemIListPartitioner<T>((IList<T>)source);
            else return new SingleItemEnumerablePartitioner<T>(source);
        }

        /// <summary>Partitions an enumerable one item at a time.</summary>
        /// <typeparam name="T">Specifies the type of data contained in the list.</typeparam>
        private sealed class SingleItemEnumerablePartitioner<T> : OrderablePartitioner<T>
        {
            /// <summary>The enumerable to be partitioned.</summary>
            private readonly IEnumerable<T> _source;

            /// <summary>Initializes the partitioner.</summary>
            /// <param name="source">The enumerable to be partitioned.</param>
            internal SingleItemEnumerablePartitioner(IEnumerable<T> source) : base(true, false, true) =>
                _source = source;

            /// <summary>Gets whether this partitioner supports dynamic partitioning (it does).</summary>
            public override bool SupportsDynamicPartitions => true;

            public override IList<IEnumerator<KeyValuePair<long, T>>> GetOrderablePartitions(int partitionCount)
            {
                if (partitionCount < 1) throw new ArgumentOutOfRangeException(nameof(partitionCount));
                var dynamicPartitioner = new DynamicGenerator(_source.GetEnumerator(), false);
                return (from i in Enumerable.Range(0, partitionCount) select dynamicPartitioner.GetEnumerator()).ToList();
            }

            /// <summary>Gets a list of the specified static number of partitions.</summary>
            /// <param name="partitionCount">The static number of partitions to create.</param>
            /// <returns>The list of created partitions ready to be iterated.</returns>
            public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions() => new DynamicGenerator(_source.GetEnumerator(), true);

            /// <summary>Dynamically generates a partitions on a shared enumerator.</summary>
            private class DynamicGenerator : IEnumerable<KeyValuePair<long, T>>, IDisposable
            {
                /// <summary>The source enumerator shared amongst all partitions.</summary>
                private readonly IEnumerator<T> _sharedEnumerator;
                /// <summary>The next available position to be yielded.</summary>
                private long _nextAvailablePosition;
                /// <summary>The number of partitions remaining to be disposed, potentially including this dynamic generator.</summary>
                private int _remainingPartitions;
                /// <summary>Whether this dynamic partitioner has been disposed.</summary>
                private bool _disposed;

                /// <summary>Initializes the dynamic generator.</summary>
                /// <param name="sharedEnumerator">The enumerator shared by all partitions.</param>
                /// <param name="requiresDisposal">Whether this generator will be disposed.</param>
                public DynamicGenerator(IEnumerator<T> sharedEnumerator, bool requiresDisposal)
                {
                    _sharedEnumerator = sharedEnumerator;
                    _nextAvailablePosition = -1;
                    _remainingPartitions = requiresDisposal ? 1 : 0;
                }

                /// <summary>Closes the shared enumerator if all other partitions have completed.</summary>
                void IDisposable.Dispose()
                {
                    if (!_disposed && Interlocked.Decrement(ref _remainingPartitions) == 0)
                    {
                        _disposed = true;
                        _sharedEnumerator.Dispose();
                    }
                }

                /// <summary>Increments the number of partitions in use and returns a new partition.</summary>
                /// <returns>The new partition.</returns>
                public IEnumerator<KeyValuePair<long, T>> GetEnumerator()
                {
                    Interlocked.Increment(ref _remainingPartitions);
                    return GetEnumeratorCore();
                }
                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                /// <summary>Creates a partition.</summary>
                /// <returns>The new partition.</returns>
                private IEnumerator<KeyValuePair<long, T>> GetEnumeratorCore()
                {
                    try
                    {
                        while (true)
                        {
                            T nextItem;
                            long position;
                            lock (_sharedEnumerator)
                            {
                                if (_sharedEnumerator.MoveNext())
                                {
                                    position = _nextAvailablePosition++;
                                    nextItem = _sharedEnumerator.Current;
                                }
                                else yield break;
                            }
                            yield return new KeyValuePair<long, T>(position, nextItem);
                        }
                    }
                    finally { if (Interlocked.Decrement(ref _remainingPartitions) == 0) _sharedEnumerator.Dispose(); }
                }
            }
        }

        /// <summary>Partitions a list one item at a time.</summary>
        /// <typeparam name="T">Specifies the type of data contained in the list.</typeparam>
        private sealed class SingleItemIListPartitioner<T> : OrderablePartitioner<T>
        {
            /// <summary>The list to be partitioned.</summary>
            private readonly IList<T> _source;

            /// <summary>Initializes the partitioner.</summary>
            /// <param name="source">The list to be partitioned.</param>
            internal SingleItemIListPartitioner(IList<T> source) : base(true, false, true) => _source = source;

            /// <summary>Gets whether this partitioner supports dynamic partitioning (it does).</summary>
            public override bool SupportsDynamicPartitions => true;

            /// <summary>Gets a list of the specified static number of partitions.</summary>
            /// <param name="partitionCount">The static number of partitions to create.</param>
            /// <returns>The list of created partitions ready to be iterated.</returns>
            public override IList<IEnumerator<KeyValuePair<long, T>>> GetOrderablePartitions(int partitionCount)
            {
                if (partitionCount < 1) throw new ArgumentOutOfRangeException(nameof(partitionCount));
                var dynamicPartitioner = GetOrderableDynamicPartitions();
                return (from i in Enumerable.Range(0, partitionCount) select dynamicPartitioner.GetEnumerator()).ToList();
            }

            /// <summary>Creates a dynamic partitioner for creating a dynamic number of partitions.</summary>
            /// <returns>The dynamic partitioner.</returns>
            public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions() => GetOrderableDynamicPartitionsCore(_source, new StrongBox<int>(0));

            /// <summary>An enumerable that creates individual enumerators that all work together to partition the list.</summary>
            /// <param name="source">The list being partitioned.</param>
            /// <param name="nextIteration">An integer shared between partitions denoting the next available index in the source.</param>
            /// <returns>An enumerable that generates enumerators which participate in partitioning the list.</returns>
            private static IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitionsCore(IList<T> source, StrongBox<int> nextIteration)
            {
                while (true)
                {
                    var iteration = Interlocked.Increment(ref nextIteration.Value) - 1;
                    if (iteration >= 0 && iteration < source.Count) yield return new KeyValuePair<long, T>(iteration, source[iteration]);
                    else yield break;
                }
            }
        }
    }
}
