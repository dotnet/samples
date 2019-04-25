// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create the module class.
class MyHttpModule : public CHttpModule
{
public:
    REQUEST_NOTIFICATION_STATUS
    OnMapPath(
        IN IHttpContext * pHttpContext,
        IN IMapPathProvider * pProvider
    )
    {
        // Create an HRESULT to receive return values from methods.
        HRESULT hr;

        // Buffer that contains the physical path to set.
        WCHAR wszPhysicalPath[] = L"C:\\TEMP";

        // Set the physical path.
        hr = pProvider->SetPhysicalPath(
            wszPhysicalPath,(DWORD)wcslen(wszPhysicalPath));
        
        // Test for an error.
        if (FAILED(hr))
        {
            // Set the error status.
            pProvider->SetErrorStatus( hr );
            // End additional processing.
            return RQ_NOTIFICATION_FINISH_REQUEST;
        }

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
    UNREFERENCED_PARAMETER( pGlobalInfo );

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_MAP_PATH,
        0
    );
}
// </Snippet1>