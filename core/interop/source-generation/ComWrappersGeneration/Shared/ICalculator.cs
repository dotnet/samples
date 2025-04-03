using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Tutorial;

[GeneratedComInterface]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(IID)]
internal partial interface ICalculator
{
    int Add(int a, int b);
    int Subtract(int a, int b);
    public const string IID = "c67121c6-cf26-431f-adc7-d12fe2448841";
}
