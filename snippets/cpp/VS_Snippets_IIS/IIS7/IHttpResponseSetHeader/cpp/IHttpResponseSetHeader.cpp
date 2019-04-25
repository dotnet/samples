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

        // Create an HRESULT to receive return values from methods.
        HRESULT hr;
        
        // Set the "Refresh" header name.
        char szRefreshName[] = "Refresh";
        // Set the "Refresh" header value.
        char szRefreshValue[] = "30";
        // Set the "Content-Type" header value.
        char szContentType[] = "text/plain";
        // Set the "Server" header value.
        char szServerValue[] = "MyServer/7.0";

        // Retrieve a pointer to the response.
        IHttpResponse * pHttpResponse = pHttpContext->GetResponse();

        // Test for an error.
        if (pHttpResponse != NULL)
        {

            // Set the "Refresh" header.
            hr = pHttpResponse->SetHeader(
                szRefreshName,szRefreshValue,
                (USHORT)strlen(szRefreshValue),FALSE);

            // Test for an error.
            if (FAILED(hr))
            {
                // Set the error status.
                pProvider->SetErrorStatus( hr );
                // End additional processing.
                return RQ_NOTIFICATION_FINISH_REQUEST;
            }

            // Set the "Content-Type" header.
            hr = pHttpResponse->SetHeader(
                HttpHeaderContentType,szContentType,
                (USHORT)strlen(szContentType),TRUE);

            // Test for an error.
            if (FAILED(hr))
            {
                // Set the error status.
                pProvider->SetErrorStatus( hr );
                // End additional processing.
                return RQ_NOTIFICATION_FINISH_REQUEST;
            }

            // Set the "Server" header.
            hr = pHttpResponse->SetHeader(
                "Server",szServerValue,
                (USHORT)strlen(szServerValue),TRUE);

            // Test for an error.
            if (FAILED(hr))
            {
                // Set the error status.
                pProvider->SetErrorStatus( hr );
                // End additional processing.
                return RQ_NOTIFICATION_FINISH_REQUEST;
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