// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

#define NOMINMAX

#include <atomic>
#include <memory>
#include <set>
#include <mutex>
#include <shared_mutex>
#include <vector>
#include <thread>
#include <string>
#include <condition_variable>
#include <map>
#include "profilercommon.h"
#include "eventpipemetadatareader.h"
#include "eventpipeeventprinter.h"

#define SHORT_LENGTH    32
#define STRING_LENGTH  256
#define LONG_LENGTH   1024

template <class MetaInterface>
class COMPtrHolder
{
public:
    COMPtrHolder()
    {
        m_ptr = NULL;
    }

    COMPtrHolder(MetaInterface* ptr)
    {
        if (ptr != NULL)
        {
            ptr->AddRef();
        }
        m_ptr = ptr;
    }

    ~COMPtrHolder()
    {
        if (m_ptr != NULL)
        {
            m_ptr->Release();
            m_ptr = NULL;
        }
    }
    MetaInterface* operator->()
    {
        return m_ptr;
    }

    MetaInterface** operator&()
    {
       // _ASSERT(m_ptr == NULL);
        return &m_ptr;
    }

    operator MetaInterface*()
    {
        return m_ptr;
    }
private:
    MetaInterface* m_ptr;
};

template<class Key, class Value>
class ThreadSafeMap
{
  private:
     std::map<Key, Value> _map;
     mutable std::shared_mutex _mutex;

  public:
    typename std::map<Key, Value>::const_iterator find(Key key) const
    { 
        std::shared_lock lock(_mutex);
        return _map.find(key); 
    }

    typename std::map<Key, Value>::const_iterator end() const
    {
        return _map.end();
    }

    // Returns true if new value was inserted
    bool insertNew(Key key, Value value) 
    {
        std::unique_lock lock(_mutex);

        if (_map.find(key) != _map.end())
        { 
            return false;
        }
        
        _map[key] = value;
        return true;
    }
};

class EventPipeProfiler : public ICorProfilerCallback10
{
private:
    ICorProfilerInfo12 *_pEventPipeProfilerInfo12;
    EVENTPIPE_SESSION _session;
    EVENTPIPE_PROVIDER _provider;
    EVENTPIPE_EVENT _allTypesEvent;
    ThreadSafeMap<EVENTPIPE_PROVIDER, String> _providerNameCache;
    ThreadSafeMap<LPCBYTE, EventPipeMetadataInstance> _metadataCache;
    std::atomic<int> _refCount;

public:

    EventPipeProfiler();
    virtual ~EventPipeProfiler();
    HRESULT STDMETHODCALLTYPE Initialize(IUnknown* pICorProfilerInfoUnk) override;
    HRESULT STDMETHODCALLTYPE Shutdown() override;

