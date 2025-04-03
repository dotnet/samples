// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "EventPipeProfiler.h"
#include "corhlpr.h"
#include "profilercommon.h"
#include <string>
#include <sstream>
#include <assert.h>
#include <cstdlib>
#include <ctime>

using std::shared_ptr;
using std::vector;
using std::mutex;
using std::lock_guard;
using std::map;
using std::thread;

EventPipeProfiler::EventPipeProfiler() :
    _pEventPipeProfilerInfo12(),
    _session(),
    _provider(),
    _allTypesEvent(),
    _providerNameCache(),
    _metadataCache(),
    _refCount(0)
{

}

EventPipeProfiler::~EventPipeProfiler()
{
    if (this->_pEventPipeProfilerInfo12 != nullptr)
    {
        this->_pEventPipeProfilerInfo12->Release();
        this->_pEventPipeProfilerInfo12 = nullptr;
    }
}

HRESULT STDMETHODCALLTYPE EventPipeProfiler::Initialize(IUnknown *pICorProfilerInfoUnk)
{
    HRESULT hr = S_OK;
    if (FAILED(hr = pICorProfilerInfoUnk->QueryInterface(__uuidof(ICorProfilerInfo12), (void **)&_pEventPipeProfilerInfo12)))
    {
        printf("FAIL: failed to QI for ICorProfilerInfo12.\n");
        return hr;
    }

    if (FAILED(hr = _pEventPipeProfilerInfo12->SetEventMask2(COR_PRF_MONITOR_JIT_COMPILATION
                                                        | COR_PRF_DISABLE_ALL_NGEN_IMAGES,
                                                       COR_PRF_HIGH_MONITOR_EVENT_PIPE)))
    {
        printf("FAIL: ICorProfilerInfo::SetEventMask2() failed hr=0x%x\n", hr);
        return hr;
    }
    
    // Define our event
    if (FAILED(hr = DefineEvent()))
    {
        return hr;
    }

    // Write it to any listeners that have enabled our provider
    if (FAILED(hr = WriteEvent()))
    {
        return hr;
    }

    // Start a session for listening
    if (FAILED(hr = StartSession()))
    {
        return hr;
    }

    return S_OK;
}

// For the purposes of this sample we create an event that has all the built-in types
// to demonstrate how to serialize them.
HRESULT EventPipeProfiler::DefineEvent()
{
    HRESULT hr = S_OK;
    if (FAILED(hr = _pEventPipeProfilerInfo12->EventPipeCreateProvider(WCHAR("MySuperAwesomeEventPipeProvider"), &_provider)))
    {
        printf("FAIL: could not create EventPipe provider hr=0x%x\n", hr);
        return hr;
    }

    // Create a param descriptor for every type
    COR_PRF_EVENTPIPE_PARAM_DESC allTypesParams[] = {
        { COR_PRF_EVENTPIPE_BOOLEAN,  0,                          WCHAR("Boolean") },
        { COR_PRF_EVENTPIPE_CHAR,     0,                          WCHAR("Char") },
        { COR_PRF_EVENTPIPE_SBYTE,    0,                          WCHAR("SByte") },
        { COR_PRF_EVENTPIPE_BYTE,     0,                          WCHAR("Byte") },
        { COR_PRF_EVENTPIPE_INT16,    0,                          WCHAR("Int16") },
        { COR_PRF_EVENTPIPE_UINT16,   0,                          WCHAR("UInt16") },
        { COR_PRF_EVENTPIPE_INT32,    0,                          WCHAR("Int32") },
        { COR_PRF_EVENTPIPE_UINT32,   0,                          WCHAR("UInt32") },
        { COR_PRF_EVENTPIPE_INT64,    0,                          WCHAR("Int64") },
        { COR_PRF_EVENTPIPE_UINT64,   0,                          WCHAR("UInt64") },
        { COR_PRF_EVENTPIPE_SINGLE,   0,                          WCHAR("Single") },
        { COR_PRF_EVENTPIPE_DOUBLE,   0,                          WCHAR("Double") },
        { COR_PRF_EVENTPIPE_GUID,     0,                          WCHAR("Guid") },
        { COR_PRF_EVENTPIPE_STRING,   0,                          WCHAR("String") },
        { COR_PRF_EVENTPIPE_DATETIME, 0,                          WCHAR("DateTime") },
        { COR_PRF_EVENTPIPE_ARRAY,    COR_PRF_EVENTPIPE_INT32,    WCHAR("IntArray")}
    };

    const size_t allTypesParamsCount = sizeof(allTypesParams) / sizeof(allTypesParams[0]);
    hr = _pEventPipeProfilerInfo12->EventPipeDefineEvent(
            _provider,                      // Provider
            WCHAR("AllTypesEvent"),         // Name
            1,                              // ID
            0,                              // Keywords
            1,                              // Version
            COR_PRF_EVENTPIPE_LOGALWAYS,    // Level
            12,                             // opcode
            true,                           // Needs stack
            allTypesParamsCount,            // size of params
            allTypesParams,                 // Param descriptors
            &_allTypesEvent                 // [OUT] event ID
        );
    if (FAILED(hr))
    {
        printf("FAIL: could not create EventPipe event with all types hr=0x%x\n", hr);
        return hr;
    }

    return S_OK;
}

