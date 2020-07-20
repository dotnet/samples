using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IDynamicInterfaceCastableSample
{
    class NativeObject : IDynamicInterfaceCastable, IDisposable
    {
        public string Name { get; }

        private delegate int QueryInterfaceDelegate(IntPtr _this, ref Guid iid, out IntPtr ppv);
        private delegate uint ReleaseDelegate(IntPtr _this);

        [StructLayout(LayoutKind.Sequential)]
        private struct IUnknownVtbl
        {
            public QueryInterfaceDelegate QueryInterface;
            public IntPtr AddRef;
            public ReleaseDelegate Release;
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

        private readonly Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> _interfaceTypeToImplType;
        private readonly IntPtr _objPtr;
        private readonly IUnknownVtbl _unknownVtbl;
        private readonly Dictionary<RuntimeTypeHandle, NativeImpl> _typeToNativeImpl = new Dictionary<RuntimeTypeHandle, NativeImpl>();

        public NativeObject(string name, IntPtr obj, Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> interfaceMap)
        {
            Name = name;
            _objPtr = obj;
            _interfaceTypeToImplType = interfaceMap;
            VtblPtr vtblPtr = Marshal.PtrToStructure<VtblPtr>(obj);
            _unknownVtbl = Marshal.PtrToStructure<IUnknownVtbl>(vtblPtr.Vtbl);
        }

        bool IDynamicInterfaceCastable.IsInterfaceImplemented(RuntimeTypeHandle interfaceType, bool throwIfNotImplemented)
        {
            if (!_interfaceTypeToImplType.ContainsKey(interfaceType))
                return false;

            if (_typeToNativeImpl.ContainsKey(interfaceType))
                return true;

            Type type = Type.GetTypeFromHandle(interfaceType);
            Guid guid = type.GUID;
            bool success = _unknownVtbl.QueryInterface(_objPtr, ref guid, out IntPtr ppv) == 0;
            if (!success)
                return false;

            _typeToNativeImpl.Add(interfaceType, new NativeImpl { Ptr = ppv });
            return true;
        }

        RuntimeTypeHandle IDynamicInterfaceCastable.GetInterfaceImplementation(RuntimeTypeHandle interfaceType)
        {
            Type type = Type.GetTypeFromHandle(interfaceType);
            if (!_typeToNativeImpl.ContainsKey(interfaceType) || !_interfaceTypeToImplType.ContainsKey(interfaceType))
                return default;

            return _interfaceTypeToImplType[interfaceType];
        }

        public T GetVtbl<T>(RuntimeTypeHandle interfaceType, out IntPtr ptr)
        {
            if (!_typeToNativeImpl.TryGetValue(interfaceType, out NativeImpl impl))
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
            _unknownVtbl.Release(_objPtr);

            // Release for every successful QI
            for (var i = 0; i < _typeToNativeImpl.Count; i++)
                _unknownVtbl.Release(_objPtr);
        }
    }
}
