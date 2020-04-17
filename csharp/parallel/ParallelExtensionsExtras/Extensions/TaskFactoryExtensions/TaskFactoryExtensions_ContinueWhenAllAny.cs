//--------------------------------------------------------------------------
// 
//  Copyright (c) Microsoft Corporation.  All rights reserved. 
// 
//  File: TaskFactoryExtensions_ContinueWhenAllAny.cs
//
//--------------------------------------------------------------------------

namespace System.Threading.Tasks
{
    public static partial class TaskFactoryExtensions
    {
        /// <summary>
        /// Creates a continuation Task that will compplete upon
        /// the completion of a set of provided Tasks.
        /// </summary>
        /// <param name="factory">The TaskFactory to use to create the continuation task.</param>
        /// <param name="tasks">The array of tasks from which to continue.</param>
        /// <returns>A task that, when completed, will return the array of completed tasks.</returns>
        public static Task<Task[]> WhenAll(
            this TaskFactory factory, params Task[] tasks) => factory.ContinueWhenAll(tasks, completedTasks => completedTasks);

        /// <summary>
        /// Creates a continuation Task that will compplete upon
        /// the completion of a set of provided Tasks.
        /// </summary>
        /// <param name="factory">The TaskFactory to use to create the continuation task.</param>
        /// <param name="tasks">The array of tasks from which to continue.</param>
        /// <returns>A task that, when completed, will return the array of completed tasks.</returns>
        public static Task<Task<TAntecedentResult>[]> WhenAll<TAntecedentResult>(
            this TaskFactory factory, params Task<TAntecedentResult>[] tasks) => factory.ContinueWhenAll(tasks, completedTasks => completedTasks) as Task<Task<TAntecedentResult>[]>;

        /// <summary>
        /// Creates a continuation Task that will complete upon
        /// the completion of any one of a set of provided Tasks.
        /// </summary>
        /// <param name="factory">The TaskFactory to use to create the continuation task.</param>
        /// <param name="tasks">The array of tasks from which to continue.</param>
        /// <returns>A task that, when completed, will return the completed task.</returns>
        public static Task<Task> WhenAny(
            this TaskFactory factory, params Task[] tasks) => factory.ContinueWhenAny(tasks, completedTask => completedTask);

        /// <summary>
        /// Creates a continuation Task that will complete upon
        /// the completion of any one of a set of provided Tasks.
        /// </summary>
        /// <param name="factory">The TaskFactory to use to create the continuation task.</param>
        /// <param name="tasks">The array of tasks from which to continue.</param>
        /// <returns>A task that, when completed, will return the completed task.</returns>
        public static Task<Task<TAntecedentResult>> WhenAny<TAntecedentResult>(
            this TaskFactory factory, params Task<TAntecedentResult>[] tasks) => factory.ContinueWhenAny(tasks, completedTask => completedTask);
    }
}