// This method will write a single event with static data to demonstrate how to serialize the
// different types that EventPipe can handle. In a real world application the data would likely
// be dynamic, but the serialization step would be the same.
HRESULT EventPipeProfiler::WriteEvent()
{
    printf("Writing AllTypesEvent\n");

    COR_PRF_EVENT_DATA eventData[16];

    // { COR_PRF_EVENTPIPE_BOOLEAN, WCHAR("Boolean") }
    BOOL b = TRUE;
    eventData[0].ptr = reinterpret_cast<UINT64>(&b);
    eventData[0].size = sizeof(BOOL);
    // { COR_PRF_EVENTPIPE_CHAR, WCHAR("Char") }
    WCHAR ch = 'A';
    eventData[1].ptr = reinterpret_cast<UINT64>(&ch);
    eventData[1].size = sizeof(WCHAR);
    // { COR_PRF_EVENTPIPE_SBYTE, WCHAR("SByte") }
    int8_t i8t = -124;
    eventData[2].ptr = reinterpret_cast<UINT64>(&i8t);
    eventData[2].size = sizeof(int8_t);
    // { COR_PRF_EVENTPIPE_BYTE, WCHAR("Byte") }
    uint8_t ui8t = 125;
    eventData[3].ptr = reinterpret_cast<UINT64>(&ui8t);
    eventData[3].size = sizeof(uint8_t);
    // { COR_PRF_EVENTPIPE_INT16, WCHAR("Int16") }
    int16_t i16t = -35;
    eventData[4].ptr = reinterpret_cast<UINT64>(&i16t);
    eventData[4].size = sizeof(int16_t);
    // { COR_PRF_EVENTPIPE_UINT16, WCHAR("UInt16") }
    uint16_t u16t = 98;
    eventData[5].ptr = reinterpret_cast<UINT64>(&u16t);
    eventData[5].size = sizeof(uint16_t);
    // { COR_PRF_EVENTPIPE_INT32, WCHAR("Int32") }
    int32_t i32t = -560;
    eventData[6].ptr = reinterpret_cast<UINT64>(&i32t);
    eventData[6].size = sizeof(int32_t);
    // { COR_PRF_EVENTPIPE_UINT32, WCHAR("UInt32") }
    uint32_t ui32t = 561;
    eventData[7].ptr = reinterpret_cast<UINT64>(&ui32t);
    eventData[7].size = sizeof(uint32_t);
    // { COR_PRF_EVENTPIPE_INT64, WCHAR("Int64") }
    int64_t i64t = 2147483648LL;
    eventData[8].ptr = reinterpret_cast<UINT64>(&i64t);
    eventData[8].size = sizeof(int64_t);
    // { COR_PRF_EVENTPIPE_UINT64, WCHAR("UInt64") }
    uint64_t ui64t = 2147483649ULL;
    eventData[9].ptr = reinterpret_cast<UINT64>(&ui64t);
    eventData[9].size = sizeof(uint64_t);
    // { COR_PRF_EVENTPIPE_SINGLE, WCHAR("Single") }
    float f = 3.0f;
    eventData[10].ptr = reinterpret_cast<UINT64>(&f);
    eventData[10].size = sizeof(float);
    // { COR_PRF_EVENTPIPE_DOUBLE, WCHAR("Double") }
    double d = 3.023;
    eventData[11].ptr = reinterpret_cast<UINT64>(&d);
    eventData[11].size = sizeof(double);
    // { COR_PRF_EVENTPIPE_GUID, WCHAR("Guid") }
    GUID guid = { 0x176FBED1,0xA55C,0x4796, { 0x98,0xCA,0xA9,0xDA,0x0E,0xF8,0x83,0xE7 }};
    eventData[12].ptr = reinterpret_cast<UINT64>(&guid);
    eventData[12].size = sizeof(GUID);
    // { COR_PRF_EVENTPIPE_STRING, WCHAR("String") }
    LPCWCH str = WCHAR("Hello, this is a string!");
    eventData[13].ptr = reinterpret_cast<UINT64>(str);
    eventData[13].size = static_cast<UINT32>(wcslen(str) + 1 /*include null char*/) * sizeof(WCHAR);
    // { COR_PRF_EVENTPIPE_DATETIME, WCHAR("DateTime") }
    // TraceEvent uses DateTime.FromFileTime() to parse
    uint64_t dateTime = 132243707160000000ULL;
    eventData[14].ptr = reinterpret_cast<UINT64>(&dateTime);
    eventData[14].size = sizeof(uint64_t);
    
    // EventPipe also supports arbitrary length arrays of built in types. Array types
    // are length prefixed.

    // { COR_PRF_EVENTPIPE_FLAG_ARRAY_TYPE, COR_PRF_EVENTPIPE_INT32, WCHAR("IntArray")}
    constexpr INT32 arraySize = 2 + (100 * sizeof(INT32));
    BYTE dataSource[arraySize];
    size_t offset = 0;
    // Write the array length, this can change for each call to EventPipeWriteEvent
    WriteToBuffer<UINT16>(dataSource, arraySize, &offset, 100);

    for (int i = 0; i < 100; ++i)
    {
        WriteToBuffer<INT32>(dataSource, arraySize, &offset, 100 - i);
    }

    eventData[15].ptr = reinterpret_cast<UINT64>(&dataSource[0]);
    eventData[15].size = arraySize;

    HRESULT hr = _pEventPipeProfilerInfo12->EventPipeWriteEvent(
                    _allTypesEvent,
                    sizeof(eventData)/sizeof(COR_PRF_EVENT_DATA),
                    eventData,
                    NULL,
                    NULL);
    if (FAILED(hr))
    {
        printf("FAIL: EventPipeWriteEvent failed for AllTypesEvent with hr=0x%x\n", hr);
        return hr;
    }

    return S_OK;
}

