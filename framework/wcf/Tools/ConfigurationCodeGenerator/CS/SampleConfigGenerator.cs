//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Text;
using System.Xml;

namespace Microsoft.Samples.Tools.ConfigurationCodeGenerator
{
    class SampleConfigGenerator
    {
        string generatedBEElementClassName, generatedSBCollectionElementClassName, assemblyTypeInformation;
        string beConfigName, sbConfigName;
        readonly string xmlFile = "sampleConfig.xml";
        XmlTextWriter writer;

        internal SampleConfigGenerator(string generatedBEElementClassName, string generatedSBCollectionElementClassName, string assemblyTypeInformation)
        {
            this.generatedBEElementClassName = generatedBEElementClassName;
            this.generatedSBCollectionElementClassName = generatedSBCollectionElementClassName;
            this.assemblyTypeInformation = assemblyTypeInformation;
            writer = new XmlTextWriter(xmlFile, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
        }

        internal string XmlFileName
        {
            get
            {
                return xmlFile;
            }
        }

        internal void Generate()
        {
            writer.WriteStartDocument();

            writer.WriteStartElement("configuration");
            writer.WriteStartElement("system.serviceModel");
            writer.WriteStartElement("extensions");

            if (generatedBEElementClassName != null)
            {
                writer.WriteStartElement("bindingElementExtensions");
                WriteAddElement(true);
                writer.WriteEndElement(); //bindingElementExtensions
            }
            if (generatedSBCollectionElementClassName != null)
            {
                writer.WriteStartElement("bindingExtensions");
                WriteAddElement(false);
                writer.WriteEndElement(); //bindingExtensions
            }
            
            writer.WriteEndElement(); //extensions

            writer.WriteComment("Now that your config sections are registered, services or client endpoints in config can use these in their bindings");
            if (generatedBEElementClassName != null)
            {
                writer.WriteComment("You can create a customBinding in config like this to set an endpoint to use your custom binding");
                writer.WriteComment("<customBinding><binding name=\"myBinding\"><binaryMessageEncoding><" + beConfigName + "/>" + "</binding></customBinding>");
                writer.WriteComment("Then you can use it in an endpoint: ");
                writer.WriteComment("<endpoint name=\"testendpoint\" binding=\"customBinding\" bindingConfiguration=\"myBinding\" ></endpoint>");
            }
            if (generatedSBCollectionElementClassName != null)
            {
                writer.WriteComment("You can use the standard binding in config like this to set an endpoint to use your custom binding");
                writer.WriteComment("<endpoint name=\"testendpoint\" binding=\"" + sbConfigName + "\"" + "></endpoint>");
            }
            writer.WriteEndElement(); //system.serviceModel
            writer.WriteEndElement(); //configuration

            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        void WriteAddElement(bool isBindingElement)
        {
            string name = null, type = null;
            writer.WriteStartElement("add");
            if (isBindingElement)
            {
                type = generatedBEElementClassName;
            }
            else
            {
                type = generatedSBCollectionElementClassName;
            }
            string sectionName = type.Substring(type.LastIndexOf(".") + 1);
            if (isBindingElement)
            {
                name = sectionName.Substring(0, sectionName.IndexOf(Constants.ElementSuffix));
            }
            else
            {
                name = sectionName.Substring(0, sectionName.IndexOf(Constants.CollectionElementSuffix));
            }
            name = Helpers.TurnFirstCharLower(name);
            if (isBindingElement)
            {
                beConfigName = name;
            }
            else
            {
                sbConfigName = name;
            }
            type = type + ", " + assemblyTypeInformation;
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", type);
            writer.WriteEndElement(); //add
        }
    }
}
