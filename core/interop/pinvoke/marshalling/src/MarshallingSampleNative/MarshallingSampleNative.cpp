#include "common.h"
#include <string>
#include <string.h>
#include <stdlib.h>

// string functions

extern "C" DLL_EXPORT int STDMETHODCALLTYPE CountBytesInString(char* value)
{
    // value is ANSI (on Windows)/ UTF8 (on Linux/Mac) null-terminated string
    // and can be null.
    if (value == nullptr)
    {
        return -1;
    }
    else
    {
        return strlen(value);
    }
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE CountUtf16StringSize(WCHAR* value)
{
    // value is UTF16 null-terminated string
    // and can be null.
    if (value == nullptr)
    {
        return -1;
    }
    else
    {
        return std::char_traits<WCHAR>::length(value);
    }
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE CountPlatformSpecificCharacters(char_t* value)
{
    // char_t is defined as WCHAR on Windows and as char (UTF8) on Linux/Mac
    if (value == nullptr)
    {
        return -1;
    }
    else
    {
#ifdef WINDOWS
        return wcslen(value);
#else
        return strlen(value);
#endif
    }
}

static char* CreateCopyOfString(const char* stringValue, int& stringValueSize)
{
    stringValueSize = strlen(stringValue) + 1; // + 1 for null terminator

#ifdef WINDOWS
    // On Windows the returned buffer is going to be freed with CoTaskMemFree, so it needs to be allocated
    // with CoTaskMemAlloc or CoTaskMemRealloc.
    char* buffer = (char*)CoTaskMemAlloc(stringValueSize);
#else
    // On Linux and Mac the returned buffer is going to be freed with free(), so it needs to be allocated
    // with malloc/calloc/realloc.
    char* buffer = (char*)malloc(stringValueSize);
#endif

    if (buffer == nullptr)
    {
        return nullptr;
    }

    memcpy(buffer, stringValue, stringValueSize);
    return buffer;
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE GetStringIntoCalleeAllocatedBuffer(char ** value)
{
    const char* stringValue = "Native string value - callee allocated";

    int stringValueSize = -1;
    *value = CreateCopyOfString(stringValue, stringValueSize);
    return stringValueSize;
}

extern "C" DLL_EXPORT char * STDMETHODCALLTYPE ReturnStringIntoCalleeAllocatedBuffer()
{
    const char* stringValue = "Native string value - callee allocated - return";

    int stringValueSize = -1;
    return CreateCopyOfString(stringValue, stringValueSize);
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE GetStringIntoCallerAllocatedBuffer(char *buffer, int *bufferSize)
{
    const char* stringValue = "Native string value - caller allocated";
    int stringValueSize = strlen(stringValue) + 1; // + 1 for null terminator

    if (buffer == nullptr || *bufferSize < stringValueSize)
    {
        *bufferSize = stringValueSize;
        return -1;
    }
    else
    {
        memcpy(buffer, stringValue, stringValueSize);
        *bufferSize = stringValueSize;
        return 0;
    }
}


// Int32 functions
extern "C" DLL_EXPORT int STDMETHODCALLTYPE AcceptInt32Argument(int value)
{
    return value;
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE AcceptInt32ByRefArgument(/*[In]*/ int * pValue)
{
    return *pValue;
}

extern "C" DLL_EXPORT void STDMETHODCALLTYPE GetInt32OutArgument(/*[Out]*/ int * pValue)
{
    *pValue = 9;
}

extern "C" DLL_EXPORT void STDMETHODCALLTYPE ModifyInt32InOutArgument(/*[In,Out]*/ int * pValue)
{
    (*pValue)++;
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE ReturnInt32Argument(int value)
{
    return value;
}


// Boolean functions
extern "C" DLL_EXPORT int STDMETHODCALLTYPE AcceptBOOLArgument(BOOL value)
{
    return value ? 1 : 0;
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE AcceptBOOLByRefArgument(BOOL* pValue)
{
    return (*pValue) ? 1 : 0;
}

extern "C" DLL_EXPORT void STDMETHODCALLTYPE GetBOOLOutArgument(/*[Out]*/ BOOL* pValue)
{
    *pValue = FALSE;
}

extern "C" DLL_EXPORT void STDMETHODCALLTYPE ModifyBOOLInOutArgument(/*[In,Out]*/ BOOL* pValue)
{
    *pValue = !(*pValue);
}

extern "C" DLL_EXPORT BOOL STDMETHODCALLTYPE ReturnBOOLArgument(BOOL value)
{
    return value;
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE CountTrueValues(BOOL value1, bool value2, bool value3)
{
    int count = 0;

    count += (value1 == TRUE) ? 1 : 0;
    count += (value2 == true) ? 1 : 0;
    count += (value3 == true) ? 1 : 0;

    return count;
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE CountTrueValuesWindows(BOOL value1, bool value2, bool value3, VARIANT_BOOL value4)
{
    int count = 0;

    count += CountTrueValues(value1, value2, value3);
    count += (value4 == VARIANT_TRUE) ? 1 : 0;

    return count;
}


// Enum functions
enum EnumFlags
{
    None = 0,
    A = 1,
    B = 2,
    C = 4
};

extern "C" DLL_EXPORT int STDMETHODCALLTYPE CountEnumFlags(int enumValue)
{
    return (enumValue & EnumFlags::A ? 1 : 0) + (enumValue & EnumFlags::B ? 1 : 0) + (enumValue & EnumFlags::C ? 1 : 0);
}


// Numeric functions
extern "C" DLL_EXPORT unsigned char STDMETHODCALLTYPE SumBytes(unsigned char inValue, /*[In]*/ unsigned char* inRef, /*[In,Out]*/ unsigned char* inOutRef, /*[Out]*/ unsigned char* outRef)
{
    unsigned char result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}

extern "C" DLL_EXPORT char STDMETHODCALLTYPE SumSBytes(char inValue, /*[In]*/ char* inRef, /*[In,Out]*/ char* inOutRef, /*[Out]*/ char* outRef)
{
    char result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}

extern "C" DLL_EXPORT unsigned short STDMETHODCALLTYPE SumUShorts(unsigned short inValue, /*[In]*/ unsigned short* inRef, /*[In,Out]*/ unsigned short* inOutRef, /*[Out]*/ unsigned short* outRef)
{
    unsigned short result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}

extern "C" DLL_EXPORT short STDMETHODCALLTYPE SumShorts(short inValue, /*[In]*/ short* inRef, /*[In,Out]*/ short* inOutRef, /*[Out]*/ short* outRef)
{
    short result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}

extern "C" DLL_EXPORT unsigned int STDMETHODCALLTYPE SumUInts(unsigned int inValue, /*[In]*/ unsigned int* inRef, /*[In,Out]*/ unsigned int* inOutRef, /*[Out]*/ unsigned int* outRef)
{
    unsigned int result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}

extern "C" DLL_EXPORT int STDMETHODCALLTYPE SumInts(int inValue, /*[In]*/ int* inRef, /*[In,Out]*/ int* inOutRef, /*[Out]*/ int* outRef)
{
    int result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}

extern "C" DLL_EXPORT uint64_t STDMETHODCALLTYPE SumULongs(uint64_t inValue, /*[In]*/ uint64_t* inRef, /*[In,Out]*/ uint64_t* inOutRef, /*[Out]*/ uint64_t* outRef)
{
    uint64_t result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}

extern "C" DLL_EXPORT int64_t STDMETHODCALLTYPE SumLongs(int64_t inValue, /*[In]*/ int64_t* inRef, /*[In,Out]*/ int64_t* inOutRef, /*[Out]*/ int64_t* outRef)
{
    int64_t result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}

extern "C" DLL_EXPORT float STDMETHODCALLTYPE SumFloats(float inValue, /*[In]*/ float* inRef, /*[In,Out]*/ float* inOutRef, /*[Out]*/ float* outRef)
{
    float result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}

extern "C" DLL_EXPORT double STDMETHODCALLTYPE SumDoubles(double inValue, /*[In]*/ double* inRef, /*[In,Out]*/ double* inOutRef, /*[Out]*/ double* outRef)
{
    double result = inValue + *inRef + *inOutRef;
    *inOutRef = result;
    *outRef = result;
    return result;
}


#if !WINDOWS
typedef struct tagDEC {
#if BIGENDIAN
    union {
        struct {
            uint8_t sign;
            uint8_t scale;
        };
        uint16_t signscale;
    };
    uint16_t wReserved;
#else
    uint16_t wReserved;
    union {
        struct {
            uint8_t scale;
            uint8_t sign;
        };
        uint16_t signscale;
    };
#endif
    int32_t Hi32;
    union {
        struct {
            int32_t Lo32;
            int32_t Mid32;
        };
        uint64_t Lo64;
    };
} DECIMAL, * LPDECIMAL;
#endif

extern "C" DLL_EXPORT DECIMAL STDMETHODCALLTYPE SumDecimals(DECIMAL inValue, /*[In]*/ DECIMAL* inRef, /*[In,Out]*/ DECIMAL* inOutRef, /*[Out]*/ DECIMAL* outRef)
{
    DECIMAL result = inValue;
    result.Lo64 += inRef->Lo64;
    result.Lo64 += inOutRef->Lo64;
    result.Lo64 += outRef->Lo64;

    *inOutRef = result;
    *outRef = result;
    return result;
}


// GUID functions
extern "C" DLL_EXPORT int STDMETHODCALLTYPE CompareGuids(GUID a, GUID b)
{
    return a.Data1 == b.Data1 && a.Data2 == b.Data2 && a.Data3 == b.Data3 && (memcmp(a.Data4, b.Data4, sizeof(a.Data4)) == 0);
}

static GUID _zeroGuid = GUID{ 0, 0, 0, { 0, 0, 0, 0, 0, 0, 0, 0 } };

extern "C" DLL_EXPORT GUID STDMETHODCALLTYPE CountZeroGuids(GUID inValue, /*[In]*/ GUID& inRefA, /*[In]*/ GUID* inRefB, /*[In,Out]*/ GUID* inOutRef, /*[Out]*/ GUID* outRef)
{
    int result = CompareGuids(inValue, _zeroGuid);
    result += CompareGuids(inRefA, _zeroGuid);
    result += CompareGuids(*inRefB, _zeroGuid);
    result += CompareGuids(*inOutRef, _zeroGuid);

    *inOutRef = _zeroGuid;
    inOutRef->Data1 = result;
    *outRef = _zeroGuid;
    outRef->Data1 = result;

    GUID resultGuid = _zeroGuid;
    resultGuid.Data1 = result;
    return resultGuid;
}
