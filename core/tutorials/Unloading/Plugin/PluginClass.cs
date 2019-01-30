using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace Plugin
{
    public class PluginClass : Interface
    {
        static GCHandle pluginHandle;

        public static Interface GetInterface()
        {
            PluginClass plugin = new PluginClass();

            // We create the GC handle just to demonstrate how we can do necessary cleanup before
            // unloading. Creating a normal GC handle for the plugin instance would prevent unloading
            // from succeeding.
            pluginHandle = GCHandle.Alloc(plugin);

            // So we register handler for the Unloading event of the context that we are running in and 
            // free the GC handle in there.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyLoadContext currentContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
            currentContext.Unloading += OnPluginUnloadingRequested;

            return plugin;
        }

        private static void OnPluginUnloadingRequested(AssemblyLoadContext obj)
        {
            // Free the GC handle so that the unload can succeed
            pluginHandle.Free();
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
