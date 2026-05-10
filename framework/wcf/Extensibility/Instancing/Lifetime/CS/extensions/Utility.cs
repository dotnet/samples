//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

namespace Microsoft.ServiceModel.Samples
{
    internal class Utility
    {
        public static void WriteMessageToConsole(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
