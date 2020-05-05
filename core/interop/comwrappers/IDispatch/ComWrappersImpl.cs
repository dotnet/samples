#nullable enable

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ComTypes = System.Runtime.InteropServices.ComTypes;

internal unsafe class ComWrappersImpl : ComWrappers
{
    private static readonly ComInterfaceEntry* wrapperEntry;

    // This class only exposes IDispatch and the vtable is always the same.
    // The below isn't the most efficient but it is reasonable for prototyping.
    // If additional interfaces want to be exposed, add them here.
    static ComWrappersImpl()
    {
        GetIUnknownImpl(out IntPtr fpQueryInteface, out IntPtr fpAddRef, out IntPtr fpRelease);

        var vtbl = new IDispatchVtbl()
        {
            IUnknownImpl = new IUnknownVtbl()
            {
                QueryInterface = fpQueryInteface,
                AddRef = fpAddRef,
                Release = fpRelease
            },
            GetTypeInfoCount = Marshal.GetFunctionPointerForDelegate(IDispatchVtbl.pGetTypeInfoCount),
            GetTypeInfo = Marshal.GetFunctionPointerForDelegate(IDispatchVtbl.pGetTypeInfo),
            GetIDsOfNames = Marshal.GetFunctionPointerForDelegate(IDispatchVtbl.pGetIDsOfNames),
            Invoke = Marshal.GetFunctionPointerForDelegate(IDispatchVtbl.pInvoke)
        };
        var vtblRaw = RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDispatchVtbl), sizeof(IDispatchVtbl));
        Marshal.StructureToPtr(vtbl, vtblRaw, false);

        wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDispatchVtbl), sizeof(ComInterfaceEntry));
        wrapperEntry->IID = typeof(IDispatch).GUID;
        wrapperEntry->Vtable = vtblRaw;
    }

    protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
    {
        Debug.Assert(obj is IDispatch);
        Debug.Assert(wrapperEntry != null);

        // Always return the same table mappings.
        count = 1;
        return wrapperEntry;
    }

    protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
    {
        throw new NotImplementedException();
    }

    protected override void ReleaseObjects(IEnumerable objects)
    {
        throw new NotImplementedException();
    }

    [Guid("00020400-0000-0000-C000-000000000046")]
    public interface IDispatch
    {
        void GetTypeInfoCount(out int pctinfo);

        void GetTypeInfo(int iTInfo, int lcid, out IntPtr info);

        void GetIDsOfNames(
            ref Guid iid,
            string[] names,
            int cNames,
            int lcid,
            int[] rgDispId);

        void Invoke(
            int dispIdMember,
            ref Guid riid,
            int lcid,
            ComTypes.INVOKEKIND wFlags,
            ref ComTypes.DISPPARAMS pDispParams,
            IntPtr VarResult,
            IntPtr pExcepInfo,
            IntPtr puArgErr);
    }

    public struct IUnknownVtbl
    {
        public IntPtr QueryInterface;
        public IntPtr AddRef;
        public IntPtr Release;
    }

    public struct IDispatchVtbl
    {
        public IUnknownVtbl IUnknownImpl;
        public IntPtr GetTypeInfoCount;
        public IntPtr GetTypeInfo;
        public IntPtr GetIDsOfNames;
        public IntPtr Invoke;

        public delegate int _GetTypeInfoCount(IntPtr thisPtr, out int i);
        public delegate int _GetTypeInfo(IntPtr thisPtr, int itinfo, int lcid, out IntPtr i);
        public delegate int _GetIDsOfNames(
            IntPtr thisPtr,
            ref Guid iid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 3)]
            string[] names,
            int namesCount,
            int lcid,
            [Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 3)]
            int[] dispIds);
        public delegate int _Invoke(
            IntPtr thisPtr,
            int dispIdMember,
            ref Guid riid,
            int lcid,
            ComTypes.INVOKEKIND wFlags,
            ref ComTypes.DISPPARAMS pDispParams,
            IntPtr VarResult,
            IntPtr pExcepInfo,
            IntPtr puArgErr);

        public static _GetTypeInfoCount pGetTypeInfoCount = new _GetTypeInfoCount(GetTypeInfoCountInternal);
        public static _GetTypeInfo pGetTypeInfo = new _GetTypeInfo(GetTypeInfoInternal);
        public static _GetIDsOfNames pGetIDsOfNames = new _GetIDsOfNames(GetIDsOfNamesInternal);
        public static _Invoke pInvoke = new _Invoke(InvokeInternal);

        public static int GetTypeInfoCountInternal(IntPtr thisPtr, out int i)
        {
            i = 0;
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDispatch>((ComInterfaceDispatch*)thisPtr);
                inst.GetTypeInfoCount(out i);
            }
            catch (Exception e)
            {
                return e.HResult;
            }
            return 0; // S_OK;
        }

        public static int GetTypeInfoInternal(IntPtr thisPtr, int itinfo, int lcid, out IntPtr i)
        {
            i = IntPtr.Zero;
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDispatch>((ComInterfaceDispatch*)thisPtr);
                inst.GetTypeInfo(itinfo, lcid, out i);
            }
            catch (Exception e)
            {
                return e.HResult;
            }
            return 0; // S_OK;
        }

        public static int GetIDsOfNamesInternal(
            IntPtr thisPtr,
            ref Guid iid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 3)]
            string[] names,
            int namesCount,
            int lcid,
            [Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 3)]
            int[] dispIds)
        {
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDispatch>((ComInterfaceDispatch*)thisPtr);
                inst.GetIDsOfNames(ref iid, names, namesCount, lcid, dispIds);
            }
            catch (Exception e)
            {
                return e.HResult;
            }
            return 0; // S_OK;
        }

        public static int InvokeInternal(
            IntPtr thisPtr,
            int dispIdMember,
            ref Guid riid,
            int lcid,
            ComTypes.INVOKEKIND wFlags,
            ref ComTypes.DISPPARAMS pDispParams,
            IntPtr VarResult,
            IntPtr pExcepInfo,
            IntPtr puArgErr)
        {
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDispatch>((ComInterfaceDispatch*)thisPtr);
                inst.Invoke(dispIdMember, ref riid, lcid, wFlags, ref pDispParams, VarResult, pExcepInfo, puArgErr);
            }
            catch (Exception e)
            {
                return e.HResult;
            }
            return 0; // S_OK;
        }
    }
}
