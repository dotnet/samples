using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Win32;

namespace Tutorial;

[GeneratedComClass]
[Guid(Clsid)]
internal partial class Calculator : ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    internal const string Clsid = Clsids.Calculator;
}
