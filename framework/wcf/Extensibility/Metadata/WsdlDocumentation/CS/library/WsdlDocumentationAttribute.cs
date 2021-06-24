//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.ServiceModel.Samples
{
    public class WsdlDocumentationAttribute :
    Attribute,
    IContractBehavior,
    IOperationBehavior,
        IWsdlExportExtension
    {
        ContractDescription contractDescription;
        OperationDescription operationDescription;
        string text;
        XmlElement customWsdlDocElement = null;

        public WsdlDocumentationAttribute(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// This constructor takes an XmlElement if the sample 
        /// were to be modified to import the documentation element
        /// as XML. This sample does not use this constructor.
        /// </summary>
        /// <param name="wsdlDocElement"></param>
        public WsdlDocumentationAttribute(XmlElement wsdlDocElement)
        {
            this.customWsdlDocElement = wsdlDocElement;
        }

        public XmlElement WsdlDocElement
        {
            get { return this.customWsdlDocElement; }
            set { this.customWsdlDocElement = value; }
        }

        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        #region WSDL Export

        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
            Debug.WriteLine("Inside ExportContract");
            if (contractDescription != null)
            {
                // Inside this block it is the contract-level comment attribute.
                // This.Text returns the string for the contract attribute.
                // Set the doc element; if this isn't done first, there is no XmlElement in the 
                // DocumentElement property.
                context.WsdlPortType.Documentation = string.Empty;
                // Contract comments.
                XmlDocument owner = context.WsdlPortType.DocumentationElement.OwnerDocument;
                XmlElement summaryElement = owner.CreateElement("summary");
                summaryElement.InnerText = this.Text;
                context.WsdlPortType.DocumentationElement.AppendChild(summaryElement);
            }
            else
            {
                Operation operation = context.GetOperation(operationDescription);
                if (operation != null)
                {
                    // We are dealing strictly with the operation here.
                    // This.Text returns the string for the operation-level attributes.
                    // Set the doc element; if this isn't done first, there is no XmlElement in the 
                    // DocumentElement property.
                    operation.Documentation = String.Empty;

                    // Operation C# triple comments.
                    XmlDocument owner = operation.DocumentationElement.OwnerDocument;
                    XmlElement newSummaryElement = owner.CreateElement("summary");
                    newSummaryElement.InnerText = this.Text;
                    operation.DocumentationElement.AppendChild(newSummaryElement);

                    // Get returns information
                    ParameterInfo returnValue = operationDescription.SyncMethod.ReturnParameter;
                    object[] returnAttrs = returnValue.GetCustomAttributes(typeof(WsdlParamOrReturnDocumentationAttribute), false);
                    if (returnAttrs.Length != 0)
                    {
                        // <returns>text.</returns>
                        XmlElement returnsElement = owner.CreateElement("returns");
                        returnsElement.InnerText = ((WsdlParamOrReturnDocumentationAttribute)returnAttrs[0]).ParamComment;
                        operation.DocumentationElement.AppendChild(returnsElement);
                    }

                    // Get parameter information.
                    ParameterInfo[] args = operationDescription.SyncMethod.GetParameters();
                    for (int i = 0; i < args.Length; i++)
                    {
                        object[] docAttrs = args[i].GetCustomAttributes(typeof(WsdlParamOrReturnDocumentationAttribute), false);
                        if (docAttrs.Length == 1)
                        {
                            // <param name="Int1">Text.</param>
                            XmlElement newParamElement = owner.CreateElement("param");
                            XmlAttribute paramName = owner.CreateAttribute("name");
                            paramName.Value = args[i].Name;
                            newParamElement.InnerText = ((WsdlParamOrReturnDocumentationAttribute)docAttrs[0]).ParamComment;
                            newParamElement.Attributes.Append(paramName);
                            operation.DocumentationElement.AppendChild(newParamElement);
                        }
                    }
                }
            }
        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            Debug.WriteLine("ExportEndpoint called.");
        }

        #endregion

        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
            return;
        }

        public void ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, ClientRuntime proxy)
        {
            return;
        }

        public void ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, DispatchRuntime dispatch)
        {
            this.contractDescription = description;
            Debug.WriteLine("Added the {0} contract description to exporter.", description.Name);
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
            return;
        }

        public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
        {
            this.operationDescription = description;
            Debug.WriteLine("Added the {0} operation description to exporter.", description.Name);
        }

        public void Validate(OperationDescription description)
        {
            return;
        }

        #endregion

    }

    public static class Targets
    {
        public const AttributeTargets ParamReturnTargets
          = AttributeTargets.ReturnValue | AttributeTargets.Parameter | AttributeTargets.GenericParameter;
    }

    [AttributeUsage(Targets.ParamReturnTargets)]
    public class WsdlParamOrReturnDocumentationAttribute : Attribute
    {
        public WsdlParamOrReturnDocumentationAttribute(string docComment)
        {
            this.docValue = docComment;
        }

        string docValue;

        public string ParamComment
        {
            get { return docValue; }
            set { docValue = value; }
        }
    }
}

