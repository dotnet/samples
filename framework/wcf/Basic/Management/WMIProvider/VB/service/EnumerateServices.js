var locator = WScript.CreateObject ("WbemScripting.SWbemLocator");
var provider = locator.ConnectServer ("", "root/ServiceModel");
var wcf;
var objSet;
var serviceCount;

wcf = GetObject("winmgmts:root/ServiceModel");
objSet = wcf.InstancesOf ("Service");
serviceCount = objSet.Count;

WScript.Echo (serviceCount + " service(s) found.");

var serviceReferenceList = "";
var services = new Enumerator (objSet);
for (;!services.atEnd();services.moveNext ())
{
	var service = services.item ();

	var query = endpointsQuery = "ASSOCIATORS OF {" + "Service.DistinguishedName=\"" + service.DistinguishedName + "\",ProcessId=" + service.ProcessId + "} WHERE AssocClass = ServiceToEndpointAssociation";
	var serviceReference = "Service.DistinguishedName=\"" + service.DistinguishedName + "\",ProcessId=" + service.ProcessId;
	if (serviceReferenceList != "")
	{
		serviceReferenceList = serviceReferenceList + "#";
	}
	serviceReferenceList = serviceReferenceList + serviceReference;
}

var serviceReferenceArray = serviceReferenceList.split('#'); 
serviceReferenceArray.sort(); 