HRESULT EventPipeProfiler::StartSession()
{
    // Microsoft-Windows-DotNETRuntime is the provider for the runtime events, listen to all events
    // from the runtime.
    COR_PRF_EVENTPIPE_PROVIDER_CONFIG providers[] = {
        { WCHAR("Microsoft-Windows-DotNETRuntime"),  0xFFFFFFFFFFFFFFFF, 5, NULL }
    };

    HRESULT hr = _pEventPipeProfilerInfo12->EventPipeStartSession(sizeof(providers) / sizeof(providers[0]),
                                                    providers,
                                                    false,
                                                    &_session);
    if (FAILED(hr))
    {
        printf("Failed to start event pipe session with hr=0x%x\n", hr);
        return hr;
    }

    return S_OK;
}

HRESULT STDMETHODCALLTYPE EventPipeProfiler::Shutdown()
{
    _pEventPipeProfilerInfo12->EventPipeStopSession(_session);
    return S_OK;
}

// EventPipeEventDelivered is how the profiler receives EventPipe events for a session. it is called synchronously 
// on the thread that generates the event. It will block the runtime's execution until it returns
// so be careful not to start any long running operations. This method can (and will) be called 
// concurrently from multiple threads.
HRESULT STDMETHODCALLTYPE EventPipeProfiler::EventPipeEventDelivered(
    EVENTPIPE_PROVIDER provider,
    DWORD eventId,
    DWORD eventVersion,
    ULONG cbMetadataBlob,
    LPCBYTE metadataBlob,
    ULONG cbEventData,
    LPCBYTE eventData,
    LPCGUID pActivityId,
    LPCGUID pRelatedActivityId,
    ThreadID eventThread,
    ULONG numStackFrames,
    UINT_PTR stackFrames[])
{
    String name = GetOrAddProviderName(provider);
    
    EventPipeMetadataInstance metadata = GetOrAddMetadata(metadataBlob, cbMetadataBlob);
    EventPipeEventPrinter printer;
    printer.PrintEvent(name.ToNativeStr(),
                       metadata,
                       eventData,
                       cbEventData,
                       pActivityId,
                       pRelatedActivityId,
                       eventThread,
                       stackFrames,
                       numStackFrames);
    return S_OK;
}

