// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>
#include <wchar.h>

// Create the module class.
class MyHttpModule : public CHttpModule
{
private:

    // Create a handle for the event viewer.
    HANDLE m_hEventLog;

    // Define a method the retrieves the current execution statistics.
    void RetrieveExecutionStats(
        IHttpContext * pHttpContext, LPCSTR szNotification )
    {
        HRESULT hr = S_OK;
        DWORD  dwNotification = 0;
        DWORD  dwNotificationStart = 0;
        PCWSTR pszModule = NULL;

        // Retrieve the current execution statistics.
        hr = pHttpContext->GetCurrentExecutionStats(
            &dwNotification,&dwNotificationStart,
            &pszModule,NULL,NULL,NULL);

        // Test for an error.
        if (SUCCEEDED(hr))
        {
            // Create strings for the statistics.
            char szNotificationStart[256];
            char szTimeElapsed[256];
            
            // Retrieve and format the statistics.
            sprintf_s(szNotificationStart,255,
                "Tick count at start of notification: %u",
                dwNotificationStart);
            // Pause for one second.
            Sleep(1000);
            // Retrieve and format the statistics.
            sprintf_s(szTimeElapsed,255,
                "Ticks elapsed since start of notification: %u",
                GetTickCount() - dwNotificationStart);
            
            // Allocate space for the module name.
            char * pszBuffer = (char*) pHttpContext->AllocateRequestMemory(
                (DWORD) wcslen(pszModule)+1 );
            
            // Test for an error.
            if (pszBuffer!=NULL)
            {
                // Return the module information to the web client.
                wcstombs(pszBuffer,pszModule,wcslen(pszModule));
                // Create an array of strings.
                LPCSTR szBuffer[4] = {szNotification,pszBuffer,
                    szNotificationStart,szTimeElapsed};
                // Write the strings to the Event Viewer.
                WriteEventViewerLog(szBuffer,4);
            }
        }
    }

public:

    REQUEST_NOTIFICATION_STATUS
    OnBeginRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        // Retrieve and return the execution statistics.
        RetrieveExecutionStats(pHttpContext,"OnBeginRequest");
        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

    REQUEST_NOTIFICATION_STATUS
    OnMapRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IMapHandlerProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        // Retrieve and return the execution statistics.
        RetrieveExecutionStats(pHttpContext,"OnMapRequestHandler");
        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

    REQUEST_NOTIFICATION_STATUS
    OnSendResponse(
        IN IHttpContext * pHttpContext,
        IN ISendResponseProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        // Retrieve and return the execution statistics.
        RetrieveExecutionStats(pHttpContext,"OnSendResponse");
        // Return processing to the pipeline.
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
            // Return an error if we cannot create the instance.
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

    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_BEGIN_REQUEST | RQ_MAP_REQUEST_HANDLER | RQ_SEND_RESPONSE,
        0
    );
}

// </Snippet1>