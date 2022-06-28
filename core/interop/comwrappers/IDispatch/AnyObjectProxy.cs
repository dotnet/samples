#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

public class AnyObjectProxy : ComWrappersImpl.IDispatch,
    ICustomQueryInterface /* WORKAROUND for WinForms WebBrowser control API */
{
    private static ComWrappers g_ComWrappers = new ComWrappersImpl();

    private const int DISP_E_UNKNOWNNAME = unchecked((int)0x80020006);

    private readonly Type objType;
    private readonly object obj;
    private readonly IntPtr wrapperPtr;
    private readonly Dictionary<string, int> nameToDispId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<int, MemberInfo> dispIdToMemberInfo = new Dictionary<int, MemberInfo>();
    public AnyObjectProxy(object obj)
    {
        this.obj = obj;
        this.objType = this.obj.GetType();

        // The cached string to int is the function name to desired DispID.
        // The DispIDs can be stored but need to consistent. The Invoke() call will be passed the
        // DispID and will use that to Invoke whatever member is desired.
        //
        // The below caching could instead be done using MethodInfo and PropertyInfo instances.
        // This would enable the Invoke() implementation to be more efficient.
        //
        int dispIdNext = 100; // Starting from 100 is typical convention.
        foreach (MemberInfo mi in this.objType.GetMembers())
        {
            this.dispIdToMemberInfo[dispIdNext] = mi;
            this.nameToDispId[mi.Name] = dispIdNext++;
        }

        // The return pointer is to the IUnknown interface implementation. A QueryInterface() to
        // the appropriate interface prior to passing to unmanaged code should be done in most circumstances.
        this.wrapperPtr = g_ComWrappers.GetOrCreateComInterfaceForObject(this, CreateComInterfaceFlags.None);
    }

    ~AnyObjectProxy()
    {
        if (this.wrapperPtr != IntPtr.Zero)
        {
            Marshal.Release(this.wrapperPtr);
        }
    }

    void ComWrappersImpl.IDispatch.GetTypeInfoCount(out int pctinfo)
    {
        // Will always be called.
        // Returning 0 is a completely acceptable return value.
        pctinfo = 0;
    }

    void ComWrappersImpl.IDispatch.GetTypeInfo(int iTInfo, int lcid, out IntPtr info)
    {
        // If GetTypeInfoCount() returns 0, this function will not be called.
        throw new NotImplementedException();
    }

    void ComWrappersImpl.IDispatch.GetIDsOfNames(ref Guid iid, string[] names, int cNames, int lcid, int[] rgDispId)
    {
        Debug.Assert(iid == Guid.Empty);
        for (int i = 0; i < cNames; ++i)
        {
            int dispId = 0;
            if (!this.nameToDispId.TryGetValue(names[i], out dispId))
            {
                throw new COMException(null, DISP_E_UNKNOWNNAME);
            }

            rgDispId[i] = dispId;
        }
    }

    void ComWrappersImpl.IDispatch.Invoke(
        int dispIdMember,
        ref Guid riid,
        int lcid,
        INVOKEKIND wFlags,
        ref DISPPARAMS pDispParams,
        IntPtr VarResult,
        IntPtr pExcepInfo,
        IntPtr puArgErr)
    {
        MemberInfo? memberInfo;
        if (!this.dispIdToMemberInfo.TryGetValue(dispIdMember, out memberInfo))
        {
            throw new COMException(null, DISP_E_UNKNOWNNAME);
        }

        BindingFlags invokeFlags = BindingFlags.Public | BindingFlags.Instance;
        if (wFlags.HasFlag(INVOKEKIND.INVOKE_FUNC)
                && memberInfo.MemberType == MemberTypes.Method)
        {
            invokeFlags |= BindingFlags.InvokeMethod;
        }
        else
        {
            throw new NotImplementedException("Operation not implemented.");
        }

        // Use reflection to dispatch to the indicated function.
        // Note that this is exactly what the internal implementation of IDispatch does so there
        // isn't a lot of difference in cost.
        var result = this.objType.InvokeMember(
            memberInfo.Name,
            invokeFlags,
            null,
            this.obj,
            MarshalArguments(ref pDispParams));

        if (result != null)
        {
            // Lots of special cases should be addressed here.
            //  * Arrays, IEnumerable
            //  * IDispatch/IUnknown instances
            //  * .NET object could be wrapped by ComWrappers
            //  * .NET objects that are already COM objects can be safely passed on
            //  * etc
            Marshal.GetNativeVariantForObject(result, VarResult);
        }
    }

    private static object[] MarshalArguments(ref DISPPARAMS pDispParams)
    {
        if (pDispParams.cArgs == 0)
        {
            return Array.Empty<object>();
        }

        // Lots of special cases should be addressed here.
        //  * Arrays
        //  * IDispatch/IUnknown instances
        //  * .NET objects passed back as arguments
        //  * etc
        object[] result = Marshal.GetObjectsForNativeVariants(pDispParams.rgvarg, pDispParams.cArgs)!;
        // https://docs.microsoft.com/windows/win32/api/oaidl/nf-oaidl-idispatch-invoke
        // Arguments are stored in pDispParams->rgvarg in reverse order, so the first argument is the one with the highest index in the array.
        Array.Reverse(result);
        return result;
    }

    /* WORKAROUND for WinForms WebBrowser control API */
    [ThreadStatic] bool inGetInterface = false; // Needed since the ComWrappers API calls this prior
                                                // to calling user defined interfaces.
    CustomQueryInterfaceResult ICustomQueryInterface.GetInterface(ref Guid iid, out IntPtr ppv)
    {
        Debug.Assert(this.wrapperPtr != IntPtr.Zero);
        ppv = IntPtr.Zero;

        // Return the ComWrappers IDispatch implementation
        // instead of the one provided by the runtime.
        if (!this.inGetInterface
            && iid == ComWrappersImpl.IDispatch.IID)
        {
            try
            {
                this.inGetInterface = true;
                int hr = Marshal.QueryInterface(this.wrapperPtr, ref iid, out ppv);
                Debug.Assert(hr == 0);
            }
            finally
            {
                this.inGetInterface = false;
            }
        }

        return (ppv == IntPtr.Zero)
            ? CustomQueryInterfaceResult.NotHandled
            : CustomQueryInterfaceResult.Handled;
    }
}
