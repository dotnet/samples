
// Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.UsingUsing
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.UsingUsing", ConfigurationName="Microsoft.Samples.UsingUsing.ICalculator")]
    public interface ICalculator
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.UsingUsing/ICalculator/Add", ReplyAction="http://Microsoft.Samples.UsingUsing/ICalculator/AddResponse")]
        double Add(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.UsingUsing/ICalculator/Subtract", ReplyAction="http://Microsoft.Samples.UsingUsing/ICalculator/SubtractResponse")]
        double Subtract(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.UsingUsing/ICalculator/Multiply", ReplyAction="http://Microsoft.Samples.UsingUsing/ICalculator/MultiplyResponse")]
        double Multiply(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.UsingUsing/ICalculator/Divide", ReplyAction="http://Microsoft.Samples.UsingUsing/ICalculator/DivideResponse")]
        double Divide(double n1, double n2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorChannel : Microsoft.Samples.UsingUsing.ICalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.UsingUsing.ICalculator>, Microsoft.Samples.UsingUsing.ICalculator
    {
        
        public CalculatorClient()
        {
        }
        
        public CalculatorClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public CalculatorClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
