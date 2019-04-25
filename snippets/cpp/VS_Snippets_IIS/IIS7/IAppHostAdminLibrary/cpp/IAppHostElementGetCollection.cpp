// <snippet2>

#pragma once

#include <stdio.h>
#include <string.h>
#include <ahadmin.h>

int main()
{
    IAppHostAdminManager        * pMgr        = NULL;
    IAppHostElement             * pParentElem = NULL;
    IAppHostElementCollection   * pElemColl   = NULL;
    IAppHostElement             * pElem       = NULL;
    IAppHostPropertyCollection  * pElemProps  = NULL;
    IAppHostProperty            * pElemProp   = NULL;

    HRESULT hr                   = S_OK;
    BSTR    bstrConfigCommitPath = SysAllocString( L"MACHINE/WEBROOT/APPHOST" );
    BSTR    bstrSectionName      = SysAllocString( L"system.applicationHost/sites" );
    BSTR    bstrPropertyName     = SysAllocString( L"name" );
    DWORD   dwElementCount       = 0;
    VARIANT vtValue;
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
            printf_s( "ERROR: Unable to get asp configuration section.\n" );
        }
        goto exit;
    }


    // Get the site collection
    hr = pParentElem->get_Collection( &pElemColl );
    if ( FAILED ( hr ) || ( &pElemColl == NULL ) )
    {
        wprintf_s ( L"ERROR: Unable to access element collection of %s\n", bstrSectionName );
        goto exit;
    }

    // Get the elements
    wprintf_s( L"Seaching for elements in %s\n", bstrSectionName );

    hr = pElemColl->get_Count( &dwElementCount );
    for ( USHORT i = 0; i < dwElementCount; i++ )
    {
        VARIANT vtItemIndex;
        vtItemIndex.vt = VT_I2;
        vtItemIndex.iVal = i;

        // Add a new section group
        hr = pElemColl->get_Item( vtItemIndex, &pElem );
        if ( FAILED( hr ) || ( &pElem == NULL ) )
        {
            wprintf_s( L"ERROR: Unable to find element: %d\n", i );
            goto loop_cleanup;
        }

        // Get the child elements
        hr = pElem->get_Properties( &pElemProps );
        if ( FAILED( hr ) || ( &pElemProps == NULL ) )
        {
            printf_s( "ERROR: Unable to access attributes\n" );
            goto exit;
        }

        hr = pElemProps->get_Item( vtPropertyName, &pElemProp );
        if ( FAILED( hr ) || ( pElemProp == NULL ) )
        {
            printf_s( "ERROR: Unable to access attribute\n" );
            goto exit;
        }

        hr = pElemProp->get_Value( &vtValue );
        if ( FAILED( hr ) )
        {    
            wprintf_s( L"ERROR: Unable to access attribute value: %s\n", bstrPropertyName );
            goto exit;
        }

        wprintf_s( L"Site name is: %s\n", vtValue.bstrVal );

loop_cleanup:
        if ( pElem != NULL )
        {
            printf_s("\treleasing collection element\n");
            pElem->Release(); 
            pElem = NULL;
        }
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
    if ( pElem != NULL )
    {
        pElem->Release();
        pElem = NULL;
    }
    if ( pElemColl != NULL )
    {
        pElemColl->Release(); 
        pElemColl = NULL;
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

    SysFreeString( bstrConfigCommitPath );
    SysFreeString( bstrSectionName );

    // Uninitialize
    CoUninitialize();

    return 0;
};

// </snippet2>