// <snippet4>

#pragma once

#include <stdio.h>
#include <string.h>
#include <ahadmin.h>

int main()
{
    IAppHostWritableAdminManager *    pWMgr       = NULL;
    IAppHostElement              *    pElement    = NULL;
    
    HRESULT  hr                   = S_OK;
    BSTR     bstrConfigCommitPath = SysAllocString(L"MACHINE/WEBROOT/APPHOST");
    BSTR     bstrSectionName      = SysAllocString(L"system.applicationHost/sites");
    BSTR     bstrConfigSource     = SysAllocString(L"sites.config");
    BSTR     bstrConfigSourceAttr = SysAllocString(L"configSource");
    VARIANT  vtConfigSource;
    vtConfigSource.vt             = VT_BSTR;
    vtConfigSource.bstrVal        = bstrConfigSource;

    // Initialize
    hr = CoInitializeEx( NULL, COINIT_MULTITHREADED );
    if ( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to initialize\n" );
        goto exit;
    } 

    // Create
    hr = CoCreateInstance( __uuidof( AppHostWritableAdminManager ), NULL, 
            CLSCTX_INPROC_SERVER,
            __uuidof( IAppHostWritableAdminManager ), (void**) &pWMgr );
    if( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to create an IAppHostWritableAdminManager instance\n" );
        goto exit;
    }

    // Set the commit path
    hr = pWMgr->put_CommitPath( bstrConfigCommitPath );
    if ( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to set the configuration path" );
        goto exit;
    }

    // Get the admin section
    hr = pWMgr->GetAdminSection( bstrSectionName, bstrConfigCommitPath, &pElement );
    if ( FAILED( hr ) || ( &pElement == NULL ) )
    {
        if ( E_ACCESSDENIED == hr )
        {
            printf_s( "ERROR: Access to configuration denied.\n" );
            printf_s( "       Run sample as an administrator.\n" );
        }
        else
        {
            printf_s( "ERROR: Unable to get asp configuration section.\n" );
        }
        goto exit;
    }
        
    // Set the metadata
    hr = pElement->SetMetadata( bstrConfigSourceAttr, vtConfigSource );
    if ( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to set metadata\n" );
        goto exit;
    }

    pWMgr->CommitChanges();
exit:
    // Exiting / Unwinding
    if ( pElement != NULL )
    {
        pElement -> Release ();
        pElement = NULL;
    }

    if ( pWMgr != NULL )
    {
        pWMgr->Release(); 
        pWMgr = NULL;
    }

    SysFreeString( bstrConfigCommitPath );
    SysFreeString( bstrSectionName );
    SysFreeString( bstrConfigSource );
    SysFreeString( bstrConfigSourceAttr );


    // Uninitialize
    CoUninitialize();

    return 0;
};
// </snippet4>