using System;
using System.Runtime.InteropServices;

namespace Contracts
{
    [Guid(Guids.IServer)]
    public interface IServer
    {
        int Add(int i, int j);
    }

    [ComImport]
    [CoClass(typeof(ServerClass))]
    [Guid(Guids.IServer)]
    public interface Server : IServer { }

    [ComImport]
    [Guid(Guids.CLSID_Server)]
    public class ServerClass
    {
    }

    public partial class Guids
    {
        public const string IServer = "00870BD6-8779-4534-B8B0-F2E1DFF7BB4A";
        public const string CLSID_Server = "4378D242-7D40-441D-8743-B93068C43B30";
    }
}
