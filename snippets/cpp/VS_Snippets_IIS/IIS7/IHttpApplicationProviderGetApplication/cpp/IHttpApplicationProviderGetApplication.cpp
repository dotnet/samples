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
    OnGlobalApplicationStart(
        IN IHttpApplicationStartProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        
        // Retrieve a pointer to the IHttpApplication class.
        IHttpApplication * pHttpApplication =
            pProvider->GetApplication();

        // Retrieve a pointer to the application configuration path.
        PCWSTR pwszPhysicalPath =
            pHttpApplication->GetApplicationPhysicalPath();

        // Test for an error.
        if (m_pHttpContext!=NULL)
        {
            // Allocate space for the user name.
            char * pszBuffer = (char*) m_pHttpContext->AllocateRequestMemory(
                (DWORD) wcslen(pwszPhysicalPath)+1 );
        
            // Test for an error.
            if (pszBuffer!=NULL)
            {
                // Return the user information to the Web client.
                wcstombs(pszBuffer,pwszPhysicalPath,wcslen(pwszPhysicalPath));
                // Create an array of strings.
                LPCSTR szBuffer[3] = {"OnGlobalApplicationStart",
                    "Application Physical Path:",pszBuffer};
                // Write the strings to the Event Viewer.
                WriteEventViewerLog(szBuffer,3);
            }
        }

        // Return processing to the pipeline.
        return GL_NOTIFICATION_CONTINUE;
    }

    GLOBAL_NOTIFICATION_STATUS
    OnGlobalPreBeginRequest(
        IN IPreBeginRequestProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        // Retrieve a pointer to the context.
        m_pHttpContext = pProvider->GetHttpContext();
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
        // Initialize the context pointer to NULL.
        m_pHttpContext = NULL;
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
    // Create a pointer for the module context.
    IHttpContext * m_pHttpContext;

    // Define a method that writes to the Event Viewer.
    BOOL WriteEventViewerLog(LPCSTR * lpStrings, WORD wNumStrings)
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != m_hEventLog)
        {
            // Write any strings to the Event Viewer and return.
            return ReportEvent(
                m_hEventLog, EVENTLOG_INFORMATION_TYPE,
                0, 0, NULL, wNumStrings, 0, lpStrings, NULL );
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
        pGlobalModule, GL_APPLICATION_START | GL_PRE_BEGIN_REQUEST );
}
// </Snippet1>