//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Threading
{
    /// <summary>Provides a reduction variable for aggregating data across multiple threads involved in a computation.</summary>
    /// <typeparam name="T">Specifies the type of the data being aggregated.</typeparam>
    [DebuggerDisplay("Count={_values.Count}")]
    [DebuggerTypeProxy(typeof(ReductionVariable_DebugView<>))]
    public sealed class ReductionVariable<T>
    {
        /// <summary>The factory used to initialize a value on a thread.</summary>
        private readonly Func<T> _seedFactory;
        /// <summary>Thread-local storage for each thread's value.</summary>
        private readonly ThreadLocal<StrongBox<T>> _threadLocal;
        /// <summary>The list of all thread-local values for later enumeration.</summary>
        private readonly ConcurrentQueue<StrongBox<T>> _values = new ConcurrentQueue<StrongBox<T>>();

        /// <summary>Initializes the instances.</summary>
        public ReductionVariable() => _threadLocal = new ThreadLocal<StrongBox<T>>(CreateValue);

        /// <summary>Initializes the instances.</summary>
        /// <param name="seedFactory">
        /// The function invoked to provide the initial value for a thread.  
        /// If null, the default value of T will be used as the seed.
        /// </param>
        public ReductionVariable(Func<T> seedFactory) : this() => _seedFactory = seedFactory;

        /// <summary>Creates a value for the current thread and stores it in the central list of values.</summary>
        /// <returns>The boxed value.</returns>
        private StrongBox<T> CreateValue()
        {
            var s = new StrongBox<T>(_seedFactory != null ? _seedFactory() : default);
            _values.Enqueue(s);
            return s;
        }

        /// <summary>Gets or sets the value for the current thread.</summary>
        public T Value
        {
            get => _threadLocal.Value.Value;
            set => _threadLocal.Value.Value = value;
        }

        /// <summary>Gets the values for all of the threads that have used this instance.</summary>
        public IEnumerable<T> Values => _values.Select(s => s.Value);

        /// <summary>Applies an accumulator function over the values in this variable.</summary>
        /// <param name="function">An accumulator function to be invoked on each value.</param>
        /// <returns>The accumulated value.</returns>
        public T Reduce(Func<T, T, T> function) => Values.Aggregate(function);

        /// <summary>
        /// Applies an accumulator function over the values in this variable.
        /// The specified seed is used as the initial accumulator value.
        /// </summary>
        /// <param name="function">An accumulator function to be invoked on each value.</param>
        /// <returns>The accumulated value.</returns>
        public TAccumulate Reduce<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> function) =>
            Values.Aggregate(seed, function);
    }

    /// <summary>Debug view for the reductino variable</summary>
    /// <typeparam name="T">Specifies the type of the data being aggregated.</typeparam>
    internal sealed class ReductionVariable_DebugView<T>
    {
        private readonly ReductionVariable<T> _variable;

        public ReductionVariable_DebugView(ReductionVariable<T> variable) => _variable = variable;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Values => _variable.Values.ToArray();
    }
}
