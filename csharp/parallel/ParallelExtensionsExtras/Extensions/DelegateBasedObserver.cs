//--------------------------------------------------------------------------
// 
//  Copyright (c) Microsoft Corporation.  All rights reserved. 
// 
//  File: IProducerConsumerCollectionExtensions.cs
//
//--------------------------------------------------------------------------

namespace System
{
    internal class DelegateBasedObserver<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        internal DelegateBasedObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            _onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
            _onError = onError ?? throw new ArgumentNullException(nameof(onError));
            _onCompleted = onCompleted ?? throw new ArgumentNullException(nameof(onCompleted));
        }

        public void OnCompleted() => _onCompleted();
        public void OnError(Exception error) => _onError(error);
        public void OnNext(T value) => _onNext(value);
    }
}
