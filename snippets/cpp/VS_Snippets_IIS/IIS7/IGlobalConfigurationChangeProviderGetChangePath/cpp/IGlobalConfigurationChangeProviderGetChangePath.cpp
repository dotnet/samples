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
    OnGlobalConfigurationChange(
        IN IGlobalConfigurationChangeProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        
        // Test for an error.
        if (NULL != m_pHttpContext)
        {
            // Retrieve the path that has changed.
            PCWSTR pwszChangePath = NULL;
            pwszChangePath = pProvider->GetChangePath();
            // Test for an error.
            if (NULL != pwszChangePath)
            {
                // Allocate storage for the path.
                char * pszChangePath =
                    (char *) m_pHttpContext->AllocateRequestMemory(
                    (DWORD) wcslen(pwszChangePath)+1 );
                // Test for an error.
                if (NULL != pszChangePath)
                {
                    // Convert the path.
                    wcstombs(pszChangePath,pwszChangePath,
                        wcslen(pwszChangePath));
                    // Create an array of strings.
                    LPCSTR szBuffer[2] = {"Path Changed"};
                    // Store the path.
                    szBuffer[1] = pszChangePath;
                    // Write the strings to the Event Viewer.
                    WriteEventViewerLog(szBuffer,2);
                }
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
        MyGlobalModule::m_hEventLog = RegisterEventSource( NULL,"IISADMIN" );
        // Initialize the context pointer to NULL.
        MyGlobalModule::m_pHttpContext = NULL;
    }

    ~MyGlobalModule()
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != MyGlobalModule::m_hEventLog)
        {
            DeregisterEventSource( MyGlobalModule::m_hEventLog );
            MyGlobalModule::m_hEventLog = NULL;
            MyGlobalModule::m_pHttpContext = NULL;
        }
    }

private:

    // Create a handle for the event viewer.
    HANDLE m_hEventLog;
    // Create a pointer for the module context.
    IHttpContext * m_pHttpContext;

    // Define a method that writes to the Event Viewer.
    BOOL WriteEventViewerLog(LPCSTR szBuffer[], WORD wNumStrings)
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != MyGlobalModule::m_hEventLog)
        {
            // Write any strings to the Event Viewer and return.
            return ReportEvent(
                MyGlobalModule::m_hEventLog,
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
        pGlobalModule,
        GL_CONFIGURATION_CHANGE | GL_PRE_BEGIN_REQUEST );
}
// </Snippet1>