// <snippet1>

#pragma once

#include <stdio.h>
#include <string.h>
#include <ahadmin.h>

int main()
{
    IAppHostAdminManager            * pMgr        = NULL;
    IAppHostElement                 * pParentElem = NULL;
    IAppHostChildElementCollection  * pChildElems = NULL;
    IAppHostElement                 * pChildElem  = NULL;
    
    HRESULT    hr                = S_OK;
    BSTR    bstrSectionName      = SysAllocString( L"system.webServer/asp" );
    BSTR    bstrChildElemName    = NULL;
    DWORD   dwElementCount       = 0;

    // Initialize
    hr = CoInitializeEx( NULL, COINIT_MULTITHREADED );
    if ( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to initialize\n" );
        goto exit;
    } 

    // Create an admin manager
    hr = CoCreateInstance( __uuidof( AppHostAdminManager ), NULL, 
            CLSCTX_INPROC_SERVER,
            __uuidof( IAppHostAdminManager ), (void**) &pMgr );
    if( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to create an IAppHostAdminManager\n" );
        goto exit;
    }
    
    // Get the admin section
    wprintf_s( L"Getting %s\n", bstrSectionName );

    hr = pMgr->GetAdminSection( bstrSectionName, NULL, &pParentElem );
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

    // Get the child elements
    wprintf_s( L"Getting child elements\n" );
    
    hr = pParentElem->get_ChildElements( &pChildElems );
    if ( FAILED( hr ) || ( &pChildElems == NULL ) )
    {
        wprintf_s( L"ERROR: Unable to access child elements of %s\n", bstrSectionName );
        goto exit;
    }
    
    // Loop through child elements
    wprintf_s( L"Seaching for child elements of %s\n", bstrSectionName );

    hr = pChildElems->get_Count( &dwElementCount );
    for( USHORT i = 0; i < dwElementCount; i++ )
    {
        VARIANT vtItemIndex;
        vtItemIndex.vt = VT_I2;
        vtItemIndex.iVal = i;

        // Get the section group
        
        hr = pChildElems->get_Item( vtItemIndex, &pChildElem );
        if ( FAILED( hr ) || ( &pChildElem == NULL ) )
        {
            wprintf_s( L"ERROR: Unable to find child element: %d\n", i );
            goto loop_cleanup;
        }

        // Get the name

        hr = pChildElem->get_Name ( &bstrChildElemName );
        if ( FAILED( hr ) )
        {
            wprintf_s( L"ERROR: Unable to get child element name.\n" );
            goto loop_cleanup; 
        }

        wprintf_s( L"\tChild element found: %s\n", bstrChildElemName );

loop_cleanup:
        if ( pChildElem != NULL )
        {
            pChildElem->Release(); 
            pChildElem = NULL;
        }
        SysFreeString( bstrChildElemName );
    }

exit:
    // Exiting / Unwinding
    if ( pChildElems != NULL )
    {
        pChildElems->Release(); 
        pChildElems = NULL;
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

    SysFreeString( bstrChildElemName );
    SysFreeString( bstrSectionName );

    // Uninitialize
    CoUninitialize();

    return 0;
};

// </snippet1>