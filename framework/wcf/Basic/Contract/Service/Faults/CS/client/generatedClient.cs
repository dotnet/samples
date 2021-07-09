
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.Runtime.Serialization;
    
[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://Microsoft.Samples.Faults", ClrNamespace="Microsoft.Samples.Faults")]

namespace Microsoft.Samples.Faults
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class MathFault : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string OperationField;
        
        private string ProblemTypeField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Operation
        {
            get
            {
                return this.OperationField;
            }
            set
            {
                this.OperationField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemType
        {
            get
            {
                return this.ProblemTypeField;
            }
            set
            {
                this.ProblemTypeField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Faults", ConfigurationName="Microsoft.Samples.Faults.ICalculator")]
    public interface ICalculator
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Faults/ICalculator/Add", ReplyAction="http://Microsoft.Samples.Faults/ICalculator/AddResponse")]
        int Add(int n1, int n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Faults/ICalculator/Subtract", ReplyAction="http://Microsoft.Samples.Faults/ICalculator/SubtractResponse")]
        int Subtract(int n1, int n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Faults/ICalculator/Multiply", ReplyAction="http://Microsoft.Samples.Faults/ICalculator/MultiplyResponse")]
        int Multiply(int n1, int n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Faults/ICalculator/Divide", ReplyAction="http://Microsoft.Samples.Faults/ICalculator/DivideResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Samples.Faults.MathFault), Action="http://Microsoft.Samples.Faults/ICalculator/DivideMathFaultFault", Name="MathFault")]
        int Divide(int n1, int n2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalculatorChannel : Microsoft.Samples.Faults.ICalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.Faults.ICalculator>, Microsoft.Samples.Faults.ICalculator
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
        
        public int Add(int n1, int n2)
        {
            return base.Channel.Add(n1, n2);
        }
        
        public int Subtract(int n1, int n2)
        {
            return base.Channel.Subtract(n1, n2);
        }
        
        public int Multiply(int n1, int n2)
        {
            return base.Channel.Multiply(n1, n2);
        }
        
        public int Divide(int n1, int n2)
        {
            return base.Channel.Divide(n1, n2);
        }
    }
}

