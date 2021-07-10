//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Description;
using System.Xml;

namespace Microsoft.Samples.HttpCookieSession
{
    public class HttpCookieSessionBindingElementImporter : IPolicyImportExtension
    {
        void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, 
            PolicyConversionContext context)
        {
            foreach (XmlElement assertion in context.GetBindingAssertions())
            {
                if (assertion.Name == HttpCookiePolicyStrings.HttpCookiePolicyElement
                    && assertion.NamespaceURI == HttpCookiePolicyStrings.Namespace)
                {
                    HttpCookieSessionBindingElement bindingElement =
                        new HttpCookieSessionBindingElement();

                    XmlAttribute attribute =
                        assertion.Attributes[HttpCookiePolicyStrings.ExchangeTerminateAttribute];

                    if(attribute != null)
                    {
                        bindingElement.ExchangeTerminateMessage = true;
                    }

                    context.BindingElements.Add(bindingElement);
                    break;
                }
            }
        }
    }
}
