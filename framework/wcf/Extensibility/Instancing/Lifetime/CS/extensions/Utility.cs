//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

namespace Microsoft.ServiceModel.Samples
{
    class Utility
    {
        public static void WriteMessageToConsole(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
