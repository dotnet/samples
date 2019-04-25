// <snippet5>

#pragma once

#include <stdio.h>
#include <string.h>
#include <ahadmin.h>

int main()
{
    IAppHostAdminManager        * pMgr        = NULL;
    IAppHostElement             * pParentElem = NULL;
    IAppHostPropertyCollection  * pElemProps  = NULL;
    IAppHostProperty            * pElemProp   = NULL;

    HRESULT hr                    = S_OK;
    BSTR    bstrConfigCommitPath  = SysAllocString( L"MACHINE/WEBROOT/APPHOST" );
    BSTR    bstrSectionName       = SysAllocString( L"system.webServer/defaultDocument" );
    BSTR    bstrPropertyName      = SysAllocString( L"enabled" );
    VARIANT vtValue;
    VARIANT vtPropertyName;
    vtPropertyName.vt             = VT_BSTR;
    vtPropertyName.bstrVal        = bstrPropertyName;

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
        printf_s( "ERROR: Unable to create an IAppHostAdminManager\n" );
        goto exit;
    }
    
    // Get the admin section
    hr = pMgr->GetAdminSection( bstrSectionName, bstrConfigCommitPath, &pParentElem );
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

    // Get the child elements
    hr = pParentElem->get_Properties( &pElemProps );
    if ( FAILED( hr ) || ( &pElemProps == NULL ) )
    {
        wprintf_s( L"ERROR: Unable to access attributes %s\n", bstrSectionName );
        goto exit;
    }

    // Get the requested property
    hr = pElemProps->get_Item( vtPropertyName, &pElemProp );
    if ( FAILED( hr ) || ( pElemProp == NULL ) )
    {
        wprintf_s( L"ERROR: Unable to access attribute: %s\n", bstrPropertyName );
        goto exit;
    }

    // Get the property value
    hr = pElemProp->get_Value( &vtValue );
    if ( FAILED( hr ) )
    {    
        wprintf_s( L"ERROR: Unable to access attribute value: %s\n", bstrPropertyName );
        goto exit;
    }

    wprintf_s( L"defaultDocument is %sabled\n", 
        ( ( vtValue.boolVal == VARIANT_TRUE) ? L"en" : L"dis" ) ) ;
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
    if ( pMgr != NULL )
    {
        pMgr->Release(); 
        pMgr = NULL;
    }

    SysFreeString( bstrPropertyName );
    SysFreeString( bstrConfigCommitPath );
    SysFreeString( bstrSectionName );

    // Uninitialize
    CoUninitialize();

    return 0;
};
// </snippet5>