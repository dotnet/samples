// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create the module class.
class MyHttpModule : public CHttpModule
{
public:

    // --------------- RQ_BEGIN_REQUEST ---------------
    REQUEST_NOTIFICATION_STATUS
    OnBeginRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnBeginRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostBeginRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostBeginRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_AUTHENTICATE_REQUEST ---------------
    REQUEST_NOTIFICATION_STATUS
    OnAuthenticateRequest(
        IN IHttpContext * pHttpContext,
        IN IAuthenticationProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnAuthenticateRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostAuthenticateRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostAuthenticateRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_AUTHORIZE_REQUEST ---------------
    REQUEST_NOTIFICATION_STATUS
    OnAuthorizeRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnAuthorizeRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostAuthorizeRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostAuthorizeRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_RESOLVE_REQUEST_CACHE ---------------
    REQUEST_NOTIFICATION_STATUS
    OnResolveRequestCache(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnResolveRequestCache");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostResolveRequestCache(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostResolveRequestCache");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_MAP_REQUEST_HANDLER ---------------
    REQUEST_NOTIFICATION_STATUS
    OnMapRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IMapHandlerProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnMapRequestHandler");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostMapRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostMapRequestHandler");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_ACQUIRE_REQUEST_STATE ---------------
    REQUEST_NOTIFICATION_STATUS
    OnAcquireRequestState(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnAcquireRequestState");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostAcquireRequestState(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostAcquireRequestState");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_PRE_EXECUTE_REQUEST_HANDLER ---------------
    REQUEST_NOTIFICATION_STATUS
    OnPreExecuteRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPreExecuteRequestHandler");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostPreExecuteRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostPreExecuteRequestHandler");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_EXECUTE_REQUEST_HANDLER ---------------
    REQUEST_NOTIFICATION_STATUS
    OnExecuteRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnExecuteRequestHandler");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostExecuteRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostExecuteRequestHandler");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_RELEASE_REQUEST_STATE ---------------
    REQUEST_NOTIFICATION_STATUS
    OnReleaseRequestState(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnReleaseRequestState");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostReleaseRequestState(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostReleaseRequestState");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_UPDATE_REQUEST_CACHE ---------------
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
    // --------------- RQ_LOG_REQUEST ---------------
    REQUEST_NOTIFICATION_STATUS
    OnLogRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnLogRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostLogRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostLogRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_END_REQUEST ---------------
    REQUEST_NOTIFICATION_STATUS
    OnEndRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnEndRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostEndRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostEndRequest");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_SEND_RESPONSE ---------------
    REQUEST_NOTIFICATION_STATUS
    OnSendResponse(
        IN IHttpContext * pHttpContext,
        IN ISendResponseProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnSendResponse");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostSendResponse(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostSendResponse");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- RQ_MAP_PATH ---------------
    REQUEST_NOTIFICATION_STATUS
    OnMapPath(
        IN IHttpContext * pHttpContext,
        IN IMapPathProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnMapPath");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostMapPath(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostMapPath");
        return RQ_NOTIFICATION_CONTINUE;
    } 
    // --------------- RQ_READ_ENTITY ---------------
    REQUEST_NOTIFICATION_STATUS
    OnReadEntity(
        IN IHttpContext * pHttpContext,
        IN IReadEntityProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnReadEntity");
        return RQ_NOTIFICATION_CONTINUE;
    }
    REQUEST_NOTIFICATION_STATUS
    OnPostReadEntity(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnPostReadEntity");
        return RQ_NOTIFICATION_CONTINUE;
    } 
    // --------------- RQ_CUSTOM_NOTIFICATION ---------------
    REQUEST_NOTIFICATION_STATUS
    OnCustomRequestNotification(
        IN IHttpContext * pHttpContext,
        IN ICustomNotificationProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnCustomRequestNotification");
        return RQ_NOTIFICATION_CONTINUE;
    }
    // --------------- Asynchronous Completeion ---------------
    REQUEST_NOTIFICATION_STATUS
    OnAsyncCompletion(
        IN IHttpContext * pHttpContext,
        IN DWORD dwNotification,
        IN BOOL fPostNotification,
        IN IHttpEventProvider * pProvider,
        IN IHttpCompletionInfo * pCompletionInfo 
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( dwNotification );
        UNREFERENCED_PARAMETER( fPostNotification );
        UNREFERENCED_PARAMETER( pProvider );
        UNREFERENCED_PARAMETER( pCompletionInfo );
        WriteEventViewerLog("OnAsyncCompletion");
        return RQ_NOTIFICATION_CONTINUE;
    }

    MyHttpModule()
    {
        // Open a handle to the Event Viewer.
        m_hEventLog = RegisterEventSource( NULL,"IISADMIN" );
        if (NULL != m_hEventLog)
        {
            WriteEventViewerLog("MyHttpModule");
        }
    }

    ~MyHttpModule()
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != m_hEventLog)
        {
            WriteEventViewerLog("~MyHttpModule");
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
        RQ_BEGIN_REQUEST |
            RQ_AUTHENTICATE_REQUEST |
            RQ_AUTHORIZE_REQUEST |
            RQ_RESOLVE_REQUEST_CACHE |
            RQ_MAP_REQUEST_HANDLER |
            RQ_ACQUIRE_REQUEST_STATE |
            RQ_PRE_EXECUTE_REQUEST_HANDLER |
            RQ_EXECUTE_REQUEST_HANDLER |
            RQ_RELEASE_REQUEST_STATE |
            RQ_UPDATE_REQUEST_CACHE |
            RQ_LOG_REQUEST |
            RQ_END_REQUEST |
            RQ_CUSTOM_NOTIFICATION |
            RQ_SEND_RESPONSE |
            RQ_READ_ENTITY |
            RQ_MAP_PATH,
        // Specify the post-event notifications.
        RQ_BEGIN_REQUEST |
            RQ_AUTHENTICATE_REQUEST |
            RQ_AUTHORIZE_REQUEST |
            RQ_RESOLVE_REQUEST_CACHE |
            RQ_MAP_REQUEST_HANDLER |
            RQ_ACQUIRE_REQUEST_STATE |
            RQ_PRE_EXECUTE_REQUEST_HANDLER |
            RQ_EXECUTE_REQUEST_HANDLER |
            RQ_RELEASE_REQUEST_STATE |
            RQ_UPDATE_REQUEST_CACHE |
            RQ_LOG_REQUEST |
            RQ_END_REQUEST |
            RQ_CUSTOM_NOTIFICATION |
            RQ_SEND_RESPONSE |
            RQ_READ_ENTITY |
            RQ_MAP_PATH
    );
}
// </Snippet1>