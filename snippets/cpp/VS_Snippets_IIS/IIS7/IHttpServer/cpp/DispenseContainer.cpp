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

// The DispensedContext class is a 
// custom IHttpStoredContext implementer.
class DispensedContext : public IHttpStoredContext
{
public:
    // The public constructor for the 
    // DispensedContext class.
    // Writes to the event log.
    DispensedContext() : m_eventWriter(L"IISADMIN")
    {
        wstring constructor(L"DispensedContext::DispensedContext");
        m_eventWriter.ReportInfo(constructor);
    }

    // Writes trace information to the event log
    // for this method.
    void Display()
    {
        wstring className(L"DispensedContext::Display");
        m_eventWriter.ReportInfo(className);
    }
   
    // The public pure virtual CleanupStoredContext
    // method that all instantiable classes implementing 
    // the IHttpStoredContext must implement.
    // This method calls delete on this.
    virtual VOID CleanupStoredContext(VOID)
    {
        wstring className(L"DispensedContext::CleanupStoredContext");
        m_eventWriter.ReportInfo(className);
        delete this;
    }
protected:
    // The protected virtual desturctor of 
    // the DispensedContext class. Clients 
    // should not call delete directly; rather, 
    // they should call the CleanupStoredContext 
    // method. Writes information to the event log.
    virtual ~DispensedContext()
    {
        wstring constructor(L"DispensedContext::~DispensedContext");
        m_eventWriter.ReportInfo(constructor);
    }
private:
    // Specify the private CEventWriter.
    CEventWriter m_eventWriter;
};

// The CDispensedContainerModule class extends the 
// CGlobalModule class by creating a custom stored 
// container that can be disposed.
class CDispensedContainerModule : public CGlobalModule
{
public:
    // RegisterGlobalModule is the public 
    // static method that creates an instance 
    // of the CDispenseContainerModule class.
    // dwServerVersion: the current server version.
    // pModuleInfo: the current 
    // IHttpModuleRegistrationInfo pointer.
    // pGlobalInfo: the current IHttpServer pointer.
    static HRESULT RegisterGlobalModule
    (
        DWORD dwServerVersion,
        IHttpModuleRegistrationInfo* pModuleInfo,
        IHttpServer* pGlobalInfo
    )
    {        
        // Both the IHttpServer pointer and the
        // IHttpModuleRegistrationInfo pointers 
        // are necessary for further processing.
        if ((NULL == pGlobalInfo) || (NULL == pModuleInfo))
        {
            return E_INVALIDARG;
        }

        // Get the id of the module.
        HTTP_MODULE_ID moduleId = 
            pModuleInfo->GetId();

        // The HTTP_MODULE_ID cannot be NULL because
        // this is needed for calling the SetContext method
        // on an IDispensedHttpModuleContextContainer pointer.
        if (NULL == moduleId)
        {
            return E_INVALIDARG;
        }

        // Create a new CGlobalCacheModule pointer.
        CDispensedContainerModule* containerModule = 
            new CDispensedContainerModule(pGlobalInfo, moduleId);

        // Return an out-of-memory error if the 
        // containerModule is NULL after the 
        // call to the new operator.
        if (NULL == containerModule)
        {            
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }        

        // Attempt to set global notification 
        // for GL_CACHE_OPERATION events
        // by using the traceModule as a listener.
        HRESULT hr = pModuleInfo->SetGlobalNotifications
            (containerModule, GL_CACHE_OPERATION);                            

        // Return the HRESULT from the call to 
        // the SetGlobalNotifications method.
        return hr;
    }    

    // The virtual method that is called 
    // when a cache operation occurs.
    // pProvider: the current ICacheProvider pointer.
    // return: GL_NOTIFICATION_CONTINUE.
    virtual GLOBAL_NOTIFICATION_STATUS OnGlobalCacheOperation
    (
        IN ICacheProvider* pProvider
    )
    {      
        // Get the current IHttpModuleContextContainer.
        IHttpModuleContextContainer* contextContainer =
            this->GetModuleContextContainer();

        // If the IHttpModuleContextContainer pointer
        // is NULL, or the call to create the container
        // or the custom context failed, 
        // return GL_NOTIFICATION_CONTINUE.
        if ((NULL == contextContainer) ||
            (FAILED(m_hr)))
        {
            return GL_NOTIFICATION_CONTINUE;
        }

        // Get the IHttpStoredContext pointer from 
        // the IHttpModuleContextContainer pointer.
        IHttpStoredContext* storedContext =
            contextContainer->GetModuleContext(m_moduleId);

        // Attempt to safely cast this container 
        // to a DispensedContext pointer.
        DispensedContext* dispensedContext =
            dynamic_cast<DispensedContext*>(storedContext);        

        // If the IHttpStoredContext pointer is 
        // safely cast to a DispensedContext, 
        // then ask it to display itself.
        if (NULL != dispensedContext)
        {
            dispensedContext->Display();
        }

        // Return GL_NOTIFICATION_CONTINUE.
        return GL_NOTIFICATION_CONTINUE;
    }
    
