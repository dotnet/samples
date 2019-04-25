// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

IHttpServer * g_pGlobalInfo;

// Create the module's global class.
class MyGlobalModule : public CGlobalModule
{
public:
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalCacheOperation(
        IN ICacheProvider * pProvider
    )
    {
        if (CACHE_OPERATION_RETRIEVE == pProvider->GetCacheOperation())
        {
            // Retrieve an IHttpCacheKey interface.
            IHttpCacheKey * pCacheKey = pProvider->GetCacheKey();
            // Test for an error.
            if (NULL == pCacheKey) return GL_NOTIFICATION_CONTINUE;
            
            // Initialize an IHttpCacheSpecificData interface pointer.
            IHttpCacheSpecificData * pCacheSpecificData = NULL;
            
            // Peform a cache enumeration operation.
            HRESULT hr = g_pGlobalInfo->DoCacheOperation(
                CACHE_OPERATION_ENUM,pCacheKey,&pCacheSpecificData,NULL);
            
            // Test for an error.
            if (FAILED(hr)) return GL_NOTIFICATION_HANDLED;
        }

        // Return processing to the pipeline.
        return GL_NOTIFICATION_CONTINUE;
    }

    VOID Terminate()
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

    g_pGlobalInfo = pGlobalInfo;

    // Create an instance of the global module class.
    MyGlobalModule * pGlobalModule = new MyGlobalModule;
    // Test for an error.
    if (NULL == pGlobalModule)
    {
        return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
    }
    // Set the global notifications and exit.
    return pModuleInfo->SetGlobalNotifications(
        pGlobalModule, GL_CACHE_OPERATION );
}
// </Snippet1>