
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceModel;
using System.ServiceProcess;

namespace Microsoft.Samples.WindowsService
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.WindowsService")]
    public interface ICalculator
    {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }

    [RunInstaller(true)]
    public class ProjectInstaller : Installer 
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;
    
        public ProjectInstaller() 
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "WCFWindowsServiceSample";				
            Installers.Add(process);
            Installers.Add(service);
        }
    }

    // Service class which implements the service contract.

    public class WindowsCalculatorService : ServiceBase
    {
        public ServiceHost serviceHost = null;


        public static void Main() 
        {
            ServiceBase.Run(new WindowsCalculatorService());
        }

        public WindowsCalculatorService()
        {
            ServiceName = "WCFWindowsServiceSample";
        }

        //Start the Windows service.

        protected override void OnStart(string[] args) 
        {

            if (serviceHost!=null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the WcfCalculatorService type and provide the base address.
            serviceHost = new ServiceHost(typeof(WcfCalculatorService));

            // Open the ServiceHostBase to create listeners and start listening for messages.
            serviceHost.Open();
        }


        // Stop the Windows service.
        protected override void OnStop() 
        {
            if (serviceHost!=null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }

    public class WcfCalculatorService : ICalculator
    {

        public double Add(double n1, double n2)
        {
            double result = n1 + n2;
            return result;
        }

        public double Subtract(double n1, double n2)
        {
            double result = n1 - n2;
            return result;
        }

        public double Multiply(double n1, double n2)
        {
            double result = n1 * n2;
            return result;
        }

        public double Divide(double n1, double n2)
        {
            double result = n1 / n2;
            return result;
        }
    }
}
