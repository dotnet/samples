//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Samples.CustomChannelsTester.TestSpec;

namespace Microsoft.Samples.Tools.CustomChannelsTester
{
    //This class parses the XML files and sets up the parameters for the tests accordingly.   
    public class Loader
    {
        readonly SchemaValidator validator;

        public static readonly string TestSpecNamespace = "http://WCF/TestSpec";
        public static readonly string TestSpecXsdLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "testspec.xsd");

        public Loader()
        {
            this.validator = new SchemaValidator(Loader.TestSpecNamespace, Loader.TestSpecXsdLocation);
        }

        internal void LoadSpec(string inputFileName)
        {
            if (Parameters.InputFileName != null)
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(inputFileName);
                validator.Validate(doc);

                XmlSerializer serializer = new XmlSerializer(typeof(TestSpec));

                TextReader reader = new StreamReader(inputFileName);
                TestSpec ts = (TestSpec)serializer.Deserialize(reader);
                reader.Close();

                Parameters.IsAsync = GetContractOption(ts.ServiceContract.IsAsync);
                Parameters.IsSession = GetContractOption(ts.ServiceContract.IsSession);
                Parameters.IsOneWay = GetContractOption(ts.ServiceContract.IsOneWay);
                Parameters.IsCallBack = GetContractOption(ts.ServiceContract.IsCallBack);

                if (ts.TestDetails.ServerMachineName == null)
                    Log.Trace("Running both Server and Client on the same machine");
                else
                    Parameters.ServerMachineName = ts.TestDetails.ServerMachineName;
                
                if (ts.TestDetails.ServerPortNumber == null)
                    Log.Trace("No port number on the server machine specified");
                else
                    Parameters.ServerPortNumber = Int32.Parse(ts.TestDetails.ServerPortNumber);

                if (ts.TestDetails.ClientCallBackAddress == null)
                    Log.Trace("No Client CallBack Address specified, using default address for Duplex CallBack contracts if required");
                else
                {
                    //The ClientBaseAddress specified should be a valid URI
                    try
                    {
                        Parameters.ClientCallBackAddress = new Uri(ts.TestDetails.ClientCallBackAddress);
                    }
                    catch (Exception e)
                    {
                        if (Parameters.IsCallBack != ContractOption.False) 
                            Log.Exception(e);
                    }
                }

                if (ts.TestDetails.ServerTimeout == null)
                    Log.Trace("Default value used - Server will timeout after 120 seconds");
                else
                    Parameters.ServerTimeout = new TimeSpan(0, 0, Int32.Parse(ts.TestDetails.ServerTimeout));
                
                if (ts.TestDetails.ClientTimeout == null)
                    Log.Trace("Default value used - Client will wait for 30 seconds before starting");
                else
                    Parameters.ClientTimeout = new TimeSpan(0, 0, Int32.Parse(ts.TestDetails.ClientTimeout));
                
                if (ts.TestDetails.NumberOfClients == null)
                    Log.Trace("Default value used - Number of Client per Service = 1");
                else
                    Parameters.NumberOfClients = Int32.Parse(ts.TestDetails.NumberOfClients);
                
                if (ts.TestDetails.MessagesPerClient == null)
                    Log.Trace("Default value used - Number of Messages sent per Client = 1");
                else
                    Parameters.MessagesPerClient = Int32.Parse(ts.TestDetails.MessagesPerClient);                
            }
            
