#pragma once

#ifdef MIXEDLIBRARY_EXPORTS
#define MIXEDLIBRARY_API __declspec(dllexport)
#else
#define MIXEDLIBRARY_API __declspec(dllimport)
#endif

extern "C"
{
    MIXEDLIBRARY_API void __stdcall NativeEntryPoint_CallManaged(const wchar_t *msg);

    MIXEDLIBRARY_API void __stdcall NativeEntryPoint_CallNative(const wchar_t *msg);
}
