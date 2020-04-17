//--------------------------------------------------------------------------
// 
//  Copyright (c) Microsoft Corporation.  All rights reserved. 
// 
//  File: ParallelOptionsExtensions.cs
//
//--------------------------------------------------------------------------

namespace System.Threading.Tasks
{
    /// <summary>Extension methods for ParallelOptions.</summary>
    public static class ParallelOptionsExtensions
    {
        /// <summary>Copies a ParallelOptions instance to a shallow clone.</summary>
        /// <param name="options">The options to be cloned.</param>
        /// <returns>The shallow clone.</returns>
        public static ParallelOptions ShallowClone(this ParallelOptions options) => new ParallelOptions
        {
            CancellationToken = options.CancellationToken,
            MaxDegreeOfParallelism = options.MaxDegreeOfParallelism,
            TaskScheduler = options.TaskScheduler
        };
    }
}
