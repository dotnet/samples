
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.Concurrency
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Concurrency", ConfigurationName="Microsoft.Samples.Concurrency.ICalculatorConcurrency")]
    public interface ICalculatorConcurrency
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Concurrency/ICalculator/Add", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculator/AddResponse")]
        double Add(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://Microsoft.Samples.Concurrency/ICalculator/Add", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculator/AddResponse")]
        System.IAsyncResult BeginAdd(double n1, double n2, System.AsyncCallback callback, object asyncState);
        
        double EndAdd(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Concurrency/ICalculator/Subtract", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculator/SubtractResponse")]
        double Subtract(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://Microsoft.Samples.Concurrency/ICalculator/Subtract", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculator/SubtractResponse")]
        System.IAsyncResult BeginSubtract(double n1, double n2, System.AsyncCallback callback, object asyncState);
        
        double EndSubtract(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Concurrency/ICalculator/Multiply", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculator/MultiplyResponse")]
        double Multiply(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://Microsoft.Samples.Concurrency/ICalculator/Multiply", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculator/MultiplyResponse")]
        System.IAsyncResult BeginMultiply(double n1, double n2, System.AsyncCallback callback, object asyncState);
        
        double EndMultiply(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Concurrency/ICalculator/Divide", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculator/DivideResponse")]
        double Divide(double n1, double n2);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://Microsoft.Samples.Concurrency/ICalculator/Divide", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculator/DivideResponse")]
        System.IAsyncResult BeginDivide(double n1, double n2, System.AsyncCallback callback, object asyncState);
        
        double EndDivide(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Concurrency/ICalculatorConcurrency/GetConcurrencyMode", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculatorConcurrency/GetConcurrencyModeRe" +
            "sponse")]
        string GetConcurrencyMode();
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://Microsoft.Samples.Concurrency/ICalculatorConcurrency/GetConcurrencyMode", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculatorConcurrency/GetConcurrencyModeRe" +
            "sponse")]
        System.IAsyncResult BeginGetConcurrencyMode(System.AsyncCallback callback, object asyncState);
        
        string EndGetConcurrencyMode(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Concurrency/ICalculatorConcurrency/GetOperationCount", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculatorConcurrency/GetOperationCountRes" +
            "ponse")]
        int GetOperationCount();
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://Microsoft.Samples.Concurrency/ICalculatorConcurrency/GetOperationCount", ReplyAction="http://Microsoft.Samples.Concurrency/ICalculatorConcurrency/GetOperationCountRes" +
            "ponse")]
        System.IAsyncResult BeginGetOperationCount(System.AsyncCallback callback, object asyncState);
        
        int EndGetOperationCount(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorConcurrencyChannel : Microsoft.Samples.Concurrency.ICalculatorConcurrency, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorConcurrencyClient : System.ServiceModel.ClientBase<Microsoft.Samples.Concurrency.ICalculatorConcurrency>, Microsoft.Samples.Concurrency.ICalculatorConcurrency
    {
        
        public CalculatorConcurrencyClient()
        {
        }
        
        public CalculatorConcurrencyClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public CalculatorConcurrencyClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorConcurrencyClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public CalculatorConcurrencyClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public double Add(double n1, double n2)
        {
            return base.Channel.Add(n1, n2);
        }
        
        public System.IAsyncResult BeginAdd(double n1, double n2, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginAdd(n1, n2, callback, asyncState);
        }
        
        public double EndAdd(System.IAsyncResult result)
        {
            return base.Channel.EndAdd(result);
        }
        
        public double Subtract(double n1, double n2)
        {
            return base.Channel.Subtract(n1, n2);
        }
        
        public System.IAsyncResult BeginSubtract(double n1, double n2, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginSubtract(n1, n2, callback, asyncState);
        }
        
        public double EndSubtract(System.IAsyncResult result)
        {
            return base.Channel.EndSubtract(result);
        }
        
        public double Multiply(double n1, double n2)
        {
            return base.Channel.Multiply(n1, n2);
        }
        
        public System.IAsyncResult BeginMultiply(double n1, double n2, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginMultiply(n1, n2, callback, asyncState);
        }
        
        public double EndMultiply(System.IAsyncResult result)
        {
            return base.Channel.EndMultiply(result);
        }
        
        public double Divide(double n1, double n2)
        {
            return base.Channel.Divide(n1, n2);
        }
        
        public System.IAsyncResult BeginDivide(double n1, double n2, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginDivide(n1, n2, callback, asyncState);
        }
        
        public double EndDivide(System.IAsyncResult result)
        {
            return base.Channel.EndDivide(result);
        }
        
        public string GetConcurrencyMode()
        {
            return base.Channel.GetConcurrencyMode();
        }
        
        public System.IAsyncResult BeginGetConcurrencyMode(System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginGetConcurrencyMode(callback, asyncState);
        }
        
        public string EndGetConcurrencyMode(System.IAsyncResult result)
        {
            return base.Channel.EndGetConcurrencyMode(result);
        }
        
        public int GetOperationCount()
        {
            return base.Channel.GetOperationCount();
        }
        
        public System.IAsyncResult BeginGetOperationCount(System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginGetOperationCount(callback, asyncState);
        }
        
        public int EndGetOperationCount(System.IAsyncResult result)
        {
            return base.Channel.EndGetOperationCount(result);
        }
    }
}
