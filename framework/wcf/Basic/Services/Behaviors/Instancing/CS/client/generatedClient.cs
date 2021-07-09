
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.Instancing
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Instancing", ConfigurationName="Microsoft.Samples.Instancing.ICalculator", SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface ICalculator
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculator/Add", ReplyAction="http://Microsoft.Samples.Instancing/ICalculator/AddResponse")]
        double Add(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculator/Subtract", ReplyAction="http://Microsoft.Samples.Instancing/ICalculator/SubtractResponse")]
        double Subtract(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculator/Multiply", ReplyAction="http://Microsoft.Samples.Instancing/ICalculator/MultiplyResponse")]
        double Multiply(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculator/Divide", ReplyAction="http://Microsoft.Samples.Instancing/ICalculator/DivideResponse")]
        double Divide(double n1, double n2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorChannel : Microsoft.Samples.Instancing.ICalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.Instancing.ICalculator>, Microsoft.Samples.Instancing.ICalculator
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Instancing", ConfigurationName="Microsoft.Samples.Instancing.ICalculatorInstance", SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface ICalculatorInstance
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculator/Add", ReplyAction="http://Microsoft.Samples.Instancing/ICalculator/AddResponse")]
        double Add(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculator/Subtract", ReplyAction="http://Microsoft.Samples.Instancing/ICalculator/SubtractResponse")]
        double Subtract(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculator/Multiply", ReplyAction="http://Microsoft.Samples.Instancing/ICalculator/MultiplyResponse")]
        double Multiply(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculator/Divide", ReplyAction="http://Microsoft.Samples.Instancing/ICalculator/DivideResponse")]
        double Divide(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculatorInstance/GetInstanceContextMode", ReplyAction="http://Microsoft.Samples.Instancing/ICalculatorInstance/GetInstanceContextModeR" +
            "esponse")]
        string GetInstanceContextMode();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculatorInstance/GetInstanceId", ReplyAction="http://Microsoft.Samples.Instancing/ICalculatorInstance/GetInstanceIdResponse")]
        int GetInstanceId();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Instancing/ICalculatorInstance/GetOperationCount", ReplyAction="http://Microsoft.Samples.Instancing/ICalculatorInstance/GetOperationCountRespon" +
            "se")]
        int GetOperationCount();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorInstanceChannel : Microsoft.Samples.Instancing.ICalculatorInstance, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorInstanceClient : System.ServiceModel.ClientBase<Microsoft.Samples.Instancing.ICalculatorInstance>, Microsoft.Samples.Instancing.ICalculatorInstance
    {
        
        public CalculatorInstanceClient()
        {
        }
        
        public CalculatorInstanceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public CalculatorInstanceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorInstanceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorInstanceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
        
        public string GetInstanceContextMode()
        {
            return base.Channel.GetInstanceContextMode();
        }
        
        public int GetInstanceId()
        {
            return base.Channel.GetInstanceId();
        }
        
        public int GetOperationCount()
        {
            return base.Channel.GetOperationCount();
        }
    }
}
