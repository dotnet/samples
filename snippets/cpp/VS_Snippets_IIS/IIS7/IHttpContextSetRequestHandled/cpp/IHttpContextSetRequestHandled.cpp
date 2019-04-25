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
    OnPreExecuteRequestHandler(
        IN IHttpContext* pHttpContext,
        IN IHttpEventProvider* pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );

        // Retrieve an IScriptMapInfo interface.
        IScriptMapInfo * pScriptMapInfo = pHttpContext->GetScriptMap();

        // Test for an error.
        if (NULL != pScriptMapInfo)
        {
            // Create buffers to store the script processor path.
            PCWSTR pwszScriptProcessor;
            DWORD  cchScriptProcessor = 0;

            // Retrieve the script processor.
            pwszScriptProcessor =
                pScriptMapInfo->GetScriptProcessor(&cchScriptProcessor);

            // Test for an error.
            if ((pwszScriptProcessor != NULL) && (cchScriptProcessor > 0))
            {
                // Test for an ASP request.
                if (NULL != wcsstr(pwszScriptProcessor,L"\\asp.dll"))
                {
                    // Clear the existing response.
                    pHttpContext->GetResponse()->Clear();
                    // Set the MIME type to plain text.
                    pHttpContext->GetResponse()->SetHeader(
                        HttpHeaderContentType,"text/plain",
                        (USHORT)strlen("text/plain"),TRUE);
                    // Return a status message.
                    WriteResponseMessage(pHttpContext,
                        "ASP requests have been handled.");
                    // Indicate that handlers for this request have completed.
                    pHttpContext->SetRequestHandled();                    
                }
            }
        }         
        // Return processing to the pipeline.    
        return RQ_NOTIFICATION_CONTINUE;
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

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_PRE_EXECUTE_REQUEST_HANDLER,
        0
    );
}
// </Snippet1>