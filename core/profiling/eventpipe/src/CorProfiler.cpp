// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "CorProfiler.h"
#include "corhlpr.h"
#include "CComPtr.h"
#include "profiler_pal.h"
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

CorProfiler::CorProfiler() :
    _pCorProfilerInfo12(),
    _session(),
    _provider(),
    _allTypesEvent(),
    _providerNameCache(),
    _cacheLock(),
    _metadataCache(),
    _refCount(0)
{

}

CorProfiler::~CorProfiler()
{
    if (this->_pCorProfilerInfo12 != nullptr)
    {
        this->_pCorProfilerInfo12->Release();
        this->_pCorProfilerInfo12 = nullptr;
    }
}

HRESULT STDMETHODCALLTYPE CorProfiler::Initialize(IUnknown *pICorProfilerInfoUnk)
{
    HRESULT hr = S_OK;
    if (FAILED(hr = pICorProfilerInfoUnk->QueryInterface(__uuidof(ICorProfilerInfo12), (void **)&_pCorProfilerInfo12)))
    {
        printf("FAIL: failed to QI for ICorProfilerInfo12.\n");
        return hr;
    }

    if (FAILED(hr = _pCorProfilerInfo12->SetEventMask2(COR_PRF_MONITOR_JIT_COMPILATION
                                                        | COR_PRF_DISABLE_ALL_NGEN_IMAGES,
                                                       COR_PRF_HIGH_MONITOR_EVENT_PIPE)))
    {
        printf("FAIL: ICorProfilerInfo::SetEventMask2() failed hr=0x%x\n", hr);
        return hr;
    }
    
    // Write an event
    if (FAILED(hr = DefineEvent()) || FAILED(hr = WriteEvent()))
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

HRESULT CorProfiler::DefineEvent()
{
    HRESULT hr = S_OK;
    if (FAILED(hr = _pCorProfilerInfo12->EventPipeCreateProvider(WCHAR("MySuperAwesomeEventPipeProvider"), &_provider)))
    {
        printf("FAIL: could not create EventPipe provider hr=0x%x\n", hr);
        return hr;
    }

    // Create a param descriptor for every type
    COR_PRF_EVENTPIPE_PARAM_DESC allTypesParams[] = {
        { COR_PRF_EVENTPIPE_BOOLEAN,  0, WCHAR("Boolean") },
        { COR_PRF_EVENTPIPE_CHAR,     0, WCHAR("Char") },
        { COR_PRF_EVENTPIPE_SBYTE,    0, WCHAR("SByte") },
        { COR_PRF_EVENTPIPE_BYTE,     0, WCHAR("Byte") },
        { COR_PRF_EVENTPIPE_INT16,    0, WCHAR("Int16") },
        { COR_PRF_EVENTPIPE_UINT16,   0, WCHAR("UInt16") },
        { COR_PRF_EVENTPIPE_INT32,    0, WCHAR("Int32") },
        { COR_PRF_EVENTPIPE_UINT32,   0, WCHAR("UInt32") },
        { COR_PRF_EVENTPIPE_INT64,    0, WCHAR("Int64") },
        { COR_PRF_EVENTPIPE_UINT64,   0, WCHAR("UInt64") },
        { COR_PRF_EVENTPIPE_SINGLE,   0, WCHAR("Single") },
        { COR_PRF_EVENTPIPE_DOUBLE,   0, WCHAR("Double") },
        { COR_PRF_EVENTPIPE_GUID,     0, WCHAR("Guid") },
        { COR_PRF_EVENTPIPE_STRING,   0, WCHAR("String") },
        { COR_PRF_EVENTPIPE_DATETIME, 0, WCHAR("DateTime") }
    };

    const size_t allTypesParamsCount = sizeof(allTypesParams) / sizeof(allTypesParams[0]);
    hr = _pCorProfilerInfo12->EventPipeDefineEvent(
            _provider,                      // Provider
            WCHAR("AllTypesEvent"),         // Name
            1,                              // ID
            0,                              // Keywords
            1,                              // Version
            COR_PRF_EVENTPIPE_LOGALWAYS,    // Level
            12,                              // opcode
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

HRESULT CorProfiler::WriteEvent()
{
    printf("Writing AllTypesEvent\n");

    COR_PRF_EVENT_DATA eventData[15];

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

    HRESULT hr = _pCorProfilerInfo12->EventPipeWriteEvent(
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

HRESULT CorProfiler::StartSession()
{
    // Microsoft-Windows-DotNETRuntime is the provider for the runtime events, listen to all events
    // from the runtime.
    COR_PRF_EVENTPIPE_PROVIDER_CONFIG providers[] = {
        { WCHAR("Microsoft-Windows-DotNETRuntime"),  0xFFFFFFFFFFFFFFFF, 5, NULL }
    };

    HRESULT hr = _pCorProfilerInfo12->EventPipeStartSession(sizeof(providers) / sizeof(providers[0]),
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

HRESULT STDMETHODCALLTYPE CorProfiler::Shutdown()
{
    _pCorProfilerInfo12->EventPipeStopSession(_session);
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::AppDomainCreationStarted(AppDomainID appDomainId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::AppDomainCreationFinished(AppDomainID appDomainId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::AppDomainShutdownStarted(AppDomainID appDomainId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::AppDomainShutdownFinished(AppDomainID appDomainId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::AssemblyLoadStarted(AssemblyID assemblyId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::AssemblyLoadFinished(AssemblyID assemblyId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::AssemblyUnloadStarted(AssemblyID assemblyId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::AssemblyUnloadFinished(AssemblyID assemblyId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ModuleLoadStarted(ModuleID moduleId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ModuleLoadFinished(ModuleID moduleId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ModuleUnloadStarted(ModuleID moduleId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ModuleUnloadFinished(ModuleID moduleId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ModuleAttachedToAssembly(ModuleID moduleId, AssemblyID AssemblyId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ClassLoadStarted(ClassID classId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ClassLoadFinished(ClassID classId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ClassUnloadStarted(ClassID classId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ClassUnloadFinished(ClassID classId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::FunctionUnloadStarted(FunctionID functionId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::JITCompilationStarted(FunctionID functionId, BOOL fIsSafeToBlock)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::JITCompilationFinished(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::JITCachedFunctionSearchStarted(FunctionID functionId, BOOL *pbUseCachedFunction)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::JITCachedFunctionSearchFinished(FunctionID functionId, COR_PRF_JIT_CACHE result)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::JITFunctionPitched(FunctionID functionId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::JITInlining(FunctionID callerId, FunctionID calleeId, BOOL *pfShouldInline)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ThreadCreated(ThreadID threadId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ThreadDestroyed(ThreadID threadId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ThreadAssignedToOSThread(ThreadID managedThreadId, DWORD osThreadId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RemotingClientInvocationStarted()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RemotingClientSendingMessage(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RemotingClientReceivingReply(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RemotingClientInvocationFinished()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RemotingServerReceivingMessage(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RemotingServerInvocationStarted()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RemotingServerInvocationReturned()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RemotingServerSendingReply(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::UnmanagedToManagedTransition(FunctionID functionId, COR_PRF_TRANSITION_REASON reason)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ManagedToUnmanagedTransition(FunctionID functionId, COR_PRF_TRANSITION_REASON reason)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RuntimeSuspendStarted(COR_PRF_SUSPEND_REASON suspendReason)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RuntimeSuspendFinished()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RuntimeSuspendAborted()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RuntimeResumeStarted()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RuntimeResumeFinished()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RuntimeThreadSuspended(ThreadID threadId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RuntimeThreadResumed(ThreadID threadId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::MovedReferences(ULONG cMovedObjectIDRanges, ObjectID oldObjectIDRangeStart[], ObjectID newObjectIDRangeStart[], ULONG cObjectIDRangeLength[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ObjectAllocated(ObjectID objectId, ClassID classId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ObjectsAllocatedByClass(ULONG cClassCount, ClassID classIds[], ULONG cObjects[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ObjectReferences(ObjectID objectId, ClassID classId, ULONG cObjectRefs, ObjectID objectRefIds[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RootReferences(ULONG cRootRefs, ObjectID rootRefIds[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionThrown(ObjectID thrownObjectId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionSearchFunctionEnter(FunctionID functionId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionSearchFunctionLeave()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionSearchFilterEnter(FunctionID functionId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionSearchFilterLeave()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionSearchCatcherFound(FunctionID functionId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionOSHandlerEnter(UINT_PTR __unused)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionOSHandlerLeave(UINT_PTR __unused)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionUnwindFunctionEnter(FunctionID functionId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionUnwindFunctionLeave()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionUnwindFinallyEnter(FunctionID functionId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionUnwindFinallyLeave()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionCatcherEnter(FunctionID functionId, ObjectID objectId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionCatcherLeave()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::COMClassicVTableCreated(ClassID wrappedClassId, REFGUID implementedIID, void *pVTable, ULONG cSlots)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::COMClassicVTableDestroyed(ClassID wrappedClassId, REFGUID implementedIID, void *pVTable)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionCLRCatcherFound()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ExceptionCLRCatcherExecute()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ThreadNameChanged(ThreadID threadId, ULONG cchName, WCHAR name[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::GarbageCollectionStarted(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::SurvivingReferences(ULONG cSurvivingObjectIDRanges, ObjectID objectIDRangeStart[], ULONG cObjectIDRangeLength[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::GarbageCollectionFinished()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::FinalizeableObjectQueued(DWORD finalizerFlags, ObjectID objectID)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::RootReferences2(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[], COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::HandleCreated(GCHandleID handleId, ObjectID initialObjectId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::HandleDestroyed(GCHandleID handleId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::InitializeForAttach(IUnknown *_pCorProfilerInfo12Unk, void *pvClientData, UINT cbClientData)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ProfilerAttachComplete()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ProfilerDetachSucceeded()
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ReJITCompilationStarted(FunctionID functionId, ReJITID rejitId, BOOL fIsSafeToBlock)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::GetReJITParameters(ModuleID moduleId, mdMethodDef methodId, ICorProfilerFunctionControl *pFunctionControl)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ReJITCompilationFinished(FunctionID functionId, ReJITID rejitId, HRESULT hrStatus, BOOL fIsSafeToBlock)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ReJITError(ModuleID moduleId, mdMethodDef methodId, FunctionID functionId, HRESULT hrStatus)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::MovedReferences2(ULONG cMovedObjectIDRanges, ObjectID oldObjectIDRangeStart[], ObjectID newObjectIDRangeStart[], SIZE_T cObjectIDRangeLength[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::SurvivingReferences2(ULONG cSurvivingObjectIDRanges, ObjectID objectIDRangeStart[], SIZE_T cObjectIDRangeLength[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ConditionalWeakTableElementReferences(ULONG cRootRefs, ObjectID keyRefIds[], ObjectID valueRefIds[], GCHandleID rootIds[])
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::GetAssemblyReferences(const WCHAR *wszAssemblyPath, ICorProfilerAssemblyReferenceProvider *pAsmRefProvider)
{
    printf("GetAssemblyReferences\n");
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::ModuleInMemorySymbolsUpdated(ModuleID moduleId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::DynamicMethodJITCompilationStarted(FunctionID functionId, BOOL fIsSafeToBlock, LPCBYTE ilHeader, ULONG cbILHeader)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::DynamicMethodJITCompilationFinished(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::DynamicMethodUnloaded(FunctionID functionId)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CorProfiler::EventPipeEventDelivered(
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

HRESULT CorProfiler::EventPipeProviderCreated(EVENTPIPE_PROVIDER provider)
{
    String name = GetOrAddProviderName(provider);
    wprintf(L"CorProfiler::EventPipeProviderCreated provider=%s\n", name.ToCStr());

    // Add all events from any new provider to the session
    COR_PRF_EVENTPIPE_PROVIDER_CONFIG providerConfig = { name.ToNativeStr(), 0xFFFFFFFFFFFFFFFF, 5, NULL };
    HRESULT hr = _pCorProfilerInfo12->EventPipeAddProviderToSession(_session, providerConfig);
    if (FAILED(hr))
    {
        printf("EventPipeAddProviderToSession failed with hr=0x%x\n", hr);
        return hr;
    }

    return S_OK;
}

String CorProfiler::GetOrAddProviderName(EVENTPIPE_PROVIDER provider)
{
    lock_guard<mutex> guard(_cacheLock);

    auto it = _providerNameCache.find(provider);
    if (it == _providerNameCache.end())
    {
        WCHAR nameBuffer[LONG_LENGTH];
        ULONG nameCount;
        HRESULT hr = _pCorProfilerInfo12->EventPipeGetProviderInfo(provider,
                                                                   LONG_LENGTH,
                                                                   &nameCount,
                                                                   nameBuffer);
        if (FAILED(hr))
        {
            printf("EventPipeGetProviderInfo failed with hr=0x%x\n", hr);
            return WCHAR("GetProviderInfo failed");
        }

        _providerNameCache.insert({provider, String(nameBuffer)});

        it = _providerNameCache.find(provider);
        assert(it != _providerNameCache.end());
    }

    return it->second;
}

EventPipeMetadataInstance CorProfiler::GetOrAddMetadata(LPCBYTE pMetadata, ULONG cbMetadata)
{
    // TODO: holding the lock while parsing metdata is not the best plan. Metadata parsing
    // is kind of slow and could cause perf issues.
    lock_guard<mutex> guard(_cacheLock);

    auto it = _metadataCache.find(pMetadata);
    if (it == _metadataCache.end())
    {
        EventPipeMetadataReader reader;
        EventPipeMetadataInstance parsedMetadata = reader.Parse(pMetadata, cbMetadata);
        _metadataCache.insert({pMetadata, parsedMetadata});

        it = _metadataCache.find(pMetadata);
        assert(it != _metadataCache.end());
    }

    return it->second;
}
