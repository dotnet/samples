// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create a pointer for the global server interface.
IHttpServer * g_pGlobalInfo = NULL;

// Create a global file handle.
HANDLE g_hFile = NULL;

// Create a utility method for asynchronous completion.
VOID
__stdcall
MyCompletionRoutine(
    DWORD dwErrorCode,
    DWORD dwNumberOfBytesTransfered,
    LPOVERLAPPED lpOverlapped)
{
    if ((g_hFile != NULL) && (g_hFile != INVALID_HANDLE_VALUE))
    {
        CloseHandle(g_hFile);
    }    
    return;
}

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
        UNREFERENCED_PARAMETER( pHttpContext );
        UNREFERENCED_PARAMETER( pProvider );

        BOOL fAuthenticated = FALSE;

        // Retrieve an IHttpUser interface.
        IHttpUser * pHttpUser = pHttpContext->GetUser();
        char * pszUserName = NULL;
        PCWSTR pwszUserName = NULL;

        // Test for an error.
        if (pHttpUser != NULL)
        {
            // Retrieve the user name.
            pwszUserName = pHttpUser->GetUserName();
            // Test for an error.
            if (pwszUserName!=NULL)
            {
                // Test for anonymous user.
                if (wcslen(pwszUserName)>0)
                {
                    // Set the flag to indicate an authenticated user.
                    fAuthenticated = TRUE;
                }
            }
        }

        // Test for an authenticated user.
        if (fAuthenticated == FALSE)
        {
            // Clear the existing response.
            pHttpContext->GetResponse()->Clear();
            // Return an access denied message.
            pHttpContext->GetResponse()->SetStatus(401,"Access denied.",0,0);
            // End additional processing.
            return RQ_NOTIFICATION_FINISH_REQUEST;
        }

        g_hFile = CreateFile(TEXT("d:\\inetpub\\wwwroot\\myfile.txt"),
            GENERIC_WRITE,0,NULL,CREATE_ALWAYS,
            FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, NULL);
        
        if (g_hFile == INVALID_HANDLE_VALUE) 
        { 
            char szBuffer[256] = "";
            sprintf_s(szBuffer,256,"Could not open file (error %d)\n", GetLastError());
            WriteResponseMessage(pHttpContext,szBuffer);
            return RQ_NOTIFICATION_FINISH_REQUEST;
        }
        else
        {
            OVERLAPPED oOverlapped;
            oOverlapped.hEvent = NULL;
            oOverlapped.Offset = 0;
            oOverlapped.OffsetHigh = 0;
            DWORD dwBytesWritten = 0;
            g_pGlobalInfo->AssociateWithThreadPool(g_hFile,&MyCompletionRoutine);
            //pszUserName = (char*) pHttpContext->AllocateRequestMemory( (DWORD) wcslen(pwszUserName)+1 );
            //wcstombs(pszUserName,pwszUserName,wcslen(pwszUserName));
            //WriteFile(g_hFile, pszUserName, strlen(pszUserName), &dwBytesWritten, &oOverlapped);
            WriteFile(g_hFile, "Hello", strlen("Hello"), &dwBytesWritten, oOverlapped);
            return RQ_NOTIFICATION_PENDING;
        }
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
    UNREFERENCED_PARAMETER( pGlobalInfo );

    // Store the pointer for the global server interface.
    g_pGlobalInfo = pGlobalInfo;

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_SEND_RESPONSE,
        0
    );
}
// </Snippet1>