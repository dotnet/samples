//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Threading;

namespace Microsoft.ServiceModel.Samples
{
    [ServiceContract]
    interface IPoolService
    {
        [OperationContract]
        string GetData();

        [OperationContract]
        string GetDataNoPooling();
    }
        
    [ObjectPooling]    
    public class PoolService : IPoolService, IObjectControl
    {
        private bool canBePooled;

        public PoolService()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Service instance created.");
            Console.ForegroundColor = ConsoleColor.Gray;

            canBePooled = true;
        }

        public string GetData()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Pool Service GetData() invoked.");
            Console.ForegroundColor = ConsoleColor.Gray;

            return "Reply from Pool Service.";
        }

        public string GetDataNoPooling()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Pool Service GetDataNoPooling() invoked.");
            Console.ForegroundColor = ConsoleColor.Gray;
            
            this.canBePooled = false;
            return "Reply from no pooling Service method.";
        }

        #region IObjectControl Members

        public void Activate()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Activate method called.");
            Console.ForegroundColor = ConsoleColor.Gray;

        }

        public void Deactivate()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Deactivate method called.");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public bool CanBePooled
        {
            get
            {
                return this.canBePooled;
            }
            set
            {
                this.canBePooled = value;
            }
        }

        #endregion
    
    }

    class Service
    {
        static void Main(string[] args)
        {
            PrintLegend();
            
            ServiceHost host =
                new ServiceHost(typeof(PoolService));
            
            host.Open();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Service is running.");
            Console.ForegroundColor = ConsoleColor.Gray;            
            
	    Console.WriteLine("Press <ENTER> to complete test.");
            Console.ReadLine();
            host.Close();     
        }

        static void PrintLegend()
        {            
            Console.WriteLine("=================================");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Green: Messages from the service host.");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Blue: Messages from the service instance.");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Red: Messages from the object pool.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("=================================");
            Console.WriteLine("");
        }
    }
}
