// <snippet8>

#pragma once

#include <stdio.h>
#include <string.h>
#include <ahadmin.h>

int main()
{
    IAppHostWritableAdminManager        * pWMgr       = NULL;
    IAppHostConfigManager               * pCfgMgr     = NULL;
    IAppHostConfigFile                  * pCfgFile    = NULL; 
    IAppHostSectionGroup                * pRtSctnGrp  = NULL;
    IAppHostSectionGroup                * pSctnGrp    = NULL;
    IAppHostSectionDefinitionCollection * pSctnDefCol = NULL;
    IAppHostSectionDefinition           * pSctnDef    = NULL;

    HRESULT hr                    = S_OK;
    BSTR    bstrConfigCommitPath  = SysAllocString( L"MACHINE/WEBROOT/APPHOST" );
    BSTR    bstrSectionGroupName  = SysAllocString( L"mySectionGroup" );
    BSTR    bstrSectionName       = SysAllocString( L"myNewSection" );
    BSTR    bstrDeny              = SysAllocString( L"Deny" );
    VARIANT vtSectionGroupName;
    vtSectionGroupName.vt         = VT_BSTR;
    vtSectionGroupName.bstrVal    = bstrSectionGroupName;
    VARIANT vtSectionName;
    vtSectionName.vt              = VT_BSTR;
    vtSectionName.bstrVal         = bstrSectionName;

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

    // Get the root section group
    hr = pCfgFile->get_RootSectionGroup( &pRtSctnGrp );
    if ( FAILED( hr ) || ( &pRtSctnGrp == NULL ) )
    {
        printf_s( "ERROR: Unable to access root section group\n" );
        goto exit;
    }

    // Add a new section group
    hr = pRtSctnGrp->get_Item( vtSectionGroupName, &pSctnGrp );
    if ( FAILED( hr ) || ( &pSctnGrp == NULL ) )
    {
        wprintf_s( L"ERROR: Unable to find section group: %s\n", bstrSectionGroupName );
        goto exit;
    }

    // Get the section collection
    hr = pSctnGrp->get_Sections( &pSctnDefCol );
    if ( FAILED( hr ) || ( &pSctnDefCol == NULL ) )
    {
        printf_s( "ERROR: Unable to access section collection\n" );
        goto exit;
    }

    // Add the new section    
    hr = pSctnDefCol->get_Item( vtSectionName, &pSctnDef );
    if ( FAILED( hr ) || ( &pSctnDef == NULL ) )
    {
        wprintf_s( L"ERROR: Unable to find section: %s\n", bstrSectionName );
        goto exit;
    }

    // Set the section attributes
    pSctnDef->put_OverrideModeDefault( bstrDeny );

    // Commit the changes to the configuration system
    pWMgr->CommitChanges();

exit:
    // Exiting / Unwinding
    if ( pRtSctnGrp != NULL )
    {
        pRtSctnGrp->Release(); 
        pRtSctnGrp = NULL;
    }
    if ( pSctnGrp    != NULL )
    {
        pSctnGrp->Release(); 
        pSctnGrp = NULL;
    }
    if ( pSctnDefCol != NULL )
    {
        pSctnDefCol->Release(); 
        pSctnDefCol = NULL;
    }
    if ( pSctnDef != NULL )
    {
        pSctnDef->Release(); 
        pSctnDef = NULL;
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
    SysFreeString( bstrDeny );

    // Uninitialize
    CoUninitialize();

    return 0;
};
// </snippet8>