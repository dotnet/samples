#pragma once
#include <stdint.h>
#include <wchar.h>

#ifdef WINDOWS

#include <windows.h>
#define DLL_EXPORT __declspec(dllexport)

typedef WCHAR char_t;

#else // !WINDOWS

#ifndef STDMETHODCALLTYPE
#define STDMETHODCALLTYPE
#endif

#if __GNUC__ >= 4
#define DLL_EXPORT __attribute__ ((visibility ("default")))
#else
#define DLL_EXPORT
#endif

typedef int BOOL;
#define TRUE 1
#define FALSE 0

typedef short VARIANT_BOOL;
#define VARIANT_TRUE -1
#define VARIANT_FALSE 0

typedef char16_t WCHAR;
typedef char char_t;

typedef struct _GUID {
    uint32_t      Data1;
    uint16_t      Data2;
    uint16_t      Data3;
    unsigned char Data4[8];
} GUID;

#endif
