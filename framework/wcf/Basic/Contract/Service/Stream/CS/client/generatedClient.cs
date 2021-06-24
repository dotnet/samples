
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.Samples.Stream
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.Stream", ConfigurationName="Microsoft.Samples.Stream.IStreamingSample")]
    public interface IStreamingSample
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Stream/IStreamingSample/GetStream", ReplyAction="http://Microsoft.Samples.Stream/IStreamingSample/GetStreamResponse")]
        System.IO.Stream GetStream(string data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Stream/IStreamingSample/UploadStream", ReplyAction="http://Microsoft.Samples.Stream/IStreamingSample/UploadStreamResponse")]
        bool UploadStream(System.IO.Stream stream);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Stream/IStreamingSample/EchoStream", ReplyAction="http://Microsoft.Samples.Stream/IStreamingSample/EchoStreamResponse")]
        System.IO.Stream EchoStream(System.IO.Stream stream);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.Stream/IStreamingSample/GetReversedStream", ReplyAction="http://Microsoft.Samples.Stream/IStreamingSample/GetReversedStreamResponse")]
        System.IO.Stream GetReversedStream();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IStreamingSampleChannel : Microsoft.Samples.Stream.IStreamingSample, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class StreamingSampleClient : System.ServiceModel.ClientBase<Microsoft.Samples.Stream.IStreamingSample>, Microsoft.Samples.Stream.IStreamingSample
    {
        
        public StreamingSampleClient()
        {
        }
        
        public StreamingSampleClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public StreamingSampleClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public StreamingSampleClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public StreamingSampleClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.IO.Stream GetStream(string data)
        {
            return base.Channel.GetStream(data);
        }
        
        public bool UploadStream(System.IO.Stream stream)
        {
            return base.Channel.UploadStream(stream);
        }
        
        public System.IO.Stream EchoStream(System.IO.Stream stream)
        {
            return base.Channel.EchoStream(stream);
        }
        
        public System.IO.Stream GetReversedStream()
        {
            return base.Channel.GetReversedStream();
        }
    }
}
