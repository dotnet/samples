
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.UseAndStyle
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.UseAndStyle", ConfigurationName="Microsoft.Samples.UseAndStyle.IUseAndStyleCalculator")]
    public interface IUseAndStyleCalculator
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.UseAndStyle/IUseAndStyleCalculator/Add", ReplyAction="http://Microsoft.Samples.UseAndStyle/IUseAndStyleCalculator/AddResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        double Add(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.UseAndStyle/IUseAndStyleCalculator/Subtract", ReplyAction="http://Microsoft.Samples.UseAndStyle/IUseAndStyleCalculator/SubtractResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        double Subtract(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.UseAndStyle/IUseAndStyleCalculator/Multiply", ReplyAction="http://Microsoft.Samples.UseAndStyle/IUseAndStyleCalculator/MultiplyResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        double Multiply(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.UseAndStyle/IUseAndStyleCalculator/Divide", ReplyAction="http://Microsoft.Samples.UseAndStyle/IUseAndStyleCalculator/DivideResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        double Divide(double n1, double n2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IUseAndStyleCalculatorChannel : Microsoft.Samples.UseAndStyle.IUseAndStyleCalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UseAndStyleCalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.UseAndStyle.IUseAndStyleCalculator>, Microsoft.Samples.UseAndStyle.IUseAndStyleCalculator
    {
        
        public UseAndStyleCalculatorClient()
        {
        }
        
        public UseAndStyleCalculatorClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public UseAndStyleCalculatorClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public UseAndStyleCalculatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public UseAndStyleCalculatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
