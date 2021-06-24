
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Web.Hosting;
using Microsoft.Web.Administration;
using Microsoft.Win32;

namespace Microsoft.ServiceModel.Samples.Hosting
{
    class HostedUdpTransportConfigurationImpl
    {
        static HostedUdpTransportConfigurationImpl singleton;
        static object syncRoot = new object();
        List<Uri> baseAddresses;
        HostedUdpTransportManager transportManager;

        HostedUdpTransportConfigurationImpl()
        {
            List<string> bindingInfoList = GetBindingInfoList();

            baseAddresses = new List<Uri>();
            foreach (string bindingInfo in bindingInfoList)
            {
                int port;
                if (!int.TryParse(bindingInfo, out port))
                {
                    throw new InvalidDataException(string.Format("The binding information '{0}' is invalid for the protocol '{1}'",
                        bindingInfo, UdpConstants.Scheme));
                }

                // FUTURE: Use configured host from WAS config as HTTP case.
                UriBuilder builder = new UriBuilder(UdpConstants.Scheme, "localhost", port,
                    HostingEnvironment.ApplicationVirtualPath);

                baseAddresses.Add(builder.Uri);
            }

            // FUTURE: add support with multiple transport managers.
            transportManager = new HostedUdpTransportManager(baseAddresses[0]);
            UdpTransportManager.Add(transportManager);
        }

        private static List<string> GetBindingInfoList()
        {
            IDictionary<string, List<string>> table = null;

            if (IsIis7())
            {
                ConfigurationElement ceSite = WebConfigurationManagerWrapper.GetSite(HostingEnvironment.SiteName);
                table = WebConfigurationManagerWrapper.GetProtocolBindingTable(ceSite);
            }
            else
            {
                Site site = ServerManagerWrapper.GetSite(HostingEnvironment.SiteName);
                table = ServerManagerWrapper.GetProtocolBindingTable(site);
            }

            if (!table.ContainsKey(UdpConstants.Scheme))
            {
                throw new InvalidOperationException(string.Format("No binding is found for the protocol '{0}'", UdpConstants.Scheme));
            }

            return table[UdpConstants.Scheme];
        }

        [SecuritySafeCritical]
        internal static bool IsIis7()
        {
            int iisVersion = -1;
            object majorVersion = UnsafeGetMajorVersionFromRegistry();
            if (majorVersion != null && majorVersion.GetType().Equals(typeof(int)))
            {
                iisVersion = (int)majorVersion;
            }
            return iisVersion >= 7;
        }

        internal const string subKey = @"Software\Microsoft\InetSTP";

        /// <SecurityNote>
        /// Critical - asserts registry access to get a single value from the registry
        ///            caller should not leak value
        /// </SecurityNote>
        [SecurityCritical]
        [RegistryPermission(SecurityAction.Assert, Read = @"HKEY_LOCAL_MACHINE\" + subKey)]
        internal static object UnsafeGetMajorVersionFromRegistry()
        {
            using (RegistryKey localMachine = Registry.LocalMachine)
            {
                using (RegistryKey versionKey = localMachine.OpenSubKey(subKey))
                {
                    return versionKey != null ? versionKey.GetValue("MajorVersion") : null;
                }
            }
        }

        static object StaticLock
        {
            get
            {
                return syncRoot;
            }
        }

        public Uri[] GetBaseAddresses(string virtualPath)
        {
            Uri[] addresses = new Uri[baseAddresses.Count];
            for (int i = 0; i < baseAddresses.Count; i++)
            {
                addresses[i] = new Uri(baseAddresses[i], virtualPath);
            }

            return addresses;
        }

        public HostedUdpTransportManager TransportManager
        {
            get
            {
                return this.transportManager;
            }
        }

        public static HostedUdpTransportConfigurationImpl Value
        {
            get
            {
                if (singleton != null)
                {
                    return singleton;
                }

                lock (StaticLock)
                {
                    if (singleton != null)
                    {
                        return singleton;
                    }

                    singleton = new HostedUdpTransportConfigurationImpl();
                    return singleton;
                }
            }
        }
    }

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    internal static class ServerManagerWrapper
    {
        internal static Site GetSite(string name)
        {
            return new ServerManager().Sites[name];
        }

        internal static IDictionary<string, List<string>> GetProtocolBindingTable(Site site)
        {
            IDictionary<string, List<string>> bindingList = new Dictionary<string, List<string>>();
            foreach (Binding binding in site.Bindings)
            {
                string protocol = binding.Protocol.ToLowerInvariant();
                string bindingInformation = binding.BindingInformation;

                if (!bindingList.ContainsKey(protocol))
                {
                    bindingList.Add(protocol, new List<string>());
                }
                bindingList[protocol].Add(bindingInformation);
            }
            return bindingList;
        }
    }

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    internal static class WebConfigurationManagerWrapper
    {
        private static MethodInfo getSectionMethod;
        private static MethodInfo GetSectionMethod
        {
            get
            {
                if (getSectionMethod == null)
                {
                    Type type = typeof(WebConfigurationManager);

                    getSectionMethod = type.GetMethod(
                        MetabaseSettingsIis7Constants.WebConfigGetSectionMethodName,
                        new Type[3] { typeof(string), typeof(string), typeof(string) }
                        );
                }
                return getSectionMethod;
            }
        }

        // Helper Method to get a site configuration
        internal static ConfigurationElement GetSite(string siteName)
        {
            ConfigurationSection sitesSection = WebConfigGetSection(null, null, MetabaseSettingsIis7Constants.SitesSectionName);
            ConfigurationElementCollection sitesCollection = sitesSection.GetCollection();

            return FindElement(sitesCollection, MetabaseSettingsIis7Constants.NameAttributeName, siteName);
        }

        private static ConfigurationSection WebConfigGetSection(string siteName, string virtualPath, string sectionName)
        {
            return (ConfigurationSection)GetSectionMethod.Invoke(null, new object[] { siteName, virtualPath, sectionName });
        }

        // Helper method to find element based on an string attribute.
        private static ConfigurationElement FindElement(ConfigurationElementCollection collection, string attributeName, string value)
        {
            foreach (ConfigurationElement element in collection)
            {
                if (String.Equals((string)element[attributeName], value, StringComparison.OrdinalIgnoreCase))
                {
                    return element;
                }
            }

            return null;
        }

        internal static IDictionary<string, List<string>> GetProtocolBindingTable(ConfigurationElement site)
        {
            IDictionary<string, List<string>> bindingList = new Dictionary<string, List<string>>();
            foreach (ConfigurationElement binding in site.GetCollection(MetabaseSettingsIis7Constants.BindingsElementName))
            {
                string protocol = ((string)binding[MetabaseSettingsIis7Constants.ProtocolAttributeName]).ToLowerInvariant();
                string bindingInformation = (string)binding[MetabaseSettingsIis7Constants.BindingInfoAttributeName];

                if (!bindingList.ContainsKey(protocol))
                {
                    bindingList.Add(protocol, new List<string>());
                }
                bindingList[protocol].Add(bindingInformation);
            }
            return bindingList;
        }
    }

    internal static class MetabaseSettingsIis7Constants
    {
        internal const string SitesSectionName = "system.applicationHost/sites";

        internal const string BindingsElementName = "bindings";
        internal const string ProtocolAttributeName = "protocol";
        internal const string BindingInfoAttributeName = "bindingInformation";
        internal const string NameAttributeName = "name";

        internal const string WebConfigGetSectionMethodName = "GetSection";
    }

}

