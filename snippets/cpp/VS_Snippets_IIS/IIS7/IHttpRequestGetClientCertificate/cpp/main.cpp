#include "precomp.h"

//  Global server instance
#define __MAIN_G_

int                                 g_requestCnt;   // really doesn't belong here
 IHttpServer *                       g_pHttpServer = NULL;
 IHttpModuleRegistrationInfo*                      g_m=0;

//  Global module context id
//PVOID                               g_pModuleContext = NULL;

//  The RegisterModule entrypoint implementation.
//  This method is called by the server when the module DLL is 
//  loaded in order to create the module factory,
//  and register for server events.
HRESULT
__stdcall
RegisterModule(
			   DWORD                          /* dwServerVersion*/,
			   IHttpModuleRegistrationInfo *   pModuleInfo,
			   IHttpServer *                   pHttpServer
			   )
{
	HRESULT                             hr = S_OK;
	CMyHttpModuleFactory  *             pFactory = NULL;

	CLEAR_TRACE_WIN("RegisterModule one time call when loading DLL");
	TRC_MSGW_FULL(L"Trace Window Reset");

	if ( pModuleInfo == NULL || pHttpServer == NULL )       // ricka todo 2do loose this 
	{
		hr = HRESULT_FROM_WIN32( ERROR_INVALID_PARAMETER );
		LOG_ERR_HR(hr, "Null params at init:" );
		goto Finished;
	}

	// step 1: save the IHttpServer and the module context id for future use
//	g_pModuleContext = pModuleInfo->GetId();
	g_m = pModuleInfo;
	g_pHttpServer = pHttpServer;

	// step 2: create the module factory
	pFactory = new CMyHttpModuleFactory();
	if ( pFactory == NULL )
	{
		hr = HRESULT_FROM_WIN32( ERROR_NOT_ENOUGH_MEMORY );
		LOG_ERR_HR(hr,"new CMyHttpModuleFactory() hr=");
		goto Finished;
	}

	// step 3: register for server events
	// TODO: register for more server events here
	hr = pModuleInfo->SetRequestNotifications( pFactory, // module factory 
		RQ_BEGIN_REQUEST,
		0 );                                           //  server post event mask 

	//  server post event mask 
	if ( FAILED( hr ) )
	{
		LOG_ERR_HR(hr,"Failure at  SetRequestNotifications");
		goto Finished;
	}

	pFactory = NULL;

Finished:

	if ( pFactory != NULL )
	{
		delete pFactory;
		pFactory = NULL;
	}   

	return hr;
}
