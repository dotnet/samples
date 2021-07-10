
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace = "http://Microsoft.Samples.Untyped", ConfigurationName = "ICalculator")]
public interface ICalculator
{

    [System.ServiceModel.OperationContractAttribute(Action = "http://test/Message_RequestAction", ReplyAction = "http://test/Message_ReplyAction")]
    System.ServiceModel.Channels.Message ComputeSum(System.ServiceModel.Channels.Message request);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface ICalculatorChannel : ICalculator, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class CalculatorClient : System.ServiceModel.ClientBase<ICalculator>, ICalculator
{

    public CalculatorClient()
    {
    }

    public CalculatorClient(string endpointConfigurationName)
        :
            base(endpointConfigurationName)
    {
    }

    public CalculatorClient(string endpointConfigurationName, string remoteAddress)
        :
            base(endpointConfigurationName, remoteAddress)
    {
    }

    public CalculatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress)
        :
            base(endpointConfigurationName, remoteAddress)
    {
    }

    public CalculatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress)
        :
            base(binding, remoteAddress)
    {
    }

    public System.ServiceModel.Channels.Message ComputeSum(System.ServiceModel.Channels.Message request)
    {
        return base.Channel.ComputeSum(request);
    }
}
