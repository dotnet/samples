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
        
        // Test for an error.
        if (NULL != g_pHttpServer)
        {
            // Retrieve a pointer to an IHttpPerfCounterInfo interface.
            IHttpPerfCounterInfo * pHttpPerfCounterInfo =
                g_pHttpServer->GetPerfCounterInfo();
            // Test for an error.
            if (NULL != pHttpPerfCounterInfo)
            {
                // Increment the first counter by 1.
                pHttpPerfCounterInfo->IncrementCounter(1,1);
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
        // Initialize the context pointer to NULL.
        MyGlobalModule::m_pHttpContext = NULL;
    }

    ~MyGlobalModule()
    {
        // Set the context pointer to NULL.
        MyGlobalModule::m_pHttpContext = NULL;
    }

private:
    // Create a pointer for the module context.
    IHttpContext * m_pHttpContext;
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

    // Store the pointer for the global server interface.
    g_pHttpServer = pGlobalInfo;

    // Set the global notifications and exit.
    return pModuleInfo->SetGlobalNotifications(
        pGlobalModule, GL_PRE_BEGIN_REQUEST );
}
// </Snippet1>