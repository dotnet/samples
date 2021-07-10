//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.Samples.ServiceModel
{
    class Client
    {
        static void Main()
        {
            TraceSource ts = new TraceSource("ClientCalculatorTraceSource");

            // Start the calculator activity
            Guid newGuid = Guid.NewGuid();
            Trace.CorrelationManager.ActivityId = newGuid;
            ts.TraceEvent(TraceEventType.Start, 0, "Calculator Activity");

            // Create a proxy with given client endpoint configuration
            using (CalculatorProxy proxy = new CalculatorProxy())
            {
                // Save the calculator activity id to transfer back and forth from/to it
                Guid originalGuid = Trace.CorrelationManager.ActivityId;

                // Create and start the Add activity                
                // Generate a new activity id
                newGuid = Guid.NewGuid();
                // Transfer from the calculator activity to the new (Add) activity
                // The value for the "from" in the transfer is implicit; it is the activity id 
                // previously set in Trace.CorrelationManager.ActivityId
                // The value for the "to" is explicitly passed as the newGuid parameter
                ts.TraceTransfer(0, "Transferring...", newGuid);
                // Set the new activity id in Trace.CorrelationManager.ActivityId; it is now in scope
                // for subsequently emitted traces
                Trace.CorrelationManager.ActivityId = newGuid;
                // Emit the Start trace for the new activity
                ts.TraceEvent(TraceEventType.Start, 0, "Add Activity");

                // Now make the Add request
                double value1 = 100.00D;
                double value2 = 15.99D;
                ts.TraceEvent(TraceEventType.Information, 0,
                    "Client sends Add request message.");
                double result = proxy.Add(value1, value2);

                // Trace that you have received the response
                ts.TraceEvent(TraceEventType.Information, 0,
                    "Client receives Add response message.");
                Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

                // Transfer back to the Calculator activity and stop the current activity
                ts.TraceTransfer(667, "Transferring...", originalGuid);
                ts.TraceEvent(TraceEventType.Stop, 0, "Add Activity");

                // Set the calculator activity back in scope
                Trace.CorrelationManager.ActivityId = originalGuid;


                // Call the Subtract service operation
                newGuid = Guid.NewGuid();
                ts.TraceTransfer(0, "Transferring...", newGuid);
                Trace.CorrelationManager.ActivityId = newGuid;
                ts.TraceEvent(TraceEventType.Start, 0, "Subtract Activity");

                value1 = 100.00D;
                value2 = 15.99D;
                ts.TraceEvent(TraceEventType.Information, 0,
                    "Client sends Subtract request message.");
                result = proxy.Subtract(value1, value2);
                ts.TraceEvent(TraceEventType.Information, 0,
                    "Client receives Subtract response message.");
                Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

                ts.TraceTransfer(667, "Transferring...", originalGuid);
                ts.TraceEvent(TraceEventType.Stop, 0, "Subtract Activity");
                Trace.CorrelationManager.ActivityId = originalGuid;


                // Call the Multiply service operation
                newGuid = Guid.NewGuid();
                ts.TraceTransfer(0, "Transferring...", newGuid);
                Trace.CorrelationManager.ActivityId = newGuid;
                ts.TraceEvent(TraceEventType.Start, 0, "Multiply Activity");

                value1 = 100.00D;
                value2 = 15.99D;
                ts.TraceEvent(TraceEventType.Information, 0,
                    "Client sends Multiply request message.");
                result = proxy.Multiply(value1, value2);
                ts.TraceEvent(TraceEventType.Information, 0,
                    "Client receives Multiply response message.");
                Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

                ts.TraceTransfer(667, "Transferring...", originalGuid);
                ts.TraceEvent(TraceEventType.Stop, 0, "Multiply Activity");
                Trace.CorrelationManager.ActivityId = originalGuid;


                // Call the Divide service operation
                newGuid = Guid.NewGuid();
                ts.TraceTransfer(0, "Transferring...", newGuid);
                Trace.CorrelationManager.ActivityId = newGuid;
                ts.TraceEvent(TraceEventType.Start, 0, "Divide Activity");

                value1 = 100.00D;
                value2 = 15.99D;
                ts.TraceEvent(TraceEventType.Information, 0,
                    "Client sends Divide request message.");
                result = proxy.Divide(value1, value2);
                ts.TraceEvent(TraceEventType.Information, 0,
                    "Client receives Divide response message.");
                Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);

                ts.TraceTransfer(667, "Transferring...", originalGuid);
                ts.TraceEvent(TraceEventType.Stop, 0, "Divide Activity");
                Trace.CorrelationManager.ActivityId = originalGuid;
            }

            ts.TraceEvent(TraceEventType.Stop, 0, "Calculator Activity");

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();

        }
    }
}
