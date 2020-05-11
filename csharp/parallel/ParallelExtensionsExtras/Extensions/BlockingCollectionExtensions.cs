//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace System.Collections.Concurrent
{
    /// <summary>Extension methods for BlockingCollection.</summary>
    public static class BlockingCollectionExtensions
    {
        /// <summary>
        /// Gets a partitioner for a BlockingCollection that consumes and yields the contents of the BlockingCollection.</summary>
        /// <typeparam name="T">Specifies the type of data in the collection.</typeparam>
        /// <param name="collection">The collection for which to create a partitioner.</param>
        /// <returns>A partitioner that completely consumes and enumerates the contents of the collection.</returns>
        /// <remarks>
        /// Using this partitioner with a Parallel.ForEach loop or with PLINQ eliminates the need for those
        /// constructs to do any additional locking.  The only synchronization in place is that used by the
        /// BlockingCollection internally.
        /// </remarks>
        public static Partitioner<T> GetConsumingPartitioner<T>(this BlockingCollection<T> collection) => new BlockingCollectionPartitioner<T>(collection);

        /// <summary>Provides a partitioner that consumes a blocking collection and yields its contents.</summary>
        /// <typeparam name="T">Specifies the type of data in the collection.</typeparam>
        private class BlockingCollectionPartitioner<T> : Partitioner<T>
        {
            /// <summary>The target collection.</summary>
            private readonly BlockingCollection<T> _collection;

            /// <summary>Initializes the partitioner.</summary>
            /// <param name="collection">The collection to be enumerated and consumed.</param>
            internal BlockingCollectionPartitioner(BlockingCollection<T> collection) => _collection = collection ?? throw new ArgumentNullException("collection");

            /// <summary>Gets whether additional partitions can be created dynamically.</summary>
            public override bool SupportsDynamicPartitions => true;

            /// <summary>Partitions the underlying collection into the given number of partitions.</summary>
            /// <param name="partitionCount">The number of partitions to create.</param>
            /// <returns>A list containing partitionCount enumerators.</returns>
            public override IList<IEnumerator<T>> GetPartitions(int partitionCount)
            {
                if (partitionCount < 1) throw new ArgumentOutOfRangeException(nameof(partitionCount));
                var dynamicPartitioner = GetDynamicPartitions();
                return Enumerable.Range(0, partitionCount).Select(_ => dynamicPartitioner.GetEnumerator()).ToArray();
            }

            /// <summary>
            /// Creates an object that can partition the underlying collection into a variable number of partitions.
            /// </summary>
            /// <returns>An object that can create partitions over the underlying data source.</returns>
            public override IEnumerable<T> GetDynamicPartitions() => _collection.GetConsumingEnumerable();
        }

        /// <summary>Adds the contents of an enumerable to the BlockingCollection.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="target">The target BlockingCollection to be augmented.</param>
        /// <param name="source">The source enumerable containing the data to be added.</param>
        /// <param name="completeAddingWhenDone">
        /// Whether to mark the target BlockingCollection as complete for adding when 
        /// all elements of the source enumerable have been transfered.
        /// </param>
        public static void AddFromEnumerable<T>(this BlockingCollection<T> target, IEnumerable<T> source, bool completeAddingWhenDone)
        {
            try { foreach (var item in source) target.Add(item); }
            finally { if (completeAddingWhenDone) target.CompleteAdding(); }
        }

        /// <summary>Adds the contents of an observable to the BlockingCollection.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="target">The target BlockingCollection to be augmented.</param>
        /// <param name="source">The source observable containing the data to be added.</param>
        /// <param name="completeAddingWhenDone">
        /// Whether to mark the target BlockingCollection as complete for adding when 
        /// all elements of the source observable have been transfered.
        /// </param>
        /// <returns>An IDisposable that may be used to cancel the transfer.</returns>
        public static IDisposable AddFromObservable<T>(this BlockingCollection<T> target, IObservable<T> source, bool completeAddingWhenDone)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Subscribe(new DelegateBasedObserver<T>
            (
                onNext: item => target.Add(item),
                onError: error => { if (completeAddingWhenDone) target.CompleteAdding(); },
                onCompleted: () => { if (completeAddingWhenDone) target.CompleteAdding(); }
            ));
        }

        /// <summary>Creates an IProducerConsumerCollection-facade for a BlockingCollection instance.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="collection">The BlockingCollection.</param>
        /// <returns>
        /// An IProducerConsumerCollection that wraps the provided BlockingCollection.
        /// </returns>
        public static IProducerConsumerCollection<T> ToProducerConsumerCollection<T>(
            this BlockingCollection<T> collection) => ToProducerConsumerCollection(collection, Timeout.Infinite);

        /// <summary>Creates an IProducerConsumerCollection-facade for a BlockingCollection instance.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="collection">The BlockingCollection.</param>
        /// <param name="millisecondsTimeout">-1 for infinite blocking add and take operations. 0 for non-blocking, 1 or greater for blocking with timeout.</param>
        /// <returns>An IProducerConsumerCollection that wraps the provided BlockingCollection.</returns>
        public static IProducerConsumerCollection<T> ToProducerConsumerCollection<T>(
            this BlockingCollection<T> collection, int millisecondsTimeout) => new ProducerConsumerWrapper<T>(collection, millisecondsTimeout, new CancellationToken());

        /// <summary>Creates an IProducerConsumerCollection-facade for a BlockingCollection instance.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        /// <param name="collection">The BlockingCollection.</param>
        /// <param name="millisecondsTimeout">-1 for infinite blocking add and take operations. 0 for non-blocking, 1 or greater for blocking with timeout.</param>
        /// <param name="cancellationToken">The CancellationToken to use for any blocking operations.</param>
        /// <returns>An IProducerConsumerCollection that wraps the provided BlockingCollection.</returns>
        public static IProducerConsumerCollection<T> ToProducerConsumerCollection<T>(
            this BlockingCollection<T> collection, int millisecondsTimeout, CancellationToken cancellationToken) => new ProducerConsumerWrapper<T>(collection, millisecondsTimeout, cancellationToken);

        /// <summary>Provides a producer-consumer collection facade for a BlockingCollection.</summary>
        /// <typeparam name="T">Specifies the type of the elements in the collection.</typeparam>
        internal sealed class ProducerConsumerWrapper<T> : IProducerConsumerCollection<T>
        {
            private readonly BlockingCollection<T> _collection;
            private readonly int _millisecondsTimeout;
            private readonly CancellationToken _cancellationToken;

            public ProducerConsumerWrapper(
                BlockingCollection<T> collection, int millisecondsTimeout, CancellationToken cancellationToken)
            {
                if (millisecondsTimeout < -1) throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
                _collection = collection ?? throw new ArgumentNullException(nameof(collection));
                _millisecondsTimeout = millisecondsTimeout;
                _cancellationToken = cancellationToken;
            }

            public void CopyTo(T[] array, int index) => _collection.CopyTo(array, index);

            public T[] ToArray() => _collection.ToArray();

            public bool TryAdd(T item) => _collection.TryAdd(item, _millisecondsTimeout, _cancellationToken);

            public bool TryTake(out T item) => _collection.TryTake(out item, _millisecondsTimeout, _cancellationToken);

            public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_collection).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void CopyTo(Array array, int index) => ((ICollection)_collection).CopyTo(array, index);

            public int Count => _collection.Count;

            public bool IsSynchronized => ((ICollection)_collection).IsSynchronized;

            public object SyncRoot => ((ICollection)_collection).SyncRoot;
        }
    }
}
