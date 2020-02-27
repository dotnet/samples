using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace Plugin
{
    public class PluginClass : Interface
    {
        public static Interface GetInterface()
        {
            PluginClass plugin = new PluginClass();

            // We register handler for the Unloading event of the context that we are running in
            // so that we can perform cleanup of stuff that would otherwise prevent unloading
            // (Like freeing GCHandles for objects of types loaded into the unloadable AssemblyLoadContext,
            // terminating threads running code in assemblies loaded into the unloadable AssemblyLoadContext,
            // etc.)
            // NOTE: this is optional and likely not required for basic scenarios
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyLoadContext currentContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
            currentContext.Unloading += OnPluginUnloadingRequested;

            return plugin;
        }

        private static void OnPluginUnloadingRequested(AssemblyLoadContext obj)
        {
            PluginDependency.Logger.LogMessage("Cleanup of stuff preventing unloading");
        }

        // Plugin interface methods implementation

        public string GetMessage()
        {
            return "Hello from the unloadable plugin";
        }

        public Version GetVersion()
        {
            return new Version(1, 0);
        }
    }
}
