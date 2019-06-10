using System;
using System.Collections;

class Program
{
    static void Main(string[] args)
    {
        foreach (EnvironmentVariableTarget member in Enum.GetValues(typeof(EnvironmentVariableTarget)))
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT && member != EnvironmentVariableTarget.Process)
            {
                continue;
            }

            Console.WriteLine($"Environment variables for {member}:");
            foreach (DictionaryEntry item in Environment.GetEnvironmentVariables(member))
            {
                Console.WriteLine($"   {item.Key}: {item.Value}");
            }
            Console.WriteLine();
        }
    }
}

