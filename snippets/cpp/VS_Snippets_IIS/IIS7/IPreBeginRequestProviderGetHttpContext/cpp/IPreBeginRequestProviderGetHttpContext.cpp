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
    OnGlobalPreBeginRequest(
        IN IPreBeginRequestProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );

        // Retrieve a pointer to the IHttpContext for the request.
        IHttpContext * pHttpContext = pProvider->GetHttpContext();
        // Test for an error.
        if (NULL != pHttpContext)
        {
            // Retrieve a pointer to an IHttpSite class.
            IHttpSite * pHttpSite = pHttpContext->GetSite();
            // Test for an error.
            if (NULL != pHttpSite)
            {
                // Retrieve the site name.
                PCWSTR pwszSiteName = pHttpSite->GetSiteName();
                // Test for an error.
                if (NULL != pwszSiteName)
                {
                    // Allocate storage for the site name.
                    char * pszSiteName =
                        (char *) pHttpContext->AllocateRequestMemory(
                        (DWORD) wcslen(pwszSiteName)+1 );
                    // Test for an error.
                    if (NULL != pszSiteName)
                    {
                        // Convert the site name.
                        wcstombs(pszSiteName,pwszSiteName,
                            wcslen(pwszSiteName));
                        // Create an array of strings.
                        LPCSTR szBuffer[2] = {"Site Name",""};
                        // Store the site name.
                        szBuffer[1] = pszSiteName;
                        // Write the strings to the Event Viewer.
                        WriteEventViewerLog(szBuffer,2);
                    }
                }
            }
        }
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
    }

    ~MyGlobalModule()
    {
        // Test if the handle for the Event Viewer is open.
        if (NULL != MyGlobalModule::m_hEventLog)
        {
            DeregisterEventSource( MyGlobalModule::m_hEventLog );
            MyGlobalModule::m_hEventLog = NULL;
        }
    }

private:

    // Handle for the Event Viewer.
    HANDLE m_hEventLog;

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
        pGlobalModule, GL_PRE_BEGIN_REQUEST );
}
// </Snippet1>