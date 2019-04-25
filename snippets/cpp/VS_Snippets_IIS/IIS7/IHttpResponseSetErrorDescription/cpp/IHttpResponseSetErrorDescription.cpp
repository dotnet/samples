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
    OnSendResponse(
        IN IHttpContext * pHttpContext,
        IN ISendResponseProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );

        // Retrieve a a pointer to the current response.
        IHttpResponse * pHttpResponse = pHttpContext->GetResponse();

        // Test for errors.
        if (NULL != pHttpResponse)
        {
            USHORT uStatusCode = 0;
            USHORT uSubStatus = 0;

            // Retrieve the current HTTP status code.
            pHttpResponse->GetStatus(&uStatusCode,&uSubStatus);

            // Process only 404.0 errors.
            if (uStatusCode==404 && uSubStatus==0)
            {
                DWORD cchDescription = 0;
                
                // Retrieve the current error description.
                PCWSTR pwszErrorDescription =
                    pHttpResponse->GetErrorDescription(&cchDescription);

                // Process only if no error description is currently defined.
                if (cchDescription==0)
                {
                    // Define the new error description.
                    PCWSTR wszNewDescription =
                        L"The file that you requested cannot be found.";
                    // Configure the new error description.
                    pHttpResponse->SetErrorDescription(
                        wszNewDescription,wcslen(wszNewDescription),TRUE);               
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
    UNREFERENCED_PARAMETER( pGlobalInfo );

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_SEND_RESPONSE,
        0
    );
}
// </Snippet1>