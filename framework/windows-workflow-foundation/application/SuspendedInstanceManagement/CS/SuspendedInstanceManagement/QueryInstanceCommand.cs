//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Linq;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    class QueryInstanceCommand : WorkflowInstanceCommand
    {
        public QueryInstanceCommand()
        {
        }

        public override void Execute()
        {
            InstancesDataContext dataContext = new InstancesDataContext(this.ConnectionString);

            IQueryable<Instance> suspendedInstances =
                from instance in dataContext.Instances
                where instance.IsSuspended == true
                select instance;

            if (suspendedInstances.ToArray().Length == 0)
            {
                Console.WriteLine("No suspendeded instances are found\n");
            }
            else
            {
                Console.WriteLine("\r\nQuerying suspended instances\n");

                foreach (Instance instance in suspendedInstances)
                {
                    Console.WriteLine("InstanceId: {0}, SuspensionExceptionName: {1}, SuspensionReason:{2}",instance.InstanceId, instance.SuspensionExceptionName, instance.SuspensionReason);
                }

                Console.WriteLine("\r\nQuerying instances is done\n");
            }
        }
    }
}