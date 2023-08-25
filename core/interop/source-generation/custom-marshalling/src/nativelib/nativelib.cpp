#include <iostream>
#include <codecvt>
#include <locale>
#include <cstring>

#ifdef WINDOWS
#include <Windows.h>

#define DLL_EXPORT __declspec(dllexport)

#ifndef STDMETHODCALLTYPE
#define STDMETHODCALLTYPE __stdcall
#endif

#else // !WINDOWS

#ifndef STDMETHODCALLTYPE
#define STDMETHODCALLTYPE
#endif

#if __GNUC__ >= 4
#define DLL_EXPORT __attribute__ ((visibility ("default")))
#else
#define DLL_EXPORT
#endif

#endif

struct error_data
{
    int code;
    bool is_fatal_error;
    char32_t* message;
};

extern "C" DLL_EXPORT void STDMETHODCALLTYPE PrintString(char32_t* chars)
{
    std::wstring_convert<std::codecvt_utf8<char32_t>, char32_t> converter;
    std::cout << converter.to_bytes(chars) << std::endl;
}

extern "C" DLL_EXPORT void STDMETHODCALLTYPE PrintStrings(char32_t** stringArray, int count)
{
    std::wstring_convert<std::codecvt_utf8<char32_t>, char32_t> converter;
    for (int i = 0; i < count; ++i)
    {
        std::cout << converter.to_bytes(stringArray[i]) << std::endl;
    }
}

extern "C" DLL_EXPORT void STDMETHODCALLTYPE ReverseStringInPlace(char32_t** chars)
{
    size_t len = std::char_traits<char32_t>::length(*chars);
    for (int i = 0; i < len / 2; ++i)
        std::swap((*chars)[i], (*chars)[len - i - 1]);
}

extern "C" DLL_EXPORT char32_t* STDMETHODCALLTYPE ReverseString(char32_t* chars)
{
    size_t len = std::char_traits<char32_t>::length(chars);
    char32_t* reversed = new char32_t[len + 1];
    for (int i = 0; i < len; ++i)
        reversed[i] = chars[len - i - 1];

    reversed[len] = 0; // null-terminate
    return reversed;
}

extern "C" DLL_EXPORT void STDMETHODCALLTYPE PrintErrorData(error_data data)
{
    std::cout << "error_data" << std::endl;
    std::cout << "code           : " << data.code << std::endl;
    std::cout << "is_fatal_error : " << data.is_fatal_error << std::endl;

    std::wstring_convert<std::codecvt_utf8<char32_t>, char32_t> converter;
    std::cout << "message        : " << converter.to_bytes(data.message) << std::endl;
}

extern "C" DLL_EXPORT error_data STDMETHODCALLTYPE GetFatalErrorIfNegative(int code)
{
    std::u32string message(code < 0 ? U"Fatal Error " : U"Error ");
    for (int i = 0; i < abs(code); ++i)
        message.append(U"\u2717");

    char32_t *message_buffer = new char32_t[message.length() + 1];
    int byte_len = message.length() * sizeof(char32_t);
    ::memcpy(message_buffer, message.c_str(), byte_len);
    message_buffer[message.length()] = 0; // null-terminate

    return { code, code < 0, message_buffer };
}

extern "C" DLL_EXPORT error_data* STDMETHODCALLTYPE GetErrors(int* codes, int len)
{
    int bytes_to_alloc = sizeof(error_data) * len;
#ifdef WINDOWS
    // On Windows the returned array is going to be freed with CoTaskMemFree, so it needs to be allocated
    // with CoTaskMemAlloc or CoTaskMemRealloc.
    error_data* ret = (error_data*)::CoTaskMemAlloc(bytes_to_alloc);
#else
    // On non-Windows the returned array is going to be freed with free(), so it needs to be allocated
    // with malloc/calloc/realloc.
    error_data* ret = (error_data*)::malloc(bytes_to_alloc);
#endif

    for (int i = 0; i < len; ++i)
        ret[i] = GetFatalErrorIfNegative(codes[i]);

    return ret;
}

struct error_data_buffer
{
    error_data data[4];
};

extern "C" DLL_EXPORT void STDMETHODCALLTYPE GetErrorCodes(error_data_buffer errors, int* codes)
{
    for (int i = 0; i < 4; ++i)
        codes[i] = errors.data[i].code;
}
