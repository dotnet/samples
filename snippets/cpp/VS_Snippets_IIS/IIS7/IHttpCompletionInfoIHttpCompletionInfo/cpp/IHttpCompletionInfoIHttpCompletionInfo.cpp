// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>
#include <wchar.h>

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
        // Buffer to store the byte count.
        DWORD cbSent = 0;
        // Buffer to store if asyncronous completion is pending.
        BOOL fCompletionExpected = false;
        // Create an example string to return to the Web client.
        char szBuffer[] = "Hello World!";
        
        // Clear the existing response.
        pHttpContext->GetResponse()->Clear();
        // Set the MIME type to plain text.
        pHttpContext->GetResponse()->SetHeader(
            HttpHeaderContentType,"text/plain",
            (USHORT)strlen("text/plain"),TRUE);
        
        // Create a data chunk.
        HTTP_DATA_CHUNK dataChunk;
        // Set the chunk to a chunk in memory.
        dataChunk.DataChunkType = HttpDataChunkFromMemory;
        // Set the chunk to the buffer.
        dataChunk.FromMemory.pBuffer =
            (PVOID) szBuffer;
        // Set the chunk size to the buffer size.
        dataChunk.FromMemory.BufferLength =
            (USHORT) strlen(szBuffer);
        // Insert the data chunk into the response.
        hr = pHttpContext->GetResponse()->WriteEntityChunks(
            &dataChunk,1,TRUE,TRUE,&cbSent,&fCompletionExpected);

        // Test for a failure.
        if (FAILED(hr))
        {
            // Set the HTTP status.
            pHttpContext->GetResponse()->SetStatus(
                500,"Server Error",0,hr);
            // End additional processing.
            return RQ_NOTIFICATION_FINISH_REQUEST;
        }
        
        // Test for pending asynchronous operations.
        if (fCompletionExpected)
        {
            return RQ_NOTIFICATION_PENDING;
        }

        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }
    
    REQUEST_NOTIFICATION_STATUS
    OnMapRequestHandler(
        IN IHttpContext * pHttpContext,
        IN IMapHandlerProvider * pProvider
    )
    {
        // Create an HRESULT to receive return values from methods.
        HRESULT hr;
        // Buffer to store the byte count.
        DWORD cbSent = 0;
        // Buffer to store if asyncronous completion is pending.
        BOOL fCompletionExpected = false;

        // Flush the response to the client.
        hr = pHttpContext->GetResponse()->Flush(
            TRUE,FALSE,&cbSent,&fCompletionExpected);

        // Test for a failure.
        if (FAILED(hr))
        {
            // Set the HTTP status.
            pHttpContext->GetResponse()->SetStatus(
                500,"Server Error",0,hr);
        }

        // Test for pending asynchronous operations.
        if (fCompletionExpected)
        {
            return RQ_NOTIFICATION_PENDING;
        }

        // End additional processing.
        return RQ_NOTIFICATION_CONTINUE;
    }

    REQUEST_NOTIFICATION_STATUS
        OnAsyncCompletion(
        IN IHttpContext * pHttpContext,
        IN DWORD dwNotification,
        IN BOOL fPostNotification,
        IN IHttpEventProvider * pProvider,
        IN IHttpCompletionInfo * pCompletionInfo
        )
    {        
        if ( NULL != pCompletionInfo )
        {
            // Create strings for completion information.
            char szNotification[256] = "";
            char szBytes[256] = "";
            char szStatus[256] = "";

            // Retrieve and format the completion information.
            sprintf_s(szNotification,255,"Notification: %u",
                dwNotification);
            sprintf_s(szBytes,255,"Completion Bytes: %u",
                pCompletionInfo->GetCompletionBytes());
            sprintf_s(szStatus,255,"Completion Status: 0x%08x",
                pCompletionInfo->GetCompletionStatus());

            // Create an array of strings.
            LPCSTR szBuffer[3] = {szNotification,szBytes,szStatus};
            // Write the strings to the Event Viewer.
            WriteEventViewerLog(szBuffer,3);
        }
        
        // Return processing to the pipeline.
        return RQ_NOTIFICATION_CONTINUE;
    }

    MyHttpModule(void)
    {
        // Open a handle to the Event Viewer.
        m_hEventLog = RegisterEventSource( NULL,"IISADMIN" );
    }

    ~MyHttpModule(void)
    {
        // Test if the handle for the Event Viewer is open.
        if (NULL != m_hEventLog)
        {
            // Close the handle to the Event Viewer.
            DeregisterEventSource( m_hEventLog );
            m_hEventLog = NULL;
        }
    }

private:

    // Handle for the Event Viewer.
    HANDLE m_hEventLog;

    // Define a method that writes to the Event Viewer.
    BOOL WriteEventViewerLog(LPCSTR * lpStrings, WORD wNumStrings)
    {
        // Test whether the handle for the Event Viewer is open.
        if (NULL != m_hEventLog)
        {
            // Write any strings to the Event Viewer and return.
            return ReportEvent(
                m_hEventLog, EVENTLOG_INFORMATION_TYPE,
                0, 0, NULL, wNumStrings, 0, lpStrings, NULL );
        }
        return FALSE;
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
        RQ_BEGIN_REQUEST | RQ_MAP_REQUEST_HANDLER,
        0
    );
}
// </Snippet1>