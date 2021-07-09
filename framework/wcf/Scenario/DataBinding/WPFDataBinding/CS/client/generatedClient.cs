
//  Copyright (c) Microsoft Corporation. All rights reserved.

using System.Runtime.Serialization;
    
namespace Microsoft.Samples.DataBinding
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Album", Namespace="http://Microsoft.Samples.DataBinding")]
    public partial class Album : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private double PriceField;
        
        private string TitleField;
        
        private Microsoft.Samples.DataBinding.Track[] TracksField;
        
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
        public double Price
        {
            get
            {
                return this.PriceField;
            }
            set
            {
                this.PriceField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title
        {
            get
            {
                return this.TitleField;
            }
            set
            {
                this.TitleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Microsoft.Samples.DataBinding.Track[] Tracks
        {
            get
            {
                return this.TracksField;
            }
            set
            {
                this.TracksField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Track", Namespace="http://Microsoft.Samples.DataBinding")]
    public partial class Track : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int DurationField;
        
        private string NameField;
        
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
        public int Duration
        {
            get
            {
                return this.DurationField;
            }
            set
            {
                this.DurationField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                this.NameField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.Samples.DataBinding", ConfigurationName="Microsoft.Samples.DataBinding.IAlbumService")]
    public interface IAlbumService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.DataBinding/IAlbumService/GetAlbumList", ReplyAction="http://Microsoft.Samples.DataBinding/IAlbumService/GetAlbumListResponse")]
        Microsoft.Samples.DataBinding.Album[] GetAlbumList();
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://Microsoft.Samples.DataBinding/IAlbumService/GetAlbumList", ReplyAction="http://Microsoft.Samples.DataBinding/IAlbumService/GetAlbumListResponse")]
        System.IAsyncResult BeginGetAlbumList(System.AsyncCallback callback, object asyncState);
        
        Microsoft.Samples.DataBinding.Album[] EndGetAlbumList(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.Samples.DataBinding/IAlbumService/AddAlbum", ReplyAction="http://Microsoft.Samples.DataBinding/IAlbumService/AddAlbumResponse")]
        void AddAlbum(string title);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://Microsoft.Samples.DataBinding/IAlbumService/AddAlbum", ReplyAction="http://Microsoft.Samples.DataBinding/IAlbumService/AddAlbumResponse")]
        System.IAsyncResult BeginAddAlbum(string title, System.AsyncCallback callback, object asyncState);
        
        void EndAddAlbum(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAlbumServiceChannel : Microsoft.Samples.DataBinding.IAlbumService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetAlbumListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        
        private object[] results;
        
        public GetAlbumListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState)
        {
            this.results = results;
        }
        
        public Microsoft.Samples.DataBinding.Album[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return ((Microsoft.Samples.DataBinding.Album[])(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AlbumServiceClient : System.ServiceModel.ClientBase<Microsoft.Samples.DataBinding.IAlbumService>, Microsoft.Samples.DataBinding.IAlbumService
    {
        
        private BeginOperationDelegate onBeginGetAlbumListDelegate;
        
        private EndOperationDelegate onEndGetAlbumListDelegate;
        
        private System.Threading.SendOrPostCallback onGetAlbumListCompletedDelegate;
        
        private BeginOperationDelegate onBeginAddAlbumDelegate;
        
        private EndOperationDelegate onEndAddAlbumDelegate;
        
        private System.Threading.SendOrPostCallback onAddAlbumCompletedDelegate;
        
        public AlbumServiceClient()
        {
        }
        
        public AlbumServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public AlbumServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public AlbumServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public AlbumServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public event System.EventHandler<GetAlbumListCompletedEventArgs> GetAlbumListCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> AddAlbumCompleted;
        
        public Microsoft.Samples.DataBinding.Album[] GetAlbumList()
        {
            return base.Channel.GetAlbumList();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetAlbumList(System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginGetAlbumList(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public Microsoft.Samples.DataBinding.Album[] EndGetAlbumList(System.IAsyncResult result)
        {
            return base.Channel.EndGetAlbumList(result);
        }
        
        private System.IAsyncResult OnBeginGetAlbumList(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginGetAlbumList(callback, asyncState);
        }
        
        private object[] OnEndGetAlbumList(System.IAsyncResult result)
        {
            Microsoft.Samples.DataBinding.Album[] retVal = this.EndGetAlbumList(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetAlbumListCompleted(object state)
        {
            if ((this.GetAlbumListCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetAlbumListCompleted(this, new GetAlbumListCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetAlbumListAsync()
        {
            this.GetAlbumListAsync(null);
        }
        
        public void GetAlbumListAsync(object userState)
        {
            if ((this.onBeginGetAlbumListDelegate == null))
            {
                this.onBeginGetAlbumListDelegate = new BeginOperationDelegate(this.OnBeginGetAlbumList);
            }
            if ((this.onEndGetAlbumListDelegate == null))
            {
                this.onEndGetAlbumListDelegate = new EndOperationDelegate(this.OnEndGetAlbumList);
            }
            if ((this.onGetAlbumListCompletedDelegate == null))
            {
                this.onGetAlbumListCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetAlbumListCompleted);
            }
            base.InvokeAsync(this.onBeginGetAlbumListDelegate, null, this.onEndGetAlbumListDelegate, this.onGetAlbumListCompletedDelegate, userState);
        }
        
        public void AddAlbum(string title)
        {
            base.Channel.AddAlbum(title);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginAddAlbum(string title, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginAddAlbum(title, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndAddAlbum(System.IAsyncResult result)
        {
            base.Channel.EndAddAlbum(result);
        }
        
        private System.IAsyncResult OnBeginAddAlbum(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            string title = ((string)(inValues[0]));
            return this.BeginAddAlbum(title, callback, asyncState);
        }
        
        private object[] OnEndAddAlbum(System.IAsyncResult result)
        {
            this.EndAddAlbum(result);
            return null;
        }
        
        private void OnAddAlbumCompleted(object state)
        {
            if ((this.AddAlbumCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.AddAlbumCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void AddAlbumAsync(string title)
        {
            this.AddAlbumAsync(title, null);
        }
        
        public void AddAlbumAsync(string title, object userState)
        {
            if ((this.onBeginAddAlbumDelegate == null))
            {
                this.onBeginAddAlbumDelegate = new BeginOperationDelegate(this.OnBeginAddAlbum);
            }
            if ((this.onEndAddAlbumDelegate == null))
            {
                this.onEndAddAlbumDelegate = new EndOperationDelegate(this.OnEndAddAlbum);
            }
            if ((this.onAddAlbumCompletedDelegate == null))
            {
                this.onAddAlbumCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAddAlbumCompleted);
            }
            base.InvokeAsync(this.onBeginAddAlbumDelegate, new object[] {
                        title}, this.onEndAddAlbumDelegate, this.onAddAlbumCompletedDelegate, userState);
        }
    }
}
