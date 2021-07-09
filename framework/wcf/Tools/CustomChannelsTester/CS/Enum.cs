//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace Microsoft.Samples.Tools.CustomChannelsTester
{
    //Contract options possible through the Test Spec
    public enum ContractOption
    {
        True,
        False,
        Both,
    }

    //All possible types of service contracts specified in the Interfaces.cs
    public enum ServiceContract
    {
        IAsyncSessionOneWay,
        IAsyncSessionTwoWay,
        IAsyncOneWay,
        IAsyncTwoWay,
        ISyncSessionOneWay,
        ISyncSessionTwoWay,
        ISyncOneWay,
        ISyncTwoWay,
        IDuplexContract,
        IDuplexSessionContract
    }
}
