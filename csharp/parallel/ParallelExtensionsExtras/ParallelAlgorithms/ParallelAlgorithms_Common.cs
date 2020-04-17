//--------------------------------------------------------------------------
// 
//  Copyright (c) Microsoft Corporation.  All rights reserved. 
// 
//  File: ParallelAlgorithms_Common.cs
//
//--------------------------------------------------------------------------

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
