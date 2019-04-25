// <Snippet4>
#pragma warning( disable : 4290 )
#pragma warning( disable : 4530 )

#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <tchar.h>
#include <initguid.h>
#include <httptrace.h>
#include <httpserv.h>
#include <httpcach.h>

#include <string>
using namespace std;

// The CConvert class mirrors the Convert class that is 
// defined in the .NET Framework. It converts primitives 
// and other data types to wstring types.
class CConvert
{
public:
    // The ToByteString converts a double-byte 
    // character string to a single-byte string.
    // str: the double-byte string to convert.
    // return: a single-byte string copied from str.
    static string ToByteString(const wstring& str)
    {
        // Get the length of the 
        // double-byte string.
        size_t length = str.length();

        // Create a temporary char pointer.
        char* byteChar = new char[length+1];
        byteChar[0] = '\0';
        // Copy the double-byte character string
        // into the single-byte string.        
        size_t charsReturned = 0;
        wcstombs_s(&charsReturned, byteChar, 
                   length+1, str.c_str(), length+1);
        // Create a string to return.
        string retString = byteChar;
        // Delete the temporary string and
        // set that string to NULL.
        delete[] byteChar;
        byteChar = NULL;

        // Return the single-byte string.
        return retString;
    }
};

// The CEventWriter class writes XML 
// documents and strings to the event log.
class CEventWriter
{
public:
    // Creates the CEventWriter class.
    // name: the name of the 
    // event log to open.
    CEventWriter(const wstring& name)
    {                
        #ifdef UNICODE
        m_eventLog = RegisterEventSource(NULL, name.c_str());
        #else
        string multiName = CConvert::ToByteString(name);
        m_eventLog = RegisterEventSource(NULL, multiName.c_str());
        #endif        
    }

    // Creates the destructor for the 
    // CEventWriter class. This destructor
    // closes the HANDLE to the event 
    // log if that HANDLE is open.
    virtual ~CEventWriter()
    {
        // If the HANDLE to the event 
        // log is open, close it.
        if (NULL != m_eventLog)
        {
            // Deregister the event log HANDLE.
            DeregisterEventSource(m_eventLog);
            // Set the HANDLE to NULL.
            m_eventLog = NULL;
        }
    }

    // The ReportInfo method writes 
    // a wstring to the event log.
    // info: the wstring to write.
    // return: true if the event log is written.
    BOOL ReportInfo(const wstring& info)
    {
        return ReportEvent(EVENTLOG_INFORMATION_TYPE, info);
    }

    // The ReportError method writes 
    // a wstring to the event log.
    // error: the wstring to write.
    // return: true if the event log is written.
    BOOL ReportError(const wstring& error)
    {
        return ReportEvent(EVENTLOG_ERROR_TYPE, error);
    }
protected:
    // The ReportEvent method accepts an event type
    // and a wstring, and attempts to write that 
    // event to the event log.
    // type: the type of the event.
    // data: the wstring to write to the event log.
    // return: true if the event log is written;
    // otherwise, false.
    BOOL ReportEvent(WORD type, const wstring& data)
    {
        // If the m_eventLog HANDLE 
        // is NULL, return false.
        if (NULL == m_eventLog)
        {
            return FALSE;
        }

        #ifndef _DEBUG
        // If the current build is not debug,
        // return so the event log is not written.
        return TRUE;
        #endif

        #ifdef UNICODE
        // The unicode version of the ReportEvent
        // method requires double-byte strings.
        PCWSTR arr[1];
        arr[0] = data.c_str();
        return ::ReportEvent(m_eventLog,
                             type,
                             0, 0, NULL, 1, 
                             0, arr, (void*)arr);
        #else
        // The non-unicode version of the ReportEvent
        // method requires single-byte strings.
        string multiByte = 
            CConvert::ToByteString(data);
        LPCSTR arr[1];
        arr[0] = multiByte.c_str();
        return ::ReportEvent(m_eventLog,
                             type,
                             0, 0, NULL, 1,
                             0, arr, (void*)arr);
        #endif
    }
private:
    // Specify the HANDLE to the 
    // event log for writing.
    HANDLE m_eventLog;
};

// Declare and initialize a custom GUID for custom events.
static const GUID s_customGuid = 
    {0xa087707e,0x103,0x4b78, {0xb6,0x73,0xf2,0x59,0x8c,0x14,0x2a,0x68}};

