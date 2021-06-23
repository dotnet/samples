//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SuspendedInstanceManagementReader reader = new SuspendedInstanceManagementReader(args, "-", ':');

                WorkflowInstanceCommand command = InstanceCommandSelectorFactory.CreateWorkflowInstanceCommand(reader);

                command.Execute();

                Console.WriteLine("Press <ENTER> to stop");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine("\r\n" + exception.Message);
                string usageMessage = string.Format("\nSyntax:\n{0} -Command:<CommandName> -Server:<ServerName> -Database:<DatabaseName> -InstanceId:<InstanceId>", Process.GetCurrentProcess().ProcessName);
                Console.WriteLine(usageMessage);
                string supportedCommand = string.Format("Supported Command = <Query | Resume| Terminate>");
                Console.WriteLine(supportedCommand);
            }
        }
    }
}