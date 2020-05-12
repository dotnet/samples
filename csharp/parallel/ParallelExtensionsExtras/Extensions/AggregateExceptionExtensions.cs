//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;

namespace System
{
    /// <summary>Extension methods for AggregateException.</summary>
    public static class AggregateExceptionExtensions
    {
        /// <summary>Invokes a handler on each Exception contained by this AggregateException.</summary>
        /// <param name="aggregateException">The AggregateException.</param>
        /// <param name="predicate">
        /// The predicate to execute for each exception. The predicate accepts as an argument the Exception
        /// to be processed and returns a Boolean to indicate whether the exception was handled.
        /// </param>
        /// <param name="leaveStructureIntact">
        /// Whether the rethrown AggregateException should maintain the same hierarchy as the original.
        /// </param>
        public static void Handle(
            this AggregateException aggregateException,
            Func<Exception, bool> predicate, bool leaveStructureIntact)
        {
            if (aggregateException == null) throw new ArgumentNullException(nameof(aggregateException));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            // If leaveStructureIntact, use this implementation
            if (leaveStructureIntact)
            {
                var result = HandleRecursively(aggregateException, predicate);
                if (result != null) throw result;
            }
            // Otherwise, default back to the implementation on AggregateException
            else aggregateException.Handle(predicate);
        }

        private static AggregateException HandleRecursively(
            AggregateException aggregateException, Func<Exception, bool> predicate)
        {
            // Maintain a list of exceptions to be rethrown
            List<Exception> innerExceptions = null;

            // Loop over all of the inner exceptions
            foreach (var inner in aggregateException.InnerExceptions)
            {
                // If the inner exception is itself an aggregate, process recursively
                if (inner is AggregateException innerAsAggregate)
                {
                    // Process recursively, and if we get back a new aggregate, store it
                    AggregateException newChildAggregate = HandleRecursively(innerAsAggregate, predicate);
                    if (newChildAggregate != null)
                    {
                        if (innerExceptions != null) innerExceptions = new List<Exception>();
                        innerExceptions.Add(newChildAggregate);
                    }
                }
                // Otherwise, if the exception does not match the filter, store it
                else if (!predicate(inner))
                {
                    if (innerExceptions != null) innerExceptions = new List<Exception>();
                    innerExceptions.Add(inner);
                }
            }

            // If there are any remaining exceptions, return them in a new aggregate.
            return innerExceptions.Count > 0 ?
                new AggregateException(aggregateException.Message, innerExceptions) :
                null;
        }
    }
}
