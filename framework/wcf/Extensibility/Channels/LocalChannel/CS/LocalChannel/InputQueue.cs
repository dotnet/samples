//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Samples.LocalChannel
{

    sealed class InputQueue<T> : IDisposable where T : class
    {
        static WaitCallback completeOutstandingReadersCallback;
        static WaitCallback completeWaitersFalseCallback;
        static WaitCallback completeWaitersTrueCallback;
        static WaitCallback onDispatchCallback;
        static WaitCallback onInvokeDequeuedCallback;

        QueueState queueState;

        ItemQueue itemQueue;

        Queue<IQueueReader> readerQueue;

        List<IQueueWaiter> waiterList;

        public InputQueue()
        {
            this.itemQueue = new ItemQueue();
            this.readerQueue = new Queue<IQueueReader>();
            this.waiterList = new List<IQueueWaiter>();
            this.queueState = QueueState.Open;
        }

        public InputQueue(Func<Action<AsyncCallback, IAsyncResult>> asyncCallbackGenerator)
            : this()
        {
            AsyncCallbackGenerator = asyncCallbackGenerator;
        }

        public int PendingCount
        {
            get
            {
                lock (ThisLock)
                {
                    return this.itemQueue.ItemCount;
                }
            }
        }

        public Action<T> DisposeItemCallback
        {
            get;
            set;
        }

        Func<Action<AsyncCallback, IAsyncResult>> AsyncCallbackGenerator
        {
            get;
            set;
        }

        object ThisLock
        {
            get { return this.itemQueue; }
        }

        public IAsyncResult BeginDequeue(TimeSpan timeout, AsyncCallback callback, object state)
        {
            Item item = default(Item);

            lock (ThisLock)
            {
                if (queueState == QueueState.Open)
                {
                    if (itemQueue.HasAvailableItem)
                    {
                        item = itemQueue.DequeueAvailableItem();
                    }
                    else
                    {
                        AsyncQueueReader reader = new AsyncQueueReader(this, timeout, callback, state);
                        readerQueue.Enqueue(reader);
                        return reader;
                    }
                }
                else if (queueState == QueueState.Shutdown)
                {
                    if (itemQueue.HasAvailableItem)
                    {
                        item = itemQueue.DequeueAvailableItem();
                    }
                    else if (itemQueue.HasAnyItem)
                    {
                        AsyncQueueReader reader = new AsyncQueueReader(this, timeout, callback, state);
                        readerQueue.Enqueue(reader);
                        return reader;
                    }
                }
            }

            InvokeDequeuedCallback(item.DequeuedCallback);
            return new CompletedAsyncResult<T>(item.GetValue(), callback, state);
        }

        public IAsyncResult BeginWaitForItem(TimeSpan timeout, AsyncCallback callback, object state)
        {
            lock (ThisLock)
            {
                if (queueState == QueueState.Open)
                {
                    if (!itemQueue.HasAvailableItem)
                    {
                        AsyncQueueWaiter waiter = new AsyncQueueWaiter(timeout, callback, state);
                        waiterList.Add(waiter);
                        return waiter;
                    }
                }
                else if (queueState == QueueState.Shutdown)
                {
                    if (!itemQueue.HasAvailableItem && itemQueue.HasAnyItem)
                    {
                        AsyncQueueWaiter waiter = new AsyncQueueWaiter(timeout, callback, state);
                        waiterList.Add(waiter);
                        return waiter;
                    }
                }
            }

            return new CompletedAsyncResult<bool>(true, callback, state);
        }

        public void Close()
        {
            Dispose();
        }

        public T Dequeue(TimeSpan timeout)
        {
            T value;

            if (!this.Dequeue(timeout, out value))
            {
                throw new TimeoutException(ExceptionMessages.TimeoutInputQueueDequeue);
            }

            return value;
        }

        public bool Dequeue(TimeSpan timeout, out T value)
        {
            WaitQueueReader reader = null;
            Item item = new Item();

            lock (ThisLock)
            {
                if (queueState == QueueState.Open)
                {
                    if (itemQueue.HasAvailableItem)
                    {
                        item = itemQueue.DequeueAvailableItem();
                    }
                    else
                    {
                        reader = new WaitQueueReader(this);
                        readerQueue.Enqueue(reader);
                    }
                }
                else if (queueState == QueueState.Shutdown)
                {
                    if (itemQueue.HasAvailableItem)
                    {
                        item = itemQueue.DequeueAvailableItem();
                    }
                    else if (itemQueue.HasAnyItem)
                    {
                        reader = new WaitQueueReader(this);
                        readerQueue.Enqueue(reader);
                    }
                    else
                    {
                        value = default(T);
                        return true;
                    }
                }
                else // queueState == QueueState.Closed
                {
                    value = default(T);
                    return true;
                }
            }

            if (reader != null)
            {
                return reader.Wait(timeout, out value);
            }
            else
            {
                InvokeDequeuedCallback(item.DequeuedCallback);
                value = item.GetValue();
                return true;
            }
        }

        public void Dispatch()
        {
            IQueueReader reader = null;
            Item item = new Item();
            IQueueReader[] outstandingReaders = null;
            IQueueWaiter[] waiters = null;
            bool itemAvailable = true;

            lock (ThisLock)
            {
                itemAvailable = !((queueState == QueueState.Closed) || (queueState == QueueState.Shutdown));
                this.GetWaiters(out waiters);

                if (queueState != QueueState.Closed)
                {
                    itemQueue.MakePendingItemAvailable();

                    if (readerQueue.Count > 0)
                    {
                        item = itemQueue.DequeueAvailableItem();
                        reader = readerQueue.Dequeue();

                        if (queueState == QueueState.Shutdown && readerQueue.Count > 0 && itemQueue.ItemCount == 0)
                        {
                            outstandingReaders = new IQueueReader[readerQueue.Count];
                            readerQueue.CopyTo(outstandingReaders, 0);
                            readerQueue.Clear();

                            itemAvailable = false;
                        }
                    }
                }
            }

            if (outstandingReaders != null)
            {
                if (completeOutstandingReadersCallback == null)
                {
                    completeOutstandingReadersCallback = new WaitCallback(CompleteOutstandingReadersCallback);
                }

                ThreadPool.QueueUserWorkItem(completeOutstandingReadersCallback, outstandingReaders);
            }

            if (waiters != null)
            {
                CompleteWaitersLater(itemAvailable, waiters);
            }

            if (reader != null)
            {
                InvokeDequeuedCallback(item.DequeuedCallback);
                reader.Set(item);
            }
        }

        public bool EndDequeue(IAsyncResult result, out T value)
        {
            CompletedAsyncResult<T> typedResult = result as CompletedAsyncResult<T>;

            if (typedResult != null)
            {
                value = CompletedAsyncResult<T>.End(result);
                return true;
            }

            return AsyncQueueReader.End(result, out value);
        }

        public T EndDequeue(IAsyncResult result)
        {
            T value;

            if (!this.EndDequeue(result, out value))
            {
                throw new TimeoutException();
            }

            return value;
        }

        public bool EndWaitForItem(IAsyncResult result)
        {
            CompletedAsyncResult<bool> typedResult = result as CompletedAsyncResult<bool>;
            if (typedResult != null)
            {
                return CompletedAsyncResult<bool>.End(result);
            }

            return AsyncQueueWaiter.End(result);
        }

        public void EnqueueAndDispatch(T item)
        {
            EnqueueAndDispatch(item, null);
        }

        // dequeuedCallback is called as an item is dequeued from the InputQueue.  The 
        // InputQueue lock is not held during the callback.  However, the user code will
        // not be notified of the item being available until the callback returns.  If you
        // are not sure if the callback will block for a long time, then first call 
        // IOThreadScheduler.ScheduleCallback to get to a "safe" thread.
        public void EnqueueAndDispatch(T item, Action dequeuedCallback)
        {
            EnqueueAndDispatch(item, dequeuedCallback, true);
        }

        public void EnqueueAndDispatch(Exception exception, Action dequeuedCallback, bool canDispatchOnThisThread)
        {
            EnqueueAndDispatch(new Item(exception, dequeuedCallback), canDispatchOnThisThread);
        }

        public void EnqueueAndDispatch(T item, Action dequeuedCallback, bool canDispatchOnThisThread)
        {
            EnqueueAndDispatch(new Item(item, dequeuedCallback), canDispatchOnThisThread);
        }

        public bool EnqueueWithoutDispatch(T item, Action dequeuedCallback)
        {
            return EnqueueWithoutDispatch(new Item(item, dequeuedCallback));
        }

        public bool EnqueueWithoutDispatch(Exception exception, Action dequeuedCallback)
        {
            return EnqueueWithoutDispatch(new Item(exception, dequeuedCallback));
        }


        public void Shutdown()
        {
            this.Shutdown(null);
        }

        // Don't let any more items in. Differs from Close in that we keep around
        // existing items in our itemQueue for possible future calls to Dequeue
        public void Shutdown(Func<Exception> pendingExceptionGenerator)
        {
            IQueueReader[] outstandingReaders = null;
            lock (ThisLock)
            {
                if (queueState == QueueState.Shutdown)
                {
                    return;
                }

                if (queueState == QueueState.Closed)
                {
                    return;
                }

                this.queueState = QueueState.Shutdown;

                if (readerQueue.Count > 0 && this.itemQueue.ItemCount == 0)
                {
                    outstandingReaders = new IQueueReader[readerQueue.Count];
                    readerQueue.CopyTo(outstandingReaders, 0);
                    readerQueue.Clear();
                }
            }

            if (outstandingReaders != null)
            {
                for (int i = 0; i < outstandingReaders.Length; i++)
                {
                    Exception exception = (pendingExceptionGenerator != null) ? pendingExceptionGenerator() : null;
                    outstandingReaders[i].Set(new Item(exception, null));
                }
            }
        }

        public bool WaitForItem(TimeSpan timeout)
        {
            WaitQueueWaiter waiter = null;
            bool itemAvailable = false;

            lock (ThisLock)
            {
                if (queueState == QueueState.Open)
                {
                    if (itemQueue.HasAvailableItem)
                    {
                        itemAvailable = true;
                    }
                    else
                    {
                        waiter = new WaitQueueWaiter();
                        waiterList.Add(waiter);
                    }
                }
                else if (queueState == QueueState.Shutdown)
                {
                    if (itemQueue.HasAvailableItem)
                    {
                        itemAvailable = true;
                    }
                    else if (itemQueue.HasAnyItem)
                    {
                        waiter = new WaitQueueWaiter();
                        waiterList.Add(waiter);
                    }
                    else
                    {
                        return true;
                    }
                }
                else // queueState == QueueState.Closed
                {
                    return true;
                }
            }

            if (waiter != null)
            {
                return waiter.Wait(timeout);
            }
            else
            {
                return itemAvailable;
            }
        }

        public void Dispose()
        {
            bool dispose = false;

            lock (ThisLock)
            {
                if (queueState != QueueState.Closed)
                {
                    queueState = QueueState.Closed;
                    dispose = true;
                }
            }

            if (dispose)
            {
                while (readerQueue.Count > 0)
                {
                    IQueueReader reader = readerQueue.Dequeue();
                    reader.Set(default(Item));
                }

                while (itemQueue.HasAnyItem)
                {
                    Item item = itemQueue.DequeueAnyItem();
                    DisposeItem(item);
                    InvokeDequeuedCallback(item.DequeuedCallback);
                }
            }
        }        

        void DisposeItem(Item item)
        {
            T value = item.Value;
            if (value != null)
            {
                if (value is IDisposable)
                {
                    ((IDisposable)value).Dispose();
                }
                else
                {
                    Action<T> disposeItemCallback = this.DisposeItemCallback;
                    if (disposeItemCallback != null)
                    {
                        disposeItemCallback(value);
                    }
                }
            }
        }

        static void CompleteOutstandingReadersCallback(object state)
        {
            IQueueReader[] outstandingReaders = (IQueueReader[])state;

            for (int i = 0; i < outstandingReaders.Length; i++)
            {
                outstandingReaders[i].Set(default(Item));
            }
        }

        static void CompleteWaiters(bool itemAvailable, IQueueWaiter[] waiters)
        {
            for (int i = 0; i < waiters.Length; i++)
            {
                waiters[i].Set(itemAvailable);
            }
        }

        static void CompleteWaitersFalseCallback(object state)
        {
            CompleteWaiters(false, (IQueueWaiter[])state);
        }

        static void CompleteWaitersLater(bool itemAvailable, IQueueWaiter[] waiters)
        {
            if (itemAvailable)
            {
                if (completeWaitersTrueCallback == null)
                {
                    completeWaitersTrueCallback = new WaitCallback(CompleteWaitersTrueCallback);
                }

                ThreadPool.QueueUserWorkItem(completeWaitersTrueCallback, waiters);
            }
            else
            {
                if (completeWaitersFalseCallback == null)
                {
                    completeWaitersFalseCallback = new WaitCallback(CompleteWaitersFalseCallback);
                }

                ThreadPool.QueueUserWorkItem(completeWaitersFalseCallback, waiters);
            }
        }

        static void CompleteWaitersTrueCallback(object state)
        {
            CompleteWaiters(true, (IQueueWaiter[])state);
        }

        static void InvokeDequeuedCallback(Action dequeuedCallback)
        {
            if (dequeuedCallback != null)
            {
                dequeuedCallback();
            }
        }

        static void InvokeDequeuedCallbackLater(Action dequeuedCallback)
        {
            if (dequeuedCallback != null)
            {
                if (onInvokeDequeuedCallback == null)
                {
                    onInvokeDequeuedCallback = new WaitCallback(OnInvokeDequeuedCallback);
                }

                ThreadPool.QueueUserWorkItem(onInvokeDequeuedCallback, dequeuedCallback);
            }
        }

        static void OnDispatchCallback(object state)
        {
            ((InputQueue<T>)state).Dispatch();
        }

        static void OnInvokeDequeuedCallback(object state)
        {
            Action dequeuedCallback = (Action)state;
            dequeuedCallback();
        }

        void EnqueueAndDispatch(Item item, bool canDispatchOnThisThread)
        {
            bool disposeItem = false;
            IQueueReader reader = null;
            bool dispatchLater = false;
            IQueueWaiter[] waiters = null;
            bool itemAvailable = true;

            lock (ThisLock)
            {
                itemAvailable = !((queueState == QueueState.Closed) || (queueState == QueueState.Shutdown));
                this.GetWaiters(out waiters);

                if (queueState == QueueState.Open)
                {
                    if (canDispatchOnThisThread)
                    {
                        if (readerQueue.Count == 0)
                        {
                            itemQueue.EnqueueAvailableItem(item);
                        }
                        else
                        {
                            reader = readerQueue.Dequeue();
                        }
                    }
                    else
                    {
                        if (readerQueue.Count == 0)
                        {
                            itemQueue.EnqueueAvailableItem(item);
                        }
                        else
                        {
                            itemQueue.EnqueuePendingItem(item);
                            dispatchLater = true;
                        }
                    }
                }
                else // queueState == QueueState.Closed || queueState == QueueState.Shutdown
                {
                    disposeItem = true;
                }
            }

            if (waiters != null)
            {
                if (canDispatchOnThisThread)
                {
                    CompleteWaiters(itemAvailable, waiters);
                }
                else
                {
                    CompleteWaitersLater(itemAvailable, waiters);
                }
            }

            if (reader != null)
            {
                InvokeDequeuedCallback(item.DequeuedCallback);
                reader.Set(item);
            }

            if (dispatchLater)
            {
                if (onDispatchCallback == null)
                {
                    onDispatchCallback = new WaitCallback(OnDispatchCallback);
                }

                ThreadPool.QueueUserWorkItem(onDispatchCallback, this);
            }
            else if (disposeItem)
            {
                InvokeDequeuedCallback(item.DequeuedCallback);
                DisposeItem(item);
            }
        }

        // This will not block, however, Dispatch() must be called later if this function
        // returns true.
        bool EnqueueWithoutDispatch(Item item)
        {
            lock (ThisLock)
            {
                // Open
                if (queueState != QueueState.Closed && queueState != QueueState.Shutdown)
                {
                    if (readerQueue.Count == 0 && waiterList.Count == 0)
                    {
                        itemQueue.EnqueueAvailableItem(item);
                        return false;
                    }
                    else
                    {
                        itemQueue.EnqueuePendingItem(item);
                        return true;
                    }
                }
            }

            DisposeItem(item);
            InvokeDequeuedCallbackLater(item.DequeuedCallback);
            return false;
        }

        void GetWaiters(out IQueueWaiter[] waiters)
        {
            if (waiterList.Count > 0)
            {
                waiters = waiterList.ToArray();
                waiterList.Clear();
            }
            else
            {
                waiters = null;
            }
        }

        // Used for timeouts. The InputQueue must remove readers from its reader queue to prevent
        // dispatching items to timed out readers.
        bool RemoveReader(IQueueReader reader)
        {
            lock (ThisLock)
            {
                if (queueState == QueueState.Open || queueState == QueueState.Shutdown)
                {
                    bool removed = false;

                    for (int i = readerQueue.Count; i > 0; i--)
                    {
                        IQueueReader temp = readerQueue.Dequeue();
                        if (object.ReferenceEquals(temp, reader))
                        {
                            removed = true;
                        }
                        else
                        {
                            readerQueue.Enqueue(temp);
                        }
                    }

                    return removed;
                }
            }

            return false;
        }

        enum QueueState
        {
            Open,
            Shutdown,
            Closed
        }

        interface IQueueReader
        {
            void Set(Item item);
        }

        interface IQueueWaiter
        {
            void Set(bool itemAvailable);
        }

        struct Item
        {
            Action dequeuedCallback;
            Exception exception;
            T value;

            public Item(T value, Action dequeuedCallback)
                : this(value, null, dequeuedCallback)
            {
            }

            public Item(Exception exception, Action dequeuedCallback)
                : this(null, exception, dequeuedCallback)
            {
            }

            Item(T value, Exception exception, Action dequeuedCallback)
            {
                this.value = value;
                this.exception = exception;
                this.dequeuedCallback = dequeuedCallback;
            }

            public Action DequeuedCallback
            {
                get { return this.dequeuedCallback; }
            }

            public Exception Exception
            {
                get { return this.exception; }
            }

            public T Value
            {
                get { return this.value; }
            }

            public T GetValue()
            {
                if (this.exception != null)
                {
                    throw this.exception;
                }

                return this.value;
            }
        }

        class AsyncQueueReader : AsyncResult, IQueueReader
        {
            static TimerCallback timerCallback = new TimerCallback(AsyncQueueReader.TimerCallback);

            bool expired;
            InputQueue<T> inputQueue;
            T item;
            Timer timer;

            public AsyncQueueReader(InputQueue<T> inputQueue, TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                if (inputQueue.AsyncCallbackGenerator != null)
                {
                    base.VirtualCallback = inputQueue.AsyncCallbackGenerator();
                }
                this.inputQueue = inputQueue;
                if (timeout != TimeSpan.MaxValue)
                {
                    this.timer = new Timer(timerCallback, this, timeout, TimeSpan.FromMilliseconds(-1));
                }
            }

            public static bool End(IAsyncResult result, out T value)
            {
                AsyncQueueReader readerResult = AsyncResult.End<AsyncQueueReader>(result);

                if (readerResult.expired)
                {
                    value = default(T);
                    return false;
                }
                else
                {
                    value = readerResult.item;
                    return true;
                }
            }

            public void Set(Item item)
            {
                this.item = item.Value;
                if (this.timer != null)
                {
                    this.timer.Change(-1, -1);
                }
                Complete(false, item.Exception);
            }

            static void TimerCallback(object state)
            {
                AsyncQueueReader thisPtr = (AsyncQueueReader)state;
                if (thisPtr.inputQueue.RemoveReader(thisPtr))
                {
                    thisPtr.expired = true;
                    thisPtr.Complete(false);
                }
            }
        }

        class AsyncQueueWaiter : AsyncResult, IQueueWaiter
        {
            static TimerCallback timerCallback = new TimerCallback(AsyncQueueWaiter.TimerCallback);
            bool itemAvailable;

            object thisLock = new object();

            Timer timer;

            public AsyncQueueWaiter(TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                if (timeout != TimeSpan.MaxValue)
                {
                    this.timer = new Timer(timerCallback, this, timeout, TimeSpan.FromMilliseconds(-1));
                }
            }

            object ThisLock
            {
                get
                {
                    return this.thisLock;
                }
            }

            public static bool End(IAsyncResult result)
            {
                AsyncQueueWaiter waiterResult = AsyncResult.End<AsyncQueueWaiter>(result);
                return waiterResult.itemAvailable;
            }

            public void Set(bool itemAvailable)
            {
                bool timely;

                lock (ThisLock)
                {
                    timely = (this.timer == null) || this.timer.Change(-1, -1);
                    this.itemAvailable = itemAvailable;
                }

                if (timely)
                {
                    Complete(false);
                }
            }

            static void TimerCallback(object state)
            {
                AsyncQueueWaiter thisPtr = (AsyncQueueWaiter)state;
                thisPtr.Complete(false);
            }
        }

        class ItemQueue
        {
            int head;
            Item[] items;
            int pendingCount;
            int totalCount;

            public ItemQueue()
            {
                this.items = new Item[1];
            }

            public bool HasAnyItem
            {
                get { return this.totalCount > 0; }
            }

            public bool HasAvailableItem
            {
                get { return this.totalCount > this.pendingCount; }
            }

            public int ItemCount
            {
                get { return this.totalCount; }
            }

            public Item DequeueAnyItem()
            {
                if (this.pendingCount == this.totalCount)
                {
                    this.pendingCount--;
                }
                return DequeueItemCore();
            }

            public Item DequeueAvailableItem()
            {
                return DequeueItemCore();
            }

            public void EnqueueAvailableItem(Item item)
            {
                EnqueueItemCore(item);
            }

            public void EnqueuePendingItem(Item item)
            {
                EnqueueItemCore(item);
                this.pendingCount++;
            }

            public void MakePendingItemAvailable()
            {
                this.pendingCount--;
            }

            Item DequeueItemCore()
            {
                Item item = this.items[this.head];
                this.items[this.head] = new Item();
                this.totalCount--;
                this.head = (this.head + 1) % this.items.Length;
                return item;
            }

            void EnqueueItemCore(Item item)
            {
                if (this.totalCount == this.items.Length)
                {
                    Item[] newItems = new Item[this.items.Length * 2];
                    for (int i = 0; i < this.totalCount; i++)
                    {
                        newItems[i] = this.items[(head + i) % this.items.Length];
                    }
                    this.head = 0;
                    this.items = newItems;
                }
                int tail = (this.head + this.totalCount) % this.items.Length;
                this.items[tail] = item;
                this.totalCount++;
            }
        }

        class WaitQueueReader : IQueueReader
        {
            Exception exception;
            InputQueue<T> inputQueue;
            T item;

            ManualResetEvent waitEvent;

            public WaitQueueReader(InputQueue<T> inputQueue)
            {
                this.inputQueue = inputQueue;
                waitEvent = new ManualResetEvent(false);
            }

            public void Set(Item item)
            {
                lock (this)
                {
                    this.exception = item.Exception;
                    this.item = item.Value;
                    waitEvent.Set();
                }
            }

            public bool Wait(TimeSpan timeout, out T value)
            {
                bool isSafeToClose = false;
                try
                {
                    if (!TimeoutHelper.WaitOne(waitEvent, timeout))
                    {
                        if (this.inputQueue.RemoveReader(this))
                        {
                            value = default(T);
                            isSafeToClose = true;
                            return false;
                        }
                        else
                        {
                            waitEvent.WaitOne();
                        }
                    }

                    isSafeToClose = true;
                }
                finally
                {
                    if (isSafeToClose)
                    {
                        waitEvent.Close();
                    }
                }

                if (this.exception != null)
                {
                    throw this.exception;
                }

                value = item;
                return true;
            }
        }

        class WaitQueueWaiter : IQueueWaiter
        {
            bool itemAvailable;

            ManualResetEvent waitEvent;

            public WaitQueueWaiter()
            {
                waitEvent = new ManualResetEvent(false);
            }

            public void Set(bool itemAvailable)
            {
                lock (this)
                {
                    this.itemAvailable = itemAvailable;
                    waitEvent.Set();
                }
            }

            public bool Wait(TimeSpan timeout)
            {
                if (!TimeoutHelper.WaitOne(waitEvent, timeout))
                {
                    return false;
                }

                return this.itemAvailable;
            }
        }
    }
}
