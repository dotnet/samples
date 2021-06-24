//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Configuration;
using System.Diagnostics.Eventing;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Configuration;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Microsoft.Samples.WCFAnalyticTracingExtensibility
{

    public class WCFUserEventProvider
    {
        const string DiagnosticsConfigSectionName = "system.serviceModel/diagnostics";

        const int ErrorEventId   = 301;
        const int WarningEventId = 302;
        const int InfoEventId    = 303;

        const int Version   = 0;
        const int Task      = 0;
        const int Opcode    = 0;
        const long Keywords = (long)0x20000000001e0004;
        const byte Channel  = 0x12;

        const int ErrorLevel   = 0x2;
        const int WarningLevel = 0x3;
        const int InfoLevel    = 0x4;

        EventDescriptor errorDescriptor;
        EventDescriptor warningDescriptor;
        EventDescriptor infoDescriptor;

        bool hostReferenceIsComplete;
        string hostReference;

        private String HostReference
        {
            get
            {
                if (hostReferenceIsComplete == false)
                {
                    CreateHostReference();
                }
                return hostReference;
            }
        }

        EventProvider innerEventProvider;

        public WCFUserEventProvider()
        {
            Guid providerId;
            if (HostingEnvironment.IsHosted)
            {
                DiagnosticSection config = (DiagnosticSection)WebConfigurationManager.GetSection(DiagnosticsConfigSectionName);
                providerId = new Guid(config.EtwProviderId);
                hostReferenceIsComplete = false;
            }
            else
            {
                DiagnosticSection config = (DiagnosticSection)ConfigurationManager.GetSection(DiagnosticsConfigSectionName);
                providerId = new Guid(config.EtwProviderId);
                hostReference = string.Empty;
                hostReferenceIsComplete = true;
            }

            innerEventProvider = new EventProvider(providerId);

            errorDescriptor = new EventDescriptor(ErrorEventId, Version, Channel, ErrorLevel, Opcode, Task, Keywords);
            warningDescriptor = new EventDescriptor(WarningEventId, Version, Channel, WarningLevel, Opcode, Task, Keywords);
            infoDescriptor = new EventDescriptor(InfoEventId, Version, Channel, InfoLevel, Opcode, Task, Keywords);
        }

        public bool WriteErrorEvent(string name, string payload)
        {
            if (!innerEventProvider.IsEnabled(errorDescriptor.Level, errorDescriptor.Keywords))
            {
                return true;
            }
            return innerEventProvider.WriteEvent(ref errorDescriptor, name, HostReference, payload);
        }

        public bool WriteWarningEvent(string name, string payload)
        {
            if (!innerEventProvider.IsEnabled(warningDescriptor.Level, warningDescriptor.Keywords))
            {
                return true;
            }
            return innerEventProvider.WriteEvent(ref warningDescriptor, name, HostReference, payload);
        }

        public bool WriteInformationEvent(string name, string payload)
        {
            if (!innerEventProvider.IsEnabled(infoDescriptor.Level, infoDescriptor.Keywords))
            {
                return true;
            }
            return innerEventProvider.WriteEvent(ref infoDescriptor, name, HostReference, payload);
        }

        private void CreateHostReference()
        {
            if (OperationContext.Current != null)
            {
                ServiceHostBase serviceHostBase = OperationContext.Current.Host;

                VirtualPathExtension virtualPathExtension = serviceHostBase.Extensions.Find<VirtualPathExtension>();
                if (virtualPathExtension != null && virtualPathExtension.VirtualPath != null)
                {

                    //     HostReference Format
                    //     <SiteName><ApplicationVirtualPath>|<ServiceVirtualPath>|<ServiceName> 

                    string serviceName = serviceHostBase.Description.Name;
                    string applicationVirtualPath = HostingEnvironment.ApplicationVirtualPath;
                    string serviceVirtualPath = virtualPathExtension.VirtualPath.Replace("~", string.Empty);

                    hostReference = string.Format("{0}{1}|{2}|{3}", HostingEnvironment.SiteName, applicationVirtualPath, serviceVirtualPath, serviceName);

                    hostReferenceIsComplete = true;
                    return;
                }
            }

            // If the entire host reference is not available, fall back to site name and app virtual path.  This will happen
            // if you try to emit a trace from outside an operation (e.g. startup) before an in operation trace has been emitted.
            hostReference = string.Format("{0}{1}", HostingEnvironment.SiteName, HostingEnvironment.ApplicationVirtualPath);
        }
    }
}
