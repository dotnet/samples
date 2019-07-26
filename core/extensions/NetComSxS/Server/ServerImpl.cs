using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Server
{
    [ComVisible(true)]
    [Guid(Contracts.Guids.CLSID_Server)]
    public class ServerImpl : Contracts.IServer
    {
        public int Add(int i, int j)
        {
            return i + j;
        }
    }
}
