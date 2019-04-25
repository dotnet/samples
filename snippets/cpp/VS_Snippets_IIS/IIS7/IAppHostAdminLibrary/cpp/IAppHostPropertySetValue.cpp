// <snippet6>

#pragma once

#include <stdio.h>
#include <string.h>
#include <ahadmin.h>

int main()
{
    IAppHostWritableAdminManager * pWMgr       = NULL;
    IAppHostElement              * pParentElem = NULL;
    IAppHostPropertyCollection   * pElemProps  = NULL;
    IAppHostProperty             * pElemProp   = NULL;

    HRESULT hr                   = S_OK;
    BSTR    bstrConfigCommitPath = SysAllocString( L"MACHINE/WEBROOT/APPHOST" );
    BSTR    bstrSectionName      = SysAllocString( L"system.webServer/defaultDocument" );
    BSTR    bstrPropertyName     = SysAllocString( L"enabled" );
    VARIANT vtValue;
    vtValue.vt                   = VT_BOOL;
    vtValue.boolVal              = VARIANT_FALSE;
    VARIANT vtPropertyName;
    vtPropertyName.vt            = VT_BSTR;
    vtPropertyName.bstrVal       = bstrPropertyName;

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
        printf_s( "ERROR: Unable to create an IAppHostWritableAdminManager\n" );
        goto exit;
    }
    
    wprintf_s( L"Setting CommitPath: %s\n", bstrConfigCommitPath );
    pWMgr->put_CommitPath( bstrConfigCommitPath );

    if( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to set the commit path" );
        goto exit;
    }

    // Get the admin section
    wprintf_s( L"Getting section: %s\n", bstrSectionName );

    hr = pWMgr->GetAdminSection( bstrSectionName, bstrConfigCommitPath, &pParentElem );
    if ( FAILED( hr ) || ( &pParentElem == NULL ) )
    {
        if ( E_ACCESSDENIED == hr )
        {            
            printf_s( "ERROR: Access to configuration denied.\n" );
            printf_s( "       Run sample as an administrator.\n" );
        }
        else
        {
            printf_s( "ERROR: Unable to get configuration section.\n" );
        }
        goto exit;
    }

    // Get the property collection
    printf_s( "Getting property collection\n" );

    hr = pParentElem->get_Properties( &pElemProps );
    if ( FAILED ( hr ) || ( &pElemProps == NULL ) )
    {
        wprintf_s( L"ERROR: Unable to access attributes %s\n", bstrSectionName );
        goto exit;
    }

    // Get the property instance
    wprintf_s ( L"Getting property: %s\n", bstrPropertyName );

    hr = pElemProps->get_Item( vtPropertyName, &pElemProp );
    if ( FAILED( hr ) || ( pElemProp == NULL ) )
    {
        wprintf_s( L"ERROR: Unable to access attribute: %s\n", bstrPropertyName );
        goto exit;
    }

    // Set the property value
    wprintf_s( L"Setting property: %s to %s\n", bstrPropertyName, 
        ( vtValue.boolVal == VARIANT_TRUE ? L"true" : L"false" ) );

    hr = pElemProp->put_Value( vtValue );
    if ( FAILED( hr ) )
    {    
        wprintf_s( L"ERROR: Unable to set attribute value: %s\n", bstrPropertyName );
        goto exit;
    }

    // Commit the changes to the configuration system

    hr = pWMgr->CommitChanges();
    if( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to commit the changes to the configuration system" );
    }

exit:
    // Exiting / Unwinding
    if ( pElemProp != NULL )
    {
        pElemProp->Release();
        pElemProp = NULL;
    }
    if ( pElemProps != NULL )
    {
        pElemProps->Release(); 
        pElemProps = NULL;
    }
    if ( pParentElem != NULL )
    {
        pParentElem->Release(); 
        pParentElem = NULL;
    }
    if ( pWMgr != NULL )
    {
        pWMgr->Release(); 
        pWMgr = NULL;
    }

    SysFreeString( bstrPropertyName );
    SysFreeString( bstrConfigCommitPath );
    SysFreeString( bstrSectionName );

    // Uninitialize
    CoUninitialize();

    return 0;
};

// </snippet6>