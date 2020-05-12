//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Threading;

namespace System.Collections.Concurrent.Partitioners
{
    /// <summary>
    /// Partitions an enumerable into chunks based on user-supplied criteria.
    /// </summary>
    public static class ChunkPartitioner
    {
        /// <summary>Creates a partitioner that chooses the next chunk size based on a user-supplied function.</summary>
        /// <typeparam name="TSource">The type of the data being partitioned.</typeparam>
        /// <param name="source">The data being partitioned.</param>
        /// <param name="nextChunkSizeFunc">A function that determines the next chunk size based on the
        /// previous chunk size.</param>
        /// <returns>A partitioner.</returns>
        public static OrderablePartitioner<TSource> Create<TSource>(
            IEnumerable<TSource> source, Func<int, int> nextChunkSizeFunc) =>
            new ChunkPartitioner<TSource>(source, nextChunkSizeFunc);

        /// <summary>Creates a partitioner that always uses a user-specified chunk size.</summary>
        /// <typeparam name="TSource">The type of the data being partitioned.</typeparam>
        /// <param name="source">The data being partitioned.</param>
        /// <param name="chunkSize">The chunk size to be used.</param>
        /// <returns>A partitioner.</returns>
        public static OrderablePartitioner<TSource> Create<TSource>(
            IEnumerable<TSource> source, int chunkSize) =>
            new ChunkPartitioner<TSource>(source, chunkSize);

        /// <summary>Creates a partitioner that chooses chunk sizes between the user-specified min and max.</summary>
        /// <typeparam name="TSource">The type of the data being partitioned.</typeparam>
        /// <param name="source">The data being partitioned.</param>
        /// <param name="minChunkSize">The minimum chunk size to use.</param>
        /// <param name="maxChunkSize">The maximum chunk size to use.</param>
        /// <returns>A partitioner.</returns>
        public static OrderablePartitioner<TSource> Create<TSource>(
            IEnumerable<TSource> source, int minChunkSize, int maxChunkSize) =>
            new ChunkPartitioner<TSource>(source, minChunkSize, maxChunkSize);
    }

    /// <summary>
    /// Partitions an enumerable into chunks based on user-supplied criteria.
    /// </summary>
    internal sealed class ChunkPartitioner<T> : OrderablePartitioner<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly Func<int, int> _nextChunkSizeFunc;

