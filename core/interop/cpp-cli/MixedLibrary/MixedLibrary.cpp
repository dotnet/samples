#include "MixedLibrary.h"
#include "MixedLibrary.Exports.h"

#include <iostream>
#include <vcclr.h>

using namespace System::Runtime::InteropServices;

#if defined _M_IX86
#pragma comment(linker, "/export:NativeEntryPoint_CallManaged=_NativeEntryPoint_CallManaged@4")
#endif
extern "C" MIXEDLIBRARY_API void __stdcall NativeEntryPoint_CallManaged(const wchar_t* msg)
{
	std::wcout << L"Hello from NativeEntryPoint_CallManaged in MixedLibrary" << std::endl;
    String ^msgStr = gcnew String(msg);
    auto c = gcnew ManagedLibrary::Greet();
	c->Hello(msgStr);
}

#if defined _M_IX86 
#pragma comment(linker, "/export:NativeEntryPoint_CallNative=_NativeEntryPoint_CallNative@4")
#endif
extern "C" MIXEDLIBRARY_API void __stdcall NativeEntryPoint_CallNative(const wchar_t *msg)
{
    std::wcout << L"Hello from NativeEntryPoint_CallNative in MixedLibrary" << std::endl;
    MixedLibrary::NativeClass c;
    c.Hello(msg);;
}

void MixedLibrary::ManagedClass::Hello()
{
	Console::WriteLine("Hello from ManagedClass in MixedLibrary");
}

void MixedLibrary::ManagedClass::CallNative(String^ msg)
{
    pin_ptr<const wchar_t> msgPtr = PtrToStringChars(msg);

	NativeClass c;
	c.Hello(msgPtr);
}

void MixedLibrary::NativeClass::Hello(const wchar_t *msg)
{
	std::wcout << L"Hello from NativeClass in MixedLibrary" << std::endl;
    std::wcout << L"-- message: " << msg << std::endl;
}
