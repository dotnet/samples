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
    // If additional interfaces want to be exposed, add them here.
    static ComWrappersImpl()
    {
        GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

        // See IDispatch definition - https://docs.microsoft.com/windows/win32/api/oaidl/nn-oaidl-idispatch
        var vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDispatch), IntPtr.Size * 7);
        vtblRaw[0] = fpQueryInterface;
        vtblRaw[1] = fpAddRef;
        vtblRaw[2] = fpRelease;
        vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, int*, int>)&IDispatchVtbl.GetTypeInfoCount;
        vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, int, int, IntPtr*, int>)&IDispatchVtbl.GetTypeInfo;
        vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, Guid*, IntPtr*, int, int, int*, int>)&IDispatchVtbl.GetIDsOfNames;
        vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, int, Guid*, int, ComTypes.INVOKEKIND, ComTypes.DISPPARAMS*, IntPtr, IntPtr, IntPtr, int>)&IDispatchVtbl.Invoke;

        wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDispatch), sizeof(ComInterfaceEntry));
        wrapperEntry->IID = IDispatch.IID;
        wrapperEntry->Vtable = (IntPtr)vtblRaw;
    }

    protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
    {
        Debug.Assert(obj is IDispatch);
        Debug.Assert(wrapperEntry != null);

        // Always return the same table mappings.
        // If the object supports more than the IDispatch interface that should be returned here.
        count = 1;
        return wrapperEntry;
    }

    protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        => throw new NotImplementedException();

    protected override void ReleaseObjects(IEnumerable objects)
        => throw new NotImplementedException();

    public interface IDispatch
    {
        public static readonly Guid IID = new Guid("00020400-0000-0000-C000-000000000046");

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

    internal struct IDispatchVtbl
    {
        [UnmanagedCallersOnly]
        public static int GetTypeInfoCount(IntPtr thisPtr, int* i)
        {
            *i = 0;
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDispatch>((ComInterfaceDispatch*)thisPtr);
                inst.GetTypeInfoCount(out *i);
            }
            catch (Exception e)
            {
                return e.HResult;
            }
            return 0; // S_OK;
        }

        [UnmanagedCallersOnly]
        public static int GetTypeInfo(IntPtr thisPtr, int itinfo, int lcid, IntPtr* i)
        {
            *i = IntPtr.Zero;
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDispatch>((ComInterfaceDispatch*)thisPtr);
                inst.GetTypeInfo(itinfo, lcid, out *i);
            }
            catch (Exception e)
            {
                return e.HResult;
            }
            return 0; // S_OK;
        }

        [UnmanagedCallersOnly]
        public static int GetIDsOfNames(
            IntPtr thisPtr,
            Guid* iid,
            IntPtr* namesRaw,
            int namesCount,
            int lcid,
            int* dispIdsRaw)
        {
            try
            {
                // Marshal arguments to managed types.
                var names = new string[namesCount];
                for (int i = 0; i < namesCount; ++i)
                {
                    names[i] = Marshal.PtrToStringUni(namesRaw[i])!;
                }

                var inst = ComInterfaceDispatch.GetInstance<IDispatch>((ComInterfaceDispatch*)thisPtr);

                var dispIds = new int[names.Length];
                inst.GetIDsOfNames(ref *iid, names, namesCount, lcid, dispIds);

                // Marshal out disp IDs.
                for (int i = 0; i < dispIds.Length; ++i)
                {
                    dispIdsRaw[i] = dispIds[i];
                }
            }
            catch (Exception e)
            {
                return e.HResult;
            }
            return 0; // S_OK;
        }

        [UnmanagedCallersOnly]
        public static int Invoke(
            IntPtr thisPtr,
            int dispIdMember,
            Guid* riid,
            int lcid,
            ComTypes.INVOKEKIND wFlags,
            ComTypes.DISPPARAMS* pDispParams,
            IntPtr VarResult,
            IntPtr pExcepInfo,
            IntPtr puArgErr)
        {
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<IDispatch>((ComInterfaceDispatch*)thisPtr);
                inst.Invoke(dispIdMember, ref *riid, lcid, wFlags, ref *pDispParams, VarResult, pExcepInfo, puArgErr);
            }
            catch (Exception e)
            {
                return e.HResult;
            }
            return 0; // S_OK;
        }
    }
}
