
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.Runtime.Serialization;
    
[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://Microsoft.Samples.KnownTypes", ClrNamespace="Microsoft.Samples.KnownTypes")]

namespace Microsoft.Samples.KnownTypes
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Microsoft.Samples.KnownTypes.ComplexNumberWithMagnitude))]
    public partial class ComplexNumber : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private double imaginaryField;
        
        private double realField;
        
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
        public double imaginary
        {
            get
            {
                return this.imaginaryField;
            }
            set
            {
                this.imaginaryField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double real
        {
            get
            {
                return this.realField;
            }
            set
            {
                this.realField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class ComplexNumberWithMagnitude : Microsoft.Samples.KnownTypes.ComplexNumber
    {
        
        private double MagnitudeField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Magnitude
        {
            get
            {
                return this.MagnitudeField;
            }
            set
            {
                this.MagnitudeField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.KnownTypes", ConfigurationName="Microsoft.Samples.KnownTypes.IDataContractCalculator")]
    public interface IDataContractCalculator
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.KnownTypes/IDataContractCalculator/Add", ReplyAction="http://Microsoft.Samples.KnownTypes/IDataContractCalculator/AddResponse")]
        Microsoft.Samples.KnownTypes.ComplexNumber Add(Microsoft.Samples.KnownTypes.ComplexNumber n1, Microsoft.Samples.KnownTypes.ComplexNumber n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.KnownTypes/IDataContractCalculator/Subtract", ReplyAction="http://Microsoft.Samples.KnownTypes/IDataContractCalculator/SubtractResponse")]
        Microsoft.Samples.KnownTypes.ComplexNumber Subtract(Microsoft.Samples.KnownTypes.ComplexNumber n1, Microsoft.Samples.KnownTypes.ComplexNumber n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.KnownTypes/IDataContractCalculator/Multiply", ReplyAction="http://Microsoft.Samples.KnownTypes/IDataContractCalculator/MultiplyResponse")]
        Microsoft.Samples.KnownTypes.ComplexNumber Multiply(Microsoft.Samples.KnownTypes.ComplexNumber n1, Microsoft.Samples.KnownTypes.ComplexNumber n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.KnownTypes/IDataContractCalculator/Divide", ReplyAction="http://Microsoft.Samples.KnownTypes/IDataContractCalculator/DivideResponse")]
        Microsoft.Samples.KnownTypes.ComplexNumber Divide(Microsoft.Samples.KnownTypes.ComplexNumber n1, Microsoft.Samples.KnownTypes.ComplexNumber n2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataContractCalculatorChannel : Microsoft.Samples.KnownTypes.IDataContractCalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataContractCalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.KnownTypes.IDataContractCalculator>, Microsoft.Samples.KnownTypes.IDataContractCalculator
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
        
        public Microsoft.Samples.KnownTypes.ComplexNumber Add(Microsoft.Samples.KnownTypes.ComplexNumber n1, Microsoft.Samples.KnownTypes.ComplexNumber n2)
        {
            return base.Channel.Add(n1, n2);
        }
        
        public Microsoft.Samples.KnownTypes.ComplexNumber Subtract(Microsoft.Samples.KnownTypes.ComplexNumber n1, Microsoft.Samples.KnownTypes.ComplexNumber n2)
        {
            return base.Channel.Subtract(n1, n2);
        }
        
        public Microsoft.Samples.KnownTypes.ComplexNumber Multiply(Microsoft.Samples.KnownTypes.ComplexNumber n1, Microsoft.Samples.KnownTypes.ComplexNumber n2)
        {
            return base.Channel.Multiply(n1, n2);
        }
        
        public Microsoft.Samples.KnownTypes.ComplexNumber Divide(Microsoft.Samples.KnownTypes.ComplexNumber n1, Microsoft.Samples.KnownTypes.ComplexNumber n2)
        {
            return base.Channel.Divide(n1, n2);
        }
    }
}

