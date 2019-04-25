#include "precomp.h"

static bool g_null_WEC=false;

// <snippet1>
//  Insert data from ostringstream into the response
//  On error, Provider error status set here
//  ostringstream  buffer cleared for next call 

HRESULT  WECbyRefChunk( std::ostringstream  &os, IHttpContext *pHttpContext, 
					   IHttpEventProvider *pProvider, LONG InsertPosition= -1)
{

	HRESULT hr = S_OK;

	// create convenience string from ostringstream  
	std::string str(os.str());

	HTTP_DATA_CHUNK dc;
	dc.DataChunkType = HttpDataChunkFromMemory;
	dc.FromMemory.BufferLength = static_cast<DWORD>(str.size());
	dc.FromMemory.pBuffer = pHttpContext->AllocateRequestMemory( static_cast<DWORD>( str.size()+1) );

	if(!dc.FromMemory.pBuffer){
		hr = HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
		LOG_ERR_HR(hr,"AllocateRequestMemory");
		pProvider->SetErrorStatus(hr);
		return hr;
	}

	//  use char pointer p for convenience
	char *p = static_cast<char *>(dc.FromMemory.pBuffer);
	strcpy_s(p, str.size()+1, str.c_str());

	hr = pHttpContext->GetResponse()->WriteEntityChunkByReference( &dc, InsertPosition );

	if (FAILED(hr)){
		LOG_ERR_HR(hr,"AllocateRequestMemory");
		pProvider->SetErrorStatus( hr );
	}

	os.str("");                // clear the ostringstream for next call

	return hr;
}  
// </snippet1>

// removed from OnBeginRequest
//pHttpResponse->Clear();  // Clear the existing response.  Is this necessary? 


/* Note if we get a Request that has a non .htm page, then the line
	if( !wcsstr(url, L".htm"))
		return RQ_NOTIFICATION_CONTINUE;
		short cirquits the request and our code will never run. That's why I moved
		cnt++ up from 
		if( (++cnt%5) || pHttpResponse == NULL)
		*/

/*
std::string sContentType = std::string("text/html");

hr = pHttpContext->GetResponse()->SetHeader(HttpHeaderContentType,
sContentType.c_str(), static_cast<USHORT>(sContentType.length()), 
TRUE                                                // Replace Header
);

if (FAILED(hr)){
LOG_ERR_HR(hr,"SetHeader");
return RQ_NOTIFICATION_FINISH_REQUEST;       // End additional processing.
} 



std::string sContentType = std::string("text/html");

	//pHttpResponse->Clear();  // Clear the existing response. 

	hr = pHttpContext->GetResponse()->SetHeader(HttpHeaderContentType,
		sContentType.c_str(), static_cast<USHORT>(sContentType.length()), 
		TRUE                                                // Replace Header
		);

	if (FAILED(hr)){
		LOG_ERR_HR(hr,"SetHeader");
		return RQ_NOTIFICATION_FINISH_REQUEST;       // End additional processing.
	}
*/

// <snippet2>
REQUEST_NOTIFICATION_STATUS
CMyHttpModule::OnBeginRequest(
							  IHttpContext*       pHttpContext,
							  IHttpEventProvider* pProvider
							  )
{
	HRESULT hr;

	static long cnt;               
	InterlockedIncrement (&cnt);  // keep track of how many times we are called
	cnt++;

	IHttpRequest *pRequest = pHttpContext->GetRequest();

	PCWSTR url = pRequest->GetRawHttpRequest()->CookedUrl.pAbsPath;
	OutputDebugStringW( url  );

	// return unless requesting a HTML file

	if( !wcsstr(url, L".htm"))
		return RQ_NOTIFICATION_CONTINUE;

	IHttpResponse * pHttpResponse = pHttpContext->GetResponse();

	// Return most times so we can still view content
	if( (cnt%5) || pHttpResponse == NULL)
		return RQ_NOTIFICATION_CONTINUE;

	TRC_MSG_FULL("HTML  cnt = " << cnt  );

	static int insertPosCnt;
	int insertPos = ++insertPosCnt%2 -1;    // toggle between 0 and -1

	// Use ostringstream to create some dynamic content
	std::ostringstream os; 

	os << "<p /> first chunk  callback count = " << cnt 
		<< " insertPos = " << insertPos << "<br />";

	// 
	// WECbyRefChunk does all the work of inserting data into the response
	//

	hr = WECbyRefChunk( os, pHttpContext, pProvider, insertPos);
	if (FAILED(hr))
		return RQ_NOTIFICATION_FINISH_REQUEST;       

	os << "<br /> <b> Adding 2nd chunk in Bold </b> File insertPos = " << insertPos ;
	hr = WECbyRefChunk( os, pHttpContext, pProvider,insertPos);
	if (FAILED(hr))
		return RQ_NOTIFICATION_FINISH_REQUEST;       

	os << " <p /> Last (3rd) Chunk added with default append chunk  GetCurrentThreadId = " 
		<< GetCurrentThreadId();
	
	// any errors will be logged/handled in  WECbyRefChunk
	WECbyRefChunk( os, pHttpContext, pProvider);

	// End additional processing, not because of error, but so another request
	// doesn't wipe out our WriteEntityChunkByReference

	return RQ_NOTIFICATION_FINISH_REQUEST;       
}

// </snippet2>

// Notes: at the normal end of OnBeginRequest if we return RQ_NOTIFICATION_CONTINUE, 
// our WriteEntityChunkByReference data is lost and the normal page is returned.


///
/// Random notes
/*

// Test case sending nothing to response
// results in blank browser window as expected
if(g_null_WEC){
os.str("");         // clear string
hr = WECbyRefChunk( os, pHttpContext, pProvider, insertPos);
return RQ_NOTIFICATION_FINISH_REQUEST;
}

%windir%\System32\inetsrv\config\applicationHost.config

*/
