//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

 using System.ServiceModel;
 using System.ServiceModel.Channels;

namespace Microsoft.Samples.WSStreamedHttpBinding
{
    public enum StreamedTransferMode
    {
        Unknown = 0,
        Streamed = TransferMode.Streamed,
        StreamedRequest = TransferMode.StreamedRequest,
        StreamedResponse = TransferMode.StreamedResponse
    }
}
