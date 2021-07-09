//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Samples.Tools.CustomChannelsTester
{
    public static class Parameters
    {
        private static string inputFileName;

        public static string InputFileName
        {
            get { return inputFileName; }
            set { inputFileName = value; }
        }

        private static string serverMachineName = Environment.MachineName.ToLower();

        public static string ServerMachineName
        {
            get { return serverMachineName; }
            set { serverMachineName = value; }
        }

        private static string bindingName;

        public static string BindingName
        {
            get { return Parameters.bindingName; }
            set { Parameters.bindingName = value; }
        }        

        private static List<ServiceContract> serviceContracts = new List<ServiceContract>();

        public static List<ServiceContract> ServiceContracts
        {
            get { return serviceContracts; }
            set { serviceContracts = value; }
        }

        private static ContractOption isAsync = ContractOption.Both;
        public static ContractOption IsAsync
        {
            get { return isAsync; }
            set { isAsync = value; }
        }
        private static ContractOption isSession = ContractOption.Both;
        public static ContractOption IsSession
        {
            get { return isSession; }
            set { isSession = value; }
        }
        private static ContractOption isOneWay = ContractOption.Both;

        public static ContractOption IsOneWay
        {
            get { return isOneWay; }
            set { isOneWay = value; }
        }

        private static ContractOption isCallBack = ContractOption.False;

        public static ContractOption IsCallBack
        {
            get { return isCallBack; }
            set { isCallBack = value; }
        }

        private static int numberOfClients = 1;

        public static int NumberOfClients
        {
            get { return numberOfClients; }
            set { numberOfClients = value; }
        }

        private static int messagesPerClient = 1;

        public static int MessagesPerClient
        {
            get { return messagesPerClient; }
            set { messagesPerClient = value; }
        }

        private static TimeSpan serverTimeout = new TimeSpan(0, 0, 120);

        public static TimeSpan ServerTimeout
        {
            get { return serverTimeout; }
            set { serverTimeout = value; }
        }

        private static TimeSpan clientTimeout = new TimeSpan(0, 0, 30);

        public static TimeSpan ClientTimeout
        {
            get { return Parameters.clientTimeout; }
            set { Parameters.clientTimeout = value; }
        }        

        private static int totalMessages = 1;

        public static int TotalMessages
        {
            get { return totalMessages; }
            set { totalMessages = value; }
        }

        private static Assembly bindingAssembly;

        public static Assembly BindingAssembly
        {
            get { return Parameters.bindingAssembly; }
            set { Parameters.bindingAssembly = value; }
        }

        private static Type bindingType;

        public static Type BindingType
        {
            get { return Parameters.bindingType; }
            set { Parameters.bindingType = value; }
        }

        private static int serverPortNumber = 8000;

        public static int ServerPortNumber
        {
            get { return Parameters.serverPortNumber; }
            set { Parameters.serverPortNumber = value; }
        }

        private static Uri clientCallBackAddress = null;

        public static Uri ClientCallBackAddress
        {
            get { return Parameters.clientCallBackAddress; }
            set { Parameters.clientCallBackAddress = value; }
        }        

        private static bool result = false;

        public static bool Result
        {
            get { return Parameters.result; }
            set { Parameters.result = value; }
        }


        

    }
}
