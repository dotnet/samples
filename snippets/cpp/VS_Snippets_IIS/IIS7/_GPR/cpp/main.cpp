#include "stdafx.h"


HRESULT
//WINAPI
__stdcall
RegisterModule(
    DWORD                           dwServerVersion,
    IHttpModuleRegistrationInfo*    pModuleInfo,
    IHttpServer*                    pGlobalInfo
)
{
    HRESULT         hr              = S_OK;
    GLOBAL_MODULE*  pGlobalModule   = NULL;

    pGlobalModule = new GLOBAL_MODULE( );
    if( NULL == pGlobalModule ) {
        hr = E_OUTOFMEMORY;
        goto Finished;
    }

    hr = pGlobalModule->Initialize( );
    if( FAILED( hr ) )
        goto Finished;

	// Note: Global modules need only be registered in  <globalModules>
	// They should not be also in the <modules> section

    hr = pModuleInfo->SetGlobalNotifications( pGlobalModule, GL_PRE_BEGIN_REQUEST );
    if( FAILED( hr ) )
        goto Finished;

    pGlobalModule = NULL;

Finished:

    if( NULL != pGlobalModule )
    {
        pGlobalModule->Terminate( );
        delete pGlobalModule;
        pGlobalModule = NULL;
    }

    return hr;
}