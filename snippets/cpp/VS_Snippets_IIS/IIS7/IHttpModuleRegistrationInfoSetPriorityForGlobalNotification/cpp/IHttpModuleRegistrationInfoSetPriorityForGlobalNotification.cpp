// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create a global handle for the Event Viewer.
HANDLE g_hEventLog;

// Define the method that writes to the Event Viewer.
BOOL WriteEventViewerLog(LPCSTR szBuffer[], WORD wNumStrings);

// Create the HTTP module class.
class MyHttpModule : public CHttpModule
{
public:
    REQUEST_NOTIFICATION_STATUS
    OnBeginRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );

        // Create an array of strings.
        LPCSTR szBuffer[2] = {"MyHttpModule","OnBeginRequest"};
        // Write the strings to the Event Viewer.
        WriteEventViewerLog(szBuffer,2);

        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }
};

// Create the module's global class.
class MyGlobalModule : public CGlobalModule
{
public:
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalPreBeginRequest(
        IN IPreBeginRequestProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        
        // Create an array of strings.
        LPCSTR szBuffer[2] = {"MyGlobalModule","OnGlobalPreBeginRequest"};
        // Write the strings to the Event Viewer.
        WriteEventViewerLog(szBuffer,2);

        // Return processing to the pipeline.
        return GL_NOTIFICATION_CONTINUE;
    }

    VOID Terminate()
    {
        // Remove the class from memory.
        delete this;
    }

    MyGlobalModule()
    {
        // Open a handle to the Event Viewer.
        g_hEventLog = RegisterEventSource( NULL,"IISADMIN" );
    }

    ~MyGlobalModule()
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != g_hEventLog)
        {
            DeregisterEventSource( g_hEventLog );
            g_hEventLog = NULL;
        }
    }
};

// Create the module's class factory.
class MyHttpModuleFactory : public IHttpModuleFactory
{
public:
    HRESULT
    GetHttpModule(
        OUT CHttpModule ** ppModule, 
        IN IModuleAllocator * pAllocator
    )
    {
        UNREFERENCED_PARAMETER( pAllocator );

        // Create a new instance.
        MyHttpModule * pModule = new MyHttpModule;

        // Test for an error.
        if (!pModule)
        {
            // Return an error if the factory cannot create the instance.
            return HRESULT_FROM_WIN32( ERROR_NOT_ENOUGH_MEMORY );
        }
        else
        {
            // Return a pointer to the module.
            *ppModule = pModule;
            pModule = NULL;
            // Return a success status.
            return S_OK;
        }            
    }

    void Terminate()
    {
        // Remove the class from memory.
        delete this;
    }
};

// Define a method that writes to the Event Viewer.
BOOL WriteEventViewerLog(LPCSTR szBuffer[], WORD wNumStrings)
{
    // Test whether the handle for the Event Viewer is open.
    if (NULL != g_hEventLog)
    {
        // Write any strings to the Event Viewer and return.
        return ReportEvent(
            g_hEventLog,
            EVENTLOG_INFORMATION_TYPE,
            0, 0, NULL, wNumStrings,
            0, szBuffer, NULL );
    }
    return FALSE;
}

// Create the module's exported registration function.
HRESULT
__stdcall
RegisterModule(
    DWORD dwServerVersion,
    IHttpModuleRegistrationInfo * pModuleInfo,
    IHttpServer * pGlobalInfo
)
{
    UNREFERENCED_PARAMETER( dwServerVersion );
    UNREFERENCED_PARAMETER( pGlobalInfo );

    // Create an HRESULT to receive return values from methods.
    HRESULT hr;

    // Set the request notifications.
    hr = pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_BEGIN_REQUEST, 0 );

    // Test for an error and exit if necessary.
    if (FAILED(hr))
    {
        return hr;
    }

    // Set the request priority.
    hr = pModuleInfo->SetPriorityForRequestNotification(
        RQ_BEGIN_REQUEST,PRIORITY_ALIAS_MEDIUM);

    // Test for an error and exit if necessary.
    if (FAILED(hr))
    {
        return hr;
    }

    // Create an instance of the global module class.
    MyGlobalModule * pGlobalModule = new MyGlobalModule;
 
    // Test for an error.
    if (NULL == pGlobalModule)
    {
        return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
    }
 
    // Set the global notifications.
    hr = pModuleInfo->SetGlobalNotifications(
        pGlobalModule, GL_PRE_BEGIN_REQUEST );

    // Test for an error and exit if necessary.
    if (FAILED(hr))
    {
        return hr;
    }

    // Set the global priority.
    hr = pModuleInfo->SetPriorityForGlobalNotification(
        GL_PRE_BEGIN_REQUEST,PRIORITY_ALIAS_LOW);

    // Test for an error and exit if necessary.
    if (FAILED(hr))
    {
        return hr;
    }

    // Return a success status;
    return S_OK;
}
// </Snippet1>