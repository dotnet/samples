// <Snippet5>
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
private:
    // Specify the HANDLE to the 
    // event log for writing.
    HANDLE m_eventLog;
};

// The CGlobalTraceModule class creates the CGlobalModule 
// class and registers for GL_TRACE_EVENT events.
class CGlobalTraceModule : public CGlobalModule
{
public:
    // Creates the destructor for the 
    // CGlobalTraceModule class.
    virtual ~CGlobalTraceModule()
    {

    }

    // The RegisterGlobalModule method creates and registers 
    // a new CGlobalTraceModule for GL_TRACE_EVENT events.
    // dwServerVersion: the current server version.
    // pModuleInfo: the current IHttpModuleRegistrationInfo pointer.
    // pGlobalInfo: the current IHttpServer pointer.
    // return: ERROR_NOT_ENOUGH_MEMORY if the heap is out of 
    // memory; otherwise, the value from the call to the 
    // SetGlobalNotifications method on the pModuleInfo pointer.
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
        CGlobalTraceModule* traceModule = 
            new CGlobalTraceModule(moduleId);

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
                                    &GUID_IIS_ALL_TRACE_PROVIDERS;
        // Register for all areas.
        traceConfiguration.dwAreas = 0xffffe;
        // Register for the maximum verbosity.
        traceConfiguration.dwVerbosity = 5;
        // Enable the provider.
        traceConfiguration.fProviderEnabled = TRUE;

        // Set the trace configuration on 
        // the IHttpTraceContext pointer.
        traceContext->SetTraceConfiguration(moduleId, &traceConfiguration);                                                  

        // Attempt to set global notification 
        // for an GL_TRACE_EVENT event by using 
        // the traceModule as a listener.
        HRESULT hr = pModuleInfo->SetGlobalNotifications
            (traceModule, GL_TRACE_EVENT);

        // Return the HRESULT from the call to 
        // the SetGlobalNotifications method.
        return hr;
    }

    // The OnGlobalTraceEvent method is the callback
    // method for GL_TRACE_EVENT events in the pipeline.
    // pProvider: the IGlobalTraceEventProvider pointer.
    // return: GL_NOTIFICATION_CONTINUE.
    virtual 
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalTraceEvent
    (
        IN IGlobalTraceEventProvider*  pProvider
    )
    {
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
    // Creates the constructor for the CGlobalTraceModule 
    // class. This constructor initializes the CEventWriter
    // to write to the application event log.
    // moduleId: the current module identifier.
    CGlobalTraceModule(HTTP_MODULE_ID moduleId): 
         m_eventWriter(L"IISADMIN")
    {
        m_moduleId = moduleId;
    }
private:
    // Specify the private CEventWriter
    // for writing events.
    CEventWriter m_eventWriter;

    // Specify the HTTP_MODULE_ID
    // for this module.
    HTTP_MODULE_ID m_moduleId;
};

// The RegisterModule method is the 
// main entry point for the DLL.
// dwServerVersion: the current server version.
// pModuleInfo: the current 
// IHttpModuleRegistrationInfo pointer.
// pGlobalInfo: the current IHttpServer pointer.
// return: the value returned by calling the
// CGlobalCacheModule::RegisterGlobalModule
// method.
HRESULT
__stdcall
RegisterModule(
    DWORD dwServerVersion,
    IHttpModuleRegistrationInfo* pModuleInfo,
    IHttpServer* pGlobalInfo
)
{        
    // Call the static method for initialization.
    return CGlobalTraceModule::RegisterGlobalModule            
        (dwServerVersion, 
         pModuleInfo, 
         pGlobalInfo);             
}
// </Snippet5>