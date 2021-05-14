using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OutOfProcCOM
{
    [ComVisible(true)]
    [Guid(Contract.Constants.ServerClass)]
    [ComDefaultInterface(typeof(IServer))]
    public sealed class ExeServer : IServer
    {
        double IServer.ComputePi()
        {
            Trace.WriteLine($"Running {nameof(ExeServer)}.{nameof(IServer.ComputePi)}");
            double sum = 0.0;
            int sign = 1;
            for (int i = 0; i < 1024; ++i)
            {
                sum += sign / (2.0 * i + 1.0);
                sign *= -1;
            }

            return 4.0 * sum;
        }
    }
}
