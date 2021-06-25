//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    class InstanceCommandSelectorFactory
    {
        public static WorkflowInstanceCommand CreateWorkflowInstanceCommand(SuspendedInstanceManagementReader reader)
        {
            switch (reader.InstanceCommandType)
            {
                case InstanceCommandType.Query :
                    return new QueryInstanceCommand() { ConnectionString = reader.ConnectionString };

                case InstanceCommandType.Resume:
                    return new ResumeInstanceCommand(reader.InstanceId) { ConnectionString = reader.ConnectionString };

                case InstanceCommandType.Terminate:
                    return new TerminateInstanceCommand(reader.InstanceId) { ConnectionString = reader.ConnectionString };

                default:
                    throw new NotSupportedException();
            }
        }
    }
}