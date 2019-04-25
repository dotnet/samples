// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create a pointer for the global server interface.
IHttpServer * g_pHttpServer = NULL;

// Create the module class.
class MyHttpModule : public CHttpModule
{
public:
    REQUEST_NOTIFICATION_STATUS
    OnBeginRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );

        g_pHttpServer->NotifyFileChange(
            L"c:\\inetpub\\wwwroot\\default.aspx");

        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
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

    // Store the pointer for the global server interface.
    g_pHttpServer = pGlobalInfo;

    // Set the request priority.
    HRESULT hr = pModuleInfo->SetPriorityForRequestNotification(
        RQ_BEGIN_REQUEST,PRIORITY_ALIAS_MEDIUM);

    // Test for an error and exit if necessary.
    if (FAILED(hr))
    {
        return hr;
    }

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_BEGIN_REQUEST,
        0
    );
}
// </Snippet1>