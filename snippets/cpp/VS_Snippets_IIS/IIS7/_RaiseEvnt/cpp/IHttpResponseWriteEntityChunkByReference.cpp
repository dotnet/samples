#include "precomp.h"
#include "MyTrcEvnt.h"

//  Insert data from ostringstream into the response
//  On error, Provider error status set here
//  ostringstream  buffer cleared for next call 

HRESULT  WECbyRefChunk(std::ostringstream  &oss, IHttpContext *pHttpContext,
    IHttpEventProvider *pProvider, LONG InsertPosition = -1)
{
    // <snippet1>
    HRESULT hr = S_OK;

    IHttpTraceContext * pTraceContext = pHttpContext->GetTraceContext();
    hr = My_Events::My_COMPLETION::RaiseEvent(pTraceContext, InsertPosition);
    if (FAILED(hr)) {
        LOG_ERR_HR(hr, "RaiseEvent");
        return hr;
    }
    // </snippet1>

    // create convenience string from ostringstream  
    std::string str(oss.str());

    HTTP_DATA_CHUNK dc;
    dc.DataChunkType = HttpDataChunkFromMemory;
    dc.FromMemory.BufferLength = static_cast<DWORD>(str.size());
    dc.FromMemory.pBuffer = pHttpContext->AllocateRequestMemory(
        static_cast<DWORD>(str.size() + 1));

    if (!dc.FromMemory.pBuffer) {
        hr = HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        LOG_ERR_HR(hr, "AllocateRequestMemory");
        pProvider->SetErrorStatus(hr);
        return hr;
    }

    //  use char pointer p for convenience
    char *p = static_cast<char *>(dc.FromMemory.pBuffer);
    strcpy_s(p, str.size() + 1, str.c_str());

    hr = pHttpContext->GetResponse()->WriteEntityChunkByReference(
        &dc, InsertPosition);

    if (FAILED(hr)) {
        LOG_ERR_HR(hr, "AllocateRequestMemory");
        pProvider->SetErrorStatus(hr);
    }

    oss.str(""); // clear the ostringstream for next call

    return hr;
}


/* removed from published sample
IHttpRequest *pRequest = pHttpContext->GetRequest();
PCWSTR url = pRequest->GetRawHttpRequest()->CookedUrl.pAbsPath;

hr = pTraceContext->QuickTrace(L"WECbyRefChunk",url);
if (FAILED(hr)){
LOG_ERR_HR(hr,"QuickTrace");
return hr;
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
    InterlockedIncrement(&cnt);  // keep track of how many times we are called
    cnt++;

    IHttpRequest *pRequest = pHttpContext->GetRequest();
    PCWSTR url = pRequest->GetRawHttpRequest()->CookedUrl.pAbsPath;
    OutputDebugStringW(url);

    // return unless requesting a HTML file

    if (!wcsstr(url, L".htm"))
        return RQ_NOTIFICATION_CONTINUE;

    IHttpResponse * pHttpResponse = pHttpContext->GetResponse();

    // Return most times so we can still view content
    if ((cnt % 5) || pHttpResponse == NULL)
        return RQ_NOTIFICATION_CONTINUE;

    TRC_MSG_FULL("HTML  cnt = " << cnt);

    static int insertPosCnt;
    int insertPos = ++insertPosCnt % 2 - 1;    // toggle between 0 and -1

    // Use ostringstream to create some dynamic content
    std::ostringstream os;

    os << "<p /> first chunk  callback count = " << cnt
        << " insertPos = " << insertPos << "<br />";

    // 
    // WECbyRefChunk does all the work of inserting data into the response
    //

    hr = WECbyRefChunk(os, pHttpContext, pProvider, insertPos);
    if (FAILED(hr))
        return RQ_NOTIFICATION_FINISH_REQUEST;

    os << "<br /> <b> Adding 2nd chunk in Bold </b> File insertPos = " << insertPos;
    hr = WECbyRefChunk(os, pHttpContext, pProvider, insertPos);
    if (FAILED(hr))
        return RQ_NOTIFICATION_FINISH_REQUEST;

    os << " <p /> Last (3rd) Chunk added with default append chunk  GetCurrentThreadId = "
        << GetCurrentThreadId();

    // any errors will be logged/handled in  WECbyRefChunk
    WECbyRefChunk(os, pHttpContext, pProvider);

    // End additional processing, not because of error, but so another request
    // (from a GIF or .css style sheet on the same HTML page)
    // doesn't wipe out our WriteEntityChunkByReference. We can also get the
    // WriteEntityChunkByReference prepended to our normal HTML page. 

    return RQ_NOTIFICATION_FINISH_REQUEST;

}
// </snippet2>


/* On the above sample, with
    if( (cnt%5) || pHttpResponse == NULL)
        return RQ_NOTIFICATION_CONTINUE;

request http://n2-iis/IISmgr7-info/App/Filter.htm 4 times, on the 5th, request
http://n2-iis/WebProgNotes/css.htm
and you get the WEB-byRef prepended to the response stream:

first chunk callback count = 60 insertPos = -1

Adding 2nd chunk in Bold File insertPos = -1

Last (3rd) Chunk added with default append chunk GetCurrentThreadId = 2088

CSS T*T (tips and tricks)
Advantages:

Define common formatting once
Consistent look/feel
one place to make global changes -> less error prone
Less (repeated code) -> smaller pages -> less bandwidth AND cleaner pages (less SPAM)
Increases accessibility (potentially)
traditional web browsers + PDAs, phones
support for disabilities


HOME
--------------
Likewise, hit any page, then when you get the 5 count, hit http://n2-iis/IISmgr7-info/App/Filter.htm
and the WECbyRef stuff is prepended.
See managed\_RaiseEvnt\notes
*/



