//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace System.Threading.Tasks
{
    /// <summary>Extensions for TaskFactory.</summary>
    public static partial class TaskFactoryExtensions
    {
        /// <summary>Creates a generic TaskFactory from a non-generic one.</summary>
        /// <typeparam name="TResult">Specifies the type of Task results for the Tasks created by the new TaskFactory.</typeparam>
        /// <param name="factory">The TaskFactory to serve as a template.</param>
        /// <returns>The created TaskFactory.</returns>
        public static TaskFactory<TResult> ToGeneric<TResult>(this TaskFactory factory) => new TaskFactory<TResult>(
                factory.CancellationToken, factory.CreationOptions, factory.ContinuationOptions, factory.Scheduler);

        /// <summary>Creates a generic TaskFactory from a non-generic one.</summary>
        /// <typeparam name="TResult">Specifies the type of Task results for the Tasks created by the new TaskFactory.</typeparam>
        /// <param name="factory">The TaskFactory to serve as a template.</param>
        /// <returns>The created TaskFactory.</returns>
        public static TaskFactory ToNonGeneric<TResult>(this TaskFactory<TResult> factory) => new TaskFactory(
                factory.CancellationToken, factory.CreationOptions, factory.ContinuationOptions, factory.Scheduler);

        /// <summary>Gets the TaskScheduler instance that should be used to schedule tasks.</summary>
        public static TaskScheduler GetTargetScheduler(this TaskFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return factory.Scheduler ?? TaskScheduler.Current;
        }

        /// <summary>Gets the TaskScheduler instance that should be used to schedule tasks.</summary>
        public static TaskScheduler GetTargetScheduler<TResult>(this TaskFactory<TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return factory.Scheduler ?? TaskScheduler.Current;
        }

        /// <summary>Converts TaskCreationOptions into TaskContinuationOptions.</summary>
        /// <param name="creationOptions"></param>
        /// <returns></returns>
        private static TaskContinuationOptions ContinuationOptionsFromCreationOptions(TaskCreationOptions creationOptions) => (TaskContinuationOptions)
                ((creationOptions & TaskCreationOptions.AttachedToParent) |
                 (creationOptions & TaskCreationOptions.PreferFairness) |
                 (creationOptions & TaskCreationOptions.LongRunning));
    }
}
