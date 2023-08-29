
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Win32;
using static Tutorial.ComInterfaces;

namespace Tutorial;

[GeneratedComClass]
[Guid(ClsId)]
public partial class ClassFactory : IClassFactory
{
    public object CreateInstance(object? outer, in Guid id)
    {
        if(outer != null)
           throw new NotImplementedException();

        // if (id != new Guid(ISimpleCalculator.IID))
        //     throw new InvalidOperationException();

        return new Calculator();
    }

    public void LockServer(bool fLock) { }

    internal const string ClsId = ClsIds.ClassFactory;
}
