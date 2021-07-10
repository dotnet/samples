//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Samples.Tools.CustomChannelsTester
{
    public class Server : BaseServer 
    {
        AutoResetEvent serviceCleanup;
        ServiceHost serviceHost;
        
        public Server()
        {
            serviceCleanup = new AutoResetEvent(false);            
        }

        public void ExecuteServer()
        {
            Binding binding = Util.GetBinding(Parameters.BindingName);
            Uri baseUri = Util.GetUri(binding);       
            InitializeServer(baseUri, binding);
            RunServer();
            CleanupServer();
            Log.Trace("Services Done ...");                       
            
        }
        //Set up the services based on the parameters and the service contracts selected by the user
        private void InitializeServer(Uri baseUri, Binding binding)
        {
            Log.Trace("Binding == " + binding.Name);
            Log.Trace("Uri == " + baseUri.ToString());
            Log.Trace("Initializing Services ...");
            serviceHost = new ServiceHost(typeof(BaseServer), baseUri);
            int endpointCount = 0;
            foreach (ServiceContract serviceContract in Parameters.ServiceContracts)
            {
                try
                {                    
                    string addressString = serviceContract.ToString();
                    Log.Trace("Service contract == " + addressString);
                    switch (serviceContract)
                    {
                        case ServiceContract.IAsyncOneWay:
                            serviceHost.AddServiceEndpoint(typeof(IAsyncOneWay), binding, addressString); 
                            break;

                        case ServiceContract.IAsyncSessionOneWay:
                            serviceHost.AddServiceEndpoint(typeof(IAsyncSessionOneWay), binding, addressString);
                            break;

                        case ServiceContract.IAsyncTwoWay:
                            serviceHost.AddServiceEndpoint(typeof(IAsyncTwoWay), binding, addressString);
                            break;

                        case ServiceContract.IAsyncSessionTwoWay:
                            serviceHost.AddServiceEndpoint(typeof(IAsyncSessionTwoWay), binding, addressString);
                            break;

                        case ServiceContract.ISyncOneWay:
                            serviceHost.AddServiceEndpoint(typeof(ISyncOneWay), binding, addressString);
                            break;

                        case ServiceContract.ISyncSessionOneWay:
                            serviceHost.AddServiceEndpoint(typeof(ISyncSessionOneWay), binding, addressString);
                            break;

                        case ServiceContract.ISyncTwoWay:
                            serviceHost.AddServiceEndpoint(typeof(ISyncTwoWay), binding, addressString);
                            break;

                        case ServiceContract.ISyncSessionTwoWay:
                            serviceHost.AddServiceEndpoint(typeof(ISyncSessionTwoWay), binding, addressString);
                            break;

                        case ServiceContract.IDuplexContract:
                            serviceHost.AddServiceEndpoint(typeof(IDuplexContract), binding, addressString);
                            break;

                        case ServiceContract.IDuplexSessionContract:
                            serviceHost.AddServiceEndpoint(typeof(IDuplexSessionContract), binding, addressString);
                            break;

                        default:
                            Log.Trace(serviceContract + " type is not supported");
                            break;
                    }                    
                    Log.Trace("Listening on Endpoint :: " + serviceHost.Description.Endpoints[endpointCount++].Address);                    
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                    throw;
                }                               
            }
            try
            {
                serviceHost.Open();
            }
            catch (Exception e)
            {
                Log.Exception(e);
                throw;
            } 
        }

        //Server will for the Client to send the messages. Verifies that it gets the correct number of expected messages
        private void RunServer()
        {
            Log.Trace("Services Ready ... ");
            Log.Trace("Total Messages to be received by the Server === " + Parameters.TotalMessages);
            serviceCleanup.WaitOne(Parameters.ServerTimeout, true);
            Log.Trace("Total Messages received by the Server === " + MsgCount);
            if (Parameters.TotalMessages == MsgCount)
                Parameters.Result = true;
        }

        private void CleanupServer()
        {
            Log.Trace("Services Cleanup...");
            serviceHost.Close();            
        }

    }
}
