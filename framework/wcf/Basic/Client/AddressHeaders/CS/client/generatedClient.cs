
// Copyright (c) Microsoft Corporation.  All Rights Reserved.

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.AddressHeaders", ConfigurationName="IHello")]
public interface IHello
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.AddressHeaders/IHello/Hello", ReplyAction="http://Microsoft.Samples.AddressHeaders/IHello/HelloResponse")]
    string Hello();
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IHelloChannel : IHello, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class HelloClient : System.ServiceModel.ClientBase<IHello>, IHello
{
    
    public HelloClient()
    {
    }
    
    public HelloClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public HelloClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public HelloClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public HelloClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public string Hello()
    {
        return base.Channel.Hello();
    }
}
