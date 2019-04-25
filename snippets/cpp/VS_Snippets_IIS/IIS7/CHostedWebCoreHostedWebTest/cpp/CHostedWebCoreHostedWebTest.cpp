// <Snippet1>
#include "stdafx.h"
#include <windows.h>
#include <stdio.h>
#include <conio.h>
#include <hwebcore.h>

// NOTE: Set the project's calling convention to "__stdcall (/Gz)".

HRESULT _cdecl _tmain(int argc, _TCHAR* argv[])
{
    // Create a handle for the Web core DLL.
    HINSTANCE hDLL;

    // Specify the HRESULT for returning errors.
    HRESULT hr = S_OK;

    // Create arrays to hold paths.
    WCHAR wszInetPath[MAX_PATH];
    WCHAR wszDllPath[MAX_PATH];
    WCHAR wszCfgPath[MAX_PATH];

    // Retrieve the path of the Inetsrv folder.
    DWORD nSize = ::ExpandEnvironmentStringsW(
        L"%windir%\\system32\\inetsrv",wszInetPath,MAX_PATH);

    // Exit if the path of the Inetsrv folder cannot be determined.
    if (nSize == 0)
    {
        // Retrieve the last error.
        hr = HRESULT_FROM_WIN32(GetLastError());
        // Return an error status to the console.
        printf("Could not determine the path to the Inetsrv folder.\n");
        printf("Error: 0x%x\n",hr);
        // Return an error from the application and exit.
        return hr;
    }

    // Append the Web core DLL name to the Inetsrv path.
    wcscpy_s(wszDllPath,MAX_PATH-1,wszInetPath);
    wcscat_s(wszDllPath,MAX_PATH-1,L"\\");
    wcscat_s(wszDllPath,MAX_PATH-1,WEB_CORE_DLL_NAME);

    // Append the config file name to the Inetsrv path.
    wcscpy_s(wszCfgPath,MAX_PATH-1,wszInetPath);
    wcscat_s(wszCfgPath,MAX_PATH-1,L"\\HostedWebTest.config");

    // Create a pointer to WebCoreActivate.
    PFN_WEB_CORE_ACTIVATE pfnWebCoreActivate = NULL;

    // Create a pointer to WebCoreShutdown.
    PFN_WEB_CORE_SHUTDOWN pfnWebCoreShutdown = NULL;

    // Load the Web core DLL.
    hDLL = ::LoadLibraryW(wszDllPath);

    // Test whether the Web core DLL was loaded successfully.
    if (hDLL == NULL)
    {
        // Retrieve the last error.
        hr = HRESULT_FROM_WIN32(GetLastError());
        // Return an error status to the console.
        printf("Could not load DLL.\n");
        printf("Error: 0x%x\n",hr);
    }
    else
    {
        // Return a success status to the console.
        printf("DLL loaded successfully.\n");
        // Retrieve the address for "WebCoreActivate".
        pfnWebCoreActivate = (PFN_WEB_CORE_ACTIVATE)GetProcAddress(
            hDLL,"WebCoreActivate");
        // Test for an error.
        if (pfnWebCoreActivate==NULL)
        {
            // Retrieve the last error.
            hr = HRESULT_FROM_WIN32(GetLastError());
            // Return an error status to the console.
            printf("Could not resolve WebCoreActivate.\n");
            printf("Error: 0x%x\n",hr);
        }
        else
        {
            // Return a success status to the console.
            printf("WebCoreActivate successfully resolved.\n");
            // Retrieve the address for "WebCoreShutdown".
            pfnWebCoreShutdown = (PFN_WEB_CORE_SHUTDOWN)GetProcAddress(
                hDLL,"WebCoreShutdown");
            // Test for an error.
            if (pfnWebCoreShutdown==NULL)
            {
                // Retrieve the last error.
                hr = HRESULT_FROM_WIN32(GetLastError());
                // Return an error status to the console.
                printf("Could not resolve WebCoreShutdown.\n");
                printf("Error: 0x%x\n",hr);
            }
            else
            {
                // Return a success status to the console.
                printf("WebCoreShutdown successfully resolved.\n");
                // Return an activation status to the console.
                printf("Activating the Web core...\n");
                // Activate the Web core.
                hr = pfnWebCoreActivate(wszCfgPath,L"",L"TestWebCore");
                // Test for an error.
                if (FAILED(hr))
                {
                    // Return an error status to the console.
                    printf("WebCoreActivate failed.\n");
                    printf("Error: 0x%x\n",hr);
                }
                else
                {
                    // Return a success status to the console.
                    printf("WebCoreActivate was successful.\n");
                    // Prompt the user to continue.
                    printf("Press any key to continue...\n");
                    // Wait for a key press.
                    int iKeyPress = _getch();
                    // Return a shutdown status to the console.
                    printf("Shutting down the Web core...\n");
                    // Shut down the Web core.
                    hr = pfnWebCoreShutdown(0L);
                    // Test for an error.
                    if (FAILED(hr))
                    {
                        // Return an error status to the console.
                        printf("WebCoreShutdown failed.\n");
                        printf("Error: 0x%x\n",hr);
                    }
                    else
                    {
                        // Return a success status to the console.
                        printf("WebCoreShutdown was successful.\n");
                    }
                }
            }
        }
        // Release the DLL.
        FreeLibrary(hDLL);
    }    
    // Return the application status.
    return hr;
}
// </Snippet1>