        public ChunkPartitioner(IEnumerable<T> source, Func<int, int> nextChunkSizeFunc)
            // The keys will be ordered across both individual partitions and across partitions,
            // and they will be normalized.
            : base(true, true, true)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _nextChunkSizeFunc = nextChunkSizeFunc ?? throw new ArgumentNullException(nameof(nextChunkSizeFunc));
        }

        public ChunkPartitioner(IEnumerable<T> source, int chunkSize)
            : this(source, prev => chunkSize) // uses a function that always returns the specified chunk size
        {
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
        }

        public ChunkPartitioner(IEnumerable<T> source, int minChunkSize, int maxChunkSize) :
            this(source, CreateFuncFromMinAndMax(minChunkSize, maxChunkSize)) // uses a function that grows from min to max
        {
            if (minChunkSize <= 0 ||
                minChunkSize > maxChunkSize) throw new ArgumentOutOfRangeException(nameof(minChunkSize));
        }

        private static Func<int, int> CreateFuncFromMinAndMax(int minChunkSize, int maxChunkSize) =>
            // Create a function that returns exponentially growing chunk sizes between minChunkSize and maxChunkSize
            delegate (int prev)
            {
                if (prev < minChunkSize) return minChunkSize;
                if (prev >= maxChunkSize) return maxChunkSize;
                int next = prev * 2;
                if (next >= maxChunkSize || next < 0) return maxChunkSize;
                return next;
            };

        /// <summary>
        /// Partitions the underlying collection into the specified number of orderable partitions.
        /// </summary>
        /// <param name="partitionCount">The number of partitions to create.</param>
        /// <returns>An object that can create partitions over the underlying data source.</returns>
        public override IList<IEnumerator<KeyValuePair<long, T>>> GetOrderablePartitions(int partitionCount)
        {
            // Validate parameters
            if (partitionCount <= 0) throw new ArgumentOutOfRangeException(nameof(partitionCount));

            // Create an array of dynamic partitions and return them
            var partitions = new IEnumerator<KeyValuePair<long, T>>[partitionCount];
            var dynamicPartitions = GetOrderableDynamicPartitions(true);
            for (int i = 0; i < partitionCount; i++)
            {
                partitions[i] = dynamicPartitions.GetEnumerator(); // Create and store the next partition
            }
            return partitions;
        }

        /// <summary>Gets whether additional partitions can be created dynamically.</summary>
        public override bool SupportsDynamicPartitions => true;

        /// <summary>
        /// Creates an object that can partition the underlying collection into a variable number of
        /// partitions.
        /// </summary>
        /// <returns>
        /// An object that can create partitions over the underlying data source.
        /// </returns>
        public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions() => new EnumerableOfEnumerators(this, false);

        private IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions(bool referenceCountForDisposal) => new EnumerableOfEnumerators(this, referenceCountForDisposal);

        // The object used to dynamically create partitions
        private class EnumerableOfEnumerators : IEnumerable<KeyValuePair<long, T>>, IDisposable
        {
            private readonly ChunkPartitioner<T> _parentPartitioner;
            private readonly IEnumerator<T> _sharedEnumerator;
            private long _nextSharedIndex;
            private int _activeEnumerators;
            private bool _noMoreElements;
            private bool _disposed;
            private readonly bool _referenceCountForDisposal;

            public EnumerableOfEnumerators(ChunkPartitioner<T> parentPartitioner, bool referenceCountForDisposal)
            {

                // Store the data, including creating an enumerator from the underlying data source
                _parentPartitioner = parentPartitioner ?? throw new ArgumentNullException(nameof(parentPartitioner));
                _sharedEnumerator = parentPartitioner._source.GetEnumerator();
                _nextSharedIndex = -1;
                _referenceCountForDisposal = referenceCountForDisposal;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<KeyValuePair<long, T>> GetEnumerator()
            {
                if (_referenceCountForDisposal)
                {
                    Interlocked.Increment(ref _activeEnumerators);
                }
                return new Enumerator(this);
            }

            private void DisposeEnumerator()
            {
                if (_referenceCountForDisposal)
                {
                    if (Interlocked.Decrement(ref _activeEnumerators) == 0)
                    {
                        _sharedEnumerator.Dispose();
                    }
                }
            }

            private class Enumerator : IEnumerator<KeyValuePair<long, T>>
            {
                private readonly EnumerableOfEnumerators _parentEnumerable;
                private readonly List<KeyValuePair<long, T>> _currentChunk = new List<KeyValuePair<long, T>>();
                private int _currentChunkCurrentIndex;
                private int _lastRequestedChunkSize;
                private bool _disposed;

                public Enumerator(EnumerableOfEnumerators parentEnumerable) =>
                    _parentEnumerable = parentEnumerable ?? throw new ArgumentNullException("parentEnumerable");

                public bool MoveNext()
                {
                    if (_disposed) throw new ObjectDisposedException(GetType().Name);

                    // Move to the next cached element. If we already retrieved a chunk and if there's still
                    // data left in it, just use the next item from it.
                    ++_currentChunkCurrentIndex;
                    if (_currentChunkCurrentIndex >= 0 &&
                        _currentChunkCurrentIndex < _currentChunk.Count) return true;

                    // First, figure out how much new data we want. The previous requested chunk size is used
                    // as input to figure out how much data the user now wants.  The initial chunk size
                    // supplied is 0 so that the user delegate is made aware that this is the initial request
                    // such that it can select the initial chunk size on first request.
                    int nextChunkSize = _parentEnumerable._parentPartitioner._nextChunkSizeFunc(_lastRequestedChunkSize);
                    if (nextChunkSize <= 0) throw new InvalidOperationException(
                        "Invalid chunk size requested: chunk sizes must be positive.");
                    _lastRequestedChunkSize = nextChunkSize;

                    // Reset the list
                    _currentChunk.Clear();
                    _currentChunkCurrentIndex = 0;
                    if (nextChunkSize > _currentChunk.Capacity) _currentChunk.Capacity = nextChunkSize;

                    // Try to grab the next chunk of data
                    lock (_parentEnumerable._sharedEnumerator)
                    {
                        // If we've already discovered that no more elements exist (and we've gotten this
                        // far, which means we don't have any elements cached), we're done.
                        if (_parentEnumerable._noMoreElements) return false;

                        // Get another chunk
                        for (int i = 0; i < nextChunkSize; i++)
                        {
                            // If there are no more elements to be retrieved from the shared enumerator, mark
                            // that so that other partitions don't have to check again. Return whether we
                            // were able to retrieve any data at all.
                            if (!_parentEnumerable._sharedEnumerator.MoveNext())
                            {
                                _parentEnumerable._noMoreElements = true;
                                return _currentChunk.Count > 0;
                            }

                            ++_parentEnumerable._nextSharedIndex;
                            _currentChunk.Add(new KeyValuePair<long, T>(
                                _parentEnumerable._nextSharedIndex,
                                _parentEnumerable._sharedEnumerator.Current));
                        }
                    }

                    // We got at least some data
                    return true;
                }

                public KeyValuePair<long, T> Current
                {
                    get
                    {
                        if (_currentChunkCurrentIndex >= _currentChunk.Count)
                        {
                            throw new InvalidOperationException("There is no current item.");
                        }
                        return _currentChunk[_currentChunkCurrentIndex];
                    }
                }

                public void Dispose()
                {
                    if (!_disposed)
                    {
                        _parentEnumerable.DisposeEnumerator();
                        _disposed = true;
                    }
                }

                object IEnumerator.Current => Current;
                public void Reset() => throw new NotSupportedException();
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    if (!_referenceCountForDisposal) _sharedEnumerator.Dispose();
                    _disposed = true;
                }
            }
        }
    }
}
