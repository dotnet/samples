// <Snippet2>
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
// </Snippet2>