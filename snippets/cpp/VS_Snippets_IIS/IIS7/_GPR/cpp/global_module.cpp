#include "stdafx.h"

// <snippet1>

HRESULT GLOBAL_MODULE::Initialize( VOID  ){
	return S_OK;
}

// CGlobalModule derived classes must implement Terminate
// And free memory
//
VOID GLOBAL_MODULE::Terminate( VOID){
	delete this;
}


GLOBAL_NOTIFICATION_STATUS
GLOBAL_MODULE::OnGlobalPreBeginRequest(
									   IPreBeginRequestProvider* pProvider
									   )
{
	HRESULT         hr          = S_OK;
	IHttpContext*   pContext    = pProvider->GetHttpContext( );
	IHttpRequest*   pRequest    = pContext->GetRequest( );
	IHttpResponse*  pResponse   = pContext->GetResponse( );

	PCWSTR rqUrl = pContext->GetRequest()->GetRawHttpRequest()->CookedUrl.pAbsPath;
	OutputDebugStringW(rqUrl);

	//
	// Change only specific URL requests. 
	//

	wchar_t URLask[]     = L"/rPost.htm";
	wchar_t URLreset[]   = L"/Test.htm";
	if(!wcscmp(rqUrl,URLask)){
		hr = pRequest->SetUrl( URLreset, sizeof( URLreset )/sizeof(URLreset[0]) - 1, TRUE );
		pContext->GetTraceContext( )->QuickTrace( L"URL change from rPost to", URLreset );
	}
	if( FAILED( hr ) )
		goto Finished;


Finished:

	if( FAILED( hr ) ){
		pResponse->SetStatus( 500, "Internal Server Error", 0, hr );
		return GL_NOTIFICATION_HANDLED;
	}

	return GL_NOTIFICATION_CONTINUE;
}
// </snippet1>