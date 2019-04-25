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
    OnMapRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IMapHandlerProvider * pProvider
    )
    {
        // Create an HRESULT to receive return values from methods.
        HRESULT hr;

        // Retrieve a pointer to the URL.
        DWORD cchScriptName;
        PCWSTR pwszUrl = pHttpContext->GetScriptName(&cchScriptName);

        // Test for an error.
        if ((NULL != pwszUrl) && (cchScriptName>0))
        {
            // Compare the request URL to limit the module's scope.
            if (0 == wcscmp(pwszUrl,L"/default.htm"))
            {
                // Create buffers to contain file paths.
                PWSTR wszUrl = L"/example/default.htm";
                WCHAR wszPhysicalPath[1024] = L"";
                DWORD cbPhysicalPath = 1024;
                
                // Map a URL path to a physical path.
                pHttpContext->MapPath(wszUrl,wszPhysicalPath,&cbPhysicalPath);
                
                // Test for an error.
                if (NULL != wszPhysicalPath)
                {
                    // Create an IHttpFileInfo interface.
                    IHttpFileInfo * pHttpFileInfo = NULL;
                    // Retrieve the IHttpFileInfo interface.
                    hr = g_pHttpServer->GetFileInfo(wszPhysicalPath,
                        NULL,NULL,wszUrl,NULL,TRUE,&pHttpFileInfo);
                    // Test for an error.
                    if ((NULL != pHttpFileInfo) && (SUCCEEDED(hr)))
                    {
                        // Set the IHttpFileInfo interface.
                        pProvider->SetFileInfo(pHttpFileInfo);
                    }
                }
            }
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

    // Store the pointer for the global server interface.
    g_pHttpServer = pGlobalInfo;

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_MAP_REQUEST_HANDLER,
        0
    );
}
// </Snippet1>