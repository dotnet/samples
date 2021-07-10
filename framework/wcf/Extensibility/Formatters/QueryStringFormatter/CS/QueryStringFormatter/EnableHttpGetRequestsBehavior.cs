
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Samples
{

    public class EnableHttpGetRequestsBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            if (serviceEndpoint == null)
                throw new ArgumentNullException("serviceEndpoint");

            if (endpointDispatcher == null)
                throw new ArgumentNullException("endpointDispatcher");

            // Apply URI-based operation selector
            Dictionary<string, string> operationNameDictionary = new Dictionary<string, string>();
            foreach (OperationDescription operation in serviceEndpoint.Contract.Operations)
            {
                try
                {
                    operationNameDictionary.Add(operation.Name.ToLower(), operation.Name);
                }
                catch (ArgumentException)
                {
                    throw new Exception(String.Format("The specified contract cannot be used with case insensitive URI dispatch because there is more than one operation named {0}", operation.Name));
                }
            }
            endpointDispatcher.AddressFilter = new PrefixEndpointAddressMessageFilter(serviceEndpoint.Address);
            endpointDispatcher.ContractFilter = new MatchAllMessageFilter();
            endpointDispatcher.DispatchRuntime.OperationSelector = new UriPathSuffixOperationSelector(serviceEndpoint.Address.Uri, operationNameDictionary);
        }

        public void Validate(ServiceEndpoint serviceEndpoint)
        {
        }

        public static void ReplaceFormatterBehavior(OperationDescription operationDescription, EndpointAddress address)
        {
            // look for and remove the DataContract behavior if it is present
            IOperationBehavior formatterBehavior = operationDescription.Behaviors.Remove<DataContractSerializerOperationBehavior>();
            if (formatterBehavior == null)
            {
                // look for and remove the XmlSerializer behavior if it is present
                formatterBehavior = operationDescription.Behaviors.Remove<XmlSerializerOperationBehavior>();
                if (formatterBehavior == null)
                {
                    // look for delegating formatter behavior
                    DelegatingFormatterBehavior existingDelegatingFormatterBehavior = operationDescription.Behaviors.Find<DelegatingFormatterBehavior>();
                    if (existingDelegatingFormatterBehavior == null)
                    {
                        throw new InvalidOperationException("Could not find DataContractFormatter or XmlSerializer on the contract");
                    }
                }
            }

            //remember what the innerFormatterBehavior was
            DelegatingFormatterBehavior delegatingFormatterBehavior = new DelegatingFormatterBehavior(address);
            delegatingFormatterBehavior.InnerFormatterBehavior = formatterBehavior;
            operationDescription.Behaviors.Add(delegatingFormatterBehavior);
        }
    }

    class DelegatingFormatterBehavior : IOperationBehavior
    {
        EndpointAddress endpointAddress;

        public DelegatingFormatterBehavior(EndpointAddress address)
        {
            this.endpointAddress = address;
        }

        IOperationBehavior innerFormatterBehavior;
        internal IOperationBehavior InnerFormatterBehavior
        {
            get { return innerFormatterBehavior; }
            set { innerFormatterBehavior = value; }
        }

        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription description, System.ServiceModel.Channels.BindingParameterCollection parameters)
        {
            if (innerFormatterBehavior != null)
            {
                innerFormatterBehavior.AddBindingParameters(description, parameters);
            }
        }

        public void ApplyClientBehavior(OperationDescription description, ClientOperation runtime)
        {
            if (innerFormatterBehavior != null && runtime.Formatter == null)
            {
                // no formatter has been applied yet
                innerFormatterBehavior.ApplyClientBehavior(description, runtime);
            }
            runtime.Formatter = new QueryStringFormatter(description.Name, runtime.SyncMethod.GetParameters(), runtime.Formatter, runtime.Action, endpointAddress);
        }

        public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation runtime)
        {
            if (innerFormatterBehavior != null && runtime.Formatter == null)
            {
                // no formatter has been applied yet
                innerFormatterBehavior.ApplyDispatchBehavior(description, runtime);
            }
            runtime.Formatter = new QueryStringFormatter(description.Name, description.SyncMethod.GetParameters(), runtime.Formatter);
        }

        public void Validate(OperationDescription description)
        {
            if (innerFormatterBehavior != null)
            {
                innerFormatterBehavior.Validate(description);
            }
        }

        #endregion
    }
}
