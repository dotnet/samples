//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.Tools.CustomChannelsTester
{
    //Definition of all the service contracts against which the tests will run

    [ServiceContract (SessionMode = SessionMode.Allowed)]
    public interface IAsyncSessionOneWay
    {
        [OperationContract(IsOneWay = true, AsyncPattern = true)]
        IAsyncResult BeginSessionOneWayMethod(string msg, AsyncCallback callback, object state);
        void EndSessionOneWayMethod(IAsyncResult result);
    }

    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IAsyncSessionTwoWay
    {
        [OperationContract(IsOneWay = false, AsyncPattern = true)]
        IAsyncResult BeginSessionTwoWayMethod(string msg, AsyncCallback callback, object state);
        string EndSessionTwoWayMethod(IAsyncResult result);
    }

    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IAsyncOneWay
    {
        [OperationContract(IsOneWay = true, AsyncPattern = true)]
        IAsyncResult BeginOneWayMethod(string msg, AsyncCallback callback, object state);
        void EndOneWayMethod(IAsyncResult result);
    }

    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IAsyncTwoWay
    {
        [OperationContract(IsOneWay = false, AsyncPattern = true)]
        IAsyncResult BeginTwoWayMethod(string msg, AsyncCallback callback, object state);
        string EndTwoWayMethod(IAsyncResult result);
    }

    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface ISyncSessionOneWay
    {
        [OperationContract(IsOneWay = true, AsyncPattern = false)]
        void OneWay(string msg);
    }

    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface ISyncSessionTwoWay
    {
        [OperationContract(IsOneWay = false, AsyncPattern = false)]
        string TwoWay(string msg);
    }

    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface ISyncOneWay
    {
        [OperationContract(IsOneWay = true, AsyncPattern = false)]
        void OneWay(string msg);
    }

    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface ISyncTwoWay
    {
        [OperationContract(IsOneWay = false, AsyncPattern = false)]
        string TwoWay(string msg);
    }

    public interface ICallBack
    {
        [OperationContract]
        void CallBackMethod(string msg);        
    }

    [ServiceContract(CallbackContract = typeof(ICallBack), SessionMode = SessionMode.Allowed)]
    public interface IDuplexSessionContract
    {
        [OperationContract(IsOneWay = true)]
        void DuplexOneWay(string msg);
    }

    [ServiceContract(CallbackContract = typeof(ICallBack), SessionMode = SessionMode.NotAllowed)]
    public interface IDuplexContract
    {
        [OperationContract(IsOneWay = true)]
        void DuplexOneWay(string msg);
    }   

}
