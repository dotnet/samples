
// Copyright (c) Microsoft Corporation.  All Rights Reserved.

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "3.0.4011.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://Microsoft.Samples.XmlSerializer")]
public partial class ComplexNumber
{
    
    private double realField;
    
    private double imaginaryField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double Real
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
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double Imaginary
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
}
namespace Microsoft.Samples.XmlSerializer
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.XmlSerializer", ConfigurationName="Microsoft.Samples.XmlSerializer.IXmlSerializerCalculator")]
    public interface IXmlSerializerCalculator
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.XmlSerializer/IXmlSerializerCalculator/Add", ReplyAction="http://Microsoft.Samples.XmlSerializer/IXmlSerializerCalculator/AddResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        ComplexNumber Add(ComplexNumber n1, ComplexNumber n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.XmlSerializer/IXmlSerializerCalculator/Subtract", ReplyAction="http://Microsoft.Samples.XmlSerializer/IXmlSerializerCalculator/SubtractResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        ComplexNumber Subtract(ComplexNumber n1, ComplexNumber n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.XmlSerializer/IXmlSerializerCalculator/Multiply", ReplyAction="http://Microsoft.Samples.XmlSerializer/IXmlSerializerCalculator/MultiplyResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        ComplexNumber Multiply(ComplexNumber n1, ComplexNumber n2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.XmlSerializer/IXmlSerializerCalculator/Divide", ReplyAction="http://Microsoft.Samples.XmlSerializer/IXmlSerializerCalculator/DivideResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        ComplexNumber Divide(ComplexNumber n1, ComplexNumber n2);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IXmlSerializerCalculatorChannel : Microsoft.Samples.XmlSerializer.IXmlSerializerCalculator, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class XmlSerializerCalculatorClient : System.ServiceModel.ClientBase<Microsoft.Samples.XmlSerializer.IXmlSerializerCalculator>, Microsoft.Samples.XmlSerializer.IXmlSerializerCalculator
    {
        
        public XmlSerializerCalculatorClient()
        {
        }
        
        public XmlSerializerCalculatorClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public XmlSerializerCalculatorClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public XmlSerializerCalculatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public XmlSerializerCalculatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public ComplexNumber Add(ComplexNumber n1, ComplexNumber n2)
        {
            return base.Channel.Add(n1, n2);
        }
        
        public ComplexNumber Subtract(ComplexNumber n1, ComplexNumber n2)
        {
            return base.Channel.Subtract(n1, n2);
        }
        
        public ComplexNumber Multiply(ComplexNumber n1, ComplexNumber n2)
        {
            return base.Channel.Multiply(n1, n2);
        }
        
        public ComplexNumber Divide(ComplexNumber n1, ComplexNumber n2)
        {
            return base.Channel.Divide(n1, n2);
        }
    }
}
