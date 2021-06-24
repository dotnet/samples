
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.ServiceModel.Description;

namespace Microsoft.Samples.DCSurrogate
{
    public sealed class AllowNonSerializableTypesAttribute : Attribute, IContractBehavior, IOperationBehavior, IWsdlExportExtension
    {
        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime proxy)
        {
            foreach (OperationDescription opDesc in description.Operations)
            {
                ApplyDataContractSurrogate(opDesc);
            }
        }

        public void ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatch)
        {
            foreach (OperationDescription opDesc in description.Operations)
            {
                ApplyDataContractSurrogate(opDesc);
            }
        }

        public void Validate(ContractDescription description, ServiceEndpoint endpoint)
        {
        }

        #endregion

        #region IWsdlExportExtension Members

        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
            if (exporter == null)
                throw new ArgumentNullException("exporter");

            object dataContractExporter;
            XsdDataContractExporter xsdDCExporter;
            if (!exporter.State.TryGetValue(typeof(XsdDataContractExporter), out dataContractExporter))
            {
                xsdDCExporter = new XsdDataContractExporter(exporter.GeneratedXmlSchemas);
                exporter.State.Add(typeof(XsdDataContractExporter), xsdDCExporter);
            }
            else
            {
                xsdDCExporter = (XsdDataContractExporter)dataContractExporter;
            }
            if (xsdDCExporter.Options == null)
                xsdDCExporter.Options = new ExportOptions();

            if (xsdDCExporter.Options.DataContractSurrogate == null)
                xsdDCExporter.Options.DataContractSurrogate = new AllowNonSerializableTypesSurrogate();
        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
        }

        #endregion

        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
        {
            ApplyDataContractSurrogate(description);
        }

        public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
        {
            ApplyDataContractSurrogate(description);
        }

        public void Validate(OperationDescription description)
        {
        }

        #endregion

        private static void ApplyDataContractSurrogate(OperationDescription description)
        {
            DataContractSerializerOperationBehavior dcsOperationBehavior = description.Behaviors.Find<DataContractSerializerOperationBehavior>();
            if (dcsOperationBehavior != null)
            {
                if (dcsOperationBehavior.DataContractSurrogate == null)
                    dcsOperationBehavior.DataContractSurrogate = new AllowNonSerializableTypesSurrogate();
            }
        }

    }
}