for (var s = 0 ; s < serviceReferenceArray.length ; ++s)
{
    if (serviceReferenceArray[s] != "")
    {
        var service = wcf.Get(serviceReferenceArray[s]);

    	WScript.Echo("|-PID:           " + service.ProcessId);
    	WScript.Echo("|-DistinguishedName:  " + service.DistinguishedName);

        var query = endpointsQuery = "ASSOCIATORS OF {" + serviceReferenceArray[s] + "} WHERE AssocClass = ServiceToEndpointAssociation";

        var endpointsCollection = provider.ExecQuery(query);
        WScript.Echo ("|-Endpoints:     " + endpointsCollection.Count + " endpoints");


        var endpoints = new Enumerator (endpointsCollection);
        while (!endpoints.atEnd())
        {
            var endpoint = endpoints.item ();
            var bindings = new VBArray(endpoint.Binding.BindingElements);
            bindings = bindings.toArray();
            // Masking PeerService (infrastructure service) EndpointAddress.Uri because the service address contains GUID
            if (service.Type != "PeerService")
            {
                WScript.Echo ("  |-" + endpoint.Name);
                WScript.Echo ("    |-Address:                        " + endpoint.Address);
                WScript.Echo ("    |-CounterInstanceName:            " + endpoint.CounterInstanceName);
            }
            var addressHeaders = new VBArray(endpoint.AddressHeaders);
            addressHeaders = addressHeaders.toArray();
            WScript.Echo ("    |-AddressHeaders:                 " + addressHeaders.length);
            WScript.Echo ("    |-ContractType:                   " + endpoint.Contract.substring(0, endpoint.Contract.indexOf(",")));
            WScript.Echo ("    |-BindingElements:                " + bindings.length + " bindings");
            for (var j = 0 ; j < bindings.length ; ++j)
            {
                WScript.Echo ("      |-BindingElements[" + j + "]");
                WScript.Echo ("        |-Type:                       " + bindings[j].Path_.Class);
                if (bindings[j].Path_.Class == "NamedPipeTransportBindingElement" //Transport
                    || bindings[j].Path_.Class == "TcpTransportBindingElement"
                    || bindings[j].Path_.Class == "MsmqTransportBindingElement"
                    || bindings[j].Path_.Class == "MsmqIntegrationBindingElement"
                    || bindings[j].Path_.Class == "PeerTransportBindingElement"
                    || bindings[j].Path_.Class == "HttpTransportBindingElement"
                    || bindings[j].Path_.Class == "HttpsTransportBindingElement")
                { 
     	            WScript.Echo ("        |-ManualAddressing:           " + bindings[j].ManualAddressing);
     	            WScript.Echo ("        |-MaxBufferSize:              " + bindings[j].MaxBufferSize);
                }
                if (bindings[j].Path_.Class == "NamedPipeTransportBindingElement" || bindings[j].Path_.Class == "TcpTransportBindingElement") //ConnectionOriented
                { 
     	            WScript.Echo ("        |-ConnectionBufferSize:       " + bindings[j].ConnectionBufferSize);
     	            WScript.Echo ("        |-ConnectionPoolGroupName:    " + bindings[j].ConnectionPoolSettings.GroupName);
     	            WScript.Echo ("        |-HostNameComparisonMode:     " + bindings[j].HostNameComparisonMode);
     	            WScript.Echo ("        |-MaxOutputDelay:             " + bindings[j].MaxOutputDelay);
     	            WScript.Echo ("        |-MaxPendingConnections:      " + bindings[j].MaxPendingConnections);
     	            WScript.Echo ("        |-MaxPendingAccepts:          " + bindings[j].MaxPendingAccepts);
     	            WScript.Echo ("        |-TransferMode:               " + bindings[j].TransferMode);
                } 
                else if (bindings[j].Path_.Class == "TcpTransportBindingElement")
                { 
     	            WScript.Echo ("        |-ListenBacklog:              " + bindings[j].ListenBacklog);
     	            WScript.Echo ("        |-PortSharingEnabled:         " + bindings[j].PortSharingEnabled);
                } 
                else if (bindings[j].Path_.Class == "HttpTransportBindingElement")
                { 
     	            WScript.Echo ("        |-AllowCookies:               " + bindings[j].AllowCookies);
     	            WScript.Echo ("        |-AuthenticationScheme:       " + bindings[j].AuthenticationScheme);
     	            WScript.Echo ("        |-BypassProxyOnLocal:         " + bindings[j].BypassProxyOnLocal);
     	            WScript.Echo ("        |-HostNameComparisonMode:     " + bindings[j].HostNameComparisonMode);
     	            WScript.Echo ("        |-ProxyAddress:               " + bindings[j].ProxyAddress);
     	            WScript.Echo ("        |-ProxyAuthenticationScheme:  " + bindings[j].ProxyAuthenticationScheme);
     	            WScript.Echo ("        |-Realm:                      " + bindings[j].Realm);
     	            WScript.Echo ("        |-TransferMode:               " + bindings[j].TransferMode);
     	            WScript.Echo ("        |-UseDefaultWebProxy:         " + bindings[j].UseDefaultWebProxy);
                } 
                else if (bindings[j].Path_.Class == "PeerTransportBindingElement")
                { 
     	            WScript.Echo ("        |-ListenIPAddress:            " + bindings[j].ListenIPAddress);
     	            WScript.Echo ("        |-Port:                       " + bindings[j].Port);
     	            WScript.Echo ("        |-CredentialType:      " + bindings[j].Security.CredentialType);
                } 
                else if (bindings[j].Path_.Class == "TextMessageEncodingBindingElement" 
                    || bindings[j].Path_.Class == "BinaryMessageEncodingBindingElement"
                    || bindings[j].Path_.Class == "MtomMessageEncodingBindingElement")
                { 
     	            WScript.Echo ("        |-MaxReadPoolSize:            " + bindings[j].MaxReadPoolSize);
     	            WScript.Echo ("        |-MaxWritePoolSize:           " + bindings[j].MaxWritePoolSize);
                } 
                else if (bindings[j].Path_.Class == "BinaryMessageEncodingBindingElement")
                { 
     	            WScript.Echo ("        |-MaxSessionSize:             " + bindings[j].MaxSessionSize);
                } 
                else if (bindings[j].Path_.Class == "MtomMessageEncodingBindingElement")
                { 
     	            WScript.Echo ("        |-Encoding:                   " + bindings[j].Encoding);
     	            WScript.Echo ("        |-MessageVersion:             " + bindings[j].MessageVersion);
                } 
                else if (bindings[j].Path_.Class == "TextMessageEncodingBindingElement")
                { 
     	            WScript.Echo ("        |-Encoding:                   " + bindings[j].Encoding);
     	            WScript.Echo ("        |-MessageVersion:             " + bindings[j].MessageVersion);
                } 
                else if (bindings[j].Path_.Class == "WindowsStreamSecurityBindingElement")
                { 
     	            WScript.Echo ("        |-ProtectionLevel:            " + bindings[j].ProtectionLevel);
                } 
                else if (bindings[j].Path_.Class == "SecurityBindingElement")
                { 
     	            WScript.Echo ("        |-DefaultAlgorithmSuite:      " + bindings[j].DefaultAlgorithmSuite);
     	            WScript.Echo ("        |-IncludeTimestamp:           " + bindings[j].IncludeTimestamp);
     	            WScript.Echo ("        |-MessageSecurityVersion:     " + bindings[j].MessageSecurityVersion);
                } 
                else if (bindings[j].Path_.Class == "ReliableSessionBindingElement")
                { 
     	            WScript.Echo ("        |-AcknowledgementInterval:    " + bindings[j].AcknowledgementInterval);
     	            WScript.Echo ("        |-FlowControlEnabled:         " + bindings[j].FlowControlEnabled);
     	            WScript.Echo ("        |-InactivityTimeout:          " + bindings[j].InactivityTimeout);
     	            WScript.Echo ("        |-MaxPendingChannels:         " + bindings[j].MaxPendingChannels);
     	            WScript.Echo ("        |-MaxRetryCount:              " + bindings[j].MaxRetryCount);
     	            WScript.Echo ("        |-Ordered:                    " + bindings[j].Ordered);
                } 
            } 
            endpoints.moveNext ();
        }

        var behaviors = new VBArray(service.Behaviors);
        behaviors = behaviors.toArray();

        WScript.Echo ("|-Behaviors:     " + behaviors.length + " behaviors");
        for (var i = 0 ; i < behaviors.length ; ++i)
        {
            WScript.Echo ("      |-Behavior[" + i + "]");
            WScript.Echo ("      |-Type:                       " + behaviors[i].Path_.Class);
            if (behaviors[i].Path_.Class == "ServiceBehaviorAttribute")
            { 
     	        WScript.Echo ("        |-AddressFilterMode:               " + behaviors[i].AddressFilterMode);
     	        WScript.Echo ("        |-AutomaticSessionShutdown:        " + behaviors[i].AutomaticSessionShutdown);
     	        WScript.Echo ("        |-ConcurrencyMode:                 " + behaviors[i].ConcurrencyMode);
     	        WScript.Echo ("        |-IncludeExceptionDetailInFaults:  " + behaviors[i].IncludeExceptionDetailInFaults);
     	        WScript.Echo ("        |-InstanceContextMode:             " + behaviors[i].InstanceContextMode);
     	        WScript.Echo ("        |-TransactionIsolationLevel:       " + behaviors[i].TransactionIsolationLevel);
     	        WScript.Echo ("        |-TransactionTimeout:              " + behaviors[i].TransactionTimeout);
     	        WScript.Echo ("        |-ValidateMustUnderstand:          " + behaviors[i].ValidateMustUnderstand);
            } 
            else if (behaviors[i].Path_.Class == "ServiceMetadataBehavior")
            { 
     	        WScript.Echo ("        |-ExternalMetadataLocation:       " + behaviors[i].ExternalMetadataLocation);
     	        WScript.Echo ("        |-EnableGetEnabled:               " + behaviors[i].EnableGetEnabled);
     	        WScript.Echo ("        |-HttpGetUrl:                     " + behaviors[i].HttpGetUrl);
            } 
            else if (behaviors[i].Path_.Class == "ServiceThrottlingBehavior")
            { 
     	        WScript.Echo ("        |-MaxConcurrentCalls:             " + behaviors[i].MaxConcurrentCalls);
     	        WScript.Echo ("        |-MaxConcurrentInstances:         " + behaviors[i].MaxConcurrentInstances);
     	        WScript.Echo ("        |-MaxConcurrentSessions:          " + behaviors[i].MaxConcurrentSessions);
            } 
        } 
    }
}

