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
    OnBeginRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        // Create an HRESULT to receive return values from methods.
        HRESULT hr;

        // Allocate a 1K buffer.
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
        
        // Copy a string into the buffer.
        strcpy_s((char*)pvBuffer,cbBuffer,"Hello world!");
        // Set the entity body to the buffer.
        hr = pHttpContext->GetRequest()->SetEntityBody(
            pvBuffer,(DWORD)strlen((char*)pvBuffer));

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
        RQ_BEGIN_REQUEST,
        0
    );
}
// </Snippet1>