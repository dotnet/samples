using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static System.Runtime.InteropServices.ComWrappers;

#if NET5_0
#pragma warning disable CA1416 // This is only needed in .NET 5.
#endif

namespace Tutorial
{
    class Program
    {
        static void Main(string[] _)
        {
            var cw = new DemoComWrappers();

            var demo = new DemoImpl();

            string? value = demo.GetString();
            Console.WriteLine($"Initial string: {value ?? "<null>"}");

            // Create a managed object wrapper for the Demo object.
            // Note the returned COM interface will always be for IUnknown.
            IntPtr ccwUnknown = cw.GetOrCreateComInterfaceForObject(demo, CreateComInterfaceFlags.None);

            // Create a native object wrapper for the managed object wrapper of the Demo object.
            var rcw = cw.GetOrCreateObjectForComInstance(ccwUnknown, CreateObjectFlags.UniqueInstance);

            // Release the managed object wrapper because the native object wrapper now manages
            // its lifetime. See the native wrapper implementation that will handle the final two releases.
            int count = Marshal.Release(ccwUnknown);
            Debug.Assert(count == 2);

            var getter = (IDemoGetType)rcw;
            var store = (IDemoStoreType)rcw;

            string msg = "hello world!";
            store.StoreString(msg.Length, msg);
            Console.WriteLine($"Setting string through wrapper: {msg}");

            value = demo.GetString();
            Console.WriteLine($"Get string through managed object: {value}");

            msg = msg.ToUpper();
            demo.StoreString(msg.Length, msg.ToUpper());
            Console.WriteLine($"Setting string through managed object: {msg}");

            value = getter.GetString();
            Console.WriteLine($"Get string through wrapper: {value}");

            // The DemoComWrappers supports creation of UniqueInstances - see above. This means
            // the IDisposable contract may be provided to enable immediate COM object release.
            (rcw as IDisposable)?.Dispose();
        }
    }

    /// <summary>
    /// IUnknown based COM interface.
    /// </summary>
    /// <remarks>
    /// Win32 C/C++ definition:
    /// <code>
    /// MIDL_INTERFACE("92BAA992-DB5A-4ADD-977B-B22838EE91FD")
    /// IDemoGetType : public IUnknown
    /// {
    ///     HRESULT STDMETHODCALLTYPE GetString(_Outptr_ wchar_t** str) = 0;
    /// };
    /// </code>
    /// </remarks>
    interface IDemoGetType
    {
        /// <summary>
        /// Statically defined Interface ID.
        /// </summary>
        /// <remarks>
        /// Used instead of Type.GUID to be AOT friendly and avoid using Reflection.
        /// </remarks>
        public static Guid IID_IDemoGetType = new("92BAA992-DB5A-4ADD-977B-B22838EE91FD");

        /// <summary>
        /// Get the currently stored string.
        /// </summary>
        /// <returns>Returns the stored string or <c>null</c>.</returns>
        string? GetString();
    }

    /// <summary>
    /// IUnknown based COM interface.
    /// </summary>
    /// <remarks>
    /// Win32 C/C++ definition:
    /// <code>
    /// MIDL_INTERFACE("30619FEA-E995-41EA-8C8B-9A610D32ADCB")
    /// IDemoStoreType : public IUnknown
    /// {
    ///     HRESULT STDMETHODCALLTYPE StoreString(int len, _In_z_ const wchar_t* str) = 0;
    /// };
    /// </code>
    /// </remarks>
    interface IDemoStoreType
    {
        /// <summary>
        /// Statically defined Interface ID.
        /// </summary>
        /// <remarks>
        /// Used instead of Type.GUID to be AOT friendly and avoid using Reflection.
        /// </remarks>
        public static Guid IID_IDemoStoreType = new("30619FEA-E995-41EA-8C8B-9A610D32ADCB");

        /// <summary>
        /// Store the supplied string.
        /// </summary>
        /// <param name="len">The length of the string to store.</param>
        /// <param name="str">The string to store.</param>
        void StoreString(int len, string? str);
    }

    /// <summary>
    /// Managed implementation
    /// </summary>
    class DemoImpl : IDemoGetType, IDemoStoreType
    {
        string? _string;
        public string? GetString() => _string;
        public void StoreString(int _, string? str) => _string = str;
    }

