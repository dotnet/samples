//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Microsoft.Samples.Tools.CustomChannelsTester
{
    //Validates the XML file againt the schema from TestSpec.xsd 

    public class SchemaValidator
    {
        readonly XmlSchemaSet schemaSet;

        public SchemaValidator(string schemaNamespace, string schemaPath)
        {
            if (schemaNamespace == null)
            {
                throw new ArgumentNullException("schemaNamespace");
            }

            if (schemaNamespace.Length == 0)
            {
                throw new ArgumentException("schema namespace cannot be zero length!");
            }

            if (schemaPath == null)
            {
                throw new ArgumentNullException("schemaPath");
            }

            if (schemaPath.Length == 0)
            {
                throw new ArgumentException("schemaPath cannot be of zero length!");
            }

            if (!File.Exists(schemaPath))
            {
                throw new ArgumentException("Schema: " + schemaPath + " cannot be located!");
            }

            schemaSet = new XmlSchemaSet();
            schemaSet.Add(schemaNamespace, schemaPath);
            schemaSet.Compile();
        }

        //Only 1 thread can call this function at any time
        //Since this is an internal class i don't think there is 
        //a need for lock for performance reason, but we can put it
        public void Validate(XmlDocument document)
        {
            XPathNavigator navigator = document.CreateNavigator();
            navigator.CheckValidity(schemaSet, new ValidationEventHandler(ValidationCallback));
        }

        void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                throw args.Exception;
            }

        }
    }
}