// The CListenTraceModule class subclasses the 
// CGlobalModule class. CListenTraceModule listens
// for trace events for a custom provider, which is 
// implemented by the CRaiseTraceModule class.
class CListenTraceModule : public CGlobalModule
{
public:
    // The RegisterGlobalModule static method creates
    // a new CListenTraceModule pointer, and sets that
    // pointer to listen for trace events.
    // dwServerVersion: the current server version.
    // pModuleInfo: the current 
    // IHttpModuleRegistrationInfo pointer.
    // pGlobalInfo: the current IHttpServer pointer.
    // return: E_INVALIDARG if any of the parameters are
    // NULL; ERROR_NOT_ENOUGH_MEMORY if a new module cannot
    // be created because heap memory is exhausted; otherwise,
    // the value returned from the call to the SetGlobalNotifications
    // metohd.
    static HRESULT RegisterGlobalModule
    (
        DWORD dwServerVersion,
        IHttpModuleRegistrationInfo* pModuleInfo,
        IHttpServer* pGlobalInfo
    )
    {        
        // The IHttpModuleRegistrationInfo and 
        // IHttpServer pointers must not be NULL.
        if ((NULL == pModuleInfo) || (NULL == pGlobalInfo))
        {
            return E_INVALIDARG;
        }

        // Get the IHttpTraceContext pointer 
        // from the IHttpServer pointer.
        IHttpTraceContext* traceContext =
            pGlobalInfo->GetTraceContext();

        // Get the HTTP_MODULE_ID from the 
        // IHttpModuleRegistrationInfo pointer.
        HTTP_MODULE_ID moduleId = 
            pModuleInfo->GetId();

        // The IHttpTraceContext pointer and 
        // HTTP_MODULE_ID both must not be NULL.
        if ((NULL == traceContext) || (NULL == moduleId))
        {
            return E_INVALIDARG;
        }

        // Create a new CGlobalTraceModule pointer
        // using the HTTP_MODULE_ID from the 
        // IHttpModuleRegistrationInfo pointer.
        CListenTraceModule* traceModule = 
            new CListenTraceModule(moduleId);

        // Return an out-of-memory error if the traceModule 
        // is NULL after the call to the new operator.
        if (NULL == traceModule)
        {            
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }    

        // Declare an HTTP_TRACE_CONFIGURATION structure 
        // to pass to the SetTraceConfiguration method.
        HTTP_TRACE_CONFIGURATION traceConfiguration;

        // Set the GUID on the HTTP_TRACE_CONFIGURATION
        // to GUID_IIS_ALL_TRACE_PROVIDERS.
        traceConfiguration.pProviderGuid =                                     
                                    &s_customGuid;
        // Register for all areas.
        traceConfiguration.dwAreas = 0xffffe;
        // Register for the maximum verbosity.
        traceConfiguration.dwVerbosity = 5;
        // Enable the provider.
        traceConfiguration.fProviderEnabled = TRUE;

        // Declare an HRESULT and 
        // initialize it to E_FAIL.
        HRESULT hr = E_FAIL;

        // Set the trace configuration on 
        // the IHttpTraceContext pointer.
        hr =
            traceContext->SetTraceConfiguration(moduleId, &traceConfiguration);

        // If the call to SetTraceConfiguration does 
        // not succeed, return its HRESULT value.
        if (FAILED(hr))
        {
            return hr;
        }

        // Attempt to set global notification 
        // for an GL_TRACE_EVENT event by using 
        // the traceModule as a listener.
        hr = pModuleInfo->SetGlobalNotifications
            (traceModule, GL_TRACE_EVENT);

        // Return the HRESULT from the call to 
        // the SetGlobalNotifications method.
        return hr;
    }
    
