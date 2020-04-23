//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace System.Threading.Async
{
    /// <summary>Provides an asynchronous producer/consumer collection.</summary>
    [DebuggerDisplay("Count={CurrentCount}")]
    public sealed class AsyncProducerConsumerCollection<T> : IDisposable
    {
        /// <summary>Asynchronous semaphore used to keep track of asynchronous work.</summary>
        private AsyncSemaphore _semaphore = new AsyncSemaphore();
        /// <summary>The data stored in the collection.</summary>
        private readonly IProducerConsumerCollection<T> _collection;

        /// <summary>Initializes the asynchronous producer/consumer collection to store data in a first-in-first-out (FIFO) order.</summary>
        public AsyncProducerConsumerCollection() : this(new ConcurrentQueue<T>()) { }

        /// <summary>Initializes the asynchronous producer/consumer collection.</summary>
        /// <param name="collection">The underlying collection to use to store data.</param>
        public AsyncProducerConsumerCollection(IProducerConsumerCollection<T> collection) =>
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));

        /// <summary>Adds an element to the collection.</summary>
        /// <param name="item">The item to be added.</param>
        public void Add(T item)
        {
            if (_collection.TryAdd(item)) _semaphore.Release();
            else throw new InvalidOperationException("Invalid collection");
        }

        /// <summary>Takes an element from the collection asynchronously.</summary>
        /// <returns>A Task that represents the element removed from the collection.</returns>
        public Task<T> Take() =>
            _semaphore.WaitAsync().ContinueWith(_ =>
            {
                if (!_collection.TryTake(out T result)) throw new InvalidOperationException("Invalid collection");
                return result;
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);

        /// <summary>Gets the number of elements in the collection.</summary>
        public int Count => _collection.Count;

        /// <summary>Disposes of the collection.</summary>
        public void Dispose()
        {
            _semaphore?.Dispose();
            _semaphore = null;
        }
    }
}
