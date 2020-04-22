//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Threading.Tasks;

namespace System.Threading.Algorithms
{
    /// <summary>
    /// Provides parallelized algorithms for common operations.
    /// </summary>
    public static partial class ParallelAlgorithms
    {
        // Default, shared instance of the ParallelOptions class.  This should not be modified.
        private static readonly ParallelOptions s_defaultParallelOptions = new ParallelOptions();
    }
}
