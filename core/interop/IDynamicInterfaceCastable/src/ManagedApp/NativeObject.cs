using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IDynamicInterfaceCastableSample
{
    class NativeObject : IDynamicInterfaceCastable, IDisposable
    {
        public string Name { get; }

        private delegate int _QueryInterface(IntPtr _this, ref Guid iid, out IntPtr ppv);
        private delegate uint _Release(IntPtr _this);

        [StructLayout(LayoutKind.Sequential)]
        private struct IUnknownVtbl
        {
            public _QueryInterface QueryInterface;
            public IntPtr AddRef;
            public _Release Release;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct VtblPtr
        {
            public IntPtr Vtbl;
        }

        private class NativeImpl
        {
            public IntPtr Ptr;
            public object Vtbl;
        }

        private readonly Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> interfaceTypeToImplType;
        private readonly IntPtr objPtr;
        private readonly IUnknownVtbl unknownVtbl;
        private readonly Dictionary<RuntimeTypeHandle, NativeImpl> typeToNativeImpl = new Dictionary<RuntimeTypeHandle, NativeImpl>();

        public NativeObject(string name, IntPtr obj, Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> interfaceMap)
        {
            Name = name;
            objPtr = obj;
            interfaceTypeToImplType = interfaceMap;
            VtblPtr vtblPtr = Marshal.PtrToStructure<VtblPtr>(obj);
            unknownVtbl = Marshal.PtrToStructure<IUnknownVtbl>(vtblPtr.Vtbl);
        }

        bool IDynamicInterfaceCastable.IsInterfaceImplemented(RuntimeTypeHandle interfaceType, bool throwIfNotImplemented)
        {
            if (!interfaceTypeToImplType.ContainsKey(interfaceType))
                return false;

            if (typeToNativeImpl.ContainsKey(interfaceType))
                return true;

            Type type = Type.GetTypeFromHandle(interfaceType);
            Guid guid = type.GUID;
            bool success = unknownVtbl.QueryInterface(objPtr, ref guid, out IntPtr ppv) == 0;
            if (!success)
                return false;

            typeToNativeImpl.Add(interfaceType, new NativeImpl { Ptr = ppv });
            return true;
        }

        RuntimeTypeHandle IDynamicInterfaceCastable.GetInterfaceImplementation(RuntimeTypeHandle interfaceType)
        {
            Type type = Type.GetTypeFromHandle(interfaceType);
            if (!typeToNativeImpl.ContainsKey(interfaceType) || !interfaceTypeToImplType.ContainsKey(interfaceType))
                return default;

            return interfaceTypeToImplType[interfaceType];
        }

        public T GetVtbl<T>(RuntimeTypeHandle interfaceType, out IntPtr ptr)
        {
            if (!typeToNativeImpl.TryGetValue(interfaceType, out NativeImpl impl))
                throw new InvalidOperationException();

            ptr = impl.Ptr;
            if (impl.Vtbl != null)
                return (T)impl.Vtbl;

            VtblPtr vtblPtr = Marshal.PtrToStructure<VtblPtr>(ptr);
            T vtbl = Marshal.PtrToStructure<T>(vtblPtr.Vtbl);
            impl.Vtbl = vtbl;

            return vtbl;
        }

        public void Dispose()
        {
            unknownVtbl.Release(objPtr);

            // Release for every successful QI
            for (var i = 0; i < typeToNativeImpl.Count; i++)
                unknownVtbl.Release(objPtr);
        }
    }
}
