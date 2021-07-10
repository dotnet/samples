
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.Message
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Message", ConfigurationName="Microsoft.Samples.Message.ICalculator")]
    public interface ICalculator
    {
        
        // CODEGEN: Generating message contract since the wrapper name (MyMessage) of message MyMessage does not match the default value (Calculate)
        [System.ServiceModel.OperationContractAttribute(Action="http://test/MyMessage_action", ReplyAction="http://test/MyMessage_action")]
        Microsoft.Samples.Message.MyMessage Calculate(Microsoft.Samples.Message.MyMessage request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="MyMessage", WrapperNamespace="http://Microsoft.Samples.Message", IsWrapped=true)]
    public partial class MyMessage
    {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://Microsoft.Samples.Message")]
        public string Operation;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://Microsoft.Samples.Message", Order=0)]
        public double N1;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://Microsoft.Samples.Message", Order=1)]
        public double N2;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://Microsoft.Samples.Message", Order=2)]
        public double Result;
        
        public MyMessage()
        {
        }
        
        public MyMessage(string Operation, double N1, double N2, double Result)
        {
            this.Operation = Operation;
            this.N1 = N1;
            this.N2 = N2;
            this.Result = Result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorChannel : Microsoft.Samples.Message.ICalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.Message.ICalculator>, Microsoft.Samples.Message.ICalculator
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
        
        Microsoft.Samples.Message.MyMessage Microsoft.Samples.Message.ICalculator.Calculate(Microsoft.Samples.Message.MyMessage request)
        {
            return base.Channel.Calculate(request);
        }
        
        public void Calculate(ref string Operation, ref double N1, ref double N2, ref double Result)
        {
            Microsoft.Samples.Message.MyMessage inValue = new Microsoft.Samples.Message.MyMessage();
            inValue.Operation = Operation;
            inValue.N1 = N1;
            inValue.N2 = N2;
            inValue.Result = Result;
            Microsoft.Samples.Message.MyMessage retVal = ((Microsoft.Samples.Message.ICalculator)(this)).Calculate(inValue);
            Operation = retVal.Operation;
            N1 = retVal.N1;
            N2 = retVal.N2;
            Result = retVal.Result;
        }
    }
}
