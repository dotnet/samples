// NativeApp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Windows.h>

#include <MixedLibrary.Exports.h>

int main()
{
    const wchar_t *msg = L"from native app!";

    std::wcout << L"=== Import library ===" << std::endl;
    NativeEntryPoint_CallManaged(msg);
    std::wcout << std::endl;

    std::wcout << L"=== LoadLibrary ===" << std::endl;
    HMODULE hmod = ::LoadLibraryW(L"MixedLibrary.dll");
    if (hmod == NULL)
    {
        std::wcout << L"Failed to load MixedLibrary.dll" << std::endl;
        return -1;
    }

    typedef void(__stdcall *NativeEntryPointFunc)(const wchar_t *msg);
    NativeEntryPointFunc func = (NativeEntryPointFunc)::GetProcAddress(hmod, "NativeEntryPoint_CallManaged");
    if (func == NULL)
    {
        std::wcout << L"Failed to get NativeEntryPoint_CallManaged function" << std::endl;
        return -1;
    }

    func(msg);
    std::wcout << std::endl;
}
