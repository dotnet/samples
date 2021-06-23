//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    public class CommandLineParser
    {
        Dictionary<string, string> argumentDictionary = new Dictionary<string, string>();

        public string LookupArgument(string key)
        {
            key = key.ToUpperInvariant();

            if (argumentDictionary.ContainsKey(key))
            {
                return argumentDictionary[key];
            }
            else
            {
                return string.Empty;
            }
        }

        public void ParseCommandLineArguments(string[] arguments, string delimiter, char mediaDelimiter)
        {
            argumentDictionary.Clear();

            string[] arg = null;

            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i].IndexOf(delimiter, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    continue;
                }

                arg = arguments[i].Remove(0, delimiter.Length).Split(mediaDelimiter);
                if (arg.Length == 2)
                {
                    string key = arg[0].ToUpperInvariant();
                    string value = arg[1].ToUpperInvariant();

                    if (!argumentDictionary.ContainsKey(key))
                    {
                        argumentDictionary.Add(key, value);
                    }
                }
            }
        }
    }
}