    // Profilers must implement all the methods on whatever ICorProfiler* interfaces they override to satisfy the compiler, 
    // even if they are never used
    HRESULT STDMETHODCALLTYPE AppDomainCreationStarted(AppDomainID appDomainId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE AppDomainCreationFinished(AppDomainID appDomainId, HRESULT hrStatus) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE AppDomainShutdownStarted(AppDomainID appDomainId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE AppDomainShutdownFinished(AppDomainID appDomainId, HRESULT hrStatus) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE AssemblyLoadStarted(AssemblyID assemblyId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE AssemblyLoadFinished(AssemblyID assemblyId, HRESULT hrStatus) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE AssemblyUnloadStarted(AssemblyID assemblyId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE AssemblyUnloadFinished(AssemblyID assemblyId, HRESULT hrStatus) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ModuleLoadStarted(ModuleID moduleId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ModuleLoadFinished(ModuleID moduleId, HRESULT hrStatus) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ModuleUnloadStarted(ModuleID moduleId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ModuleUnloadFinished(ModuleID moduleId, HRESULT hrStatus) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ModuleAttachedToAssembly(ModuleID moduleId, AssemblyID AssemblyId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ClassLoadStarted(ClassID classId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ClassLoadFinished(ClassID classId, HRESULT hrStatus) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ClassUnloadStarted(ClassID classId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ClassUnloadFinished(ClassID classId, HRESULT hrStatus) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE FunctionUnloadStarted(FunctionID functionId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE JITCompilationStarted(FunctionID functionId, BOOL fIsSafeToBlock) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE JITCompilationFinished(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE JITCachedFunctionSearchStarted(FunctionID functionId, BOOL* pbUseCachedFunction) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE JITCachedFunctionSearchFinished(FunctionID functionId, COR_PRF_JIT_CACHE result) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE JITFunctionPitched(FunctionID functionId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE JITInlining(FunctionID callerId, FunctionID calleeId, BOOL* pfShouldInline) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ThreadCreated(ThreadID threadId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ThreadDestroyed(ThreadID threadId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ThreadAssignedToOSThread(ThreadID managedThreadId, DWORD osThreadId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RemotingClientInvocationStarted() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RemotingClientSendingMessage(GUID* pCookie, BOOL fIsAsync) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RemotingClientReceivingReply(GUID* pCookie, BOOL fIsAsync) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RemotingClientInvocationFinished() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RemotingServerReceivingMessage(GUID* pCookie, BOOL fIsAsync) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RemotingServerInvocationStarted() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RemotingServerInvocationReturned() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RemotingServerSendingReply(GUID* pCookie, BOOL fIsAsync) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE UnmanagedToManagedTransition(FunctionID functionId, COR_PRF_TRANSITION_REASON reason) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ManagedToUnmanagedTransition(FunctionID functionId, COR_PRF_TRANSITION_REASON reason) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RuntimeSuspendStarted(COR_PRF_SUSPEND_REASON suspendReason) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RuntimeSuspendFinished() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RuntimeSuspendAborted() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RuntimeResumeStarted() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RuntimeResumeFinished() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RuntimeThreadSuspended(ThreadID threadId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RuntimeThreadResumed(ThreadID threadId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE MovedReferences(ULONG cMovedObjectIDRanges, ObjectID oldObjectIDRangeStart[], ObjectID newObjectIDRangeStart[], ULONG cObjectIDRangeLength[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ObjectAllocated(ObjectID objectId, ClassID classId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ObjectsAllocatedByClass(ULONG cClassCount, ClassID classIds[], ULONG cObjects[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ObjectReferences(ObjectID objectId, ClassID classId, ULONG cObjectRefs, ObjectID objectRefIds[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RootReferences(ULONG cRootRefs, ObjectID rootRefIds[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionThrown(ObjectID thrownObjectId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionSearchFunctionEnter(FunctionID functionId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionSearchFunctionLeave() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionSearchFilterEnter(FunctionID functionId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionSearchFilterLeave() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionSearchCatcherFound(FunctionID functionId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionOSHandlerEnter(UINT_PTR __unused) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionOSHandlerLeave(UINT_PTR __unused) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionUnwindFunctionEnter(FunctionID functionId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionUnwindFunctionLeave() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionUnwindFinallyEnter(FunctionID functionId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionUnwindFinallyLeave() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionCatcherEnter(FunctionID functionId, ObjectID objectId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionCatcherLeave() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE COMClassicVTableCreated(ClassID wrappedClassId, REFGUID implementedIID, void* pVTable, ULONG cSlots) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE COMClassicVTableDestroyed(ClassID wrappedClassId, REFGUID implementedIID, void* pVTable) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionCLRCatcherFound() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ExceptionCLRCatcherExecute() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ThreadNameChanged(ThreadID threadId, ULONG cchName, WCHAR name[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE GarbageCollectionStarted(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE SurvivingReferences(ULONG cSurvivingObjectIDRanges, ObjectID objectIDRangeStart[], ULONG cObjectIDRangeLength[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE GarbageCollectionFinished() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE FinalizeableObjectQueued(DWORD finalizerFlags, ObjectID objectID) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE RootReferences2(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[], COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE HandleCreated(GCHandleID handleId, ObjectID initialObjectId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE HandleDestroyed(GCHandleID handleId) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE InitializeForAttach(IUnknown* pEventPipeProfilerInfoUnk, void* pvClientData, UINT cbClientData) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ProfilerAttachComplete() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ProfilerDetachSucceeded() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ReJITCompilationStarted(FunctionID functionId, ReJITID rejitId, BOOL fIsSafeToBlock) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE GetReJITParameters(ModuleID moduleId, mdMethodDef methodId, ICorProfilerFunctionControl* pFunctionControl) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ReJITCompilationFinished(FunctionID functionId, ReJITID rejitId, HRESULT hrStatus, BOOL fIsSafeToBlock) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ReJITError(ModuleID moduleId, mdMethodDef methodId, FunctionID functionId, HRESULT hrStatus) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE MovedReferences2(ULONG cMovedObjectIDRanges, ObjectID oldObjectIDRangeStart[], ObjectID newObjectIDRangeStart[], SIZE_T cObjectIDRangeLength[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE SurvivingReferences2(ULONG cSurvivingObjectIDRanges, ObjectID objectIDRangeStart[], SIZE_T cObjectIDRangeLength[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ConditionalWeakTableElementReferences(ULONG cRootRefs, ObjectID keyRefIds[], ObjectID valueRefIds[], GCHandleID rootIds[]) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE GetAssemblyReferences(const WCHAR* wszAssemblyPath, ICorProfilerAssemblyReferenceProvider* pAsmRefProvider) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE ModuleInMemorySymbolsUpdated(ModuleID moduleId) override { return S_OK; }    HRESULT STDMETHODCALLTYPE DynamicMethodJITCompilationStarted(FunctionID functionId, BOOL fIsSafeToBlock, LPCBYTE ilHeader, ULONG cbILHeader) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE DynamicMethodJITCompilationFinished(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE DynamicMethodUnloaded(FunctionID functionId) override { return S_OK; }

    // The following methods are defined in EventPipeProfiler.cpp
    HRESULT STDMETHODCALLTYPE EventPipeEventDelivered(
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
        UINT_PTR stackFrames[]) override;

    HRESULT STDMETHODCALLTYPE EventPipeProviderCreated(EVENTPIPE_PROVIDER provider) override;

    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void **ppvObject) override
    {
        if (riid == __uuidof(ICorProfilerCallback10) ||
            riid == __uuidof(ICorProfilerCallback9) ||
            riid == __uuidof(ICorProfilerCallback8) ||
            riid == __uuidof(ICorProfilerCallback7) ||
            riid == __uuidof(ICorProfilerCallback6) ||
            riid == __uuidof(ICorProfilerCallback5) ||
            riid == __uuidof(ICorProfilerCallback4) ||
            riid == __uuidof(ICorProfilerCallback3) ||
            riid == __uuidof(ICorProfilerCallback2) ||
            riid == __uuidof(ICorProfilerCallback)  ||
            riid == IID_IUnknown)
        {
            *ppvObject = this;
            this->AddRef();
            return S_OK;
        }

        *ppvObject = nullptr;
        return E_NOINTERFACE;
    }

    ULONG STDMETHODCALLTYPE AddRef(void) override
    {
        return std::atomic_fetch_add(&_refCount, 1) + 1;
    }

    ULONG STDMETHODCALLTYPE Release(void) override
    {
        int count = std::atomic_fetch_sub(&_refCount, 1) - 1;

        if (count <= 0)
        {
            delete this;
        }

        return count;
    }

private:
    // The following are helper methods
    HRESULT DefineEvent();
    HRESULT WriteEvent();
    HRESULT StartSession();

    String GetOrAddProviderName(EVENTPIPE_PROVIDER provider);
    String GetOrAddProviderNameNoLock(EVENTPIPE_PROVIDER provider);
    EventPipeMetadataInstance GetOrAddMetadata(LPCBYTE pMetadata, ULONG cbMetadata);

    template<typename T>
    static void WriteToBuffer(BYTE *pBuffer, size_t bufferLength, size_t *pOffset, T value)
    {
        _ASSERTE(bufferLength >= (*pOffset + sizeof(T)));

        *(T*)(pBuffer + *pOffset) = value;
        *pOffset += sizeof(T);
    }
};
