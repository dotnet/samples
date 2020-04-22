//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace System.Collections.Concurrent
{
    /// <summary>
    /// Provides a thread-safe, concurrent collection for use with data binding.
    /// </summary>
    /// <typeparam name="T">Specifies the type of the elements in this collection.</typeparam>
    [DebuggerDisplay("Count={Count}")]
    [DebuggerTypeProxy(typeof(IProducerConsumerCollection_DebugView<>))]
    public class ObservableConcurrentCollection<T> :
        ProducerConsumerCollectionBase<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly SynchronizationContext _context;

        /// <summary>
        /// Initializes an instance of the ObservableConcurrentCollection class with an underlying
        /// queue data structure.
        /// </summary>
        public ObservableConcurrentCollection() : this(new ConcurrentQueue<T>()) { }

        /// <summary>
        /// Initializes an instance of the ObservableConcurrentCollection class with the specified
        /// collection as the underlying data structure.
        /// </summary>
        public ObservableConcurrentCollection(IProducerConsumerCollection<T> collection) : base(collection) => _context = AsyncOperationManager.SynchronizationContext;

        /// <summary>Event raised when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>Event raised when a property on the collection changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary.
        /// </summary>
        private void NotifyObserversOfChange()
        {
            var collectionHandler = CollectionChanged;
            var propertyHandler = PropertyChanged;

            _context.Post(s =>
            {
                collectionHandler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                propertyHandler?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }, null);
        }

        protected override bool TryAdd(T item)
        {
            // Try to add the item to the underlying collection.  If we were able to,
            // notify any listeners.
            bool result = base.TryAdd(item);
            if (result) NotifyObserversOfChange();
            return result;
        }


        protected override bool TryTake(out T item)
        {
            // Try to remove an item from the underlying collection.  If we were able to,
            // notify any listeners.
            bool result = base.TryTake(out item);
            if (result) NotifyObserversOfChange();
            return result;
        }
    }
}
