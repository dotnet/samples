// <snippet9>

#pragma once

#include <stdio.h>
#include <string.h>
#include <ahadmin.h>

int main()
{
    IAppHostWritableAdminManager      * pWMgr       = NULL;
    IAppHostConfigManager             * pCfgMgr     = NULL;
    IAppHostConfigFile                * pCfgFile    = NULL; 
    IAppHostConfigLocationCollection  * pLocations  = NULL;
    IAppHostConfigLocation            * pLocation   = NULL;


    HRESULT hr                    = S_OK;
    BSTR    bstrConfigCommitPath  = SysAllocString( L"MACHINE/WEBROOT/APPHOST" );
    BSTR    bstrLocationPath      = NULL;
    DWORD   dwLocCount            = 0;

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

    // Set the commit path
    pWMgr->put_CommitPath( bstrConfigCommitPath );

    
    // Get an IAppHostConfigManager
    hr = pWMgr->get_ConfigManager( &pCfgMgr );
    if ( FAILED( hr ) || ( &pCfgMgr == NULL ) )
    {
        printf_s( "ERROR: Unable to get a config manager.\n" );
        goto exit;
    }
    
    // Get an IAppHostConfigFile
    hr = pCfgMgr->GetConfigFile( bstrConfigCommitPath, &pCfgFile );
    if ( FAILED( hr ) || ( &pCfgFile == NULL ) )
    {
        if ( E_ACCESSDENIED == hr )
        {
            printf_s( "ERROR: Access to configuration denied.\n" );
            printf_s( "       Run sample as an administrator.\n" );
        }
        else
        {
            printf_s( "ERROR: Unable to get config file.\n" );
        }
        goto exit;
    }

    hr = pCfgFile->get_Locations( &pLocations );
    if ( FAILED( hr ) || ( &pLocations == NULL ) )
    {
        printf_s( "ERROR: Unable to access configuration locations\n" );
        goto exit;
    }

    printf_s( "Finding locations...\n");

    hr = pLocations->get_Count( &dwLocCount );
    if( FAILED( hr ) )
    {
        printf_s( "ERROR: Unable to get location count\n" );
        goto exit;
    }
    
    if ( dwLocCount < 1 )
    {
        printf_s( "No locations found in the config file requested.\n" );
        goto exit;
    }

    printf_s( "%i locations found...\n", dwLocCount);
    for( USHORT i = 0; i < dwLocCount; i++ )
    {
        VARIANT vtCount;
        vtCount.vt = VT_I2;
        vtCount.iVal = i;

        hr = pLocations->get_Item( vtCount, &pLocation );
        if( FAILED( hr ) || ( &pLocation == NULL ) )
        {
            printf_s( "ERROR: Unable to get the item at the specified index." );
        }
        hr = pLocation->get_Path( &bstrLocationPath );
        if( FAILED( hr ) )
        {
            printf_s( "ERROR: Unable to read location path." );
            goto exit;
        }
        wprintf_s( L"Found Location Path: %s\r\n", bstrLocationPath );
    }

exit:
    // Exiting / Unwinding
    if ( pLocation != NULL )
    {
        pLocation->Release();
        pLocation = NULL;
    }
    if ( pLocations != NULL )
    {
        pLocations->Release();
        pLocations = NULL;
    }
    if ( pCfgFile != NULL )
    {
        pCfgFile->Release();
        pCfgFile = NULL;
    }
    if ( pCfgFile != NULL )
    {
        pCfgFile->Release();
        pCfgFile = NULL;
    }
    if ( pCfgMgr != NULL )
    {
        pCfgMgr->Release();
        pCfgMgr = NULL;
    }
    if ( pWMgr != NULL )
    {
        pWMgr->Release(); 
        pWMgr = NULL;
    }

    SysFreeString( bstrConfigCommitPath );
    SysFreeString( bstrLocationPath );

    // Uninitialize
    CoUninitialize();

    return 0;

};
// </snippet9>