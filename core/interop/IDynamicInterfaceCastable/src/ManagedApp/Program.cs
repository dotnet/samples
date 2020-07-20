using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IDynamicInterfaceCastableSample
{
    [Guid("0412BF07-0261-4191-B5DA-9B86340931CB")]
    public interface IGreet
    {
        void Hello();
    }

    [Guid("B88C195F-3052-467A-97D2-817A1698AAFB")]
    public interface ICompute
    {
        int Sum(int a, int b);
    }

    class Program
    {
        [DynamicInterfaceCastableImplementation]
        private interface IGreetImpl : IGreet
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct IGreetVtbl
            {
                public IntPtr QueryInterface;
                public IntPtr AddRef;
                public IntPtr Release;
                public HelloDelegate Hello;
            }

            private delegate void HelloDelegate(IntPtr _this);

            void IGreet.Hello()
            {
                var obj = (NativeObject)this;
                IGreetVtbl vtbl = obj.GetVtbl<IGreetVtbl>(typeof(IGreet).TypeHandle, out IntPtr ptr);
                vtbl.Hello(ptr);
            }
        }

        [DynamicInterfaceCastableImplementation]
        private interface IComputeImpl : ICompute
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct IComputeVtbl
            {
                public IntPtr QueryInterface;
                public IntPtr AddRef;
                public IntPtr Release;
                public SumDelegate Sum;
            }

            private delegate int SumDelegate(IntPtr _this, int a, int b);

            int ICompute.Sum(int a, int b)
            {
                var obj = (NativeObject)this;
                IComputeVtbl vtbl = obj.GetVtbl<IComputeVtbl>(typeof(ICompute).TypeHandle, out IntPtr ptr);
                return vtbl.Sum(ptr, a, b);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CreateObjectsDelegate([In, Out] IntPtr[] objects, int size);

        static void Main()
        {
            IntPtr nativeLib = NativeLibrary.Load("NativeLib", System.Reflection.Assembly.GetExecutingAssembly(), DllImportSearchPath.AssemblyDirectory);
            IntPtr func = NativeLibrary.GetExport(nativeLib, "CreateObjects");

            int count = 4;
            var nativePtrArray = new IntPtr[count];

            var createObjects = Marshal.GetDelegateForFunctionPointer<CreateObjectsDelegate>(func);
            createObjects(nativePtrArray, count);

            var interfaceMap = new Dictionary<RuntimeTypeHandle, RuntimeTypeHandle>
            {
                [typeof(IGreet).TypeHandle] = typeof(IGreetImpl).TypeHandle,
                [typeof(ICompute).TypeHandle] = typeof(IComputeImpl).TypeHandle
            };
            for (int i = 0; i < count; i++)
            {
               using var obj = new NativeObject($"Native Object #{i}", nativePtrArray[i], interfaceMap);
               InspectObject(obj);
            }

            NativeLibrary.Free(nativeLib);
        }

        private static void InspectObject(NativeObject obj)
        {
            Console.WriteLine(obj.Name);

            if (obj is IGreet greet)
            {
                Console.WriteLine($" - implements {nameof(IGreet)}");
                greet.Hello();
            }
            else
            {
                Console.WriteLine($" - does not implement {nameof(IGreet)}");
            }

            if (obj is ICompute compute)
            {
                Console.WriteLine($" - implements {nameof(ICompute)}");
                Console.WriteLine($"    -- Returned sum (1 + 2 + <objectNumber>): {compute.Sum(1, 2)}");
            }
            else
            {
                Console.WriteLine($" - does not implement {nameof(ICompute)}");
            }

            Console.WriteLine();
        }
    }
}