    // The GetModuleContextContainer method returns the internal
    // IDispensedHttpModuleContextContainer pointer that is returned
    // from the call into the IHttpServer pointer.
    virtual IHttpModuleContextContainer* GetModuleContextContainer(VOID)
    {
        return m_dispensedContainer;        
    }

    // public pure virtual method that all 
    // CGlobalModule non-abstract classes must
    // implement. This method calls delete on this.
    virtual VOID Terminate(VOID)
    {
        delete this;
    }    
protected:    
    // The protected constructor for the
    // CDispensedContainerModule class.
    // pGlobalInfo: the IHttpServer pointer to use
    // for retrieving an IDispensedHttpModuleContextContainer
    // pointer by calling the DispenseContainer method.
    // moduleId: the current HTTP_MODULE_ID for setting the
    // current module context with.
    CDispensedContainerModule
    (
        IHttpServer* pGlobalInfo,
        HTTP_MODULE_ID moduleId
    ) 
        : m_moduleId(moduleId)
    {
        // Initialize the internal HRESULT to E_FAIL.
        m_hr = E_FAIL;

        // Get a container that is dispensed 
        // from the IHttpServer pointer. By default, the
        // IHttpServer implementer creates a new container
        // that is both dispensed and synchronized.
        m_dispensedContainer = 
            pGlobalInfo->DispenseContainer();

        // Test for NULL on the return value from
        // the DispenseContainer method call.
        if (NULL != m_dispensedContainer)
        {
            // Create a new DispensedContext instance
            DispensedContext* context = 
                new DispensedContext;

            // If the call to the new operator 
            // returns NULL, set the HRESULT 
            // to an out-of-memory error.
            if (NULL == context)
            {
                m_hr = HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
            }
            else
            {
                // Set the custom module context 
                // on the dispensed container.
                m_hr =
                    m_dispensedContainer->SetModuleContext(context, m_moduleId);

                // If the SetModuleContext method call fails,
                // the client of the IHttpStoredContext pointer
                // is responsible for cleanup on this data.
                if (FAILED(m_hr))
                {
                    context->CleanupStoredContext();
                }        
            }
        }
    }

    // The protected destructor for the 
    // CDispensedContainerModule class.
    // This method calls the Dispose method.
    virtual ~CDispensedContainerModule()
    {
        Dispose();
    }

    // A protected Dispose method. This method
    // calls ReleaseContainer on the internal 
    // IDispensedHttpModuleContextContainer pointer
    // and then sets that pointer to NULL.
    void Dispose()
    {
        // If creating the container and the
        // custom IHttpStoredContext was successful,
        // call the ReleaseContainer on the
        // IDispensedHttpModuleContextContainer pointer.
        if (SUCCEEDED(m_hr))
        {
            // Call release on the container.
            m_dispensedContainer->ReleaseContainer();

            // Set the container to NULL.
            m_dispensedContainer = NULL;            
        }
    }

private:
    // Specify the IDispensedHttpModuleContextContainer 
    // that will be returned when callers call the 
    // GetModuleContextContainer method.
    IDispensedHttpModuleContextContainer* m_dispensedContainer;

    // Specify the HTTP_MODULE_ID pointer.
    HTTP_MODULE_ID m_moduleId;

    // Specify an HRESULT for the values return from attempting 
    // to create an IDispensedHttpModuleContextContainer pointer.
    HRESULT m_hr;
};

// The RegisterModule method is the 
// main entry point for the DLL.
// dwServerVersion: the current server version.
// pModuleInfo: the current 
// IHttpModuleRegistrationInfo pointer.
// pGlobalInfo: the current IHttpServer pointer.
// return: the value returned by calling the
// CDispenseContainerModule::RegisterGlobalModule
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
    return CDispensedContainerModule::RegisterGlobalModule            
        (dwServerVersion, 
         pModuleInfo, 
         pGlobalInfo);             
}
// </Snippet5>