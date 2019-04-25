// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Define the unique notification indentifier.
#define MY_CUSTOM_NOTIFICATION L"MyCustomNotification"

// Create a pointer for the global server interface.
IHttpServer * g_pHttpServer = NULL;

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

// Create the module's global class.
class MyGlobalModule : public CGlobalModule
{
private:

    // Create a handle for the Event Viewer.
    HANDLE m_hEventLog;
    // Create a pointer for the custom notification.
    MyCustomProvider * m_pCustomProvider;

public:

    MyGlobalModule()
    {
        // Open the global handle to the Event Viewer.
        m_hEventLog = RegisterEventSource( NULL,"IISADMIN" );
        // Initialize the pointer for the custom notification to NULL.
        m_pCustomProvider = NULL;
    }

    ~MyGlobalModule()
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

    GLOBAL_NOTIFICATION_STATUS
    OnGlobalPreBeginRequest(
        IN IPreBeginRequestProvider * pProvider
    )
    {
        // Create an array of strings.
        LPCSTR szBuffer[2] = {"MyGlobalModule","OnGlobalPreBeginRequest"};
        // Write the strings to the Event Viewer.
        WriteEventViewerLog(szBuffer,2);

        // Create the custom notification provider class.
        MyCustomProvider * m_pCustomProvider = new MyCustomProvider;

        // Test if the server and notification pointers are valid.
        if ((NULL != m_pCustomProvider) && (NULL != g_pHttpServer))
        {
            // Raise the custom notification.
            BOOL fCompletionExpected = TRUE;
            g_pHttpServer->NotifyCustomNotification(m_pCustomProvider);
        }

        // Return processing to the pipeline.
        return GL_NOTIFICATION_CONTINUE;
    }

    GLOBAL_NOTIFICATION_STATUS
    OnGlobalCustomNotification(
        IN ICustomNotificationProvider * pProvider
    )
    {
        // Retrieve the custom notification type;
        PCWSTR pNotificationType = pProvider->QueryNotificationType();

        // Test if the custom notification is correct.
        if (0 == wcscmp(pNotificationType,MY_CUSTOM_NOTIFICATION))
        {
            // Create an array of strings.
            LPCSTR szBuffer[2] = {"MyGlobalModule","OnGlobalCustomNotification"};
            // Write the strings to the Event Viewer.
            WriteEventViewerLog(szBuffer,2);
        }

        // Return processing to the pipeline.
        return GL_NOTIFICATION_CONTINUE;
    }

    VOID Terminate()
    {
        // Remove the class from memory.
        delete this;
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

    // Store the pointer for the global server interface.
    g_pHttpServer = pGlobalInfo;

    // Create an instance of the global module class.
    MyGlobalModule * pGlobalModule = new MyGlobalModule;
 
    // Test for an error.
    if (NULL == pGlobalModule)
    {
        return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
    }

    // Set the global notifications.
    hr = pModuleInfo->SetGlobalNotifications(
        pGlobalModule, GL_PRE_BEGIN_REQUEST | GL_CUSTOM_NOTIFICATION );

    // Test for an error and exit if necessary.
    if (FAILED(hr))
    {
        return hr;
    }
    
    // Return a success status;
    return S_OK;
}
// </Snippet1>