// Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.WCFClientInteropASMX
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.WCFClientInteropASMX", ConfigurationName="Microsoft.Samples.WCFClientInteropASMX.CalculatorServiceSoap")]
    public interface CalculatorServiceSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.WCFClientInteropASMX/Add", ReplyAction="*")]
        double Add(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.WCFClientInteropASMX/Subtract", ReplyAction="*")]
        double Subtract(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.WCFClientInteropASMX/Multiply", ReplyAction="*")]
        double Multiply(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.WCFClientInteropASMX/Divide", ReplyAction="*")]
        double Divide(double n1, double n2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface CalculatorServiceSoapChannel : Microsoft.Samples.WCFClientInteropASMX.CalculatorServiceSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorServiceSoapClient : System.ServiceModel.ClientBase<Microsoft.Samples.WCFClientInteropASMX.CalculatorServiceSoap>, Microsoft.Samples.WCFClientInteropASMX.CalculatorServiceSoap
    {
        
        public CalculatorServiceSoapClient()
        {
        }
        
        public CalculatorServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public CalculatorServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public double Add(double n1, double n2)
        {
            return base.Channel.Add(n1, n2);
        }
        
        public double Subtract(double n1, double n2)
        {
            return base.Channel.Subtract(n1, n2);
        }
        
        public double Multiply(double n1, double n2)
        {
            return base.Channel.Multiply(n1, n2);
        }
        
        public double Divide(double n1, double n2)
        {
            return base.Channel.Divide(n1, n2);
        }
    }
}
