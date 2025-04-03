// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//On unix make sure to compile using -ldl and -pthread flags.

//Set this value accordingly to your workspace settings
#if defined(_WIN32)
#define PathToLibrary "bin\\Release\\net8.0\\win-x64\\publish\\NativeLibrary.dll"
#elif defined(__APPLE__)
#define PathToLibrary "./bin/Release/net8.0/osx-x64/publish/NativeLibrary.dylib"
#else
#define PathToLibrary "./bin/Release/net8.0/linux-x64/publish/NativeLibrary.so"
#endif

#ifdef _WIN32
#include "windows.h"
#define symLoad GetProcAddress
#pragma comment (lib, "ole32.lib")
#else
#include "dlfcn.h"
#include <unistd.h>
#define symLoad dlsym
#define CoTaskMemFree free
#endif

#include <stdlib.h>
#include <stdio.h>

#ifndef F_OK
#define F_OK    0
#endif

int callSumFunc(char *path, char *funcName, int a, int b);
char *callSumStringFunc(char *path, char *funcName, char *a, char *b);

void* loadSymbol(char *path, char *funcName);

int main()
{
    // Check if the library file exists
    if (access(PathToLibrary, F_OK) == -1)
    {
        puts("Couldn't find library at the specified path");
        return 0;
    }

    // Sum two integers
    int sum = callSumFunc(PathToLibrary, "aotsample_add", 2, 8);
    printf("The sum is %d \n", sum);

    // Concatenate two strings
    char *sumstring = callSumStringFunc(PathToLibrary, "aotsample_sumstring", "ok", "ko");
    printf("The concatenated string is %s \n", sumstring);

    // Free string
    CoTaskMemFree(sumstring);
}

void *loadSymbol(char *path, char *funcName)
{
    // Library loading
    #ifdef _WIN32
        HINSTANCE handle = LoadLibraryA(path);
    #else
        void *handle = dlopen(path, RTLD_LAZY);
    #endif
    if (!handle)
    {
#ifdef _WIN32
        int errorCode = GetLastError();
        printf("Failed to load library at specified path. Error code: %d\n", errorCode);
#else
        puts("Failed to load library at specified path");
#endif
        return NULL;
    }

    // Declare a typedef
    typedef char *(*myFunc)(char*,char*);

    // Import Symbol named funcName

    // NativeAOT libraries do not support unloading
    // See https://github.com/dotnet/corert/issues/7887
    return symLoad(handle, funcName);
}

int callSumFunc(char *path, char *funcName, int firstInt, int secondInt)
{
    typedef int(*myFunc)(int,int);
    myFunc MyImport = (myFunc)loadSymbol(path, funcName);

    int result = MyImport(firstInt, secondInt);
    return result;
}

char *callSumStringFunc(char *path, char *funcName, char *firstString, char *secondString)
{
    // Declare a typedef
    typedef char *(*myFunc)(char*,char*);

    // Import Symbol named funcName
    myFunc MyImport = (myFunc)loadSymbol(path, funcName);

    // The C# function will return a pointer
    char *result = MyImport(firstString, secondString);
    return result;
}
