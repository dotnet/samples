//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;

namespace System.Threading.Tasks
{
    /// <summary>Represents a queue of tasks to be started and executed serially.</summary>
    public class SerialTaskQueue
    {
        /// <summary>The ordered queue of tasks to be executed. Also serves as a lock protecting all shared state.</summary>
        private readonly Queue<object> _tasks = new Queue<object>();
        /// <summary>The task currently executing, or null if there is none.</summary>
        private Task _taskInFlight;

        /// <summary>Enqueues the task to be processed serially and in order.</summary>
        /// <param name="taskGenerator">The function that generates a non-started task.</param>
        public void Enqueue(Func<Task> taskGenerator) => EnqueueInternal(taskGenerator);

        /// <summary>Enqueues the non-started task to be processed serially and in order.</summary>
        /// <param name="task">The task.</param>
        public Task Enqueue(Task task) { EnqueueInternal(task); return task; }

        /// <summary>Gets a Task that represents the completion of all previously queued tasks.</summary>
        public Task Completed() => Enqueue(new Task(() => { }));

        /// <summary>Enqueues the task to be processed serially and in order.</summary>
        /// <param name="taskOrFunction">The task or functino that generates a task.</param>
        /// <remarks>The task must not be started and must only be started by this instance.</remarks>
        private void EnqueueInternal(object taskOrFunction)
        {
            // Validate the task
            if (taskOrFunction == null) throw new ArgumentNullException(nameof(taskOrFunction));
            lock (_tasks)
            {
                // If there is currently no task in flight, we'll start this one
                if (_taskInFlight == null) StartTask_CallUnderLock(taskOrFunction);
                // Otherwise, just queue the task to be started later
                else _tasks.Enqueue(taskOrFunction);
            }
        }

        /// <summary>Called when a Task completes to potentially start the next in the queue.</summary>
        /// <param name="ignored">The task that completed.</param>
        private void OnTaskCompletion(Task ignored)
        {
            lock (_tasks)
            {
                // The task completed, so nothing is currently in flight.
                // If there are any tasks in the queue, start the next one.
                _taskInFlight = null;
                if (_tasks.Count > 0) StartTask_CallUnderLock(_tasks.Dequeue());
            }
        }

        /// <summary>Starts the provided task (or function that returns a task).</summary>
        /// <param name="nextItem">The next task or function that returns a task.</param>
        private void StartTask_CallUnderLock(object nextItem)
        {
            if (!(nextItem is Task next)) next = ((Func<Task>)nextItem)();

            if (next.Status == TaskStatus.Created) next.Start();
            _taskInFlight = next;
            next.ContinueWith(OnTaskCompletion);
        }
    }
}
