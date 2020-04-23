//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace System.Threading.Tasks
{
    /// <summary>Extensions for TaskFactory.</summary>
    public static partial class TaskFactoryExtensions
    {
        #region TaskFactory
        /// <summary>Creates a Task that has completed in the Faulted state with the specified exception.</summary>
        /// <param name="factory">The target TaskFactory.</param>
        /// <param name="exception">The exception with which the Task should fault.</param>
        /// <returns>The completed Task.</returns>
        public static Task FromException(this TaskFactory factory, Exception exception)
        {
            var tcs = new TaskCompletionSource<object>(factory.CreationOptions);
            tcs.SetException(exception);
            return tcs.Task;
        }

        /// <summary>Creates a Task that has completed in the Faulted state with the specified exception.</summary>
        /// <typeparam name="TResult">Specifies the type of payload for the new Task.</typeparam>
        /// <param name="factory">The target TaskFactory.</param>
        /// <param name="exception">The exception with which the Task should fault.</param>
        /// <returns>The completed Task.</returns>
        public static Task<TResult> FromException<TResult>(this TaskFactory factory, Exception exception)
        {
            var tcs = new TaskCompletionSource<TResult>(factory.CreationOptions);
            tcs.SetException(exception);
            return tcs.Task;
        }

        /// <summary>Creates a Task that has completed in the RanToCompletion state with the specified result.</summary>
        /// <typeparam name="TResult">Specifies the type of payload for the new Task.</typeparam>
        /// <param name="factory">The target TaskFactory.</param>
        /// <param name="result">The result with which the Task should complete.</param>
        /// <returns>The completed Task.</returns>
        public static Task<TResult> FromResult<TResult>(this TaskFactory factory, TResult result)
        {
            var tcs = new TaskCompletionSource<TResult>(factory.CreationOptions);
            tcs.SetResult(result);
            return tcs.Task;
        }

        /// <summary>Creates a Task that has completed in the Canceled state with the specified CancellationToken.</summary>
        /// <param name="factory">The target TaskFactory.</param>
        /// <param name="cancellationToken">The CancellationToken with which the Task should complete.</param>
        /// <returns>The completed Task.</returns>
        public static Task FromCancellation(this TaskFactory factory, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested) throw new ArgumentOutOfRangeException("cancellationToken");
            return new Task(() => { }, cancellationToken);
        }

        /// <summary>Creates a Task that has completed in the Canceled state with the specified CancellationToken.</summary>
        /// <typeparam name="TResult">Specifies the type of payload for the new Task.</typeparam>
        /// <param name="factory">The target TaskFactory.</param>
        /// <param name="cancellationToken">The CancellationToken with which the Task should complete.</param>
        /// <returns>The completed Task.</returns>
        public static Task<TResult> FromCancellation<TResult>(this TaskFactory factory, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested) throw new ArgumentOutOfRangeException(nameof(cancellationToken));
            return new Task<TResult>(DelegateCache<TResult>.s_defaultResult, cancellationToken);
        }

        /// <summary>A cache of delegates.</summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        private class DelegateCache<TResult>
        {
            /// <summary>Function that returns default(TResult).</summary>
            internal static readonly Func<TResult> s_defaultResult = () => default;
        }
        #endregion

        #region TaskFactory<TResult>
        /// <summary>Creates a Task that has completed in the Faulted state with the specified exception.</summary>
        /// <param name="factory">The target TaskFactory.</param>
        /// <param name="exception">The exception with which the Task should fault.</param>
        /// <returns>The completed Task.</returns>
        public static Task<TResult> FromException<TResult>(this TaskFactory<TResult> factory, Exception exception)
        {
            var tcs = new TaskCompletionSource<TResult>(factory.CreationOptions);
            tcs.SetException(exception);
            return tcs.Task;
        }

        /// <summary>Creates a Task that has completed in the RanToCompletion state with the specified result.</summary>
        /// <typeparam name="TResult">Specifies the type of payload for the new Task.</typeparam>
        /// <param name="factory">The target TaskFactory.</param>
        /// <param name="result">The result with which the Task should complete.</param>
        /// <returns>The completed Task.</returns>
        public static Task<TResult> FromResult<TResult>(this TaskFactory<TResult> factory, TResult result)
        {
            var tcs = new TaskCompletionSource<TResult>(factory.CreationOptions);
            tcs.SetResult(result);
            return tcs.Task;
        }

        /// <summary>Creates a Task that has completed in the Canceled state with the specified CancellationToken.</summary>
        /// <typeparam name="TResult">Specifies the type of payload for the new Task.</typeparam>
        /// <param name="factory">The target TaskFactory.</param>
        /// <param name="cancellationToken">The CancellationToken with which the Task should complete.</param>
        /// <returns>The completed Task.</returns>
        public static Task<TResult> FromCancellation<TResult>(this TaskFactory<TResult> factory, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested) throw new ArgumentOutOfRangeException(nameof(cancellationToken));
            return new Task<TResult>(DelegateCache<TResult>.s_defaultResult, cancellationToken);
        }
        #endregion
    }
}
