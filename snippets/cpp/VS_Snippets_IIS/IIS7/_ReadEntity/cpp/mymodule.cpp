#include "precomp.h"

//  Implementation of the OnBeginRequest method

REQUEST_NOTIFICATION_STATUS
#ifdef rBEGIN_RQST
CMyHttpModule::OnBeginRequest(
#elif rEND_RQST
CMyHttpModule::OnEndRequest(
#endif 
							IN IHttpContext * pHttpContext,
							IN IHttpEventProvider * pProvider
							)
{
	HRESULT hr;

	// Create a data chunk.
	HTTP_DATA_CHUNK dataChunk;
	// Set the chunk to a chunk in memory.
	dataChunk.DataChunkType = HttpDataChunkFromMemory;

	std::string sContentType = std::string("text/html");

	//pHttpContext->GetResponse()->Clear();
	TRC_MSGW_FULL(L"dumping all Response headers");
	PCSTR contentHdr;

	for( int i=HttpHeaderCacheControl; i < HttpHeaderRequestMaximum; i++){
		contentHdr = 0;
		contentHdr = pHttpContext->GetResponse()->GetHeader((HTTP_HEADER_ID) i);
		if(contentHdr && contentHdr[0])
			TRC_MSG("header " << i << " \"" << contentHdr << "\"");
	}

	contentHdr = 0;
	contentHdr = pHttpContext->GetResponse()->GetHeader(HttpHeaderContentType);
	if(!contentHdr ||                                       // no  header OR
		(contentHdr && sContentType!=contentHdr)           // Not HTML
		){
		TRC_MSG("returning, not a " << sContentType.c_str() );
		return RQ_NOTIFICATION_CONTINUE;
	}


	pHttpContext->GetResponse()->SetHeader(HttpHeaderContentType,
		sContentType.c_str(), (USHORT)sContentType.length(), TRUE);

	// Allocate a 1K buffer.
	DWORD cbBytesReceived = 1024;
	void * pvRequestBody = pHttpContext->AllocateRequestMemory(cbBytesReceived);

	if (pvRequestBody==NULL)
	{
		pProvider->SetErrorStatus( HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY) );
		// End additional processing.
		return RQ_NOTIFICATION_FINISH_REQUEST;
	}

	if (pHttpContext->GetRequest()->GetRemainingEntityBytes() < 1){
		TRC_MSG("empty request");
		return RQ_NOTIFICATION_CONTINUE;
	}

	// Loop through the request entity.
	while (pHttpContext->GetRequest()->GetRemainingEntityBytes() != 0)
	{

		// Retrieve the request body.
		hr = pHttpContext->GetRequest()->ReadEntityBody(
			pvRequestBody,cbBytesReceived,false,&cbBytesReceived,NULL);

		// End of data is okay. 
		if (FAILED(hr) && ERROR_HANDLE_EOF != (hr  & 0x0000FFFF) )
		{
				pProvider->SetErrorStatus( hr );
				return RQ_NOTIFICATION_FINISH_REQUEST;
		}
		TRC_MSG(pvRequestBody);
		dataChunk.FromMemory.pBuffer = pvRequestBody;
		dataChunk.FromMemory.BufferLength = cbBytesReceived;

		hr = pHttpContext->GetResponse()->WriteEntityChunks(&dataChunk,1,FALSE,TRUE,NULL);
		if (FAILED(hr))
		{
			pProvider->SetErrorStatus( hr );
			return RQ_NOTIFICATION_FINISH_REQUEST;
		}
	}
	// End additional processing.
	return RQ_NOTIFICATION_FINISH_REQUEST;

}