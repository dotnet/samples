// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create a pointer for the global server interface.
IHttpServer * g_pHttpServer = NULL;

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

        // Test for errors.
        if (NULL != g_pHttpServer)
        {
            // Retrieve the trace context.
            IHttpTraceContext * pTraceContext = g_pHttpServer->GetTraceContext();

            // Test for errors.
            if (NULL != pTraceContext)
            {
                // Retrieve the trace activity GUID.
                LPCGUID pTraceGuid = pTraceContext->GetTraceActivityId();

                // Test for errors.
                if (NULL != pTraceGuid)
                {
                    // Create a string buffer for the converted Unicode GUID.
                    WCHAR pwszGuid[256] = L"";
                    // Convert the GUID to a Unicode string.
                    int cbBytes = StringFromGUID2(*pTraceGuid, pwszGuid, 256);
                    // Allocate space for an ANSI string.
                    PSTR pszGuid =
                        (PSTR) pProvider->GetHttpContext()->AllocateRequestMemory(
                        (DWORD)cbBytes);
                    // Convert the Unicode string to an ANSI string.
                    wcstombs_s((size_t*)&cbBytes,pszGuid,cbBytes,pwszGuid,cbBytes);
                    // Test for errors.
                    if (cbBytes > 0)
                    {        
                        // Create an array of strings.
                        LPCSTR szBuffer[2] = {"Trace Activity ID",pszGuid};
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

    // Create an HRESULT to receive return values from methods.
    HRESULT hr;

    // Create an instance of the global module class.
    MyGlobalModule * pGlobalModule = new MyGlobalModule;
    // Test for an error.
    if (NULL == pGlobalModule)
    {
        return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
    }

    // Store the pointer for the global server interface.
    g_pHttpServer = pGlobalInfo;

    // Set the global notifications and exit.
    hr = pModuleInfo->SetGlobalNotifications(
        pGlobalModule, GL_PRE_BEGIN_REQUEST );

    // Test for an error and exit if necessary.
    if (FAILED(hr))
    {
        return hr;
    }

    // Set the global priority.
    hr = pModuleInfo->SetPriorityForGlobalNotification(
        GL_PRE_BEGIN_REQUEST,PRIORITY_ALIAS_HIGH);

    // Test for an error and exit if necessary.
    if (FAILED(hr))
    {
        return hr;
    }

    return S_OK;
}
// </Snippet1>