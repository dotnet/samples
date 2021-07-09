//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.Samples.Federation
{
	[ServiceContract]
	public interface IBrowseBooks
	{
        /// <summary>
        /// Allows callers to get a list of books the BookStore service sells
        /// </summary>
        /// <returns>A list of book titles</returns>
        [OperationContract]
        List<string> BrowseBooks();
	}
}

