// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// NOTE - Data needs to be passed to this module, e.g. a POST request, or it will not appear to return anything.

// Create the module class.
class MyHttpModule : public CHttpModule
{
public:
    REQUEST_NOTIFICATION_STATUS
    OnReadEntity(
        IN IHttpContext * pHttpContext,
        IN IReadEntityProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        
        // Create buffers for the request entity information.
        PVOID pBuffer = NULL;
        DWORD cbData = 0;
        DWORD cbBuffer = 0;
        
        // Retrieve the request entity.
        pProvider->GetEntity(&pBuffer,&cbData,&cbBuffer);
        
        // Create string buffers for the return messages.
        char szData[30];
        char szBuffer[30];

        // Format the return messages.
        sprintf_s(szData,29,"Data Size: %u",cbData);
        sprintf_s(szBuffer,29,"Buffer Size: %u",cbBuffer);
        
        // Create an array of strings for the event viewer log.
        LPCSTR szReturn[3] = {"Request Entity Information",szData,szBuffer};
        // Write the strings to the Event Viewer.
        WriteEventViewerLog(szReturn,3);

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
        new MyHttpModuleFactory,
        RQ_READ_ENTITY,
        0
    );
}
// </Snippet1>