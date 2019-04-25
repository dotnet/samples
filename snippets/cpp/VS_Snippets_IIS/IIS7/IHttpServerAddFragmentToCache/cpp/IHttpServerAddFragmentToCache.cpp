// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create a pointer for the global server interface.
IHttpServer * g_pGlobalInfo = NULL;

// Create the module class.
class MyHttpModule : public CHttpModule
{
public:
    REQUEST_NOTIFICATION_STATUS
    OnMapPath(
        IN IHttpContext * pHttpContext,
        IN IMapPathProvider * pProvider
    )
    {
        // Create an HRESULT to receive return values from methods.
        HRESULT hr;

        // Clear the existing response.
        pHttpContext->GetResponse()->Clear();
        // Set the MIME type to plain text.
        pHttpContext->GetResponse()->SetHeader(
            HttpHeaderContentType,"text/plain",
            (USHORT)strlen("text/plain"),TRUE);

        PCWSTR pwszFragmentName = pProvider->GetUrl(); 
        PCSTR pszBuffer = "Hello World!";
 
        HTTP_DATA_CHUNK inputDataChunk;
        inputDataChunk.DataChunkType = HttpDataChunkFromMemory;
        inputDataChunk.FromMemory.pBuffer = (PVOID) pszBuffer;
        inputDataChunk.FromMemory.BufferLength = (USHORT) strlen(pszBuffer);

        hr = g_pGlobalInfo->AddFragmentToCache(&inputDataChunk,pwszFragmentName);

        if (SUCCEEDED(hr))
        {
            WriteResponseMessage(pHttpContext,"\nSUCCEEDED\n");
        }

        // End additional processing.
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
        
        // Create a data chunk. (Defined in the Http.h file.)
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

    // Store the pointer for the global server interface.
    g_pGlobalInfo = pGlobalInfo;

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_MAP_PATH,
        0
    );
}
// </Snippet1>