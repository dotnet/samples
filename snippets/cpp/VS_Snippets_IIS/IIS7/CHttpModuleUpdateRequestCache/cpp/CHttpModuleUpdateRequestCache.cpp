// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create the module class.
class MyHttpModule : public CHttpModule
{
public:

    // Process an RQ_UPDATE_REQUEST_CACHE event.
    REQUEST_NOTIFICATION_STATUS
    OnUpdateRequestCache(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnUpdateRequestCache");
        return RQ_NOTIFICATION_CONTINUE;
    }

    // Process an RQ_UPDATE_REQUEST_CACHE post-event.
    REQUEST_NOTIFICATION_STATUS
    OnPostUpdateRequestCache(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostUpdateRequestCache");
        return RQ_NOTIFICATION_CONTINUE;
    }

    MyHttpModule()
    {
        // Open a handle to the Event Viewer.
        m_hEventLog = RegisterEventSource( NULL,"IISADMIN" );
    }

    ~MyHttpModule()
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != m_hEventLog)
        {
            // Close the handle to the Event Viewer.
            DeregisterEventSource( m_hEventLog );
            m_hEventLog = NULL;
        }
    }

private:

    // Create a handle for the event viewer.
    HANDLE m_hEventLog;

    // Define a method that writes to the Event Viewer.
    BOOL WriteEventViewerLog(LPCSTR szNotification)
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != m_hEventLog)
        {
            // Write any strings to the Event Viewer and return.
            return ReportEvent(
                m_hEventLog,
                EVENTLOG_INFORMATION_TYPE, 0, 0,
                NULL, 1, 0, &szNotification, NULL );
        }
        return FALSE;
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

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        // Specify the class factory.
        new MyHttpModuleFactory,
        // Specify the event notifications.
        RQ_UPDATE_REQUEST_CACHE,
        // Specify the post-event notifications.
        RQ_UPDATE_REQUEST_CACHE
    );
}
// </Snippet1>