    // The OnGlobalTraceEvent method is the callback
    // method for GL_TRACE_EVENT events in the pipeline.
    // This method writes the event to the event log.
    // pProvider: the IGlobalTraceEventProvider pointer.
    // return: GL_NOTIFICATION_CONTINUE.
    virtual 
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalTraceEvent
    (
        IN IGlobalTraceEventProvider*  pProvider
    )
    {
        // If the IGlobalTraceEventProvider 
        // pointer is NULL, return.
        if (NULL == pProvider)
        {
            return GL_NOTIFICATION_CONTINUE;
        }

        // Check if the this module has a 
        // subscription to the event.
        BOOL subscription = 
            pProvider->CheckSubscription(m_moduleId);
        
        // If the module is not subscribed,
        // return GL_NOTIFICATION_CONTINUE.
        if (!subscription)
        {
            return GL_NOTIFICATION_CONTINUE;
        }
        
        // Declare an HTTP_TRACE_EVENT pointer
        // and initialize that pointer to NULL.
        HTTP_TRACE_EVENT* traceEvent = NULL;
        
        // Call the GetTraceEvent method on the
        // IGlobalTraceEventProvider pointer.
        HRESULT hr = pProvider->GetTraceEvent(&traceEvent);

        // If the call to the GetTraceEvent 
        // method succeeds, write the 
        // information to the event log.
        if (SUCCEEDED(hr))
        {            
            wstring name = traceEvent->pszEventName;
            m_writer.ReportInfo(name);
        }
        
        // Return GL_NOTIFICATION_CONTINUE.
        return GL_NOTIFICATION_CONTINUE;        
    }

    // The Terminate method is required for
    // non-abstract CGlobalTraceModule classes.
    // This method calls delete on this.
    virtual VOID Terminate(VOID)
    {
        delete this;
    }
protected:
    // The protected constructor for 
    // the CListenTraceModule class.
    // This constructor initializes the internal 
    // module identifier, and initializes 
    // the writer to the event log.
    // moduleId: the current HTTP_MODULE_ID identifier.
    CListenTraceModule(HTTP_MODULE_ID moduleId) :
         m_moduleId(moduleId),
         m_writer(L"IISADMIN")
    {
        m_moduleId = moduleId;
    }

    // The protected destructor for 
    // the CListenTraceModule class.
    virtual ~CListenTraceModule()
    {

    }

private:
    // Declare the private 
    // HTTP_MODULE_ID identifier.
    HTTP_MODULE_ID m_moduleId;

    // Declare the private CEventWriter
    // for writing to the event log.
    CEventWriter m_writer;
};

// The CRaiseTraceModule extends the CHttpModule 
// class and raises a custom event into the pipeline.
class CRaiseTraceModule : public CHttpModule
{
public:
    // The CRaiseTraceModule method is the public 
    // constructor for the CRaiseTraceModule class.
    CRaiseTraceModule()
    {

    }

    // The CRaiseTraceModule method is the public
    // destructor for the CRaiseTraceModule class.
    virtual ~CRaiseTraceModule()
    {

    }

    // The OnBeginRequest method is called when a request
    // is initiated in the pipeline.  This method creates
    // and sends a custom event into the system.
    // pHttpContext: the current IHttpContext pointer.
    // pProvider: the current IHttpEventProvider pointer.
    // return: RQ_NOTIFICATION_CONTINUE.
    virtual REQUEST_NOTIFICATION_STATUS OnBeginRequest
    (
        IN IHttpContext* pHttpContext,
        IN IHttpEventProvider* pProvider
    )
    {
        // Create a custom HTTP_TRACE_EVENT event.
        HTTP_TRACE_EVENT Event;
        // Set the id of the provider to 
        // the hidden custom static GUID.
        Event.pProviderGuid = &s_customGuid;
        // Initialize the remaining fields 
        // to common default values.
        Event.dwArea =  0x0000;
        Event.pAreaGuid = &s_customGuid;
        Event.dwEvent = 1;
        Event.pszEventName = L"Custom Event";
        Event.dwEventVersion = 1;
        Event.dwVerbosity = 0;
        Event.cEventItems = 0;
        Event.pActivityGuid = NULL;
        Event.pRelatedActivityGuid = NULL;
        Event.dwTimeStamp = 0;
        Event.dwFlags = HTTP_TRACE_EVENT_FLAG_STATIC_DESCRIPTIVE_FIELDS;

        // Get the IHttpTraceContext pointer 
        // from the IHttpContext pointer.
        IHttpTraceContext* traceContext = 
            pHttpContext->GetTraceContext();

        // If the IHttpTraceContext pointer is
        // NULL, return RQ_NOTIFICATION_CONTINUE.
        if (NULL == traceContext)
        {
            return RQ_NOTIFICATION_CONTINUE;
        }

        // Declare an HRESULT an initialize
        // that HRESULT to NULL.
        HRESULT hr = E_FAIL;

        // Call the RaiseTraceEvent method on
        // the IHttpTraceContext pointer.
        hr = traceContext->RaiseTraceEvent(&Event);
        
        // Return RQ_NOTIFICATION_CONTINUE.
        return RQ_NOTIFICATION_CONTINUE;
    }
};

