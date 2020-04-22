//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Diagnostics;
using System.Linq;

namespace System
{
    /// <summary>Parallel extensions for the Delegate class.</summary>
    public static class DelegateExtensions
    {
        /// <summary>Dynamically invokes (late-bound) in parallel the methods represented by the delegate.</summary>
        /// <param name="multicastDelegate">The delegate to be invoked.</param>
        /// <param name="args">An array of objects that are the arguments to pass to the delegates.</param>
        /// <returns>The return value of one of the delegate invocations.</returns>
        public static object ParallelDynamicInvoke(this Delegate multicastDelegate, params object[] args)
        {
            if (multicastDelegate == null) throw new ArgumentNullException(nameof(multicastDelegate));
            if (args == null) throw new ArgumentNullException(nameof(args));
            return multicastDelegate.GetInvocationList()
                   .AsParallel().AsOrdered()
                   .Select(d => d.DynamicInvoke(args))
                   .Last();
        }

        /// <summary>
        /// Provides a delegate that runs the specified action and fails fast if the action throws an exception.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The wrapper delegate.</returns>
        public static Action WithFailFast(this Action action) => () =>
        {
            try { action(); }
            catch (Exception exc)
            {
                if (Debugger.IsAttached) Debugger.Break();
                else Environment.FailFast("An unhandled exception occurred.", exc);
            }
        };

        /// <summary>
        /// Provides a delegate that runs the specified function and fails fast if the function throws an exception.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <returns>The wrapper delegate.</returns>
        public static Func<T> WithFailFast<T>(this Func<T> function) => () =>
        {
            try { return function(); }
            catch (Exception exc)
            {
                if (Debugger.IsAttached) Debugger.Break();
                else Environment.FailFast("An unhandled exception occurred.", exc);
            }
            throw new Exception("Will never get here");
        };
    }
}
