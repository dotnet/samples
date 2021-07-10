//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Management;
using System.Management.Instrumentation;
using System.ServiceModel;

namespace Microsoft.Samples.ServiceModel
{
    // Define an instrumentation class.
    [InstrumentationClass(InstrumentationType.Instance)]
    public class WMIObject
    {
        public String WMIInfo = "User Defined WMI Information.";

        public void ChangeInfo(String newInfo)
        {
            WMIInfo = newInfo;
        }
    }

    // Define service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.ServiceModel")]
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

    // Let the system know that the InstallUtil.exe tool will be run
    // against this assembly in order to register the .dll's schema to WMI.
    [System.ComponentModel.RunInstaller(true)]
    public class MyInstaller : DefaultManagementProjectInstaller
    {
        public MyInstaller()
        {
            ManagementInstaller mgmtInstaller = new ManagementInstaller();
            Installers.Add(mgmtInstaller);
        }
    }   

    // Service class which implements the service contract.
    public class CalculatorService : ICalculator
    {
        private WMIObject wmiObj;

        public CalculatorService()
        {
            wmiObj = new WMIObject();

            //publish the object to WMI in order to be viewed
            Instrumentation.Publish(wmiObj);
        }

        public double Add(double n1, double n2)
        {
            return n1 + n2;
        }

        public double Subtract(double n1, double n2)
        {
            return n1 - n2;
        }

        public double Multiply(double n1, double n2)
        {
            return n1 * n2;
        }

        public double Divide(double n1, double n2)
        {
            return n1 / n2;
        }
    }

}
