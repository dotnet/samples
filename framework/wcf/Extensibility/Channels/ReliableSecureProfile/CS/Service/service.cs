//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace Microsoft.Samples.ReliableSecureProfile
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(ProcessDataService));
            host.Open();
            Console.WriteLine("The service host has started...");
            Console.WriteLine("Press <ENTER> to terminate the service.");
            Console.ReadLine();
        }
    }

    public class ProcessDataService : IProcessDataDuplex
    {
        public void ProcessData(string rawData)
        {
            IProcessDataDuplexCallBack callback = null;
            callback = OperationContext.Current.GetCallbackChannel<IProcessDataDuplexCallBack>();
            Console.WriteLine("Client called method on the service...");
            // This is to simulate the delay of 'processing' the data
            System.Threading.Thread.Sleep(5000);
            string processedData = 
                ByteArrayToString(new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(rawData)));
            callback.SendProcessedData(String.Format("Computed hash of '{0}' is '{1}'", rawData, processedData));
            Console.WriteLine("Service sent back computed hash on the callback channel...");
        }

        static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }
}