// The CRaiseTraceFactory class implements the 
// IHttpModuleFactory interface and creates a new 
// CAuthenticateModule pointer and registers that 
// pointer for authentication events.
class CRaiseTraceFactory : public IHttpModuleFactory
{
public:
    // The RegisterCHttpModule static method creates 
    // a new CRaiseTraceFactory pointer and sets
    // that pointer on the pModuleInfo as a class factory
    // by using the SetRequestNotifications method.
    // dwServerVersion: the current server version.
    // pModuleInfo: the current IHttpModuleRegistrationInfo 
    // pointer to call the SetRequestNotifications method.
    // pGlobalInfo: the current IHttpServer pointer.
    // return: the value returned from the 
    // SetRequestNotifications call on the pModuleInfo pointer.
    static HRESULT RegisterCHttpModule
    (
        DWORD dwServerVersion,
        IHttpModuleRegistrationInfo* pModuleInfo,
        IHttpServer* pGlobalInfo
    )
    {
        // Create a new CRaiseTraceFactory pointer.    
        CRaiseTraceFactory* raiseTraceFactory =
            new CRaiseTraceFactory;

        // Return an out-of-memory error if the 
        // CRaiseTraceFactory pointer is NULL.
        if (NULL == raiseTraceFactory)
        {
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }        

        // Set the new CRaiseTraceFactory pointer to 
        // receive authentication requests, and the 
        // response to send out a final response.
        HRESULT hr = 
            pModuleInfo->SetRequestNotifications
                (raiseTraceFactory,
                 RQ_BEGIN_REQUEST,
                 0);

        // If the call to the SetRequestNotifications method 
        // failed, return the HRESULT from that call.
        if (FAILED(hr))
        {
            return hr;                
        }

        // Create a listener for the custom event.
        hr = 
            CListenTraceModule::RegisterGlobalModule(dwServerVersion, pModuleInfo, pGlobalInfo);

        // Return the value from the 
        // RegisterGlobalModule method call.
        return hr;
    }
    
    // The GetHttpModule method creates a new 
    // CRaiseTraceModule pointer and assigns this pointer 
    // on the dereferenced CHttpModule pointer.
    // ppModule: the dereferenced CHttpModule pointer 
    // that will be a new CRaiseTraceModule pointer.
    // pAllocator: the optional IModuleAllocator pointer.
    // return: ERROR_NOT_ENOUGH_MEMORY if the heap is 
    // out of memory; otherwise, S_OK.
    virtual
    HRESULT
    GetHttpModule
    (
        OUT CHttpModule** ppModule, 
        IN IModuleAllocator* pAllocator
    )
    {
        // Call the UNREFERENCED_PARAMETER macro 
        // with the IModuleAllocator pointer because 
        // this parameter is never used.
        UNREFERENCED_PARAMETER(pAllocator);

        // Create a new CAuthenticateModule pointer 
        // and set that pointer on the dereferenced 
        // CHttpModule parameter.
        (*ppModule) = new CRaiseTraceModule;

        // If the dereferenced ppModule pointer 
        // is NULL, the heap has been exhausted.
        if (NULL == (*ppModule))
        {
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }

        // Return S_OK.
        return S_OK;
    }

    // The Terminate method calls delete on this.
    virtual 
    void
    Terminate()
    {
        delete this;
    }
protected:
    // The CRaiseTraceFactory method is the protected
    // constructor for the CRaiseTraceFactory class.
    CRaiseTraceFactory()
    {

    }

    // The CRaiseTraceFactory method is 
    // the protected virtual destructor for 
    // the CRaiseTraceFactory class.
    virtual ~CRaiseTraceFactory()
    {

    }
};

// The RegisterModule method is the 
// main entry point for the DLL.
// dwServerVersion: the current server version.
// pModuleInfo: the current 
// IHttpModuleRegistrationInfo pointer.
// pGlobalInfo: the current IHttpServer pointer.
// return: the value returned by calling the 
// CRaiseTraceFactory::RegisterCHttpModule method.
HRESULT
__stdcall
RegisterModule
(
    DWORD dwServerVersion,
    IHttpModuleRegistrationInfo* pModuleInfo,
    IHttpServer* pGlobalInfo
)
{
    return CRaiseTraceFactory::RegisterCHttpModule
        (dwServerVersion, 
         pModuleInfo, 
         pGlobalInfo);    
}
// </Snippet4>