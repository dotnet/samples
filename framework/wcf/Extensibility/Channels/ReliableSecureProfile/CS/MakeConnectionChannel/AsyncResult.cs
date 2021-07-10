//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Threading;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public abstract class AsyncResult : IAsyncResult
    {
        static AsyncCallback asyncCompletionWrapperCallback;
        AsyncCallback callback;
        bool completedSynchronously;
        bool endCalled;
        Exception exception;
        bool isCompleted;
        AsyncCompletion nextAsyncCompletion;
        object state;

        ManualResetEvent manualResetEvent;
        object thisLock;

        protected AsyncResult(AsyncCallback callback, object state)
        {
            this.callback = callback;
            this.state = state;
            this.thisLock = new object();
        }

        public object AsyncState
        {
            get
            {
                return state;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (manualResetEvent != null)
                {
                    return manualResetEvent;
                }

                lock (ThisLock)
                {
                    if (manualResetEvent == null)
                    {
                        manualResetEvent = new ManualResetEvent(isCompleted);
                    }
                }

                return manualResetEvent;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return completedSynchronously;
            }
        }

        public bool HasCallback
        {
            get
            {
                return this.callback != null;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return isCompleted;
            }
        }

        // used in conjunction with PrepareAsyncCompletion to allow for finally blocks
        protected Action<AsyncResult, Exception> OnCompleting { get; set; }

        object ThisLock
        {
            get
            {
                return this.thisLock;
            }
        }

        // subclasses like TraceAsyncResult can use this to wrap the callback functionality in a scope
        protected Action<AsyncCallback, IAsyncResult> VirtualCallback
        {
            get;
            set;
        }

        protected void Complete(bool completedSynchronously)
        {
            if (this.isCompleted)
            {
                throw new InvalidOperationException
                    (String.Format(ExceptionMessages.AsyncResultCompletedTwice,GetType()));
            }

            this.completedSynchronously = completedSynchronously;
            if (OnCompleting != null)
            {
                // Allow exception replacement, like a catch/throw pattern.
                try
                {
                    OnCompleting(this, this.exception);
                }
                catch (Exception exception)
                {
                    this.exception = exception;
                }
            }

            if (completedSynchronously)
            {
                // If we completedSynchronously, then there's no chance that the manualResetEvent was created so
                // we don't need to worry about a race
                this.isCompleted = true;
            }
            else
            {
                lock (ThisLock)
                {
                    this.isCompleted = true;
                    if (this.manualResetEvent != null)
                    {
                        this.manualResetEvent.Set();
                    }
                }
            }

            if (this.callback != null)
            {
                if (VirtualCallback != null)
                {
                    VirtualCallback(this.callback, this);
                }
                else
                {
                    this.callback(this);
                }
            }
        }

        protected void Complete(bool completedSynchronously, Exception exception)
        {
            this.exception = exception;
            Complete(completedSynchronously);
        }

        static void AsyncCompletionWrapperCallback(IAsyncResult result)
        {
            if (result == null)
            {
                throw new InvalidOperationException(ExceptionMessages.InvalidNullAsyncResult);
            }
            if (result.CompletedSynchronously)
            {
                return;
            }

            AsyncResult thisPtr = (AsyncResult)result.AsyncState;
            AsyncCompletion callback = thisPtr.GetNextCompletion();
            if (callback == null)
            {
                ThrowInvalidAsyncResult(result);
            }

            bool completeSelf = false;
            Exception completionException = null;
            try
            {
                completeSelf = callback(result);
            }
            catch (Exception e)
            {
                completeSelf = true;
                completionException = e;
            }

            if (completeSelf)
            {
                thisPtr.Complete(false, completionException);
            }
        }

        protected AsyncCallback PrepareAsyncCompletion(AsyncCompletion callback)
        {
            this.nextAsyncCompletion = callback;
            if (AsyncResult.asyncCompletionWrapperCallback == null)
            {
                AsyncResult.asyncCompletionWrapperCallback = new AsyncCallback(AsyncCompletionWrapperCallback);
            }
            return AsyncResult.asyncCompletionWrapperCallback;
        }

        protected bool CheckSyncContinue(IAsyncResult result)
        {
            AsyncCompletion dummy;
            return TryContinueHelper(result, out dummy);
        }

        protected bool SyncContinue(IAsyncResult result)
        {
            AsyncCompletion callback;
            if (TryContinueHelper(result, out callback))
            {
                return callback(result);
            }
            else
            {
                return false;
            }
        }

        bool TryContinueHelper(IAsyncResult result, out AsyncCompletion callback)
        {
            if (result == null)
            {
                throw new InvalidOperationException(ExceptionMessages.InvalidNullAsyncResult);
            }

            callback = null;

            if (!result.CompletedSynchronously)
            {
                return false;
            }

            callback = GetNextCompletion();
            if (callback == null)
            {
                ThrowInvalidAsyncResult("Only call Check/SyncContinue once per async operation (once per PrepareAsyncCompletion).");
            }
            return true;
        }

        AsyncCompletion GetNextCompletion()
        {
            AsyncCompletion result = this.nextAsyncCompletion;
            this.nextAsyncCompletion = null;
            return result;
        }

        static void ThrowInvalidAsyncResult(IAsyncResult result)
        {
            throw new InvalidOperationException(String.Format(ExceptionMessages.InvalidAsyncResultImplementation,result.GetType()));
        }

        static void ThrowInvalidAsyncResult(string debugText)
        {
            throw new InvalidOperationException(ExceptionMessages.InvalidAsyncResultImplementationGeneric);
        }

        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
            where TAsyncResult : AsyncResult
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            TAsyncResult asyncResult = result as TAsyncResult;

            if (asyncResult == null)
            {
                throw new ArgumentException(ExceptionMessages.InvalidAsyncResult);
            }

            if (asyncResult.endCalled)
            {
                throw new InvalidOperationException(ExceptionMessages.AsyncResultAlreadyEnded);
            }

            asyncResult.endCalled = true;

            if (!asyncResult.isCompleted)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }

            if (asyncResult.manualResetEvent != null)
            {
                asyncResult.manualResetEvent.Close();
            }

            if (asyncResult.exception != null)
            {
                throw asyncResult.exception;
            }

            return asyncResult;
        }

        enum TransactionSignalState
        {
            Ready = 0,
            Prepared,
            Completed,
            Abandoned,
        }

        // can be utilized by subclasses to write core completion code for both the sync and async paths
        // in one location, signalling chainable synchronous completion with the boolean result,
        // and leveraging PrepareAsyncCompletion for conversion to an AsyncCallback.
        // NOTE: requires that "this" is passed in as the state object to the asynchronous sub-call being used with a completion routine.
        protected delegate bool AsyncCompletion(IAsyncResult result);
   }
}
