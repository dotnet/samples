
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples")]
    public interface IErrorCalculator
    {
        [OperationContract]
        int Add(int n1, int n2);

        [OperationContract]
        int Subtract(int n1, int n2);

        [OperationContract]
        int Multiply(int n1, int n2);

        [OperationContract]
        int Divide(int n1, int n2);

        [OperationContract]
        int Factorial(int n);
    }

    // Service class which implements the service contract.  The ErrorBehaviorAttribute 
    // is used to install the custom error handler.
    [ErrorBehavior(typeof(CalculatorErrorHandler))]
    public class CalculatorService : IErrorCalculator
    {
        public int Add(int n1, int n2)
        {
            return n1 + n2;
        }

        public int Subtract(int n1, int n2)
        {
            return n1 - n2;
        }

        public int Multiply(int n1, int n2)
        {
            return n1 * n2;
        }

        public int Divide(int n1, int n2)
        {
            try
            {
                return n1 / n2;
            }
            catch (DivideByZeroException)
            {
                throw new FaultException("Invalid Argument: The second argument must not be zero.");
            }
        }

        public int Factorial(int n)
        {
            if (n < 1)
                throw new FaultException("Invalid Argument: The argument must be greater than zero.");

            int factorial = 1;
            for (int i = 1; i <= n; i++)
            {
                factorial = factorial * i;
            }
            return factorial;
        }
    }

    public class CalculatorErrorHandler : IErrorHandler
    {
        // Provide a fault. The Message fault parameter can be replaced, or set to
        // null to suppress reporting a fault.

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        // HandleError. Log an error, then allow the error to be handled as usual.
        // Return true if the error is considered as already handled

        public bool HandleError(Exception error)
        {
            using (TextWriter tw = File.AppendText(@"c:\logs\error.txt"))
            {
                if (error != null)
                {
                    tw.WriteLine("Exception: " + error.GetType().Name + " - " + error.Message);
                }
                tw.Close();
            }
            return true;
        }
    }

    // This attribute can be used to install a custom error handler for a service
    public sealed class ErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        Type errorHandlerType;

        public ErrorBehaviorAttribute(Type errorHandlerType)
        {
            this.errorHandlerType = errorHandlerType;
        }
 
        public Type ErrorHandlerType
        {
            get { return this.errorHandlerType; }
        }

        void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler;

            try
            {
                errorHandler = (IErrorHandler)Activator.CreateInstance(errorHandlerType);
            }
            catch (MissingMethodException e)
            {
                throw new ArgumentException("The errorHandlerType specified in the ErrorBehaviorAttribute constructor must have a public empty constructor.", e);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("The errorHandlerType specified in the ErrorBehaviorAttribute constructor must implement System.ServiceModel.Dispatcher.IErrorHandler.", e);
            }

            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }                                                
        }
    }
}

