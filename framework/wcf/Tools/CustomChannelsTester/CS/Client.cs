//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Samples.Tools.CustomChannelsTester
{
    public class Client
    {
        //Definition of all the ChannelFactory per ServiceContract
        IChannelFactory<IAsyncOneWay> asyncOneWayChannelFactory;
        IChannelFactory<IAsyncSessionOneWay> asyncSessionOneWayChannelFactory;
        IChannelFactory<IAsyncTwoWay> asyncTwoWayChannelFactory;
        IChannelFactory<IAsyncSessionTwoWay> asyncSessionTwoWayChannelFactory;
        IChannelFactory<ISyncOneWay> syncOneWayChannelFactory;
        IChannelFactory<ISyncSessionOneWay> syncSessionOneWayChannelFactory;
        IChannelFactory<ISyncTwoWay> syncTwoWayChannelFactory;
        IChannelFactory<ISyncSessionTwoWay> syncSessionTwoWayChannelFactory;
        DuplexChannelFactory<IDuplexContract> duplexChannelFactory;
        DuplexChannelFactory<IDuplexSessionContract> duplexSessionChannelFactory;
        
        AutoResetEvent clientStartup;
        
        List<Thread> clientThreads = new List<Thread>();

        CallBackService cb = null;
        InstanceContext ic = null;
        
        public Client()
        {
            clientStartup = new AutoResetEvent(false);
            cb = new CallBackService();
            ic = new InstanceContext(cb);
        }

        public void ExecuteClient()
        {
            //Waiting for Server Setup
            Log.Trace("Waiting for Server Setup");
            clientStartup.WaitOne(Parameters.ClientTimeout, true);
            Binding binding = Util.GetBinding(Parameters.BindingName);
            Uri baseUri = Util.GetUri(binding); 
            InitializeClient(baseUri, binding);
            RunClient(baseUri);
            CleanupClient();
            Parameters.Result = true;
            Log.Trace("Clients Done ...");
            
        }

        //Create the channel factory based on the Service Contracts selected by the user
        private void InitializeClient(Uri baseUri, Binding binding)
        {
            Log.Trace("Binding == " + binding.Name);
            Log.Trace("Initializing Clients ...");
            string uriString = baseUri.ToString();
            Log.Trace("Client Connecting to base Uri == " + baseUri);
            EndpointAddress epa;           
            foreach (ServiceContract serviceContract in Parameters.ServiceContracts)
            {
                string addressString = serviceContract.ToString();
                switch (serviceContract)
                {
                    case ServiceContract.IAsyncOneWay:
                        epa = new EndpointAddress(uriString + addressString);
                        asyncOneWayChannelFactory = new ChannelFactory<IAsyncOneWay>(binding, epa);
                        break;

                    case ServiceContract.IAsyncSessionOneWay:
                        epa = new EndpointAddress(uriString + addressString);
                        asyncSessionOneWayChannelFactory = new ChannelFactory<IAsyncSessionOneWay>(binding, epa);
                        break;

                    case ServiceContract.IAsyncTwoWay:
                        epa = new EndpointAddress(uriString + addressString);
                        asyncTwoWayChannelFactory = new ChannelFactory<IAsyncTwoWay>(binding, epa);
                        break;

                    case ServiceContract.IAsyncSessionTwoWay:
                        epa = new EndpointAddress(uriString + addressString);
                        asyncSessionTwoWayChannelFactory = new ChannelFactory<IAsyncSessionTwoWay>(binding, epa);
                        break;

                    case ServiceContract.ISyncOneWay:
                        epa = new EndpointAddress(uriString + addressString);
                        syncOneWayChannelFactory = new ChannelFactory<ISyncOneWay>(binding, epa);
                        break;

                    case ServiceContract.ISyncSessionOneWay:
                        epa = new EndpointAddress(uriString + addressString);
                        syncSessionOneWayChannelFactory = new ChannelFactory<ISyncSessionOneWay>(binding, epa);
                        break;

                    case ServiceContract.ISyncTwoWay:
                        epa = new EndpointAddress(uriString + addressString);
                        syncTwoWayChannelFactory = new ChannelFactory<ISyncTwoWay>(binding, epa);
                        break;

                    case ServiceContract.ISyncSessionTwoWay:
                        epa = new EndpointAddress(uriString + addressString);
                        syncSessionTwoWayChannelFactory = new ChannelFactory<ISyncSessionTwoWay>(binding, epa);
                        break;

                    case ServiceContract.IDuplexContract:
                        epa = new EndpointAddress(uriString + addressString);
                        duplexChannelFactory = new DuplexChannelFactory<IDuplexContract>(cb, binding, epa);
                        break;

                    case ServiceContract.IDuplexSessionContract:
                        epa = new EndpointAddress(uriString + addressString);
                        duplexSessionChannelFactory = new DuplexChannelFactory<IDuplexSessionContract>(cb,binding, epa);
                        break;

                    default:
                        Log.Trace(serviceContract +  " type is not supported");
                        break;
                }
            }
        }

        //Creating number of threads = number of clients from the config file
        private void RunClient(Uri baseUri)
        {
            Log.Trace("Clients Ready ...");
            for (int i = 0; i < Parameters.NumberOfClients; i++)
            {
                Thread messageSendingThread = new Thread(new ParameterizedThreadStart(SendMessages));
                messageSendingThread.Name = "ClientThread" + i;
                messageSendingThread.Start(baseUri);
                clientThreads.Add(messageSendingThread);
            }
            foreach (Thread messageSendingThread in clientThreads)
            {
                messageSendingThread.Join();
            }
        }

        //Closing the channelFactory  - this also closes all the channels created by this channelFactory
        private void CleanupClient()
        {
            Log.Trace("Clients Cleanup ...");
            foreach (ServiceContract serviceContract in Parameters.ServiceContracts)
            {
                switch (serviceContract)
                {
                    case ServiceContract.IAsyncOneWay:
                        asyncOneWayChannelFactory.Close();
                        break;

                    case ServiceContract.IAsyncSessionOneWay:
                        asyncSessionOneWayChannelFactory.Close();
                        break;

                    case ServiceContract.IAsyncTwoWay:
                        asyncTwoWayChannelFactory.Close();
                        break;

                    case ServiceContract.IAsyncSessionTwoWay:
                        asyncSessionTwoWayChannelFactory.Close();
                        break;

                    case ServiceContract.ISyncOneWay:
                        syncOneWayChannelFactory.Close();
                        break;

                    case ServiceContract.ISyncSessionOneWay:
                        syncSessionOneWayChannelFactory.Close();
                        break;

                    case ServiceContract.ISyncTwoWay:
                        syncTwoWayChannelFactory.Close();
                        break;

                    case ServiceContract.ISyncSessionTwoWay:
                        syncSessionTwoWayChannelFactory.Close();
                        break;
                    case ServiceContract.IDuplexContract:
                        duplexChannelFactory.Close();
                        break;

                    case ServiceContract.IDuplexSessionContract:
                        duplexSessionChannelFactory.Close();
                        break;

                    default:
                        Log.Trace(serviceContract + " type is not supported");
                        break;
                }
            }
            
        }

        //Sending messages after creation of proxy per client
        private void SendMessages(object uriObject)
        {
            Uri baseUri = (Uri)uriObject;
            string uriString = baseUri.ToString();
            EndpointAddress epa;
            foreach (ServiceContract serviceContract in Parameters.ServiceContracts)
            {
                switch (serviceContract)
                {
                    case ServiceContract.IAsyncOneWay:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        IAsyncOneWay asyncOneWay = asyncOneWayChannelFactory.CreateChannel(epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                        {
                            IAsyncResult res = asyncOneWay.BeginOneWayMethod
                                ("AsyncOneWay Sending Message to Server == " + i, null, asyncOneWay);
                            asyncOneWay.EndOneWayMethod(res);
                        }
                        ((IChannel)asyncOneWay).Close();
                        break;

                    case ServiceContract.IAsyncSessionOneWay:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        IAsyncSessionOneWay asyncSessionOneWay = asyncSessionOneWayChannelFactory.CreateChannel(epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                        {
                            IAsyncResult res = asyncSessionOneWay.BeginSessionOneWayMethod
                                ("AsyncSessionOneWay Sending Message to Server == " + i, null, asyncSessionOneWay);
                            asyncSessionOneWay.EndSessionOneWayMethod(res);
                        }
                        ((IChannel)asyncSessionOneWay).Close();
                        break;

                    case ServiceContract.IAsyncTwoWay:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        IAsyncTwoWay asyncTwoWay = asyncTwoWayChannelFactory.CreateChannel(epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                        {
                            IAsyncResult res = asyncTwoWay.BeginTwoWayMethod
                                ("AsyncTwoWay Sending Message to Server == " + i, null, asyncTwoWay);
                            string msg = asyncTwoWay.EndTwoWayMethod(res);
                            Log.Trace("AsyncTwoWay received message == " + msg);
                        }
                        ((IChannel)asyncTwoWay).Close();
                        break;

                    case ServiceContract.IAsyncSessionTwoWay:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        IAsyncSessionTwoWay asyncSessionTwoWay = asyncSessionTwoWayChannelFactory.CreateChannel(epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                        {
                            IAsyncResult res = asyncSessionTwoWay.BeginSessionTwoWayMethod
                                ("AsyncSessionTwoWay Sending Message to Server == " + i, null, asyncSessionTwoWay);
                            string msg = asyncSessionTwoWay.EndSessionTwoWayMethod(res);
                            Log.Trace("AsyncSessionTwoWay received message == " + msg);
                        }
                        ((IChannel)asyncSessionTwoWay).Close();
                        break;

                    case ServiceContract.ISyncOneWay:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        ISyncOneWay syncOneWay = syncOneWayChannelFactory.CreateChannel(epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                            syncOneWay.OneWay("SyncOneWay Sending Message to Server == " + i);
                        ((IChannel)syncOneWay).Close();
                        break;

                    case ServiceContract.ISyncSessionOneWay:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        ISyncSessionOneWay syncSessionOneWay = syncSessionOneWayChannelFactory.CreateChannel(epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                            syncSessionOneWay.OneWay("SyncSessionOneWay Sending Message to Server == " + i);
                        ((IChannel)syncSessionOneWay).Close();
                        break;

                    case ServiceContract.ISyncTwoWay:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        ISyncTwoWay syncTwoWay = syncTwoWayChannelFactory.CreateChannel(epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                        {
                            string msg = syncTwoWay.TwoWay("SyncTwoWay Sending Message to Server == " + i);
                            Log.Trace("SyncTwoWay received message == " + msg);
                        }
                        ((IChannel)syncTwoWay).Close();
                        break;

                    case ServiceContract.ISyncSessionTwoWay:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        ISyncSessionTwoWay syncSessionTwoWay = syncSessionTwoWayChannelFactory.CreateChannel(epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                        {
                            string msg = syncSessionTwoWay.TwoWay("SyncSessionTwoWay Sending Message to Server == " + i);
                            Log.Trace("SyncSessionTwoWay received message == " + msg);
                        }
                        ((IChannel)syncSessionTwoWay).Close();
                        break;

                    case ServiceContract.IDuplexContract:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        IDuplexContract duplex = duplexChannelFactory.CreateChannel(ic, epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                        {
                            duplex.DuplexOneWay("DuplexOneWay Sending Message to Server == " + i);
                            Thread.Sleep(1000);
                        }
                        ((IChannel)duplex).Close();
                        break;

                    case ServiceContract.IDuplexSessionContract:
                        epa = new EndpointAddress(uriString + serviceContract.ToString());
                        IDuplexSessionContract duplexSession = duplexSessionChannelFactory.CreateChannel(ic, epa);
                        for (int i = 0; i < Parameters.MessagesPerClient; i++)
                        {
                            duplexSession.DuplexOneWay("DuplexSessionOneWay Sending Message to Server == " + i);
                            Thread.Sleep(1000);
                        }
                        ((IChannel)duplexSession).Close();
                        break;

                    default:
                        Log.Trace(serviceContract + " type is not supported");
                        break;
                }
            }
        }
    }
}
