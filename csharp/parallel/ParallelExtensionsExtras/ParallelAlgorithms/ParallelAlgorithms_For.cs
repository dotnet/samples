//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace System.Threading.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Executes a for loop in which iterations may run in parallel.</summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        public static void For(BigInteger fromInclusive, BigInteger toExclusive, Action<BigInteger> body) =>
            For(fromInclusive, toExclusive, s_defaultParallelOptions, body);

        /// <summary>Executes a for loop in which iterations may run in parallel.</summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="options">A System.Threading.Tasks.ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        public static void For(BigInteger fromInclusive, BigInteger toExclusive, ParallelOptions options, Action<BigInteger> body)
        {
            // Determine how many iterations to run...
            var range = toExclusive - fromInclusive;

            // ... and run them.
            if (range <= 0)
            {
                // If there's nothing to do, bail
                return;
            }
            // Fast path
            else if (range <= long.MaxValue)
            {
                // If the range is within the realm of Int64, we'll delegate to Parallel.For's Int64 overloads.
                // Iterate from 0 to range, and then call the user-provided body with the scaled-back value.
                Parallel.For(0, (long)range, options, i => body(i + fromInclusive));
            }
            // Slower path
            else
            {
                // For a range larger than Int64.MaxValue, we'll rely on an enumerable of BigInteger.
                // We create a C# iterator that yields all of the BigInteger values in the requested range
                // and then ForEach over that range.
                Parallel.ForEach(Range(fromInclusive, toExclusive), options, body);
            }
        }

        /// <summary>Creates an enumerable that iterates the range [fromInclusive, toExclusive).</summary>
        /// <param name="fromInclusive">The lower bound, inclusive.</param>
        /// <param name="toExclusive">The upper bound, exclusive.</param>
        /// <returns>The enumerable of the range.</returns>
        private static IEnumerable<BigInteger> Range(BigInteger fromInclusive, BigInteger toExclusive)
        {
            for (var i = fromInclusive; i < toExclusive; i++) yield return i;
        }
    }
}