    /// <summary>
    /// User defined ComWrappers 
    /// </summary>
    sealed unsafe class DemoComWrappers : ComWrappers
    {
        static readonly IntPtr s_IDemoGetTypeVTable;
        static readonly IntPtr s_IDemoStoreVTable;
        static readonly ComInterfaceEntry* s_DemoImplDefinition;
        static readonly int s_DemoImplDefinitionLen;

        /// <summary>
        /// Preallocate COM artifacts to avoid penalty during wrapper creation.
        /// </summary>
        static DemoComWrappers()
        {
            // Get system provided IUnknown implementation.
            GetIUnknownImpl(
                out IntPtr fpQueryInterface,
                out IntPtr fpAddRef,
                out IntPtr fpRelease);

            //
            // Construct VTables for supported interfaces
            //
            {
                int tableCount = 4;
                int idx = 0;
                var vtable = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(
                    typeof(DemoComWrappers),
                    IntPtr.Size * tableCount);
                vtable[idx++] = fpQueryInterface;
                vtable[idx++] = fpAddRef;
                vtable[idx++] = fpRelease;
                vtable[idx++] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, int>)&ABI.IDemoGetTypeManagedWrapper.GetString;
                Debug.Assert(tableCount == idx);
                s_IDemoGetTypeVTable = (IntPtr)vtable;
            }
            {
                int tableCount = 4;
                int idx = 0;
                var vtable = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(
                    typeof(DemoComWrappers),
                    IntPtr.Size * tableCount);
                vtable[idx++] = fpQueryInterface;
                vtable[idx++] = fpAddRef;
                vtable[idx++] = fpRelease;
                vtable[idx++] = (IntPtr)(delegate* unmanaged<IntPtr, int, IntPtr, int>)&ABI.IDemoStoreTypeManagedWrapper.StoreString;
                Debug.Assert(tableCount == idx);
                s_IDemoStoreVTable = (IntPtr)vtable;
            }

            //
            // Construct entries for supported managed types
            //
            {
                s_DemoImplDefinitionLen = 2;
                int idx = 0;
                var entries = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(
                    typeof(DemoComWrappers),
                    sizeof(ComInterfaceEntry) * s_DemoImplDefinitionLen);
                entries[idx].IID = IDemoGetType.IID_IDemoGetType;
                entries[idx++].Vtable = s_IDemoGetTypeVTable;
                entries[idx].IID = IDemoStoreType.IID_IDemoStoreType;
                entries[idx++].Vtable = s_IDemoStoreVTable;
                Debug.Assert(s_DemoImplDefinitionLen == idx);
                s_DemoImplDefinition = entries;
            }
        }

        readonly delegate* <IntPtr, object?> _createIfSupported;

        public DemoComWrappers(bool useDynamicNativeWrapper = false)
        {
            // Determine which wrapper create function to use.
            _createIfSupported = useDynamicNativeWrapper
                ? &ABI.DemoNativeDynamicWrapper.CreateIfSupported
                : &ABI.DemoNativeStaticWrapper.CreateIfSupported;
        }

        protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
        {
            Debug.Assert(flags is CreateComInterfaceFlags.None);

            if (obj is DemoImpl)
            {
                count = s_DemoImplDefinitionLen;
                return s_DemoImplDefinition;
            }

            // Note: this implementation does not handle cases where the passed in object implements
            // one or both of the supported interfaces but is not the expected .NET class.
            count = 0;
            return null;
        }

        protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        {
            // Assert use of the UniqueInstance flag since the returned Native Object Wrapper always
            // supports IDisposable and its Dispose will always release and suppress finalization.
            // If the wrapper doesn't always support IDisposable the assert can be relaxed.
            Debug.Assert(flags.HasFlag(CreateObjectFlags.UniqueInstance));

            // Throw an exception if the type is not supported by the implementation.
            // Null can be returned as well, but an ArgumentNullException will be thrown for
            // the consumer of this ComWrappers instance.
            return _createIfSupported(externalComObject) ?? throw new NotSupportedException();
        }

