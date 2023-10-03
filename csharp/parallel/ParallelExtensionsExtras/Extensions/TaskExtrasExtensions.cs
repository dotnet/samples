//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Linq;

namespace System.Threading.Tasks
{
    /// <summary>Extensions methods for Task.</summary>
    public static class TaskExtrasExtensions
    {
        #region ContinueWith accepting TaskFactory
        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="continuationAction">The continuation action.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task ContinueWith(
            this Task task, Action<Task> continuationAction, TaskFactory factory) =>
            task.ContinueWith(continuationAction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);

        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="continuationFunction">The continuation function.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task<TResult> ContinueWith<TResult>(
            this Task task, Func<Task, TResult> continuationFunction, TaskFactory factory) =>
            task.ContinueWith(continuationFunction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);
        #endregion

        #region ContinueWith accepting TaskFactory<TResult>
        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="continuationAction">The continuation action.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task ContinueWith<TResult>(
            this Task<TResult> task, Action<Task<TResult>> continuationAction, TaskFactory<TResult> factory) =>
            task.ContinueWith(continuationAction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);

        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="continuationFunction">The continuation function.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task<TNewResult> ContinueWith<TResult, TNewResult>(
            this Task<TResult> task, Func<Task<TResult>, TNewResult> continuationFunction, TaskFactory<TResult> factory) =>
            task.ContinueWith(continuationFunction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);
        #endregion

        #region ToAsync(AsyncCallback, object)
        /// <summary>
        /// Creates a Task that represents the completion of another Task, and
        /// that schedules an AsyncCallback to run upon completion.
        /// </summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="callback">The AsyncCallback to run.</param>
        /// <param name="state">The object state to use with the AsyncCallback.</param>
        /// <returns>The new task.</returns>
        public static Task ToAsync(this Task task, AsyncCallback callback, object state)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            var tcs = new TaskCompletionSource<object>(state);
            task.ContinueWith(_ =>
            {
                tcs.SetFromTask(task);
                callback?.Invoke(tcs.Task);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Creates a Task that represents the completion of another Task, and
        /// that schedules an AsyncCallback to run upon completion.
        /// </summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="callback">The AsyncCallback to run.</param>
        /// <param name="state">The object state to use with the AsyncCallback.</param>
        /// <returns>The new task.</returns>
        public static Task<TResult> ToAsync<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            var tcs = new TaskCompletionSource<TResult>(state);
            task.ContinueWith(_ =>
            {
                tcs.SetFromTask(task);
                callback?.Invoke(tcs.Task);
            });
            return tcs.Task;
        }
        #endregion

        #region Exception Handling
        /// <summary>Suppresses default exception handling of a Task that would otherwise reraise the exception on the finalizer thread.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task IgnoreExceptions(this Task task)
        {
            task.ContinueWith(t => { _ = t.Exception; },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            return task;
        }

        /// <summary>Suppresses default exception handling of a Task that would otherwise reraise the exception on the finalizer thread.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task<T> IgnoreExceptions<T>(this Task<T> task) =>
            (Task<T>)((Task)task).IgnoreExceptions();

        /// <summary>Fails immediately when an exception is encountered.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task FailFastOnException(this Task task)
        {
            task.ContinueWith(t => Environment.FailFast("A task faulted.", t.Exception),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            return task;
        }

        /// <summary>Fails immediately when an exception is encountered.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task<T> FailFastOnException<T>(this Task<T> task) =>
            (Task<T>)((Task)task).FailFastOnException();

        /// <summary>Propagates any exceptions that occurred on the specified task.</summary>
        /// <param name="task">The Task whose exceptions are to be propagated.</param>
        public static void PropagateExceptions(this Task task)
        {
            if (!task.IsCompleted) throw new InvalidOperationException("The task has not completed.");
            if (task.IsFaulted) task.Wait();
        }

        /// <summary>Propagates any exceptions that occurred on the specified tasks.</summary>
        /// <param name="task">The Task whose exceptions are to be propagated.</param>
        public static void PropagateExceptions(this Task[] tasks)
        {
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));
            if (tasks.Any(t => t == null)) throw new ArgumentException(nameof(tasks));
            if (tasks.Any(t => !t.IsCompleted)) throw new InvalidOperationException("A task has not completed.");
            Task.WaitAll(tasks);
        }
        #endregion

        #region Observables
        /// <summary>Creates an IObservable that represents the completion of a Task.</summary>
        /// <typeparam name="TResult">Specifies the type of data returned by the Task.</typeparam>
        /// <param name="task">The Task to be represented as an IObservable.</param>
        /// <returns>An IObservable that represents the completion of the Task.</returns>
        public static IObservable<TResult> ToObservable<TResult>(this Task<TResult> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            return new TaskObservable<TResult> { _task = task };
        }

        /// <summary>An implementation of IObservable that wraps a Task.</summary>
        /// <typeparam name="TResult">The type of data returned by the task.</typeparam>
        private class TaskObservable<TResult> : IObservable<TResult>
        {
            internal Task<TResult> _task;

            public IDisposable Subscribe(IObserver<TResult> observer)
            {
                // Validate arguments.
                if (observer == null) throw new ArgumentNullException("observer");

                // Support cancelling the continuation if the observer is unsubscribed.
                var cts = new CancellationTokenSource();

                // Create a continuation to pass data along to the observer.
                _task.ContinueWith(t =>
                {
                    switch (t.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            observer.OnNext(_task.Result);
                            observer.OnCompleted();
                            break;

                        case TaskStatus.Faulted:
                            observer.OnError(_task.Exception);
                            break;

                        case TaskStatus.Canceled:
                            observer.OnError(new OperationCanceledException("The operation was canceled."));
                            break;
                    }
                }, cts.Token);

                // Support unsubscribe simply by canceling the continuation if it hasn't yet run
                return new CancelOnDispose { _source = cts };
            }
        }

        /// <summary>Translates a call to IDisposable.Dispose to a CancellationTokenSource.Cancel.</summary>
        private class CancelOnDispose : IDisposable
        {
            internal CancellationTokenSource _source;

            void IDisposable.Dispose() => _source.Cancel();
        }
        #endregion

        #region Timeouts
        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <typeparam name="TResult">Specifies the type of data contained in the task.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        public static Task WithTimeout(this Task task, TimeSpan timeout)
        {
            var result = new TaskCompletionSource<object>(task.AsyncState);
            var timer = new Timer(state => ((TaskCompletionSource<object>)state).TrySetCanceled(), result, timeout, TimeSpan.FromMilliseconds(-1));
            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result.Task;
        }

        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <typeparam name="TResult">Specifies the type of data contained in the task.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        public static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var result = new TaskCompletionSource<TResult>(task.AsyncState);
            var timer = new Timer(state => ((TaskCompletionSource<TResult>)state).TrySetCanceled(), result, timeout, TimeSpan.FromMilliseconds(-1));
            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result.Task;
        }
        #endregion

