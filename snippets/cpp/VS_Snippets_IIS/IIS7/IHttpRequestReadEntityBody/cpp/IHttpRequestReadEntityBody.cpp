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
        UNREFERENCED_PARAMETER( pProvider );

        // Create an HRESULT to receive return values from methods.
        HRESULT hr;

        // Create a data chunk.
        HTTP_DATA_CHUNK dataChunk;
        // Set the chunk to a chunk in memory.
        dataChunk.DataChunkType = HttpDataChunkFromMemory;

        // Clear the existing response.
        pHttpContext->GetResponse()->Clear();
        // Set the MIME type to plain text.
        pHttpContext->GetResponse()->SetHeader(
            HttpHeaderContentType,"text/plain",
            (USHORT)strlen("text/plain"),TRUE);

        // Allocate a 1K buffer.
        DWORD cbBytesReceived = 1024;
        void * pvRequestBody = pHttpContext->AllocateRequestMemory(cbBytesReceived);
        
        // Test for an error.
        if (NULL == pvRequestBody)
        {
            // Set the error status.
            hr = HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
            pProvider->SetErrorStatus( hr );
            // End additional processing.
            return RQ_NOTIFICATION_FINISH_REQUEST;
        }

        if (pHttpContext->GetRequest()->GetRemainingEntityBytes() > 0)
        {
            // Loop through the request entity.
            while (pHttpContext->GetRequest()->GetRemainingEntityBytes() != 0)
            {

                // Retrieve the request body.
                hr = pHttpContext->GetRequest()->ReadEntityBody(
                    pvRequestBody,cbBytesReceived,false,&cbBytesReceived,NULL);
                // Test for an error.
                if (FAILED(hr))
                {
                    // End of data is okay.
                    if (ERROR_HANDLE_EOF != (hr  & 0x0000FFFF))
                    {
                        // Set the error status.
                        pProvider->SetErrorStatus( hr );
                        // End additional processing.
                        return RQ_NOTIFICATION_FINISH_REQUEST;
                    }
                }
                dataChunk.FromMemory.pBuffer = pvRequestBody;
                dataChunk.FromMemory.BufferLength = cbBytesReceived;
                
                hr = pHttpContext->GetResponse()->WriteEntityChunks(
                    &dataChunk,1,FALSE,TRUE,NULL);
                if (FAILED(hr))
                {
                    // Set the error status.
                    pProvider->SetErrorStatus( hr );
                    // End additional processing.
                    return RQ_NOTIFICATION_FINISH_REQUEST;
                }
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