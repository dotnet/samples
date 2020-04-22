//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

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
