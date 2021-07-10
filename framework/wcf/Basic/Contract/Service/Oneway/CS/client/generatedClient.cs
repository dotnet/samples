
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.OneWay
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.OneWay", ConfigurationName="Microsoft.Samples.OneWay.IOneWayCalculator")]
    public interface IOneWayCalculator
    {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.OneWay/IOneWayCalculator/Add")]
        void Add(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.OneWay/IOneWayCalculator/Subtract")]
        void Subtract(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.OneWay/IOneWayCalculator/Multiply")]
        void Multiply(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.OneWay/IOneWayCalculator/Divide")]
        void Divide(double n1, double n2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOneWayCalculatorChannel : Microsoft.Samples.OneWay.IOneWayCalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OneWayCalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.OneWay.IOneWayCalculator>, Microsoft.Samples.OneWay.IOneWayCalculator
    {
        
        public OneWayCalculatorClient()
        {
        }
        
        public OneWayCalculatorClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public OneWayCalculatorClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public OneWayCalculatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public OneWayCalculatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public void Add(double n1, double n2)
        {
            base.Channel.Add(n1, n2);
        }
        
        public void Subtract(double n1, double n2)
        {
            base.Channel.Subtract(n1, n2);
        }
        
        public void Multiply(double n1, double n2)
        {
            base.Channel.Multiply(n1, n2);
        }
        
        public void Divide(double n1, double n2)
        {
            base.Channel.Divide(n1, n2);
        }
    }
}
