// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create a pointer for the global server interface.
IHttpServer * g_pHttpServer = NULL;

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
        // Create a boolean variable for the configuration setting.
        BOOL fDoNotLog = FALSE;
        // Create pointers to configuration objects.
        INativeConfigurationSystem * pConfigSystem = NULL;
        INativeConfigurationElement * pConfigElement = NULL;

        // Retrieve a pointer to the IMetadataInfo interface.
        IMetadataInfo * pMetadataInfo = pHttpContext->GetMetadata();
        
        // Retrieve the configuration path.
        PCWSTR pwszConfigPath = pMetadataInfo->GetMetaPath();

        // Retrieve the configuration object.
        pConfigSystem = g_pHttpServer->GetConfigObject();

        // Test for an error.
        if (NULL != pConfigSystem)
        {        
            // Retrieve the HTTP logging configuration section.
            hr = pConfigSystem->GetConfigSection(
                L"system.webServer/httpLogging",
                pwszConfigPath,&pConfigElement,NULL,NULL);
        
            // Test for an error.
            if (SUCCEEDED(hr))
            {
                // Retrieve the log settings from the configuration section.
                hr = pConfigElement->GetBooleanProperty(
                    L"dontLog",&fDoNotLog,NULL);

                // Test for an error.
                if (SUCCEEDED(hr))
                {
                    // Clear the existing response.
                    pHttpContext->GetResponse()->Clear();
                    // Set the MIME type to plain text.
                    pHttpContext->GetResponse()->SetHeader(
                        HttpHeaderContentType,"text/plain",
                        (USHORT)strlen("text/plain"),TRUE);
                            
                    // Allocate space for the configuration path.
                    PSTR pszConfigPath =
                        (PSTR) pHttpContext->AllocateRequestMemory(
                        (DWORD) wcslen(pwszConfigPath) + 1 );

                    // Test for an error.
                    if (NULL == pszConfigPath)
                    {
                        // Set the error status.
                        hr = HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
                        pProvider->SetErrorStatus( hr );
                        // End additional processing.
                        return RQ_NOTIFICATION_FINISH_REQUEST;
                    }

                    // Convert the WCHAR string to a CHAR string.
                    wcstombs(pszConfigPath,
                        pwszConfigPath,wcslen(pwszConfigPath));

                    // Return the configuration path.
                    WriteResponseMessage(pHttpContext,
                        "\nConfiguration path:\t",pszConfigPath);

                    // Return the configuration setting.
                    WriteResponseMessage(pHttpContext,"\nLogging enabled:\t",
                        (FALSE == fDoNotLog) ? "Yes" : "No");

                    // End additional processing.
                    return RQ_NOTIFICATION_FINISH_REQUEST;
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

    // Store the pointer for the global server interface.
    g_pHttpServer = pGlobalInfo;

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_BEGIN_REQUEST,
        0
    );
}
// </Snippet1>