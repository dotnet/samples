// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create the module class.
class MyHttpModule : public CHttpModule
{

private:

    // Create a pointer for a child request.
    IHttpContext * m_pChildRequestContext;

public:

    MyHttpModule(void)
    {
        m_pChildRequestContext = NULL;
    }

    REQUEST_NOTIFICATION_STATUS
    OnMapPath(
        IN IHttpContext * pHttpContext,
        IN IMapPathProvider * pProvider
    )
    {
        UNREFERENCED_PARAMETER( pProvider );

        HRESULT hr;
        BOOL fCompletionExpected;

        // Retrieve a pointer to the URL.
        PCWSTR pwszUrl = pProvider->GetUrl();

        // Only process requests for the root.
        if (0 == wcscmp(pwszUrl,L"/") || 0 == wcscmp(pwszUrl,L"/default.aspx"))
        {            
            // Clone the current context.
            hr = pHttpContext->CloneContext(
                CLONE_FLAG_BASICS, &m_pChildRequestContext );
            
            // Test for a failure.
            if (FAILED(hr))
            {
                goto Failure;
            }
            
            // Test for an error.
            if ( NULL != m_pChildRequestContext )
            {
                // Set the URL for the child request.
                hr = m_pChildRequestContext->GetRequest()->SetUrl(
                    "/example/default.aspx",
                    (DWORD)strlen("/example/default.aspx"),false);
            
                // Test for a failure.
                if (FAILED(hr))
                {
                    goto Failure;
                }
                
                // Execute the child request.
                hr = pHttpContext->ExecuteRequest(
                    TRUE, m_pChildRequestContext,
                    0, NULL, &fCompletionExpected );
                
                // Test for a failure.
                if (FAILED(hr))
                {
                    goto Failure;
                }
                
                // Test for pending asynchronous operations.
                if (fCompletionExpected)
                {
                    return RQ_NOTIFICATION_PENDING;
                }

            }

 Failure:
            // Test for a child request.
            if (NULL != m_pChildRequestContext)
            {
                // Release the child request.
                m_pChildRequestContext->ReleaseClonedContext();
                m_pChildRequestContext = NULL;
            }
        }
        
        // Return processing to the pipeline.
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
        // Test for a child request.
        if (NULL != m_pChildRequestContext)
        {
            // Release the child request.
            m_pChildRequestContext->ReleaseClonedContext();
            m_pChildRequestContext = NULL;
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
        RQ_MAP_PATH,
        0
    );
}
// </Snippet1>