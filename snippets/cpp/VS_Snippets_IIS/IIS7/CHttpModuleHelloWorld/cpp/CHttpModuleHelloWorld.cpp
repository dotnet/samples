// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create the module class.
class CHelloWorld : public CHttpModule
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
        
        // Retrieve a pointer to the response.
        IHttpResponse * pHttpResponse = pHttpContext->GetResponse();

        // Test for an error.
        if (pHttpResponse != NULL)
        {
            // Clear the existing response.
            pHttpResponse->Clear();
            // Set the MIME type to plain text.
            pHttpResponse->SetHeader(
                HttpHeaderContentType,"text/plain",
                (USHORT)strlen("text/plain"),TRUE);

            // Create a string with the response.
            PCSTR pszBuffer = "Hello World!";
            // Create a data chunk.
            HTTP_DATA_CHUNK dataChunk;
            // Set the chunk to a chunk in memory.
            dataChunk.DataChunkType = HttpDataChunkFromMemory;
            // Buffer for bytes written of data chunk.
            DWORD cbSent;
            
            // Set the chunk to the buffer.
            dataChunk.FromMemory.pBuffer =
                (PVOID) pszBuffer;
            // Set the chunk size to the buffer size.
            dataChunk.FromMemory.BufferLength =
                (USHORT) strlen(pszBuffer);
            // Insert the data chunk into the response.
            hr = pHttpResponse->WriteEntityChunks(
                &dataChunk,1,FALSE,TRUE,&cbSent);

            // Test for an error.
            if (FAILED(hr))
            {
                // Set the HTTP status.
                pHttpResponse->SetStatus(500,"Server Error",0,hr);
            }

            // End additional processing.
            return RQ_NOTIFICATION_FINISH_REQUEST;
        }

        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }
};

// Create the module's class factory.
class CHelloWorldFactory : public IHttpModuleFactory
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
        CHelloWorld * pModule = new CHelloWorld;

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

    void
    Terminate()
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
        new CHelloWorldFactory,
        RQ_BEGIN_REQUEST,
        0
    );
}
// </Snippet1>