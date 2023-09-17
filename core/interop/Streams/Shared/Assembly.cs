#if NETFRAMEWORK

namespace DNNE
{
    internal class ExportAttribute : Attribute
    {
        public ExportAttribute() { }
        public string? EntryPoint { get; set; }
    }
}

#else

// Disable runtime marshalling to work with the COM source generator.
[assembly: System.Runtime.CompilerServices.DisableRuntimeMarshalling]

#endif // !NETFRAMEWORK