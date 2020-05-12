//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace System.Threading.Tasks.Schedulers
{
    /// <summary>
    /// Provides a task scheduler that ensures only one task is executing at a time, and that tasks
    /// execute in the order that they were queued.
    /// </summary>
    public sealed class OrderedTaskScheduler : LimitedConcurrencyLevelTaskScheduler
    {
        /// <summary>Initializes an instance of the OrderedTaskScheduler class.</summary>
        public OrderedTaskScheduler() : base(1) { }
    }
}