        #region Children
        /// <summary>
        /// Ensures that a parent task can't transition into a completed state
        /// until the specified task has also completed, even if it's not
        /// already a child task.
        /// </summary>
        /// <param name="task">The task to attach to the current task as a child.</param>
        public static void AttachToParent(this Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            task.ContinueWith(t => t.Wait(), CancellationToken.None,
                TaskContinuationOptions.AttachedToParent |
                TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
        #endregion

        #region Waiting
        /// <summary>Waits for the task to complete execution, returning the task's final status.</summary>
        /// <param name="task">The task for which to wait.</param>
        /// <returns>The completion status of the task.</returns>
        /// <remarks>Unlike Wait, this method will not throw an exception if the task ends in the Faulted or Canceled state.</remarks>
        public static TaskStatus WaitForCompletionStatus(this Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            ((IAsyncResult)task).AsyncWaitHandle.WaitOne();
            return task.Status;
        }
        #endregion

        #region Then
        /// <summary>Creates a task that represents the completion of a follow-up action when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The action to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the action.</returns>
        public static Task Then(this Task task, Action next)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(delegate
            {
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        next();
                        tcs.TrySetResult(null);
                    }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a follow-up function when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The function to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the function.</returns>
        public static Task<TResult> Then<TResult>(this Task task, Func<TResult> next)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<TResult>();
            task.ContinueWith(delegate
            {
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        var result = next();
                        tcs.TrySetResult(result);
                    }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a follow-up action when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The action to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the action.</returns>
        public static Task Then<TResult>(this Task<TResult> task, Action<TResult> next)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(delegate
            {
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        next(task.Result);
                        tcs.TrySetResult(null);
                    }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a follow-up function when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The function to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the function.</returns>
        public static Task<TNewResult> Then<TResult, TNewResult>(this Task<TResult> task, Func<TResult, TNewResult> next)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<TNewResult>();
            task.ContinueWith(delegate
            {
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        tcs.TrySetResult(next(task.Result));
                    }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        public static Task Then(this Task task, Func<Task> next)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(delegate
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    // Otherwise, get the next task.  If it's null, bail.  If not,
                    // when it's done we'll have our result.
                    try { next().ContinueWith(t => tcs.TrySetFromTask(t), TaskScheduler.Default); }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task based on the result of the first task.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        public static Task Then<T>(this Task<T> task, Func<T, Task> next)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(delegate
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    // Otherwise, get the next task.  If it's null, bail.  If not,
                    // when it's done we'll have our result.
                    try { next(task.Result).ContinueWith(t => tcs.TrySetFromTask(t), TaskScheduler.Default); }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        public static Task<TResult> Then<TResult>(this Task task, Func<Task<TResult>> next)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<TResult>();
            task.ContinueWith(delegate
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    // Otherwise, get the next task.  If it's null, bail.  If not,
                    // when it's done we'll have our result.
                    try { next().ContinueWith(t => tcs.TrySetFromTask(t), TaskScheduler.Default); }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task based on the result of the first.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        public static Task<TNewResult> Then<TResult, TNewResult>(this Task<TResult> task, Func<TResult, Task<TNewResult>> next)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<TNewResult>();
            task.ContinueWith(delegate
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    // Otherwise, get the next task.  If it's null, bail.  If not,
                    // when it's done we'll have our result.
                    try { next(task.Result).ContinueWith(t => tcs.TrySetFromTask(t), TaskScheduler.Default); }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskScheduler.Default);
            return tcs.Task;
        }
        #endregion
    }
}
