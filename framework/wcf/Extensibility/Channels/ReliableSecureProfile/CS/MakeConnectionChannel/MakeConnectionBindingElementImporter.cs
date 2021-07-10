//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public sealed class MakeConnectionBindingElementImporter : IPolicyImportExtension
    {
        void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
        {
            if (importer == null)
            {
                throw new ArgumentNullException("importer");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ICollection<XmlElement> assertions = context.GetBindingAssertions();

            foreach (XmlElement assertion in assertions)
            {
                if (assertion.NamespaceURI == MakeConnectionConstants.Namespace)
                {
                    if (assertion.LocalName == MakeConnectionConstants.Policy.Assertion)
                    {
                        assertions.Remove(assertion);
                        context.BindingElements.Insert(0, new MakeConnectionBindingElement());
                        return;
                    }
                }
            }
        }
    }
}
