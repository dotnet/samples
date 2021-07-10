
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.Runtime.Serialization;
    
[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://Microsoft.Samples.DCSurrogate", ClrNamespace="Microsoft.Samples.DCSurrogate")]

namespace Microsoft.Samples.DCSurrogate
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Employee", Namespace="http://Microsoft.Samples.DCSurrogate")]
    public partial class Employee : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private System.DateTime dateHiredField;
        
        private Microsoft.Samples.DCSurrogate.Person personField;
        
        private decimal salaryField;
        
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
        public System.DateTime dateHired
        {
            get
            {
                return this.dateHiredField;
            }
            set
            {
                this.dateHiredField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Microsoft.Samples.DCSurrogate.Person person
        {
            get
            {
                return this.personField;
            }
            set
            {
                this.personField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal salary
        {
            get
            {
                return this.salaryField;
            }
            set
            {
                this.salaryField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Person", Namespace="http://Microsoft.Samples.DCSurrogate")]
    public partial class Person : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int AgeField;
        
        private string FirstNameField;
        
        private string LastNameField;
        
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
        public int Age
        {
            get
            {
                return this.AgeField;
            }
            set
            {
                this.AgeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FirstName
        {
            get
            {
                return this.FirstNameField;
            }
            set
            {
                this.FirstNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LastName
        {
            get
            {
                return this.LastNameField;
            }
            set
            {
                this.LastNameField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.DCSurrogate", ConfigurationName="IPersonnelDataService")]
public interface IPersonnelDataService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.DCSurrogate/IPersonnelDataService/AddEmployee", ReplyAction="http://Microsoft.Samples.DCSurrogate/IPersonnelDataService/AddEmployeeResponse")]
    void AddEmployee(Microsoft.Samples.DCSurrogate.Employee employee);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.DCSurrogate/IPersonnelDataService/GetEmployee", ReplyAction="http://Microsoft.Samples.DCSurrogate/IPersonnelDataService/GetEmployeeResponse")]
    Microsoft.Samples.DCSurrogate.Employee GetEmployee(string name);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IPersonnelDataServiceChannel : IPersonnelDataService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class PersonnelDataServiceClient : System.ServiceModel.ClientBase<IPersonnelDataService>, IPersonnelDataService
{
    
    public PersonnelDataServiceClient()
    {
    }
    
    public PersonnelDataServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public PersonnelDataServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public PersonnelDataServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public PersonnelDataServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }

    public void AddEmployee(Microsoft.Samples.DCSurrogate.Employee employee)
    {
        base.Channel.AddEmployee(employee);
    }

    public Microsoft.Samples.DCSurrogate.Employee GetEmployee(string name)
    {
        return base.Channel.GetEmployee(name);
    }
}
