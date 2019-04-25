// <Snippet1>
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

// The CStoredContext class implements 
// the IHttpStoredContext interface.
class CStoredContext : public IHttpStoredContext
{
public:
    // The constructor for the CStoredContext 
    // class. Initializes a CEventWriter to write
    // to the Event Log.
    CStoredContext() : m_eventWriter(L"IISADMIN")
    {
        // Create a string to write.
        wstring constructor(L"CStoredContext::Constructor");
        // Write the string to the Event Log.
        m_eventWriter.ReportInfo(constructor);
    }

    // The Display method writes 
    // L"CStoredContext::Display"
    // to the Event Log.
    virtual void Display()
    {
        // Create the string to write.
        wstring display(L"CStoredContext::Display");
        // Write the string to the Event Log.
        m_eventWriter.ReportInfo(display);
    }

    // The CleanupStoredContext is the pure virtual
    // method that all non-abstract classes implementing 
    // the IHttpStoredContext must implement.
    virtual void CleanupStoredContext()
    {        
        wstring cleanup(L"CStoredContext::CleanupStoredContext");
        m_eventWriter.ReportInfo(cleanup);
        delete this;
    }
protected:
    // The protected destructor for the 
    // CStoredContext class. This method is 
    // protected because the clients of this 
    // class should dispose of a class instance
    // by calling the CleanupStoredContext method.
    virtual ~CStoredContext()
    {
        wstring destructor(L"CStoredContext::Destructor");
        m_eventWriter.ReportInfo(destructor);
    }
private:    
    // Specify the private CEventWriter
    // for writing events.
    CEventWriter m_eventWriter;
};

// The CGlobalTraceModule class creates the CGlobalModule 
// class and registers for GL_TRACE_EVENT events.
class CGlobalContainerModule : public CGlobalModule
{
public:
    // Creates the destructor for the 
    // CGlobalTraceModule class.
    virtual ~CGlobalContainerModule()
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
        // The IHttpModuleRegistrationInfo 
        // pointermust not be NULL.
        if (NULL == pModuleInfo)
        {
            return E_INVALIDARG;
        }

        // Get the HTTP_MODULE_ID from the 
        // IHttpModuleRegistrationInfo pointer.
        HTTP_MODULE_ID moduleId = 
            pModuleInfo->GetId();

        // The HTTP_MODULE_ID pointer 
        // must not be NULL.
        if (NULL == moduleId)
        {
            return E_INVALIDARG;
        }

        // Create a new CGlobalContainerModule pointer
        // using the HTTP_MODULE_ID from the 
        // IHttpModuleRegistrationInfo pointer.
        CGlobalContainerModule* containerModule = 
            new CGlobalContainerModule(moduleId);

        // Return an out-of-memory error if the containerModule 
        // is NULL after the call to the new operator.
        if (NULL == containerModule)
        {            
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }                                          

        // Attempt to set global notification 
        // for an GL_TRACE_EVENT event by using 
        // the traceModule as a listener.
        HRESULT hr = pModuleInfo->SetGlobalNotifications
            (containerModule, GL_TRACE_EVENT);

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
        IN IGlobalTraceEventProvider* pProvider
    )
    {
        // If the IGlobalTraceEventProvider pointer 
        // is NULL, return GL_NOTIFICATION_CONTINUE.
        if (NULL == pProvider)
        {
            return GL_NOTIFICATION_CONTINUE;
        }

        // Declare an IHttpContext pointer.
        IHttpContext* httpContext = NULL;

        // Declare an HRESULT and initialize
        // the HRESULT to E_FAIL.
        HRESULT hr = E_FAIL;

        // Call the GetCurrentHttpRequestContext
        // method on the IGlobalTraceEventProvider
        // pointer.
        hr = pProvider->GetCurrentHttpRequestContext(&httpContext);

        // If the GetCurrentHttpRequestContext 
        // method failed, or the IHttpContext
        // pointer is NULL, return GL_NOTIFICATION_CONTINUE.
        if (FAILED(hr) || (NULL == httpContext))
        {
            return GL_NOTIFICATION_CONTINUE;
        }

        // Get the IHttpModuleContextContainer
        // pointer from the IHttpContext pointer.
        IHttpModuleContextContainer* container =
            httpContext->GetModuleContextContainer();

        // If the IHttpModuleContextContainer is 
        // NULL, return GL_NOTIFICATION_CONTINUE.
        if (NULL == container)
        {
            return GL_NOTIFICATION_CONTINUE;
        }

        // Get the IHttpStoredContext pointer 
        // from the IHttpModuleContextContainer
        // pointer.
        IHttpStoredContext* storedContext =
            container->GetModuleContext(m_moduleId);

        // If the IHttpStoredContext pointer is
        // non-NULL, use the dynamic_cast operator
        // to retrieve the CStoredContext pointer
        // from the storedContext.
        if (NULL != storedContext)
        {                
            // Attempt to cast the IHttpStoredContext
            // pointer to a CStoredContext pointer.
            CStoredContext* customContext =
                dynamic_cast<CStoredContext*>(storedContext);

            // If the cast does not return 
            // NULL, ask the CStoredContext 
            // poitner to display itself.
            if (NULL != customContext)
            {
                customContext->Display();
            }
        }
        if (NULL == storedContext)
        {
            // Create a custom CStoredContext pointer.
            IHttpStoredContext* customContext =
                new CStoredContext;
            // Call the SetModuleContext method using 
            // the IHttpModuleContextContainer pointer.
            container->SetModuleContext(customContext, m_moduleId);
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
    // Creates the constructor for the CGlobalTraceModule 
    // class. This constructor initializes the CEventWriter
    // to write to the application event log.
    // moduleId: the current module identifier.
    CGlobalContainerModule(HTTP_MODULE_ID moduleId)        
    {
        m_moduleId = moduleId;
    }
private:
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
// CGlobalContainerModule::RegisterGlobalModule
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
    return CGlobalContainerModule::RegisterGlobalModule            
        (dwServerVersion, 
         pModuleInfo, 
         pGlobalInfo);             
}
// </Snippet1>