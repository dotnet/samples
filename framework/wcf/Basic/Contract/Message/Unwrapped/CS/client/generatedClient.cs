
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.Unwrapped
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Unwrapped", ConfigurationName="Microsoft.Samples.Unwrapped.ICalculator")]
    public interface ICalculator
    {
        
        // CODEGEN: Generating message contract since the operation Add is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Unwrapped/ICalculator/Add", ReplyAction="http://Microsoft.Samples.Unwrapped/ICalculator/AddResponse")]
        Microsoft.Samples.Unwrapped.ResponseMessage Add(Microsoft.Samples.Unwrapped.RequestMessage request);
        
        // CODEGEN: Generating message contract since the operation Subtract is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Unwrapped/ICalculator/Subtract", ReplyAction="http://Microsoft.Samples.Unwrapped/ICalculator/SubtractResponse")]
        Microsoft.Samples.Unwrapped.ResponseMessage Subtract(Microsoft.Samples.Unwrapped.RequestMessage request);
        
        // CODEGEN: Generating message contract since the operation Multiply is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Unwrapped/ICalculator/Multiply", ReplyAction="http://Microsoft.Samples.Unwrapped/ICalculator/MultiplyResponse")]
        Microsoft.Samples.Unwrapped.ResponseMessage Multiply(Microsoft.Samples.Unwrapped.RequestMessage request);
        
        // CODEGEN: Generating message contract since the operation Divide is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Unwrapped/ICalculator/Divide", ReplyAction="http://Microsoft.Samples.Unwrapped/ICalculator/DivideResponse")]
        Microsoft.Samples.Unwrapped.ResponseMessage Divide(Microsoft.Samples.Unwrapped.RequestMessage request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RequestMessage
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://Microsoft.Samples.Unwrapped", Order=0)]
        public double n1;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://Microsoft.Samples.Unwrapped", Order=1)]
        public double n2;
        
        public RequestMessage()
        {
        }
        
        public RequestMessage(double n1, double n2)
        {
            this.n1 = n1;
            this.n2 = n2;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ResponseMessage
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://Microsoft.Samples.Unwrapped", Order=0)]
        public double result;
        
        public ResponseMessage()
        {
        }
        
        public ResponseMessage(double result)
        {
            this.result = result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorChannel : Microsoft.Samples.Unwrapped.ICalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.Unwrapped.ICalculator>, Microsoft.Samples.Unwrapped.ICalculator
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
        
        Microsoft.Samples.Unwrapped.ResponseMessage Microsoft.Samples.Unwrapped.ICalculator.Add(Microsoft.Samples.Unwrapped.RequestMessage request)
        {
            return base.Channel.Add(request);
        }
        
        public double Add(double n1, double n2)
        {
            Microsoft.Samples.Unwrapped.RequestMessage inValue = new Microsoft.Samples.Unwrapped.RequestMessage();
            inValue.n1 = n1;
            inValue.n2 = n2;
            Microsoft.Samples.Unwrapped.ResponseMessage retVal = ((Microsoft.Samples.Unwrapped.ICalculator)(this)).Add(inValue);
            return retVal.result;
        }
        
        Microsoft.Samples.Unwrapped.ResponseMessage Microsoft.Samples.Unwrapped.ICalculator.Subtract(Microsoft.Samples.Unwrapped.RequestMessage request)
        {
            return base.Channel.Subtract(request);
        }
        
        public double Subtract(double n1, double n2)
        {
            Microsoft.Samples.Unwrapped.RequestMessage inValue = new Microsoft.Samples.Unwrapped.RequestMessage();
            inValue.n1 = n1;
            inValue.n2 = n2;
            Microsoft.Samples.Unwrapped.ResponseMessage retVal = ((Microsoft.Samples.Unwrapped.ICalculator)(this)).Subtract(inValue);
            return retVal.result;
        }
        
        Microsoft.Samples.Unwrapped.ResponseMessage Microsoft.Samples.Unwrapped.ICalculator.Multiply(Microsoft.Samples.Unwrapped.RequestMessage request)
        {
            return base.Channel.Multiply(request);
        }
        
        public double Multiply(double n1, double n2)
        {
            Microsoft.Samples.Unwrapped.RequestMessage inValue = new Microsoft.Samples.Unwrapped.RequestMessage();
            inValue.n1 = n1;
            inValue.n2 = n2;
            Microsoft.Samples.Unwrapped.ResponseMessage retVal = ((Microsoft.Samples.Unwrapped.ICalculator)(this)).Multiply(inValue);
            return retVal.result;
        }
        
        Microsoft.Samples.Unwrapped.ResponseMessage Microsoft.Samples.Unwrapped.ICalculator.Divide(Microsoft.Samples.Unwrapped.RequestMessage request)
        {
            return base.Channel.Divide(request);
        }
        
        public double Divide(double n1, double n2)
        {
            Microsoft.Samples.Unwrapped.RequestMessage inValue = new Microsoft.Samples.Unwrapped.RequestMessage();
            inValue.n1 = n1;
            inValue.n2 = n2;
            Microsoft.Samples.Unwrapped.ResponseMessage retVal = ((Microsoft.Samples.Unwrapped.ICalculator)(this)).Divide(inValue);
            return retVal.result;
        }
    }
}
