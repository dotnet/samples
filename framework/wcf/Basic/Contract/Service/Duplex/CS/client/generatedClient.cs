
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.Duplex
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Duplex", ConfigurationName="Microsoft.Samples.Duplex.ICalculatorDuplex", CallbackContract=typeof(Microsoft.Samples.Duplex.ICalculatorDuplexCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface ICalculatorDuplex
    {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Duplex/ICalculatorDuplex/Clear")]
        void Clear();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Duplex/ICalculatorDuplex/AddTo")]
        void AddTo(double n);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Duplex/ICalculatorDuplex/SubtractFrom")]
        void SubtractFrom(double n);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Duplex/ICalculatorDuplex/MultiplyBy")]
        void MultiplyBy(double n);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Duplex/ICalculatorDuplex/DivideBy")]
        void DivideBy(double n);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorDuplexCallback
    {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Duplex/ICalculatorDuplex/Result")]
        void Result(double result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Duplex/ICalculatorDuplex/Equation")]
        void Equation(string eqn);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorDuplexChannel : Microsoft.Samples.Duplex.ICalculatorDuplex, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorDuplexClient : System.ServiceModel.DuplexClientBase<Microsoft.Samples.Duplex.ICalculatorDuplex>, Microsoft.Samples.Duplex.ICalculatorDuplex
    {
        
        public CalculatorDuplexClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance)
        {
        }
        
        public CalculatorDuplexClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName)
        {
        }
        
        public CalculatorDuplexClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorDuplexClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorDuplexClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress)
        {
        }
        
        public void Clear()
        {
            base.Channel.Clear();
        }
        
        public void AddTo(double n)
        {
            base.Channel.AddTo(n);
        }
        
        public void SubtractFrom(double n)
        {
            base.Channel.SubtractFrom(n);
        }
        
        public void MultiplyBy(double n)
        {
            base.Channel.MultiplyBy(n);
        }
        
        public void DivideBy(double n)
        {
            base.Channel.DivideBy(n);
        }
    }
}
