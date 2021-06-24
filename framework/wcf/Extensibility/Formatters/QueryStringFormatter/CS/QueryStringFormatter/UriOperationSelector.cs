
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace Microsoft.ServiceModel.Samples
{

    class UriPathSuffixOperationSelector : IDispatchOperationSelector
    {
        Uri endpointUri;
        Dictionary<string, string> operationNameDictionary;

        public UriPathSuffixOperationSelector(Uri endpointUri, Dictionary<string,string> operationNameDictionary)
        {
            if (operationNameDictionary == null)
                throw new ArgumentNullException("operationNameDictionary");

            if (endpointUri == null)
                throw new ArgumentNullException("endpointUri");

            if (endpointUri.ToString().EndsWith("/"))
            {
                this.endpointUri = new Uri(endpointUri.ToString().ToLower());
            }
            else
            {
                this.endpointUri = new Uri(endpointUri.ToString().ToLower() + "/");
            }
                        
            this.operationNameDictionary = operationNameDictionary;
        }

        public string SelectOperation(ref System.ServiceModel.Channels.Message message)
        {
            string operationName = null;

            Uri via = null;
            if (message.Properties.ContainsKey("Via"))
            {
                via = message.Properties["Via"] as Uri;
            }

            if (via != null && !String.IsNullOrEmpty(via.ToString()))
            {
                Uri operationUri = new Uri(via.GetLeftPart(UriPartial.Path).ToString().ToLower());
                if (endpointUri.IsBaseOf(operationUri))
                {
                    string relativeUri = endpointUri.MakeRelativeUri(operationUri).ToString();
                    string name = null;
                    if (relativeUri.Contains("/"))
                    {
                        name = relativeUri.Substring(0, relativeUri.IndexOf('/'));
                    }
                    else
                    {
                        name = relativeUri;
                    }

                    if (!String.IsNullOrEmpty(name) && operationNameDictionary.ContainsKey(name))
                    {
                        operationName = operationNameDictionary[name];
                    }
                }
            }
            if (!String.IsNullOrEmpty(operationName))
            {
                return operationName;
            }
            else
            {
                throw new Exception(String.Format("Missing or unknown operation name in URL {0}", via));
            }
        }

    }
}
