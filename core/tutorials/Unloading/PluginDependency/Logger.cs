using System;

namespace PluginDependency
{
    // This is a simple class to write message to console. It is present to demostrate
    // how a dependency of the plugin gets loaded into the HostAssemblyLoadContext
    public class Logger
    {
        public static void LogMessage(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
