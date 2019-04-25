// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create the module's global class.
class MyGlobalModule : public CGlobalModule
{
public:
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalApplicationResolveModules(
        IN IHttpApplicationResolveModulesProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        
        // Create an array of strings.
        LPCSTR szBuffer[2] = {"MyGlobalModule","OnGlobalApplicationResolveModules"};
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
        m_hEventLog = RegisterEventSource( NULL,"IISADMIN" );
    }

    ~MyGlobalModule()
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
    BOOL WriteEventViewerLog(LPCSTR szBuffer[], WORD wNumStrings)
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != m_hEventLog)
        {
            // Write any strings to the Event Viewer and return.
            return ReportEvent(
                m_hEventLog,
                EVENTLOG_INFORMATION_TYPE,
                0, 0, NULL, wNumStrings,
                0, szBuffer, NULL );
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
        pGlobalModule, GL_APPLICATION_RESOLVE_MODULES );
}
// </Snippet1>