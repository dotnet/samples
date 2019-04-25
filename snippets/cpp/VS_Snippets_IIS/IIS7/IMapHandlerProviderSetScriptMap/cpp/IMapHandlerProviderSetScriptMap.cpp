// <Snippet1>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <httpserv.h>

// Create a global variable to store the response buffer limit.
DWORD g_dwResponseBufferLimit;
// Create a global pointer that will hold the path to the script processor.
WCHAR * g_pwszDllPath;

// Define a custom IScriptMapInfo interface.
class CScriptMapInfo : public IScriptMapInfo
{
public:
    PCSTR GetAllowedVerbs( VOID ) const
    {
        // Indicate that all verbs are allowed.
        return "*\0\0";
    }
    BOOL GetAllowPathInfoForScriptMappings( VOID ) const
    {
        // Indicate that path info for script mapping is not allowed.
        return FALSE;
    }
    BOOL GetIsStarScriptMap( VOID ) const
    {
        // Indicate that the script map handler is not a wildcard mapping.
        return FALSE;
    }
    PCWSTR GetManagedType( OUT DWORD * pcchManagedType = NULL ) const
    {
        // Return the managed type for this script map handler.
        if (pcchManagedType != NULL) *pcchManagedType = 0;
        return L"";
    }
    PCWSTR GetModules( OUT DWORD * pcchModules = NULL ) const
    {
        // Return the modules for this script map handler.
        if (pcchModules != NULL) *pcchModules = (DWORD)wcslen(L"IsapiModule");
        return L"IsapiModule";
    }
    PCWSTR GetName( VOID ) const
    {
        // Return the name of the example script map.
        return L"ScriptMapInfoExample";
    }
    PCWSTR GetPath( VOID ) const
    {
        // Return that this script map handler is valid for all paths.
        return L"*";
    }
    DWORD  GetRequiredAccess( VOID ) const
    {
        // Return a "read" access requirement.
        return 1;
    }
    DWORD GetResponseBufferLimit( VOID ) const
    {
        // Return the response buffer limit.
        return g_dwResponseBufferLimit;
    }
    DWORD GetResourceType( VOID ) const
    {
        // Return an "unspecified" resource type.
        return 3;
    }
    PCWSTR GetScriptProcessor( OUT DWORD * pcchScriptProcessor = NULL ) const
    {
        // Return the length of the script processor.
        if (pcchScriptProcessor != NULL)
            *pcchScriptProcessor = (DWORD)wcslen(g_pwszDllPath);
        // Return the path of the script processor.
        return g_pwszDllPath;
    }
};

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

        // Retrieve the response buffer limit and update the global variable.
        g_dwResponseBufferLimit =
            pHttpContext->GetScriptMap()->GetResponseBufferLimit();

        // Retrieve a chunk of memory from the context object.
        g_pwszDllPath = (WCHAR*)pHttpContext->AllocateRequestMemory(
            MAX_PATH*sizeof(WCHAR));

        // Test for an error.
        if (NULL == g_pwszDllPath)
        {
            // Return an error condition and exit.
            pHttpContext->GetResponse()->SetStatus(
                500,"",0,HRESULT_FROM_WIN32( ERROR_NOT_ENOUGH_MEMORY ));
            return RQ_NOTIFICATION_FINISH_REQUEST;
        }
        else
        {
            // Define the maximum size of the path array.
            DWORD nSize = MAX_PATH*sizeof(WCHAR);
            // Retrieve the path of the Inetsrv folder.
            nSize = ::ExpandEnvironmentStringsW(
                L"%windir%\\system32\\inetsrv",g_pwszDllPath,nSize);
            // Exit if the path of the Inetsrv folder cannot be determined.
            if (nSize == 0)
            {
                // Clear the DLL path.
                wcscpy_s(g_pwszDllPath,MAX_PATH,L"");
            }
            else
            {
                // Append the Asp.dll file name to the Inetsrv path.
                wcscat_s(g_pwszDllPath,MAX_PATH,L"\\asp.dll");
            }
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
        UNREFERENCED_PARAMETER( pHttpContext );

        // Create a custom IScriptMapInfo interface.
        CScriptMapInfo * pScriptMapInfo = new CScriptMapInfo;

        // Test for an error.
        if (!pScriptMapInfo)
        {
            // Set the error condition.
            pProvider->SetErrorStatus(HRESULT_FROM_WIN32( ERROR_NOT_ENOUGH_MEMORY ));
        }
        else
        {
            // Specify the custom IScriptMapInfo interface.
            pProvider->SetScriptMap(pScriptMapInfo);
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

    // Initialize the global response buffer limit variable.
    g_dwResponseBufferLimit = 0;

    // Set the request notifications and exit.
    return pModuleInfo->SetRequestNotifications(
        new MyHttpModuleFactory,
        RQ_BEGIN_REQUEST | RQ_MAP_REQUEST_HANDLER,
        0
    );
}
// </Snippet1>