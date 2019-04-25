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
        // Allocate a 1K buffer for the request entity.
        DWORD cbBuffer = 1024;
        void * pvBuffer = pHttpContext->AllocateRequestMemory(cbBuffer);
        
        // Test for an error.
        if (NULL == pvBuffer)
        {
            // Set the error status.
            pProvider->SetErrorStatus(
                HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY));
            // End additional processing.
            return RQ_NOTIFICATION_FINISH_REQUEST;
        }

        // Create a string to return.
        char szBuffer[] = "Name=Value";
        // Specify the exact data length.
        DWORD cbData = (DWORD) strlen(szBuffer);        
        // Copy a string into the request entity buffer.
        strcpy_s((char*)pvBuffer,cbBuffer,szBuffer);
        // Set the request entity to the buffer.
        pProvider->SetEntity(pvBuffer,cbData,cbBuffer);

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
        RQ_READ_ENTITY,
        0
    );
}
// </Snippet1>