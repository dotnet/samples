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
        HRESULT hr;

        // retrieve a pointer to the request
        IHttpRequest * pHttpRequest = pHttpContext->GetRequest();

        // retrieve a pointer to the response
        IHttpResponse * pHttpResponse = pHttpContext->GetResponse();

        // test for an error
        if ((pHttpRequest != NULL) && (pHttpResponse != NULL))
        {
            // Create a buffer with an example URL.
            PCSTR pszBuffer = "http://www.microsoft.com/";
            // Set the URL for the request.
            hr = pHttpRequest->SetUrl(
                pszBuffer,(DWORD)strlen(pszBuffer),true);

            // Test for an error.
            if (FAILED(hr))
            {
                // Set the error status.
                pProvider->SetErrorStatus( hr );
                // End additional processing.
                return RQ_NOTIFICATION_FINISH_REQUEST;
            }

            PCWSTR pwszForwardedUrl = pHttpRequest->GetForwardedUrl();
            if (NULL != pwszForwardedUrl)
            {
                if (pwszForwardedUrl[0] != L'\0')
                {
                    // Allocate space for the user name.
                    PSTR pszForwardedUrl =
                        (PSTR) pHttpContext->AllocateRequestMemory(
                        (DWORD) wcslen(pwszForwardedUrl)+1 );
                    // Test for an error.
                    if (pszForwardedUrl==NULL)
                    {
                        // Set the error status.
                        pProvider->SetErrorStatus(
                            HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY) );
                    }
                    else
                    {
                        // Clear the existing response.
                        pHttpContext->GetResponse()->Clear();
                        // Set the MIME type to plain text.
                        pHttpContext->GetResponse()->SetHeader(
                            HttpHeaderContentType,"text/plain",
                            (USHORT)strlen("text/plain"),TRUE);

                        // Return the user information to the Web client.
                        wcstombs(pszForwardedUrl,pwszForwardedUrl,
                            wcslen(pwszForwardedUrl));
                        WriteResponseMessage(pHttpContext,
                            "Forwarded URL: ",pszForwardedUrl);
                        
                        // End additional processing
                        return RQ_NOTIFICATION_FINISH_REQUEST;
                    }
                }
            }
        }
 
        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

private:

    // Create a utility method that inserts a name/value pair into the response.
    HRESULT WriteResponseMessage(
        IHttpContext * pHttpContext,
        PCSTR pszName,
        PCSTR pszValue
    )
    {
        // Create an HRESULT to receive return values from methods.
        HRESULT hr;
        
        // Create a data chunk.
        HTTP_DATA_CHUNK dataChunk;
        // Set the chunk to a chunk in memory.
        dataChunk.DataChunkType = HttpDataChunkFromMemory;
        // Buffer for bytes written of data chunk.
        DWORD cbSent;

        // Set the chunk to the first buffer.
        dataChunk.FromMemory.pBuffer =
            (PVOID) pszName;
        // Set the chunk size to the first buffer size.
        dataChunk.FromMemory.BufferLength =
            (USHORT) strlen(pszName);
        // Insert the data chunk into the response.
        hr = pHttpContext->GetResponse()->WriteEntityChunks(
            &dataChunk,1,FALSE,TRUE,&cbSent);
        // Test for an error.
        if (FAILED(hr))
        {
            // Return the error status.
            return hr;
        }

        // Set the chunk to the second buffer.
        dataChunk.FromMemory.pBuffer =
            (PVOID) pszValue;
        // Set the chunk size to the second buffer size.
        dataChunk.FromMemory.BufferLength =
            (USHORT) strlen(pszValue);
        // Insert the data chunk into the response.
        hr = pHttpContext->GetResponse()->WriteEntityChunks(
            &dataChunk,1,FALSE,TRUE,&cbSent);
        // Test for an error.
        if (FAILED(hr))
        {
            // Return the error status.
            return hr;
        }

        // Return a success status.
        return S_OK;
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