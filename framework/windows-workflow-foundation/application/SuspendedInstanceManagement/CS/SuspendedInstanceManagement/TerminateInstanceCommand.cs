//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activities;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    class TerminateInstanceCommand : WorkflowInstanceCommand
    {
        string instanceId;

        public TerminateInstanceCommand(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                throw new ArgumentNullException("InstanceId");
            }

            this.instanceId = instanceId;
        }

        public override void Execute()
        {
            InstancesDataContext dataContext = new InstancesDataContext(this.ConnectionString);

            IQueryable<Instance> suspendedInstances =
                from instance in dataContext.Instances
                where instance.IsSuspended == true && instance.InstanceId == new Guid(this.instanceId)
                select instance;

            if (suspendedInstances.ToArray().Length == 0)
            {
                Console.WriteLine("InstanceId {0} is not found\n", this.instanceId);
            }
            else
            {
                ChannelFactory<IWorkflowInstanceManagement> channelFactory = new ChannelFactory<IWorkflowInstanceManagement>("controlEndpoint");
                IWorkflowInstanceManagement proxy = channelFactory.CreateChannel();

                proxy.Terminate(new Guid(instanceId), "TerminateReason");

                Console.WriteLine("Terminating InstanceId {0} is done\n", this.instanceId);
            }
        }
    }
}