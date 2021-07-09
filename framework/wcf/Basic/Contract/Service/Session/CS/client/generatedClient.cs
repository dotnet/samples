
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.Session
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Session", ConfigurationName="Microsoft.Samples.Session.ICalculatorSession", SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface ICalculatorSession
    {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Session/ICalculatorSession/Clear")]
        void Clear();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Session/ICalculatorSession/AddTo")]
        void AddTo(double n);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Session/ICalculatorSession/SubtractFrom")]
        void SubtractFrom(double n);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Session/ICalculatorSession/MultiplyBy")]
        void MultiplyBy(double n);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://Microsoft.Samples.Session/ICalculatorSession/DivideBy")]
        void DivideBy(double n);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Session/ICalculatorSession/Result", ReplyAction="http://Microsoft.Samples.Session/ICalculatorSession/ResultResponse")]
        double Result();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorSessionChannel : Microsoft.Samples.Session.ICalculatorSession, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorSessionClient : System.ServiceModel.ClientBase<Microsoft.Samples.Session.ICalculatorSession>, Microsoft.Samples.Session.ICalculatorSession
    {
        
        public CalculatorSessionClient()
        {
        }
        
        public CalculatorSessionClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public CalculatorSessionClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorSessionClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorSessionClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
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
        
        public double Result()
        {
            return base.Channel.Result();
        }
    }
}
