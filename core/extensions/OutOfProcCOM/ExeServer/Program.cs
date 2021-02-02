using System;
using System.Diagnostics;
using System.IO;

namespace OutOfProcCOM
{
    class Program
    {
        private static readonly string exePath = Path.Combine(AppContext.BaseDirectory, "ExeServer.exe");

#if EMBEDDED_TYPE_LIBRARY
        private static readonly string tlbPath = exePath;
#else
        private static readonly string tlbPath = Path.Combine(AppContext.BaseDirectory, Contract.Constants.TypeLibraryName);
#endif

        static void Main(string[] args)
        {
            using var consoleTrace = new ConsoleTraceListener();
            Trace.Listeners.Add(consoleTrace);

            if (args.Length == 1)
            {
                string regCommandMaybe = args[0];
                if (regCommandMaybe.Equals("/regserver", StringComparison.OrdinalIgnoreCase) || regCommandMaybe.Equals("-regserver", StringComparison.OrdinalIgnoreCase))
                {
                    // Register local server and type library
                    COMRegistration.LocalServer.Register(Contract.Constants.ServerClassGuid, exePath, tlbPath);
                    return;
                }
                else if (regCommandMaybe.Equals("/unregserver", StringComparison.OrdinalIgnoreCase) || regCommandMaybe.Equals("-unregserver", StringComparison.OrdinalIgnoreCase))
                {
                    // Unregister local server and type library
                    COMRegistration.LocalServer.Unregister(Contract.Constants.ServerClassGuid, tlbPath);
                    return;
                }
            }

            using var server = new COMRegistration.LocalServer();
            server.RegisterClass<ExeServer>(Contract.Constants.ServerClassGuid);

            server.Run();
        }
    }
}
