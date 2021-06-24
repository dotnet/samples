//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    abstract class WorkflowInstanceCommand
    {
        public string ConnectionString;
        public abstract void Execute();
    }
}