// This method is called synchronously from the provider creator's thread. Just like
// EventPipeEventDelivered above, any long running operations will block the runtime from continuing.
// This method will be called before any events are fired from the provider.
HRESULT EventPipeProfiler::EventPipeProviderCreated(EVENTPIPE_PROVIDER provider)
{
    String name = GetOrAddProviderName(provider);
    wprintf(L"EventPipeProfiler::EventPipeProviderCreated provider=%s\n", name.ToCStr());

    // Add all events from any new provider to the session
    COR_PRF_EVENTPIPE_PROVIDER_CONFIG providerConfig = { name.ToNativeStr(), 0xFFFFFFFFFFFFFFFF, 5, NULL };
    HRESULT hr = _pEventPipeProfilerInfo12->EventPipeAddProviderToSession(_session, providerConfig);
    if (FAILED(hr))
    {
        printf("EventPipeAddProviderToSession failed with hr=0x%x\n", hr);
        return hr;
    }

    return S_OK;
}

String EventPipeProfiler::GetOrAddProviderName(EVENTPIPE_PROVIDER provider)
{
    auto it = _providerNameCache.find(provider);
    if (it == _providerNameCache.end())
    {
        WCHAR nameBuffer[LONG_LENGTH];
        ULONG nameCount;
        HRESULT hr = _pEventPipeProfilerInfo12->EventPipeGetProviderInfo(provider,
                                                                   LONG_LENGTH,
                                                                   &nameCount,
                                                                   nameBuffer);
        if (FAILED(hr))
        {
            printf("EventPipeGetProviderInfo failed with hr=0x%x\n", hr);
            return WCHAR("GetProviderInfo failed");
        }

        _providerNameCache.insertNew(provider, String(nameBuffer));

        it = _providerNameCache.find(provider);
        assert(it != _providerNameCache.end());
    }

    return it->second;
}

EventPipeMetadataInstance EventPipeProfiler::GetOrAddMetadata(LPCBYTE pMetadata, ULONG cbMetadata)
{
    auto it = _metadataCache.find(pMetadata);
    if (it == _metadataCache.end())
    {
        EventPipeMetadataReader reader;
        EventPipeMetadataInstance parsedMetadata = reader.Parse(pMetadata, cbMetadata);
        _metadataCache.insertNew(pMetadata, parsedMetadata);

        it = _metadataCache.find(pMetadata);
        assert(it != _metadataCache.end());
    }

    return it->second;
}
