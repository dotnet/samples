//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace System.Threading
{
    /// <summary>Extension methods for CancellationToken.</summary>
    public static class CancellationTokenExtensions
    {
        /// <summary>Cancels a CancellationTokenSource and throws a corresponding OperationCanceledException.</summary>
        /// <param name="source">The source to be canceled.</param>
        public static void CancelAndThrow(this CancellationTokenSource source)
        {
            source.Cancel();
            source.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Creates a CancellationTokenSource that will be canceled when the specified token has cancellation requested.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The created CancellationTokenSource.</returns>
        public static CancellationTokenSource CreateLinkedSource(this CancellationToken token) => CancellationTokenSource.CreateLinkedTokenSource(token, new CancellationToken());
    }
}
