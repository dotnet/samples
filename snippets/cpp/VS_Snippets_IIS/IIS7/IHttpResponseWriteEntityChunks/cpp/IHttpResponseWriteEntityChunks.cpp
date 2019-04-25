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
        
        // Create an array of data chunks.
        HTTP_DATA_CHUNK dataChunk[2];

        // Buffer for bytes written of data chunk.
        DWORD cbSent;
        
        // Create string buffers.
        PCSTR pszOne = "First chunk data\n";
        PCSTR pszTwo = "Second chunk data\n";

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
            
            // Set the chunk to a chunk in memory.
            dataChunk[0].DataChunkType = HttpDataChunkFromMemory;
            // Set the chunk to the first buffer.
            dataChunk[0].FromMemory.pBuffer =
                (PVOID) pszOne;
            // Set the chunk size to the first buffer size.
            dataChunk[0].FromMemory.BufferLength =
                (USHORT) strlen(pszOne);

            // Set the chunk to a chunk in memory.
            dataChunk[1].DataChunkType = HttpDataChunkFromMemory;
            // Set the chunk to the second buffer.
            dataChunk[1].FromMemory.pBuffer =
                (PVOID) pszTwo;
            // Set the chunk size to the second buffer size.
            dataChunk[1].FromMemory.BufferLength =
                (USHORT) strlen(pszTwo);

            // Insert the data chunks into the response.
            hr = pHttpResponse->WriteEntityChunks(
                dataChunk,2,FALSE,TRUE,&cbSent);

            // Test for an error.
            if (FAILED(hr))
            {
                // Set the error status.
                pProvider->SetErrorStatus( hr );
            }
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