// <snippet3>

#pragma once

#include <stdio.h>
#include <string.h>
#include <ahadmin.h>

int main()
{
    IAppHostAdminManager *    pMgr  = NULL;
    HRESULT hr                      = S_OK;
    BSTR    bstrMetadataName        = SysAllocString( L"availableSections" );
    VARIANT vtAvailableSections;
    
    // Initialize
    hr = CoInitializeEx( NULL, COINIT_MULTITHREADED );
    if ( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to initialize\n" );
        goto exit;
    } 

    // Create
    hr = CoCreateInstance( __uuidof( AppHostAdminManager ), NULL, 
            CLSCTX_INPROC_SERVER,
            __uuidof( IAppHostAdminManager ), (void**) &pMgr );
    if( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to create an IAppHostAdminManager instance\n" );
        goto exit;
    }

    // Get the metadata
    hr = pMgr->GetMetadata( bstrMetadataName, &vtAvailableSections );
    if ( FAILED( hr ) )
    {
        if ( E_ACCESSDENIED == hr )
        {
            printf_s( "ERROR: Access to configuration denied.\n" );
            printf_s( "       Run sample as an administrator.\n" );
        }
        else
        {
            printf_s( "ERROR: Unable to get the requested metadata.\n" );
        }
        goto exit;
    }

    // Metadata returns in a comma-delimited string.
    // Split the data and return the sections one line at a time.
    wchar_t* wcMetadata = static_cast<wchar_t*>(vtAvailableSections.bstrVal);
    wchar_t  delim[]   = L",";
    wchar_t* tokenIn = NULL;
    wchar_t* tokenOut = NULL;

    tokenIn = wcstok_s( wcMetadata, delim, &tokenOut );
    while ( tokenIn != NULL ) 
    {
        wprintf_s( L"\t%s\n", tokenIn );
        tokenIn = wcstok_s( NULL, delim, &tokenOut);
    }
exit:
    // Exiting / Unwinding
    if ( pMgr != NULL )
    {
        pMgr->Release(); 
        pMgr = NULL;
    }

    SysFreeString( bstrMetadataName );

    // Uninitialize
    CoUninitialize();
    return 0;
};
// </snippet3>