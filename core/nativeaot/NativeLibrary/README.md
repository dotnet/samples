# Building Native Libraries with NativeAOT

This document will guide you through building native libraries that can be consumed by other programming languages with NativeAOT. NativeAOT can build static libraries that can be linked at compile time or shared libraries that are required at runtime.

## Create .NET Class Library project with NativeAOT support

Create a .NET class library project using `dotnet new classlib -o NativeLibrary` and follow the [Hello world](../HelloWorld/README.md) sample instruction to add NativeAOT support to it.

## Building shared libraries

```bash
> dotnet publish --use-current-runtime
```

The above command will drop a shared library (Windows `.dll`, macOS `.dylib`, Linux `.so`) in `./bin/Release/net8.0/[RID]/publish/` folder and will have the same name as the folder in which your source file is present.

### Loading shared libraries from C and importing methods

For reference, you can read the file _LoadLibrary.c_.

> [!NOTE]
> On Windows the platform (x86 and x64) must match between the .NET AOT library and the tool used to compile and link the C code. For more information, see [How to: Enable a 64-Bit, x64 hosted MSVC toolset on the command line](https://learn.microsoft.com/en-us/cpp/build/how-to-enable-a-64-bit-visual-cpp-toolset-on-the-command-line?view=msvc-170).

The first thing you'll have to do in order to have a proper "loader" that loads your shared library is to add these directives

```c
#ifdef _WIN32
#include "windows.h"
#define symLoad GetProcAddress
#else
#include "dlfcn.h"
#define symLoad dlsym
#endif
```

After these, in order to load the 'handle' of the shared library

```c
#ifdef _WIN32
HINSTANCE handle = LoadLibrary(path);
#else
void *handle = dlopen(path, RTLD_LAZY);
#endif
```

the variable path is the string that holds the path to the .so/.dll file.
From now on, the handle variable will "contain" a pointer to your shared library.
Now we'll have to define signature of the function we want to call

```c
typedef  int (*myFunc)(int,int);
```

For example here, we'll refer to the C# function underneath, which returns the sum of two integers.
Now we'll import from handle , that as we said points to our shared library , the function we want to call

```c
myFunc MyImport =  symLoad(handle, funcName);
```

where funcName is a string that contains the name of the entrypoint value defined in the UnmanagedCallersOnly field.
The last thing to do is to actually call the method we have imported.

```c
int result =  MyImport(5,3);
```

Note that the .NET Runtime does not support unloading. Once a handle to the shared library is created, the library cannot be closed with `dlclose/FreeLibrary`.

## Exporting methods

For a C# method in the native library to be consumable by external programs, it has to be explicitly exported using the `[UnmanagedCallersOnly]` attribute.
Apply the attribute to the method, specifying the `EntryPoint`:

```csharp
[UnmanagedCallersOnly(EntryPoint = "aotsample_add")]
public static int Add(int a, int b)
{
    return a + b;
}
```

After the native library is built, the above C# `Add` method will be exported as a native `add` function to consumers of the library. Here are some limitations to consider when deciding what managed method to export:

* Exported methods have to be static.
* Exported methods can only naturally accept or return primitives or value types (i.e structs), they have to marshal all reference type arguments.
* Exported methods cannot be called from regular managed C# code, an exception will be thrown.
* Exported methods cannot use regular C# exception handling, they should return error codes instead.

The sample [source code](Class1.cs) demonstrates common techniques used to stay within these limitations.

## Building static libraries

> [!WARNING]
> It's preferred to build shared libraries than static libraries:
>
> * All code in the loadable module must be compiled with C/C++ compiler version and options that are compatible with native AOT static libraries.
> * It's also not possible to mix multiple native AOT compiled static libraries within the same loadable module.
>
> These problems don't exist when you build a shared library.

```bash
> dotnet publish /p:NativeLib=Static --use-current-runtime
```

The above command will drop a static library (Windows `.lib`, macOS/Linux `.a`) in `./bin/Release/net8.0/[RID]/publish/` folder and will have the same name as the folder in which your source file is present.

<!-- markdownlint-disable MD033 -->
<details>
<summary>Extra requirements to link static libraries produced by .NET 7 (click to expand)</summary>
When linking the generated static library, it is important to also include additional framework dependencies in the linker settings, and add `NativeAOT_StaticInitialization` to the symbol table. This can be accomplished by appending the following flag to the linker settings:

* Windows: `/INCLUDE:NativeAOT_StaticInitialization`
* Linux: `-Wl,--require-defined,NativeAOT_StaticInitialization`
* macOS: `-Wl,-u,_NativeAOT_StaticInitialization`

</details>
<!-- markdownlint-enable MD033 -->

You can find a list of additional framework libraries by publishing the project as shared library (`/p:NativeLib=Shared`) with detailed verbosity (`-v d`), and looking at the output generated by the `LinkNative` target.

## References

Real-world example of using CoreRT (previous incarnation of NativeAOT) and Rust is [in this article](https://medium.com/@chyyran/calling-c-natively-from-rust-1f92c506289d).
