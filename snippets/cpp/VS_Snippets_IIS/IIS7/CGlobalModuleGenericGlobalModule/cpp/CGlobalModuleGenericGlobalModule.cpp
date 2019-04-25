// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create the module's global class.
class MyGlobalModule : public CGlobalModule
{
public:
    // --------------- GL_STOP_LISTENING ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalStopListening(
        IN IGlobalStopListeningProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalStopListening");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_CACHE_CLEANUP ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalCacheCleanup(
        VOID
    )
    {
        WriteEventViewerLog("OnGlobalCacheCleanup");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_CACHE_OPERATION ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalCacheOperation(
        IN ICacheProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalCacheOperation");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_HEALTH_CHECK ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalHealthCheck(
        VOID
    )
    {
        WriteEventViewerLog("OnGlobalHealthCheck");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_CONFIGURATION_CHANGE ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalConfigurationChange(
        IN IGlobalConfigurationChangeProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalConfigurationChange");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_FILE_CHANGE ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalFileChange(
        IN IGlobalFileChangeProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalFileChange");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_PRE_BEGIN_REQUEST ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalPreBeginRequest(
        IN IPreBeginRequestProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalPreBeginRequest");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_APPLICATION_START ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalApplicationStart(
        IN IHttpApplicationStartProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalApplicationStart");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_APPLICATION_RESOLVE_MODULES ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalApplicationResolveModules(
        IN IHttpApplicationResolveModulesProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalApplicationResolveModules");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_APPLICATION_STOP ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalApplicationStop(
        IN IHttpApplicationStopProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalApplicationStop");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_RSCA_QUERY ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalRSCAQuery(
        IN IGlobalRSCAQueryProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalRSCAQuery");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_TRACE_EVENT ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalTraceEvent(
        IN IGlobalTraceEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalTraceEvent");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_CUSTOM_NOTIFICATION ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalCustomNotification(
        IN ICustomNotificationProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalCustomNotification");
        return GL_NOTIFICATION_CONTINUE;
    }
    // --------------- GL_THREAD_CLEANUP ---------------
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalThreadCleanup(
        IN IGlobalThreadCleanupProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        WriteEventViewerLog("OnGlobalThreadCleanup");
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
        m_hEventLog = RegisterEventSource( NULL,"IISADMIN" );
        if (NULL != m_hEventLog)
        {
            WriteEventViewerLog("MyGlobalModule");
        }
    }

    ~MyGlobalModule()
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != m_hEventLog)
        {
            WriteEventViewerLog("~MyGlobalModule");
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

    // Create an instance of the global module class.
    MyGlobalModule * pGlobalModule = new MyGlobalModule;
    // Test for an error.
    if (NULL == pGlobalModule)
    {
        return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
    }
    // Set the global notifications and exit.
    return pModuleInfo->SetGlobalNotifications(
        pGlobalModule,
        GL_STOP_LISTENING |
            GL_CACHE_CLEANUP |
            GL_CACHE_OPERATION |
            GL_HEALTH_CHECK |
            GL_CONFIGURATION_CHANGE |
            GL_FILE_CHANGE |
            GL_PRE_BEGIN_REQUEST |
            GL_APPLICATION_START |
            GL_APPLICATION_RESOLVE_MODULES |
            GL_APPLICATION_STOP |
            GL_RSCA_QUERY |
            GL_TRACE_EVENT |
            GL_CUSTOM_NOTIFICATION |
            GL_THREAD_CLEANUP
        );
}
// </Snippet1>