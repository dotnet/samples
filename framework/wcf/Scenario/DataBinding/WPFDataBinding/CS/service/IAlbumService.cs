//  Copyright (c) Microsoft Corporation. All rights reserved.

using System.ServiceModel;

namespace Microsoft.Samples.DataBinding
{
    [ServiceContractAttribute(Namespace = "http://Microsoft.Samples.DataBinding")]
    interface IAlbumService
    {
        [OperationContract]
        Album[] GetAlbumList();

		[OperationContract]
        void AddAlbum(string title);
    }
}
