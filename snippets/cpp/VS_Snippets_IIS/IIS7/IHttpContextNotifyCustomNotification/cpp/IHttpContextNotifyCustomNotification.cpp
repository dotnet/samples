// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Define the unique notification indentifier.
#define MY_CUSTOM_NOTIFICATION L"MyCustomNotification"

// Create the custom notification class.
class MyCustomProvider : public ICustomNotificationProvider
{
public:
    // Create the method that will identify the custom notification.
    PCWSTR QueryNotificationType(VOID)
    {
        // Return the unique identifier string for the custom notification.
        return MY_CUSTOM_NOTIFICATION;
    }
    // Create the method that will process errors.
    VOID SetErrorStatus(HRESULT hrError)
    {
        return;
    }
};

// Create the module class.
class MyHttpModule : public CHttpModule
{
private:

    // Create a handle for the Event Viewer.
    HANDLE m_hEventLog;
    // Create a pointer for the custom notification.
    MyCustomProvider * m_pCustomProvider;

public:

    MyHttpModule()
    {
        // Open the global handle to the Event Viewer.
        m_hEventLog = RegisterEventSource( NULL,"IISADMIN" );
        // Initialize the pointer for the custom notification to NULL.
        m_pCustomProvider = NULL;
    }

    ~MyHttpModule()
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != m_hEventLog)
        {
            // Close the handle to the event viewer.
            DeregisterEventSource( m_hEventLog );
            m_hEventLog = NULL;
        }
        // Test whether the pointer for the custom notification is valid.
        if (NULL != m_pCustomProvider)
        {
            // Remove the custom notification from memory.
            delete m_pCustomProvider;
            m_pCustomProvider = NULL;
        }
    }

    REQUEST_NOTIFICATION_STATUS
    OnBeginRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );

        // Create an array of strings.
        LPCSTR szBuffer[2] = {"MyHttpModule","OnBeginRequest"};
        // Write the strings to the Event Viewer.
        WriteEventViewerLog(szBuffer,2);

        // Create the custom notification provider class.
        MyCustomProvider * m_pCustomProvider = new MyCustomProvider;

        // Test if the custom notification pointer is valid.
        if (NULL != m_pCustomProvider)
        {
            // Raise the custom notification.
            BOOL fCompletionExpected = TRUE;
            pHttpContext->NotifyCustomNotification(m_pCustomProvider, &fCompletionExpected);
        }

        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

    REQUEST_NOTIFICATION_STATUS
    OnCustomRequestNotification(
        IN IHttpContext * pHttpContext,
        IN ICustomNotificationProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );

        // Retrieve the custom notification type;
        PCWSTR pNotificationType = pProvider->QueryNotificationType();

        if (0 == wcscmp(pNotificationType,MY_CUSTOM_NOTIFICATION))
        {
            // Create an array of strings.
            LPCSTR szBuffer[2] = {"MyHttpModule","OnCustomRequestNotification"};
            // Write the strings to the Event Viewer.
            WriteEventViewerLog(szBuffer,2);
        }

        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

private:

    // Create a method that writes to the Event Viewer.
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

    // Create an HRESULT to receive return values from methods.
    HRESULT hr;

    // Set the request notifications.
    hr = pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_BEGIN_REQUEST | RQ_CUSTOM_NOTIFICATION, 0 );

    // Test for an error and exit if necessary.
    if (FAILED(hr))
    {
        return hr;
    }

    // Return a success status;
    return S_OK;
}
// </Snippet1>