
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
    OnBeginRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );

        // Create an HRESULT to receive return values from methods.
        HRESULT hr;

        // Specify the "User-Agent" header name.
        char szHeaderName[] = "User-Agent";
        // Specify the "User-Agent" header value.
        char szUserAgent[] = "Test Browser";
        // Specify the "Accept-Language" header value.
        char szAcceptLanguage[] = "en-ie";

        // Retrieve a pointer to the request.
        IHttpRequest * pHttpRequest = pHttpContext->GetRequest();

        // Test for an error.
        if (pHttpRequest != NULL)
        {
            // Replace the "User-Agent" header.
            hr = pHttpRequest->SetHeader(
                szHeaderName,szUserAgent,
                (USHORT)strlen(szUserAgent),true);

            // Test for an error.
            if (FAILED(hr))
            {
                // Set the error status.
                pProvider->SetErrorStatus( hr );
                // End additional processing.
                return RQ_NOTIFICATION_FINISH_REQUEST;
            }

            // Replace the "Accept-language" header.
            hr = pHttpRequest->SetHeader(
                HttpHeaderAcceptLanguage,szAcceptLanguage,
                (USHORT)strlen(szAcceptLanguage),true);

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
/* MERGEFORMAT 16 Aug 07  5:41 PM */
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