            Log.Trace("ServerMachineName = " + Parameters.ServerMachineName);
            Log.Trace("ServerPortNumber Number = " + Parameters.ServerPortNumber);
            if (Parameters.IsCallBack != ContractOption.False)
                Log.Trace("ClientCallBack Address = " + Parameters.ClientCallBackAddress.ToString());
            Log.Trace("ServerTimeout = " + Parameters.ServerTimeout.ToString());
            Log.Trace("ClientTimeout = " + Parameters.ClientTimeout.ToString());
            Log.Trace("NumberOfClients = " + Parameters.NumberOfClients);
            Log.Trace("MessagesPerClient = " + Parameters.MessagesPerClient);
        }

        private ContractOption GetContractOption(Contract contractChoice)
        {
            if (contractChoice == null)
                return ContractOption.False;
            if (contractChoice.ExpandAllSpecified == true && contractChoice.ExpandAll == true)
                return ContractOption.Both;
            else if (contractChoice.Value == false)
                return ContractOption.False;
            else if (contractChoice.Value == true)
                return ContractOption.True;
            else
                return ContractOption.Both;
        }

        internal void UpdateParameters()
        {
            if (Parameters.IsAsync == ContractOption.True || Parameters.IsAsync == ContractOption.Both)
            {
                if(Parameters.IsSession == ContractOption.True || Parameters.IsSession == ContractOption.Both)
                {
                    if(Parameters.IsOneWay == ContractOption.False || Parameters.IsOneWay == ContractOption.Both)
                        Parameters.ServiceContracts.Add(ServiceContract.IAsyncSessionTwoWay);
                    if(Parameters.IsOneWay == ContractOption.True || Parameters.IsOneWay == ContractOption.Both)
                        Parameters.ServiceContracts.Add(ServiceContract.IAsyncSessionOneWay);                    
                }
                if(Parameters.IsSession == ContractOption.False || Parameters.IsSession == ContractOption.Both)
                {
                    if(Parameters.IsOneWay == ContractOption.True || Parameters.IsOneWay == ContractOption.Both)
                        Parameters.ServiceContracts.Add(ServiceContract.IAsyncOneWay);                    
                    if(Parameters.IsOneWay == ContractOption.False || Parameters.IsOneWay == ContractOption.Both)
                        Parameters.ServiceContracts.Add(ServiceContract.IAsyncTwoWay);                   
                }
            }

            if (Parameters.IsAsync == ContractOption.False || Parameters.IsAsync == ContractOption.Both)
            {
                if(Parameters.IsSession == ContractOption.True || Parameters.IsSession == ContractOption.Both)
                {
                    if(Parameters.IsOneWay == ContractOption.False || Parameters.IsOneWay == ContractOption.Both)
                        Parameters.ServiceContracts.Add(ServiceContract.ISyncSessionTwoWay);
                    if(Parameters.IsOneWay == ContractOption.True || Parameters.IsOneWay == ContractOption.Both)
                        Parameters.ServiceContracts.Add(ServiceContract.ISyncSessionOneWay);                    
                }
                if(Parameters.IsSession == ContractOption.False || Parameters.IsSession == ContractOption.Both)
                {
                    if(Parameters.IsOneWay == ContractOption.True || Parameters.IsOneWay == ContractOption.Both)
                        Parameters.ServiceContracts.Add(ServiceContract.ISyncOneWay);                    
                    if(Parameters.IsOneWay == ContractOption.False || Parameters.IsOneWay == ContractOption.Both)
                        Parameters.ServiceContracts.Add(ServiceContract.ISyncTwoWay);                   
                }
            }

            if (Parameters.IsCallBack == ContractOption.True || Parameters.IsCallBack == ContractOption.Both)
            {
                if (Parameters.IsSession == ContractOption.True || Parameters.IsSession == ContractOption.Both)
                    Parameters.ServiceContracts.Add(ServiceContract.IDuplexSessionContract);
                if (Parameters.IsSession == ContractOption.False || Parameters.IsSession == ContractOption.Both)
                    Parameters.ServiceContracts.Add(ServiceContract.IDuplexContract);
            }
            Log.Trace("Number of service contracts ===  " + Parameters.ServiceContracts.Count);

            
            Parameters.TotalMessages = Parameters.NumberOfClients * Parameters.MessagesPerClient * Parameters.ServiceContracts.Count;
            
        }
    }
}
