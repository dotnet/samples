//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------


using System;
using System.Diagnostics;
using System.ServiceModel;

namespace Microsoft.Samples.ServiceModel
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.ServiceModel")]
    public interface ICalculator
    {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }

    // Service class which implements the service contract.
    public class CalculatorService : ICalculator
    {
        private TraceSource ts;

        public CalculatorService()
        {
            ts = new TraceSource("ServerCalculatorTraceSource");
        }

        public double Add(double n1, double n2)
        {
            if(Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                Guid newGuid = Guid.NewGuid();
                Trace.CorrelationManager.ActivityId = newGuid;
            }

            ts.TraceEvent(TraceEventType.Start, 0, "Add Activity");
            ts.TraceEvent(TraceEventType.Information, 0, 
                "Service receives Add request message.");

            double result = n1 + n2;

            ts.TraceEvent(TraceEventType.Information, 0,
                "Service sends Add response message.");
            ts.TraceEvent(TraceEventType.Stop, 0, "Add Activity");

            return result;
        }

        public double Subtract(double n1, double n2)
        {
            if (Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                Guid newGuid = Guid.NewGuid();
                Trace.CorrelationManager.ActivityId = newGuid;
            }

            ts.TraceEvent(TraceEventType.Start, 0, "Subtract Activity");
            ts.TraceEvent(TraceEventType.Information, 0,
                "Service receives Subtract request message.");

            double result = n1 - n2;

            ts.TraceEvent(TraceEventType.Information, 0,
                "Service sends Subtract response message.");
            ts.TraceEvent(TraceEventType.Stop, 0, "Subtract Activity");

            return result;
        }

        public double Multiply(double n1, double n2)
        {
            if (Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                Guid newGuid = Guid.NewGuid();
                Trace.CorrelationManager.ActivityId = newGuid;
            }

            ts.TraceEvent(TraceEventType.Start, 0, "Multiply Activity");
            ts.TraceEvent(TraceEventType.Information, 0,
                "Service receives Multiply request message.");

            double result = n1 * n2;

            ts.TraceEvent(TraceEventType.Information, 0,
                "Service sends Multiply response message.");
            ts.TraceEvent(TraceEventType.Stop, 0, "Multiply Activity");

            return result;
        }

        public double Divide(double n1, double n2)
        {
            if (Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                Guid newGuid = Guid.NewGuid();
                Trace.CorrelationManager.ActivityId = newGuid;
            }

            ts.TraceEvent(TraceEventType.Start, 0, "Divide Activity");
            ts.TraceEvent(TraceEventType.Information, 0,
                "Service receives Divide request message.");

            double result = n1 / n2;

            ts.TraceEvent(TraceEventType.Information, 0,
                "Service sends Divide response message.");
            ts.TraceEvent(TraceEventType.Stop, 0, "Divide Activity");

            return result;
        }
    }
}
