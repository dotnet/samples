
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.Runtime.Serialization;
    
[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://Microsoft.Samples.Data", ClrNamespace="Microsoft.Samples.Data")]

namespace Microsoft.Samples.Data
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class ComplexNumber : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private double ImaginaryField;
        
        private double RealField;
        
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
        public double Imaginary
        {
            get
            {
                return this.ImaginaryField;
            }
            set
            {
                this.ImaginaryField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Real
        {
            get
            {
                return this.RealField;
            }
            set
            {
                this.RealField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Data", ConfigurationName="Microsoft.Samples.Data.IDataContractCalculator")]
    public interface IDataContractCalculator
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Data/IDataContractCalculator/Add", ReplyAction="http://Microsoft.Samples.Data/IDataContractCalculator/AddResponse")]
        Microsoft.Samples.Data.ComplexNumber Add(Microsoft.Samples.Data.ComplexNumber n1, Microsoft.Samples.Data.ComplexNumber n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Data/IDataContractCalculator/Subtract", ReplyAction="http://Microsoft.Samples.Data/IDataContractCalculator/SubtractResponse")]
        Microsoft.Samples.Data.ComplexNumber Subtract(Microsoft.Samples.Data.ComplexNumber n1, Microsoft.Samples.Data.ComplexNumber n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Data/IDataContractCalculator/Multiply", ReplyAction="http://Microsoft.Samples.Data/IDataContractCalculator/MultiplyResponse")]
        Microsoft.Samples.Data.ComplexNumber Multiply(Microsoft.Samples.Data.ComplexNumber n1, Microsoft.Samples.Data.ComplexNumber n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Data/IDataContractCalculator/Divide", ReplyAction="http://Microsoft.Samples.Data/IDataContractCalculator/DivideResponse")]
        Microsoft.Samples.Data.ComplexNumber Divide(Microsoft.Samples.Data.ComplexNumber n1, Microsoft.Samples.Data.ComplexNumber n2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataContractCalculatorChannel : Microsoft.Samples.Data.IDataContractCalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataContractCalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.Data.IDataContractCalculator>, Microsoft.Samples.Data.IDataContractCalculator
    {
        
        public DataContractCalculatorClient()
        {
        }
        
        public DataContractCalculatorClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public DataContractCalculatorClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public DataContractCalculatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public DataContractCalculatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public Microsoft.Samples.Data.ComplexNumber Add(Microsoft.Samples.Data.ComplexNumber n1, Microsoft.Samples.Data.ComplexNumber n2)
        {
            return base.Channel.Add(n1, n2);
        }
        
        public Microsoft.Samples.Data.ComplexNumber Subtract(Microsoft.Samples.Data.ComplexNumber n1, Microsoft.Samples.Data.ComplexNumber n2)
        {
            return base.Channel.Subtract(n1, n2);
        }
        
        public Microsoft.Samples.Data.ComplexNumber Multiply(Microsoft.Samples.Data.ComplexNumber n1, Microsoft.Samples.Data.ComplexNumber n2)
        {
            return base.Channel.Multiply(n1, n2);
        }
        
        public Microsoft.Samples.Data.ComplexNumber Divide(Microsoft.Samples.Data.ComplexNumber n1, Microsoft.Samples.Data.ComplexNumber n2)
        {
            return base.Channel.Divide(n1, n2);
        }
    }
}

