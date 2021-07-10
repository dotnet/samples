//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.ServiceModel.Samples
{
    public class WsdlDocumentationImporter :
    IWsdlImportExtension,
    IServiceContractGenerationExtension,
    IOperationContractGenerationExtension,
    IContractBehavior,
    IOperationBehavior
    {
        string text;

        #region WSDL Import
        public WsdlDocumentationImporter()
        {
            Debug.WriteLine("WsdlDocumentationImporter created.");
        }

        public WsdlDocumentationImporter(string comment)
        {
            this.text = comment;
            Debug.WriteLine("WsdlDocumentationImporter created.");
        }

        public string Text
        {
            get { return text; }
        }

        public void ImportContract(WsdlImporter importer, WsdlContractConversionContext context)
        {
            // Contract Documentation
            if (context.WsdlPortType.Documentation != null)
            {
                // System examines the contract behaviors to see whether any implement IWsdlImportExtension.
                context.Contract.Behaviors.Add(new WsdlDocumentationImporter(context.WsdlPortType.Documentation));
            }
            // Operation Documentation
            foreach (Operation operation in context.WsdlPortType.Operations)
            {
                if (operation.Documentation != null)
                {
                    OperationDescription operationDescription = context.Contract.Operations.Find(operation.Name);
                    if (operationDescription != null)
                    {
                        // System examines the operation behaviors to see whether any implement IWsdlImportExtension.
                        operationDescription.Behaviors.Add(new WsdlDocumentationImporter(operation.Documentation));
                    }
                }
            }
        }

        public void BeforeImport(ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy)
        {
            Debug.WriteLine("BeforeImport called.");
        }

        public void ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context) { }

        #endregion

        #region Code Generation

        public void GenerateContract(ServiceContractGenerationContext context)
        {
            Debug.WriteLine("In generate contract.");
            context.ContractType.Comments.AddRange(FormatComments(text));
        }

        public void GenerateOperation(OperationContractGenerationContext context)
        {
            context.SyncMethod.Comments.AddRange(FormatComments(text));
            Debug.WriteLine("In generate operation.");
        }

        #endregion

        #region Utility Functions

        CodeCommentStatementCollection FormatComments(string text)
        {
            /*
             * Note that in Visual C# and Visual Basic the XML comment format absorbs a 
             * documentation element with a line break in the middle. This sample
             * could take an XmlElement and create code comments in which 
             * the element never had a line break in it.
            */

            CodeCommentStatementCollection collection = new CodeCommentStatementCollection();
            collection.Add(new CodeCommentStatement("From WSDL Documentation:", true));
            collection.Add(new CodeCommentStatement(String.Empty, true));

            foreach (string line in WordWrap(this.Text, 80))
            {
                collection.Add(new CodeCommentStatement(line, true));
            }

            collection.Add(new CodeCommentStatement(String.Empty, true));
            return collection;
        }

        Collection<string> WordWrap(string text, int columnWidth)
        {
            Collection<string> lines = new Collection<string>();
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            string[] words = text.Split(' ');
            foreach (string word in words)
            {
                if ((builder.Length > 0) && ((builder.Length + word.Length + 1) > columnWidth))
                {
                    lines.Add(builder.ToString());
                    builder = new System.Text.StringBuilder();
                }
                builder.Append(word);
                builder.Append(' ');
            }
            lines.Add(builder.ToString());

            return lines;
        }

        #endregion

        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
            return;
        }

        public void ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, ClientRuntime proxy)
        {
            Debug.WriteLine("Contract behavior is never called on client.");
            return;
        }

        public void ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, DispatchRuntime dispatch)
        {
            return;//this.contractDescription = description;
        }

        public void Validate(ContractDescription description, ServiceEndpoint endpoint)
        {
            return;
        }

        #endregion

        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {
            return;
        }

        public void ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
            Debug.WriteLine("Operation behavior is never called on client.");
            return;
        }

        public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
        {
            // this.operationDescription = description;
        }

        public void Validate(OperationDescription description)
        {
            return;
        }

        #endregion
    }
}

