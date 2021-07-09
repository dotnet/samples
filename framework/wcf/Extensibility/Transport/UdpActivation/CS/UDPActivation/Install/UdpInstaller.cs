
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Activation;
using WebAdmin = Microsoft.Web.Administration;
using Microsoft.ServiceModel.Samples;
using System.Security.Principal;
using System.Web.Configuration;
using System.IO;
using System.Configuration;
using Microsoft.ServiceModel.Samples.Hosting;
using System.Reflection;

namespace Microsoft.ServiceModel.Samples.Install
{
    public struct UdpInstallerOptions
    {
        public bool ListenerAdapterChecked;
        public bool ProtocolHandlerChecked;
    }

    public static class UdpInstaller
    {
        const string ListenerAdapterPath = "system.applicationHost/listenerAdapters";
        const string ProtocolsPath = "system.web/protocols";

        public static bool IsListenerAdapterInstalled
        {
            get
            {
                using (WebAdmin.ServerManager sm = new WebAdmin.ServerManager())
                {
                    WebAdmin.Configuration wasConfiguration = sm.GetApplicationHostConfiguration();
                    WebAdmin.ConfigurationSection section = wasConfiguration.GetSection(ListenerAdapterPath);
                    WebAdmin.ConfigurationElementCollection listenerAdaptersCollection = section.GetCollection();
                    foreach (WebAdmin.ConfigurationElement e in listenerAdaptersCollection)
                    {
                        if (string.Compare((string)e.GetAttribute("name").Value, UdpConstants.Scheme, true) == 0)
                        {
                            // Already installed.
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        public static bool IsProtocolHandlerInstalled
        {
            get
            {
                Configuration rootWebConfig = GetRootWebConfiguration();
                ProtocolsSection section = (ProtocolsSection)rootWebConfig.GetSection(ProtocolsPath);
                foreach (ProtocolElement protocol in section.Protocols)
                {
                    if (string.Compare(protocol.Name, UdpConstants.Scheme, true) == 0)
                    {
                        // Already installed.
                        return true;
                    }
                }

                return false;
            }
        }

        public static void Install(UdpInstallerOptions options)
        {
            if (options.ListenerAdapterChecked)
            {
                WebAdmin.ServerManager sm = new WebAdmin.ServerManager();
                WebAdmin.Configuration wasConfiguration = sm.GetApplicationHostConfiguration();
                WebAdmin.ConfigurationSection section = wasConfiguration.GetSection(ListenerAdapterPath);
                WebAdmin.ConfigurationElementCollection listenerAdaptersCollection = section.GetCollection();
                WebAdmin.ConfigurationElement element = listenerAdaptersCollection.CreateElement();
                element.GetAttribute("name").Value = UdpConstants.Scheme;
                element.GetAttribute("identity").Value = WindowsIdentity.GetCurrent().User.Value;
                listenerAdaptersCollection.Add(element);
                sm.CommitChanges();
                wasConfiguration = null;
                sm = null;
            }

            if (options.ProtocolHandlerChecked)
            {
                Configuration rootWebConfig = GetRootWebConfiguration();
                ProtocolsSection section = (ProtocolsSection)rootWebConfig.GetSection(ProtocolsPath);
                ProtocolElement element = new ProtocolElement(UdpConstants.Scheme);
                
                element.ProcessHandlerType = typeof(UdpProcessProtocolHandler).AssemblyQualifiedName;
                element.AppDomainHandlerType = typeof(UdpAppDomainProtocolHandler).AssemblyQualifiedName;
                element.Validate = false;

                section.Protocols.Add(element);
                rootWebConfig.Save();
            }
        }

        static Configuration GetRootWebConfiguration()
        {
            return WebConfigurationManager.OpenWebConfiguration(string.Empty);
        }

        public static void Uninstall(UdpInstallerOptions options)
        {
            if (options.ListenerAdapterChecked)
            {
                WebAdmin.ServerManager sm = new WebAdmin.ServerManager();
                WebAdmin.Configuration wasConfiguration = sm.GetApplicationHostConfiguration();
                WebAdmin.ConfigurationSection section = wasConfiguration.GetSection(ListenerAdapterPath);
                WebAdmin.ConfigurationElementCollection listenerAdaptersCollection = section.GetCollection();

                for (int i = 0; i<listenerAdaptersCollection.Count; i++)
                {
                    WebAdmin.ConfigurationElement element = listenerAdaptersCollection[i];

                    if (string.Compare((string)element.GetAttribute("name").Value,
                        UdpConstants.Scheme, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        listenerAdaptersCollection.RemoveAt(i);
                    }
                }

                sm.CommitChanges();
                wasConfiguration = null;
                sm = null;
            }

            if (options.ProtocolHandlerChecked)
            {
                Configuration rootWebConfig = GetRootWebConfiguration();
                ProtocolsSection section = (ProtocolsSection)rootWebConfig.GetSection(ProtocolsPath);
                section.Protocols.Remove(UdpConstants.Scheme);
                rootWebConfig.Save();
            }
        }
    }
}

