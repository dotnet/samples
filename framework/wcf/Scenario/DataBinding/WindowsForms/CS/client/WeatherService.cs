
//  Copyright (c) Microsoft Corporation. All rights reserved.

using System.Runtime.Serialization;
    
namespace Microsoft.Samples.WindowsForms
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="WeatherData", Namespace="http://Microsoft.Samples.WindowsForms")]
    public partial class WeatherData : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int HighTemperatureField;
        
        private string LocalityField;
        
        private int LowTemperatureField;
        
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
        public int HighTemperature
        {
            get
            {
                return this.HighTemperatureField;
            }
            set
            {
                this.HighTemperatureField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Locality
        {
            get
            {
                return this.LocalityField;
            }
            set
            {
                this.LocalityField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int LowTemperature
        {
            get
            {
                return this.LowTemperatureField;
            }
            set
            {
                this.LowTemperatureField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.WindowsForms", ConfigurationName="Microsoft.Samples.WindowsForms.IWeatherService")]
    public interface IWeatherService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.WindowsForms/IWeatherService/GetWeatherData", ReplyAction="http://Microsoft.Samples.WindowsForms/IWeatherService/GetWeatherDataResponse")]
        Microsoft.Samples.WindowsForms.WeatherData[] GetWeatherData(string[] localities);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://Microsoft.Samples.WindowsForms/IWeatherService/GetWeatherData", ReplyAction="http://Microsoft.Samples.WindowsForms/IWeatherService/GetWeatherDataResponse")]
        System.IAsyncResult BeginGetWeatherData(string[] localities, System.AsyncCallback callback, object asyncState);
        
        Microsoft.Samples.WindowsForms.WeatherData[] EndGetWeatherData(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWeatherServiceChannel : Microsoft.Samples.WindowsForms.IWeatherService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetWeatherDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        
        private object[] results;
        
        public GetWeatherDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState)
        {
            this.results = results;
        }
        
        public Microsoft.Samples.WindowsForms.WeatherData[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return ((Microsoft.Samples.WindowsForms.WeatherData[])(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WeatherServiceClient : System.ServiceModel.ClientBase<Microsoft.Samples.WindowsForms.IWeatherService>, Microsoft.Samples.WindowsForms.IWeatherService
    {
        
        private BeginOperationDelegate onBeginGetWeatherDataDelegate;
        
        private EndOperationDelegate onEndGetWeatherDataDelegate;
        
        private System.Threading.SendOrPostCallback onGetWeatherDataCompletedDelegate;
        
        public WeatherServiceClient()
        {
        }
        
        public WeatherServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public WeatherServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public WeatherServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public WeatherServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public event System.EventHandler<GetWeatherDataCompletedEventArgs> GetWeatherDataCompleted;
        
        public Microsoft.Samples.WindowsForms.WeatherData[] GetWeatherData(string[] localities)
        {
            return base.Channel.GetWeatherData(localities);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetWeatherData(string[] localities, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginGetWeatherData(localities, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public Microsoft.Samples.WindowsForms.WeatherData[] EndGetWeatherData(System.IAsyncResult result)
        {
            return base.Channel.EndGetWeatherData(result);
        }
        
        private System.IAsyncResult OnBeginGetWeatherData(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            string[] localities = ((string[])(inValues[0]));
            return this.BeginGetWeatherData(localities, callback, asyncState);
        }
        
        private object[] OnEndGetWeatherData(System.IAsyncResult result)
        {
            Microsoft.Samples.WindowsForms.WeatherData[] retVal = this.EndGetWeatherData(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetWeatherDataCompleted(object state)
        {
            System.EventHandler<GetWeatherDataCompletedEventArgs> handler = this.GetWeatherDataCompleted;
            if ((handler != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                handler(this, new GetWeatherDataCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetWeatherDataAsync(string[] localities)
        {
            this.GetWeatherDataAsync(localities, null);
        }
        
        public void GetWeatherDataAsync(string[] localities, object userState)
        {
            if ((this.onBeginGetWeatherDataDelegate == null))
            {
                this.onBeginGetWeatherDataDelegate = new BeginOperationDelegate(this.OnBeginGetWeatherData);
            }
            if ((this.onEndGetWeatherDataDelegate == null))
            {
                this.onEndGetWeatherDataDelegate = new EndOperationDelegate(this.OnEndGetWeatherData);
            }
            if ((this.onGetWeatherDataCompletedDelegate == null))
            {
                this.onGetWeatherDataCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetWeatherDataCompleted);
            }
            base.InvokeAsync(this.onBeginGetWeatherDataDelegate, new object[] {
                        localities}, this.onEndGetWeatherDataDelegate, this.onGetWeatherDataCompletedDelegate, userState);
        }
    }
}
