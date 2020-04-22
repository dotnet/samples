//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace System.Threading.Tasks
{
    public static partial class TaskFactoryExtensions
    {
        #region TaskFactory with Action
        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="action">The delegate for the task.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task Create(
            this TaskFactory factory, Action action)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return new Task(action, factory.CancellationToken, factory.CreationOptions);
        }

        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="action">The delegate for the task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task Create(
            this TaskFactory factory, Action action, TaskCreationOptions creationOptions) => new Task(action, factory.CancellationToken, creationOptions);

        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="action">The delegate for the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task Create(
            this TaskFactory factory, Action<object> action, object state)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return new Task(action, state, factory.CancellationToken, factory.CreationOptions);
        }

        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="action">The delegate for the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task Create(
            this TaskFactory factory, Action<object> action, object state, TaskCreationOptions creationOptions) => new Task(action, state, factory.CancellationToken, creationOptions);
        #endregion

        #region TaskFactory with Func
        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="function">The delegate for the task.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task<TResult> Create<TResult>(
            this TaskFactory factory, Func<TResult> function)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return new Task<TResult>(function, factory.CancellationToken, factory.CreationOptions);
        }

        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="function">The delegate for the task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task<TResult> Create<TResult>(
            this TaskFactory factory, Func<TResult> function, TaskCreationOptions creationOptions) => new Task<TResult>(function, factory.CancellationToken, creationOptions);

        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="function">The delegate for the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task<TResult> Create<TResult>(
            this TaskFactory factory, Func<object, TResult> function, object state)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return new Task<TResult>(function, state, factory.CancellationToken, factory.CreationOptions);
        }

        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="function">The delegate for the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task<TResult> Create<TResult>(
            this TaskFactory factory, Func<object, TResult> function, object state, TaskCreationOptions creationOptions) => new Task<TResult>(function, state, factory.CancellationToken, creationOptions);
        #endregion

        #region TaskFactory<TResult> with Func
        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="function">The delegate for the task.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task<TResult> Create<TResult>(
            this TaskFactory<TResult> factory, Func<TResult> function)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return new Task<TResult>(function, factory.CancellationToken, factory.CreationOptions);
        }

        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="function">The delegate for the task.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task<TResult> Create<TResult>(
            this TaskFactory<TResult> factory, Func<TResult> function, TaskCreationOptions creationOptions) => new Task<TResult>(function, factory.CancellationToken, creationOptions);

        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="function">The delegate for the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task<TResult> Create<TResult>(
            this TaskFactory<TResult> factory, Func<object, TResult> function, object state)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return new Task<TResult>(function, state, factory.CancellationToken, factory.CreationOptions);
        }

        /// <summary>Creates a Task using the TaskFactory.</summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="function">The delegate for the task.</param>
        /// <param name="state">An object provided to the delegate.</param>
        /// <param name="creationOptions">Options that control the task's behavior.</param>
        /// <returns>The created task.  The task has not been scheduled.</returns>
        public static Task<TResult> Create<TResult>(
            this TaskFactory<TResult> factory, Func<object, TResult> function, object state, TaskCreationOptions creationOptions) => new Task<TResult>(function, state, factory.CancellationToken, creationOptions);
        #endregion
    }
}
