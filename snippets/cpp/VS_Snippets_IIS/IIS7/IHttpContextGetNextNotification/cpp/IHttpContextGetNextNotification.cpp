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

        // Clear the existing response.
        pHttpContext->GetResponse()->Clear();
        // Set the MIME type to plain text.
        pHttpContext->GetResponse()->SetHeader(
            HttpHeaderContentType,"text/plain",
            (USHORT)strlen("text/plain"),TRUE);

        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

    REQUEST_NOTIFICATION_STATUS
    OnAuthenticateRequest(
        IN IHttpContext * pHttpContext,
        IN IAuthenticationProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        // Attempt to retrieve the next notification and display the result.
        GetNotificationAndDisplayResult(
            pHttpContext,"OnAuthenticateRequest\n");
        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

    REQUEST_NOTIFICATION_STATUS
    OnPostAuthenticateRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        // Attempt to retrieve the next notification and display the result.
        GetNotificationAndDisplayResult(
            pHttpContext,"\nOnPostAuthenticateRequest\n");
        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

    REQUEST_NOTIFICATION_STATUS
    OnAuthorizeRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        // Attempt to retrieve the next notification and display the result.
        GetNotificationAndDisplayResult(
            pHttpContext,"\nOnAuthorizeRequest\n");
        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

    REQUEST_NOTIFICATION_STATUS
    OnPostAuthorizeRequest(
        IN IHttpContext * pHttpContext,
        IN IHttpEventProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );
        // Attempt to retrieve the next notification and display the result.
        GetNotificationAndDisplayResult(
            pHttpContext,"\nOnPostAuthorizeRequest\n");
        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

    REQUEST_NOTIFICATION_STATUS
    OnMapRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IMapHandlerProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );
        // End additional processing.        
        return RQ_NOTIFICATION_FINISH_REQUEST;
    }

private:

    // Create a helper method that attempts to retrieve the next
    // notification and returns the status to a Web client.
    void GetNotificationAndDisplayResult(
        IHttpContext * pHttpContext,
        PCSTR pszBuffer
    )
    {
        DWORD dwNotification = 0;
        BOOL fPostNotification = FALSE;
        CHttpModule * pHttpModule = NULL;
        IHttpEventProvider * pEventProvider = NULL;
        char szBuffer[256]="";

        // Attempt to retrive the next notification.
        BOOL fReturn = pHttpContext->GetNextNotification(
            RQ_NOTIFICATION_CONTINUE,
            &dwNotification,&fPostNotification,
            &pHttpModule,&pEventProvider);

        // Return the name of the notification to a Web client.
        WriteResponseMessage(pHttpContext,pszBuffer);

        // Return the status of the GetNextNotification method to a Web client.
        sprintf_s(szBuffer,255,"\tGetNextNotification return value: %s\n",
            fReturn==TRUE?"true":"false");
        WriteResponseMessage(pHttpContext,szBuffer);

        // Return the notification bitmask to a Web client.
        sprintf_s(szBuffer,255,"\tNotification: %08x\n",dwNotification);
        WriteResponseMessage(pHttpContext,szBuffer);

        // Return whether the notification is a post-notification.
        sprintf_s(szBuffer,255,"\tPost-notification: %s\n",
            fPostNotification==TRUE?"Yes":"No");
        WriteResponseMessage(pHttpContext,szBuffer);
    }

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

    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_BEGIN_REQUEST | RQ_AUTHENTICATE_REQUEST | 
        RQ_AUTHORIZE_REQUEST | RQ_MAP_REQUEST_HANDLER,
        RQ_AUTHENTICATE_REQUEST | RQ_AUTHORIZE_REQUEST
    );
}
// </Snippet1>