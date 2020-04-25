//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{
    /// <summary>
    /// Provides LINQ support for Tasks by implementing the primary standard query operators.
    /// </summary>
    public static class LinqToTasks
    {
        public static Task<TResult> Select<TSource, TResult>(this Task<TSource> source, Func<TSource, TResult> selector)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            // Use a continuation to run the selector function
            return source.ContinueWith(t => selector(t.Result), TaskContinuationOptions.NotOnCanceled);
        }

        public static Task<TResult> SelectMany<TSource, TResult>(this Task<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            // Use a continuation to run the selector function.
            return source.ContinueWith(t => selector(t.Result), TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        public static Task<TResult> SelectMany<TSource, TCollection, TResult>(
            this Task<TSource> source,
            Func<TSource, Task<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            // When the source completes, run the collectionSelector to get the next Task,
            // and continue off of it to run the result selector
            return source.ContinueWith(t =>
            {
                return collectionSelector(t.Result).
                    ContinueWith(c => resultSelector(t.Result, c.Result), TaskContinuationOptions.NotOnCanceled);
            }, TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        public static Task<TSource> Where<TSource>(this Task<TSource> source, Func<TSource, bool> predicate)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            // Create a continuation to run the predicate and return the source's result.
            // If the predicate returns false, cancel the returned Task.
            var cts = new CancellationTokenSource();
            return source.ContinueWith(t =>
            {
                var result = t.Result;
                if (!predicate(result)) cts.CancelAndThrow();
                return result;
            }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
        }

        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>(
            this Task<TOuter> outer, Task<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector) =>
            // Argument validation handled by delegated method call
            Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);

        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>(
            this Task<TOuter> outer, Task<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            // Validate arguments
            if (outer == null) throw new ArgumentNullException(nameof(outer));
            if (inner == null) throw new ArgumentNullException(nameof(inner));
            if (outerKeySelector == null) throw new ArgumentNullException(nameof(outerKeySelector));
            if (innerKeySelector == null) throw new ArgumentNullException(nameof(innerKeySelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            // First continue off of the outer and then off of the inner.  Two separate
            // continuations are used so that each may be canceled easily using the NotOnCanceled option.
            return outer.ContinueWith(delegate
            {
                var cts = new CancellationTokenSource();
                return inner.ContinueWith(delegate
                {
                    // Propagate all exceptions
                    Task.WaitAll(outer, inner);

                    // Both completed successfully, so if their keys are equal, return the result
                    if (comparer.Equals(outerKeySelector(outer.Result), innerKeySelector(inner.Result)))
                    {
                        return resultSelector(outer.Result, inner.Result);
                    }
                    // Otherwise, cancel this task.  
                    else
                    {
                        cts.CancelAndThrow();
                        return default; // won't be reached
                    }
                }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
            }, TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        public static Task<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this Task<TOuter> outer, Task<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, Task<TInner>, TResult> resultSelector) =>
            // Argument validation handled by delegated method call
            GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);

        public static Task<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this Task<TOuter> outer, Task<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, Task<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            // Validate arguments
            if (outer == null) throw new ArgumentNullException(nameof(outer));
            if (inner == null) throw new ArgumentNullException(nameof(inner));
            if (outerKeySelector == null) throw new ArgumentNullException(nameof(outerKeySelector));
            if (innerKeySelector == null) throw new ArgumentNullException(nameof(innerKeySelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            // First continue off of the outer and then off of the inner.  Two separate
            // continuations are used so that each may be canceled easily using the NotOnCanceled option.
            return outer.ContinueWith(delegate
            {
                var cts = new CancellationTokenSource();
                return inner.ContinueWith(delegate
                {
                    // Propagate all exceptions
                    Task.WaitAll(outer, inner);

                    // Both completed successfully, so if their keys are equal, return the result
                    if (comparer.Equals(outerKeySelector(outer.Result), innerKeySelector(inner.Result)))
                    {
                        return resultSelector(outer.Result, inner);
                    }
                    // Otherwise, cancel this task.
                    else
                    {
                        cts.CancelAndThrow();
                        return default; // won't be reached
                    }
                }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
            }, TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        public static Task<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(
            this Task<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

            // When the source completes, return a grouping of just the one element
            return source.ContinueWith(t =>
            {
                var result = t.Result;
                var key = keySelector(result);
                var element = elementSelector(result);
                return (IGrouping<TKey, TElement>)new OneElementGrouping<TKey, TElement> { Key = key, Element = element };
            }, TaskContinuationOptions.NotOnCanceled);
        }

        /// <summary>Represents a grouping of one element.</summary>
        /// <typeparam name="TKey">The type of the key for the element.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        private class OneElementGrouping<TKey, TElement> : IGrouping<TKey, TElement>
        {
            public TKey Key { get; internal set; }
            internal TElement Element { get; set; }
            public IEnumerator<TElement> GetEnumerator() { yield return Element; }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public static Task<TSource> OrderBy<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector)
        {
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source;
        }

        public static Task<TSource> OrderByDescending<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector)
        {
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source;
        }

        public static Task<TSource> ThenBy<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector)
        {
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source;
        }

        public static Task<TSource> ThenByDescending<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector)
        {
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source;
        }
    }
}
