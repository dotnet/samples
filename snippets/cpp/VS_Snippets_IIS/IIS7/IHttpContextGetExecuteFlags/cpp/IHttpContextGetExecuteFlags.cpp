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
        
        // Clear the existing response.
        pHttpContext->GetResponse()->Clear();
        // Set the MIME type to plain text.
        pHttpContext->GetResponse()->SetHeader(
            HttpHeaderContentType,"text/plain",
            (USHORT)strlen("text/plain"),TRUE);

        WriteResponseMessage(pHttpContext,"Execute Flags:\n");

        // Retrieve the execution flags.
        DWORD dwExecuteFlags = pHttpContext->GetExecuteFlags();

        // Test for any flags.
        if (dwExecuteFlags == 0)
        {
            // Return a generic status if no flags are set.
            WriteResponseMessage(pHttpContext,"N/A\n");
        }
        // Test for individual flags and return them to a Web client.
        else
        {
            if (dwExecuteFlags & EXECUTE_FLAG_NO_HEADERS)
                WriteResponseMessage(pHttpContext,
                "EXECUTE_FLAG_NO_HEADERS\n");
            if (dwExecuteFlags & EXECUTE_FLAG_IGNORE_CURRENT_INTERCEPTOR)
                WriteResponseMessage(pHttpContext,
                "EXECUTE_FLAG_IGNORE_CURRENT_INTERCEPTOR\n");
            if (dwExecuteFlags & EXECUTE_FLAG_IGNORE_APPPOOL)
                WriteResponseMessage(pHttpContext,
                "EXECUTE_FLAG_IGNORE_APPPOOL\n");
            if (dwExecuteFlags & EXECUTE_FLAG_DISABLE_CUSTOM_ERROR)
                WriteResponseMessage(pHttpContext,
                "EXECUTE_FLAG_DISABLE_CUSTOM_ERROR\n");
            if (dwExecuteFlags & EXECUTE_FLAG_SAME_URL)
                WriteResponseMessage(pHttpContext,
                "EXECUTE_FLAG_SAME_URL\n");
            if (dwExecuteFlags & EXECUTE_FLAG_BUFFER_RESPONSE)
                WriteResponseMessage(pHttpContext,
                "EXECUTE_FLAG_BUFFER_RESPONSE\n");
            if (dwExecuteFlags & EXECUTE_FLAG_HTTP_CACHE_ELIGIBLE)
                WriteResponseMessage(pHttpContext,
                "EXECUTE_FLAG_HTTP_CACHE_ELIGIBLE\n");
        }

        // Return processing to the pipeline.
        return RQ_NOTIFICATION_FINISH_REQUEST;
    }

private:

    // Create a utility method that inserts a string value into the response.
    HRESULT WriteResponseMessage(
        IHttpContext * pHttpContext,
        PCSTR pszBuffer
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

        // Set the chunk to the buffer.
        dataChunk.FromMemory.pBuffer =
            (PVOID) pszBuffer;
        // Set the chunk size to the buffer size.
        dataChunk.FromMemory.BufferLength =
            (USHORT) strlen(pszBuffer);
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
            // Return an error if we cannot create the instance.
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

    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_SEND_RESPONSE,
        0
    );
}
// </Snippet1>