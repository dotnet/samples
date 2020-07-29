#include <atomic>
#include <iostream>
#include <string.h>

#ifdef WINDOWS

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

typedef struct _GUID {
    unsigned int    Data1;
    unsigned short  Data2;
    unsigned short  Data3;
    unsigned char   Data4[8];
} GUID;

#define HRESULT int
#define ULONG unsigned int
#define REFIID const GUID&
#define E_NOINTERFACE 0x80004002L

bool operator==(REFIID a, REFIID b)
{
    return 0 == ::memcmp(&a, &b, sizeof(GUID));
}

// {00000000-0000-0000-C000-000000000046}
static const GUID IID_IUnknown = { 0x00000000, 0x0000, 0x0000, { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 } };
struct IUnknown
{
    virtual HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        void **ppvObject) = 0;

    virtual ULONG STDMETHODCALLTYPE AddRef(void) = 0;

    virtual ULONG STDMETHODCALLTYPE Release(void) = 0;
};

// {0412BF07-0261-4191-B5DA-9B86340931CB}
static const GUID IID_IGreet = { 0x412bf07, 0x261, 0x4191, { 0xb5, 0xda, 0x9b, 0x86, 0x34, 0x9, 0x31, 0xcb } };
struct IGreet : public IUnknown
{
    virtual void STDMETHODCALLTYPE Hello() = 0;
};

// {B88C195F-3052-467A-97D2-817A1698AAFB}
static const GUID IID_ICompute = { 0xb88c195f, 0x3052, 0x467a, { 0x97, 0xd2, 0x81, 0x7a, 0x16, 0x98, 0xaa, 0xfb } };
struct ICompute : public IUnknown
{
    virtual int STDMETHODCALLTYPE Sum(int a, int b) = 0;
};

enum class SupportedInterfaces
{
    None = 0,
    IGreet = 1,
    ICompute = 2,

    All = IGreet | ICompute
};

inline SupportedInterfaces operator&(SupportedInterfaces a, SupportedInterfaces b)
{
    return static_cast<SupportedInterfaces>(static_cast<int>(a) & static_cast<int>(b));
}

class NativeObject : public IGreet, public ICompute
{
public:
    NativeObject(int id, SupportedInterfaces supportedInterfaces)
        : _id{ id }
        , _supportedInterfaces { supportedInterfaces }
    { }

public:
    void STDMETHODCALLTYPE Hello()
    {
        std::cout << "    -- Hello World from NativeObject #" << _id << std::endl;
    }

public:
    int STDMETHODCALLTYPE Sum(int a, int b)
    {
        return a + b + _id;
    }

public:
    ULONG STDMETHODCALLTYPE AddRef()
    {
        return ++_refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        ULONG c = --_refCount;
        if (c == 0)
            delete this;

        return c;
    }

    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        void **ppvObject)
    {
        if (riid == IID_IGreet && (_supportedInterfaces & SupportedInterfaces::IGreet) == SupportedInterfaces::IGreet)
        {
            *ppvObject = static_cast<IGreet *>(this);
        }
        else if (riid == IID_ICompute && (_supportedInterfaces & SupportedInterfaces::ICompute) == SupportedInterfaces::ICompute)
        {
            *ppvObject = static_cast<ICompute *>(this);
        }
        else if (riid == IID_IUnknown)
        {
            *ppvObject = static_cast<IGreet *>(this);
        }
        else
        {
            *ppvObject = nullptr;
            return E_NOINTERFACE;
        }

        AddRef();
        return 0;
    }

private:
    std::atomic<ULONG> _refCount = { 1 };

    int _id;
    SupportedInterfaces _supportedInterfaces;
};

#if defined(WINDOWS) && defined(_M_IX86)
#pragma comment(linker, "/export:CreateObjects=_CreateObjects@8")
#endif
extern "C" DLL_EXPORT void STDMETHODCALLTYPE CreateObjects(void **outArray, int size)
{
    for (int i = 0; i < size; ++i)
    {
        int flags = i % (static_cast<int>(SupportedInterfaces::All) + 1);
        outArray[i] = new NativeObject(i, static_cast<SupportedInterfaces>(flags));
    }
}
