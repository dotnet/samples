#include <stdio.h>

#if defined(__GNUC__)
#define EXPORT extern "C" __attribute__((visibility("default")))
#elif defined(_MSC_VER)
#define EXPORT extern "C" __declspec(dllexport)
#endif 

extern "C" EXPORT int NativeSum(int a, int b)
{
    return a + b;
}
