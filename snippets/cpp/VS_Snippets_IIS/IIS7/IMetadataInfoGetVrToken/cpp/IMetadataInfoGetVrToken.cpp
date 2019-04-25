// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>
#define _WIN32_WINNT 0x0500
#include <sddl.h>

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

        // Retrieve a pointer to the IMetadataInfo interface.
        IMetadataInfo * pMetadataInfo = pHttpContext->GetMetadata();

        // Test for an error.
        if (NULL != pMetadataInfo)
        {
            // Retrieve the token key for the metadata interface.
            IHttpTokenEntry * pHttpTokenEntry = pMetadataInfo->GetVrToken();
            // Test for an error.
            if (NULL != pHttpTokenEntry)
            {
                // Clear the existing response.
                pHttpContext->GetResponse()->Clear();
                // Set the MIME type to plain text.
                pHttpContext->GetResponse()->SetHeader(
                    HttpHeaderContentType,"text/plain",
                    (USHORT)strlen("text/plain"),TRUE);
                // Retrieve the SID for the user associated with the metadata.
                PSID pSid = pHttpTokenEntry->GetSid();
                // Test for an error.
                if (NULL==pSid)
                {
                    // Return a message to the client that the SID is null.
                    WriteResponseMessage(pHttpContext,
                        "The metadata SID is ","null");
                }
                else
                {
                    // Test whether the SID is valid.
                    if (FALSE == IsValidSid(pSid))
                        // Return a message to the client that the SID is invalid.
                        WriteResponseMessage(pHttpContext,
                        "The metadata SID is ","invalid");
                    else
                    {
                        LPTSTR pszSid;
                        // Convert the SID to a string.
                        if (TRUE == ConvertSidToStringSid(pSid,&pszSid))
                        {
                            // Return a message to the client with the SID.
                            WriteResponseMessage(pHttpContext,
                                "The metadata SID is ",pszSid);
                        }
                    }
                }
                // End additional processing.
                return RQ_NOTIFICATION_FINISH_REQUEST;
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

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_BEGIN_REQUEST,
        0
    );
}
// </Snippet1>