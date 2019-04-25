#include "precomp.h"



// <snippet1>
//  Insert data from ostringstream into the response
//  On error, Provider error status set here
//  ostringstream  buffer cleared for next call 

HRESULT  WECbyRefChunk( std::ostringstream  &os, IHttpContext *pHttpContext, 
					   IHttpEventProvider *pProvider, LONG InsertPosition= -1)
{
	HRESULT hr = S_OK;

	IHttpTraceContext * pTraceContext = g_pHttpServer->GetTraceContext();

	hr = pTraceContext->QuickTrace(L"WECbyRefChunk",L"data 2",E_FAIL,6);
	if (FAILED(hr)){
		LOG_ERR_HR(hr,"QuickTrace");
		return hr;
	}
	// create convenience string from ostringstream  
	std::string str(os.str());

	HTTP_DATA_CHUNK dc;
	dc.DataChunkType = HttpDataChunkFromMemory;
	dc.FromMemory.BufferLength = static_cast<DWORD>(str.size());
	dc.FromMemory.pBuffer = pHttpContext->AllocateRequestMemory( 
		static_cast<DWORD>( str.size()+1) );

	if(!dc.FromMemory.pBuffer){
		hr = HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
		LOG_ERR_HR(hr,"AllocateRequestMemory");
		pProvider->SetErrorStatus(hr);
		return hr;
	}

	//  use char pointer p for convenience
	char *p = static_cast<char *>(dc.FromMemory.pBuffer);
	strcpy_s(p, str.size()+1, str.c_str());

	hr = pHttpContext->GetResponse()->WriteEntityChunkByReference( 
		&dc, InsertPosition );

	if (FAILED(hr)){
		LOG_ERR_HR(hr,"AllocateRequestMemory");
		pProvider->SetErrorStatus( hr );
	}

	os.str("");                // clear the ostringstream for next call

	return hr;
}  
// </snippet1>


// <snippet2>
REQUEST_NOTIFICATION_STATUS
CMyHttpModule::OnMapPath(
							  IHttpContext*       pHttpContext,
							  IHttpEventProvider* pProvider
							  )
{

	IHttpRequest *pRequest = pHttpContext->GetRequest();
	PCWSTR url = pRequest->GetRawHttpRequest()->CookedUrl.pAbsPath;
	OutputDebugStringW( url  );
	TRC_MSG(url);
	return RQ_NOTIFICATION_CONTINUE;
}

// </snippet2>
