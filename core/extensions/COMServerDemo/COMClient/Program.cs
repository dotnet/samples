using System;
using System.Runtime.InteropServices;

namespace COMClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Activation.Server();

            var pi = server.ComputePi();
            Console.WriteLine($"\u03C0 = {pi}");
        }
    }

    // The following classes are typically defined in a PIA, but for this example
    // are being defined here to simplify the scenario.
    namespace Activation
    {
        /// <summary>
        /// Managed definition of CoClass
        /// </summary>
        [ComImport]
        [CoClass(typeof(ServerClass))]
        [Guid(ContractGuids.ServerInterface)] // By TlbImp convention, set this to the GUID of the parent interface
        internal interface Server : IServer
        {
        }

        /// <summary>
        /// Managed activation for CoClass
        /// </summary>
        [ComImport]
        [Guid(ContractGuids.ServerClass)]
        internal class ServerClass
        {
        }
    }
}