        protected override void ReleaseObjects(IEnumerable objects)
        {
            throw new NotImplementedException();
        }
    }

    namespace ABI
    {
        internal enum HRESULT : int
        {
            S_OK = 0
        }

        /// <summary>
        /// Managed object wrapper for IDemoGetType.
        /// </summary>
        internal static unsafe class IDemoGetTypeManagedWrapper
        {
            [UnmanagedCallersOnly]
            public static int GetString(IntPtr _this, IntPtr* str)
            {
                try
                {
                    string? s = ComInterfaceDispatch.GetInstance<IDemoGetType>((ComInterfaceDispatch*)_this).GetString();

                    // Marshal to COM
                    *str = Marshal.StringToCoTaskMemUni(s);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return (int)HRESULT.S_OK;
            }
        }

        /// <summary>
        /// Managed object wrapper for IDemoStoreType.
        /// </summary>
        internal static unsafe class IDemoStoreTypeManagedWrapper
        {
            [UnmanagedCallersOnly]
            public static int StoreString(IntPtr _this, int len, IntPtr str)
            {
                try
                {
                    // Marshal to .NET
                    string? strLocal = str == IntPtr.Zero ? null : Marshal.PtrToStringUni(str, len);

                    // Since we've taken ownership we need to free the native memory.
                    // This is a policy decision because the API could require returning the same pointer
                    // stored and not just a copy.
                    Marshal.FreeCoTaskMem(str);

                    ComInterfaceDispatch.GetInstance<IDemoStoreType>((ComInterfaceDispatch*)_this).StoreString(len, strLocal);
                }
                catch (Exception e)
                {
                    return e.HResult;
                }

                return (int)HRESULT.S_OK;
            }
        }

        /// <summary>
        /// Native object wrapper static implementation.
        /// </summary>
        /// <remarks>
        /// This class statically implements two desired Demo interfaces and is limited to
        /// those interfaces that it implements. For scenarios involving native objects
        /// with a potentially unknown number of interfaces see <see cref="DemoNativeDynamicWrapper"/>.
        /// </remarks>
        internal class DemoNativeStaticWrapper
            : IDemoGetType
            , IDemoStoreType
            , IDisposable
        {
            bool _isDisposed = false;
            private DemoNativeStaticWrapper() { }

            ~DemoNativeStaticWrapper()
            {
                DisposeInternal();
            }

            public IntPtr IDemoGetTypeInst { get; init; }
            public IntPtr IDemoStoreTypeInst { get; init; }

            public static DemoNativeStaticWrapper? CreateIfSupported(IntPtr ptr)
            {
                int hr = Marshal.QueryInterface(ptr, ref IDemoGetType.IID_IDemoGetType, out IntPtr IDemoGetTypeInst);
                if (hr != (int)HRESULT.S_OK)
                {
                    return default;
                }

                hr = Marshal.QueryInterface(ptr, ref IDemoStoreType.IID_IDemoStoreType, out IntPtr IDemoStoreTypeInst);
                if (hr != (int)HRESULT.S_OK)
                {
                    Marshal.Release(IDemoGetTypeInst);
                    return default;
                }

                return new DemoNativeStaticWrapper()
                {
                    IDemoGetTypeInst = IDemoGetTypeInst,
                    IDemoStoreTypeInst = IDemoStoreTypeInst
                };
            }

            public void Dispose()
            {
                DisposeInternal();
                GC.SuppressFinalize(this);
            }

            public string? GetString() =>
                IDemoGetTypeNativeWrapper.GetString(IDemoGetTypeInst);

            public void StoreString(int len, string? str) =>
                IDemoStoreTypeNativeWrapper.StoreString(IDemoStoreTypeInst, len, str);

            void DisposeInternal()
            {
                if (_isDisposed)
                    return;

                // [WARNING] This is unsafe for COM objects that have specific thread affinity.
                Marshal.Release(IDemoGetTypeInst);
                Marshal.Release(IDemoStoreTypeInst);

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Native object wrapper dynamic implementation.
        /// </summary>
        /// <remarks>
        /// This class relies upon IDynamicInterfaceCastable to enable COM style casting
        /// via QueryInterface. The current implementation validates the supplied COM pointer
        /// implements the two desired Demo interfaces, but this checked could be delayed and
        /// enable a highly dynamic scenario.
        /// </remarks>
        internal class DemoNativeDynamicWrapper
            : IDynamicInterfaceCastable
            , IDisposable
        {
            bool _isDisposed = false;
            private DemoNativeDynamicWrapper() { }

            ~DemoNativeDynamicWrapper()
            {
                DisposeInternal();
            }

            public IntPtr IDemoGetTypeInst { get; init; }
            public IntPtr IDemoStoreTypeInst { get; init; }

            public static DemoNativeDynamicWrapper? CreateIfSupported(IntPtr ptr)
            {
                int hr = Marshal.QueryInterface(ptr, ref IDemoGetType.IID_IDemoGetType, out IntPtr IDemoGetTypeInst);
                if (hr != (int)HRESULT.S_OK)
                {
                    return default;
                }

                hr = Marshal.QueryInterface(ptr, ref IDemoStoreType.IID_IDemoStoreType, out IntPtr IDemoStoreTypeInst);
                if (hr != (int)HRESULT.S_OK)
                {
                    Marshal.Release(IDemoGetTypeInst);
                    return default;
                }

                return new DemoNativeDynamicWrapper()
                {
                    IDemoGetTypeInst = IDemoGetTypeInst,
                    IDemoStoreTypeInst = IDemoStoreTypeInst
                };
            }

            public RuntimeTypeHandle GetInterfaceImplementation(RuntimeTypeHandle interfaceType)
            {
                if (interfaceType.Equals(typeof(IDemoGetType).TypeHandle))
                {
                    return typeof(IDemoGetTypeNativeWrapper).TypeHandle;
                }
                else if (interfaceType.Equals(typeof(IDemoStoreType).TypeHandle))
                {
                    return typeof(IDemoStoreTypeNativeWrapper).TypeHandle;
                }

                return default;
            }

            public bool IsInterfaceImplemented(RuntimeTypeHandle interfaceType, bool throwIfNotImplemented)
            {
                if (interfaceType.Equals(typeof(IDemoGetType).TypeHandle)
                    || interfaceType.Equals(typeof(IDemoStoreType).TypeHandle))
                {
                    return true;
                }

                if (throwIfNotImplemented)
                    throw new InvalidCastException($"{nameof(DemoNativeDynamicWrapper)} doesn't support {interfaceType}");

                return false;
            }

            public void Dispose()
            {
                DisposeInternal();
                GC.SuppressFinalize(this);
            }

            void DisposeInternal()
            {
                if (_isDisposed)
                    return;

                // [WARNING] This is unsafe for COM objects that have specific thread affinity.
                Marshal.Release(IDemoGetTypeInst);
                Marshal.Release(IDemoStoreTypeInst);

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Native object wrapper for IDemoGetType.
        /// </summary>
        [DynamicInterfaceCastableImplementation]
        internal unsafe interface IDemoGetTypeNativeWrapper : IDemoGetType
        {
            public static string? GetString(IntPtr inst)
            {
                IntPtr str;
                int hr = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)inst + 3 /* IDemoGetType.GetString slot */)))(inst, &str);
                if (hr != (int)HRESULT.S_OK)
                    Marshal.ThrowExceptionForHR(hr);

                string? strLocal = Marshal.PtrToStringUni(str);
                Marshal.FreeCoTaskMem(str);

                return strLocal;
            }

            string? IDemoGetType.GetString()
            {
                var inst = ((DemoNativeDynamicWrapper)this).IDemoGetTypeInst;
                return GetString(inst);
            }
        }

        /// <summary>
        /// Native object wrapper for IDemoStoreType.
        /// </summary>
        [DynamicInterfaceCastableImplementation]
        internal unsafe interface IDemoStoreTypeNativeWrapper : IDemoStoreType
        {
            public static void StoreString(IntPtr inst, int len, string? str)
            {
                IntPtr strLocal = Marshal.StringToCoTaskMemUni(str);
                int hr = ((delegate* unmanaged<IntPtr, int, IntPtr, int>)(*(*(void***)inst + 3 /* IDemoStoreType.StoreString slot */)))(inst, len, strLocal);
                if (hr != (int)HRESULT.S_OK)
                {
                    Marshal.FreeCoTaskMem(strLocal);
                    Marshal.ThrowExceptionForHR(hr);
                }
            }

            void IDemoStoreType.StoreString(int len, string? str)
            {
                var inst = ((DemoNativeDynamicWrapper)this).IDemoStoreTypeInst;
                StoreString(inst, len, str);
            }
        }
    }
}
