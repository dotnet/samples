//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace System.Threading.Tasks
{
    /// <summary>Extensions for TaskFactory.</summary>
    public static partial class TaskFactoryExtensions
    {
        /// <summary>Creates a Task that will be completed when the specified WaitHandle is signaled.</summary>
        /// <param name="factory">The target factory.</param>
        /// <param name="waitHandle">The WaitHandle.</param>
        /// <returns>The created Task.</returns>
        public static Task FromAsync(this TaskFactory factory, WaitHandle waitHandle)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (waitHandle == null) throw new ArgumentNullException(nameof(waitHandle));

            var tcs = new TaskCompletionSource<object>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle, delegate { tcs.TrySetResult(null); }, null, -1, true);
            var t = tcs.Task;
            t.ContinueWith(_ => rwh.Unregister(null), TaskContinuationOptions.ExecuteSynchronously);
            return t;
        }
    }
}
