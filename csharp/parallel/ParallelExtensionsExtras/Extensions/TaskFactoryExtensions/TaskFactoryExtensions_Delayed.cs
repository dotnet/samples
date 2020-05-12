//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace System.Threading.Tasks
{
    public static partial class TaskFactoryExtensions
    {
        #region TaskFactory No Action
        /// <summary>Creates a Task that will complete after the specified delay.</summary>
        /// <param name="factory">The TaskFactory.</param>
        /// <param name="millisecondsDelay">The delay after which the Task should transition to RanToCompletion.</param>
        /// <returns>A Task that will be completed after the specified duration.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory, int millisecondsDelay) => StartNewDelayed(factory, millisecondsDelay, CancellationToken.None);

        /// <summary>Creates a Task that will complete after the specified delay.</summary>
        /// <param name="factory">The TaskFactory.</param>
        /// <param name="millisecondsDelay">The delay after which the Task should transition to RanToCompletion.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel the timed task.</param>
        /// <returns>A Task that will be completed after the specified duration and that's cancelable with the specified token.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int millisecondsDelay, CancellationToken cancellationToken)
        {
            // Validate arguments
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (millisecondsDelay < 0) throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));

            // Check for a pre-canceled token
            if (cancellationToken.IsCancellationRequested)
                return factory.FromCancellation(cancellationToken);

            // Create the timed task
            var tcs = new TaskCompletionSource<object>(factory.CreationOptions);
            var ctr = default(CancellationTokenRegistration);

            // Create the timer but don't start it yet.  If we start it now,
            // it might fire before ctr has been set to the right registration.
            var timer = new Timer(self =>
            {
                // Clean up both the cancellation token and the timer, and try to transition to completed
                try
                {
                    ctr.Dispose();
                }
                catch (NullReferenceException)
                {
                    // Eat this. Mono throws a NullReferenceException when constructed with
                    // default(CancellationTokenRegistration);
                }

                ((Timer)self).Dispose();
                tcs.TrySetResult(null);
            });

            // Register with the cancellation token.
            if (cancellationToken.CanBeCanceled)
            {
                // When cancellation occurs, cancel the timer and try to transition to canceled.
                // There could be a race, but it's benign.
                ctr = cancellationToken.Register(() =>
                {
                    timer.Dispose();
                    tcs.TrySetCanceled();
                });
            }

            // Start the timer and hand back the task...
            try { timer.Change(millisecondsDelay, Timeout.Infinite); }
            catch (ObjectDisposedException) { } // in case there's a race with cancellation; this is benign

            return tcs.Task;
        }
        #endregion

        #region TaskFactory with Action
        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action action)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, action, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action action,
            TaskCreationOptions creationOptions)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, action, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action action,
            CancellationToken cancellationToken)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, action, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action action,
            CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (millisecondsDelay < 0) throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            return factory
                .StartNewDelayed(millisecondsDelay, cancellationToken)
                .ContinueWith(_ => action(), cancellationToken, TaskContinuationOptions.OnlyOnRanToCompletion, scheduler);
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action<object> action, object state)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, action, state, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action<object> action, object state,
            TaskCreationOptions creationOptions)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, action, state, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action<object> action, object state,
            CancellationToken cancellationToken)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, action, state, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="action">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action<object> action, object state,
            CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (millisecondsDelay < 0) throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            // Create the task that will be returned; workaround for no ContinueWith(..., state) overload.
            var result = new TaskCompletionSource<object>(state);

            // Delay a continuation to run the action
            factory
                .StartNewDelayed(millisecondsDelay, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsCanceled) result.TrySetCanceled();
                    else
                    {
                        try
                        {
                            action(state);
                            result.TrySetResult(null);
                        }
                        catch (Exception exc) { result.TrySetException(exc); }
                    }
                }, scheduler);

            // Return the task
            return result.Task;
        }
        #endregion

        #region TaskFactory<TResult> with Func
        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(
            this TaskFactory<TResult> factory,
            int millisecondsDelay, Func<TResult> function)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, function, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(
            this TaskFactory<TResult> factory,
            int millisecondsDelay, Func<TResult> function,
            TaskCreationOptions creationOptions)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, function, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="cancellationToken">The CancellationToken to assign to the Task.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(
            this TaskFactory<TResult> factory,
            int millisecondsDelay, Func<TResult> function,
            CancellationToken cancellationToken)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, function, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="cancellationToken">The CancellationToken to assign to the Task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(
            this TaskFactory<TResult> factory,
            int millisecondsDelay, Func<TResult> function,
            CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (millisecondsDelay < 0) throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));
            if (function == null) throw new ArgumentNullException(nameof(function));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            // Create the trigger and the timer to start it
            var tcs = new TaskCompletionSource<object>();
            var timer = new Timer(obj => ((TaskCompletionSource<object>)obj).SetResult(null),
                tcs, millisecondsDelay, Timeout.Infinite);

            // Return a task that executes the function when the trigger fires
            return tcs.Task.ContinueWith(_ =>
            {
                timer.Dispose();
                return function();
            }, cancellationToken, ContinuationOptionsFromCreationOptions(creationOptions), scheduler);
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(
            this TaskFactory<TResult> factory,
            int millisecondsDelay, Func<object, TResult> function, object state)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, function, state, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="cancellationToken">The CancellationToken to assign to the Task.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(
            this TaskFactory<TResult> factory,
            int millisecondsDelay, Func<object, TResult> function, object state,
            CancellationToken cancellationToken)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, function, state, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(
            this TaskFactory<TResult> factory,
            int millisecondsDelay, Func<object, TResult> function, object state,
            TaskCreationOptions creationOptions)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return StartNewDelayed(factory, millisecondsDelay, function, state, factory.CancellationToken, creationOptions, factory.GetTargetScheduler());
        }

        /// <summary>Creates and schedules a task for execution after the specified time delay.</summary>
        /// <param name="factory">The factory to use to create the task.</param>
        /// <param name="millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name="function">The delegate executed by the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="cancellationToken">The CancellationToken to assign to the Task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <param name="scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task<TResult> StartNewDelayed<TResult>(
            this TaskFactory<TResult> factory,
            int millisecondsDelay, Func<object, TResult> function, object state,
            CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (millisecondsDelay < 0) throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));
            if (function == null) throw new ArgumentNullException(nameof(function));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            // Create the task that will be returned
            var result = new TaskCompletionSource<TResult>(state);
            Timer timer = null;

            // Create the task that will run the user's function
            var functionTask = new Task<TResult>(function, state, creationOptions);

            // When the function task completes, transfer the results to the returned task
            functionTask.ContinueWith(t =>
            {
                result.SetFromTask(t);
                timer.Dispose();
            }, cancellationToken, ContinuationOptionsFromCreationOptions(creationOptions) | TaskContinuationOptions.ExecuteSynchronously, scheduler);

            // Start the timer for the trigger
            timer = new Timer(obj => ((Task)obj).Start(scheduler),
                functionTask, millisecondsDelay, Timeout.Infinite);

            return result.Task;
        }
        #endregion
    }
}
