// <Snippet7>
#pragma warning( disable : 4290 )
#pragma warning( disable : 4530 )

#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <tchar.h>
#include <httptrace.h>
#include <httpserv.h>
#include <httpcach.h>

#include <string>
using namespace std;

// The CConvert class mirrors the Convert class that is 
// defined in the .NET Framework. It converts primitives 
// and other data types to wstring types.
class CConvert
{
public:
    // The ToString method converts a PCSTR to a wstring.
    // pcstr: the PCSTR to convert to a wstring.
    // return: the PCSTR as a wstring.
    static wstring ToString(PCSTR pcstr)
    {
        // Return the empty string 
        // if the PCSTR is NULL.
        if (NULL == pcstr)
        {
            return L"NULL";
        }

        // Get the length of the string to copy.
        size_t length = strlen(pcstr);
        // Create a new double-byte character 
        // array of length plus 1.
        wchar_t* newText = new wchar_t[length+1];

        // Copy the source into the sink string.
        for (size_t i = 0; i < length; ++i)
        {
            newText[i] = pcstr[i];
        }

        // Terminate the string with the NULL character.
        newText[length] = '\0';
        // Get a wstring from the new double-byte string.
        wstring wText = newText;
        // Call delete on the newText pointer 
        // and set this pointer to NULL.
        delete[] newText;
        newText = NULL;

        // Return the wstring copy.
        return wText;
    }

    // The ToByteString converts a double-byte 
    // character string to a single-byte string.
    // str: the double-byte string to convert.
    // return: a single-byte string copied from str.
    static string ToByteString(const wstring& str)
    {
        // Get the length of the 
        // double-byte string.
        size_t length = str.length();

        // Create a temporary char pointer.
        char* byteChar = new char[length+1];
        byteChar[0] = '\0';
        // Copy the double-byte character string
        // into the single-byte string.        
        size_t charsReturned = 0;
        wcstombs_s(&charsReturned, byteChar, 
                   length+1, str.c_str(), length+1);
        // Create a string to return.
        string retString = byteChar;
        // Delete the temporary string and
        // set that string to NULL.
        delete[] byteChar;
        byteChar = NULL;

        // Return the single-byte string.
        return retString;
    }
};

// The CResponseWriter class writes 
// text to the response stream.
class CResponseWriter
{
public:
    // Creates the CResponseWriter class.
    // response: the IHttpResponse 
    // pointer to write to.
    // throws: a _com_error exception if
    // the IHttpResponse pointer is NULL.
    CResponseWriter(IHttpResponse* response)
        throw (_com_error)
    {
        // If the response is NULL,
        // throw an exception.
        if (NULL == response)
        {
            ThrowOnFail(E_INVALIDARG);            
        }

        // Set the internal response.
        m_response = response;
    }

    // The destructor for the CResponseWriter
    // class. The CResponseWriter method 
    // sets the IHttpResponse pointer to NULL.
    virtual ~CResponseWriter()
    {
        m_response = NULL;
    }
    
    // The Write method writes the 
    // PCSTR to the response stream.
    // pszValue: the PCSTR to write.
    // throws: a _com_error if the 
    // Write method fails.
    void Write
    (
        PCSTR pszValue        
    ) throw (_com_error)
    {
        // The string must not be 
        // NULL, and its length must 
        // be greater than 0.
        if ((NULL == pszValue) || 
            (strlen(pszValue) < 1))            
        {
            // Throw an invalid argument
            // exception.
            ThrowOnFail(E_INVALIDARG);                        
        }        

        // Create a data chunk structure.
        HTTP_DATA_CHUNK dataChunk;
        // Set the chunk to a chunk in memory.
        dataChunk.DataChunkType = 
            HttpDataChunkFromMemory;

        // Set the chunk to 
        // the pszValue parameter.
        dataChunk.FromMemory.pBuffer = 
            (PVOID)pszValue;
        // Set the chunk size 
        // to the pszValue length.
        dataChunk.FromMemory.BufferLength = 
            (USHORT)strlen(pszValue);
                
        // Declare and initialize OUT 
        // parameters for the WriteEntityChunks 
        // method.
        DWORD pcbSent = 0;
        BOOL pfCompletionExpected = FALSE;

        // Write the entity chunks to
        // the response stream.
        HRESULT hr =
            m_response->WriteEntityChunks
                (&dataChunk,
                 1,
                 FALSE,
                 FALSE,
                 &pcbSent,
                 &pfCompletionExpected);

        // Throw an exception if the call
        // to WriteEntityChunks is not a 
        // success.
        ThrowOnFail(hr);        
    }
    
    // The WriteLine method writes a string
    // and newline characters to the response steam.
    // pszValue: the PCSTR string to write.
    // throws: a _com_error if the PCSTR 
    // or the newline characters are not 
    // written to the response stream.
    void WriteLine
    (
        PCSTR pszValue        
    ) throw (_com_error)
    {
        // Try to write the pszValue parameter.
        Write(pszValue);        
        
        // Try to write the newline characters.
        Write("\r\n");
    }
    
    // Convenience method that writes a name 
    // and value pair to the response stream.
    // name: the name of the parameter.
    // value: the value of the parameter.
    // throws: a _com_error exception if
    // the response stream is not written.
    void WriteLine
    (
        const wstring& name,
        const wstring& value
    ) throw (_com_error)
    {
        // Convert the UNICODE strings 
        // to mutlibyte strings.
        string nameMulti = 
            CConvert::ToByteString(name);
        string valueMulti = 
            CConvert::ToByteString(value);

        // Create the string to write.
        string writeString =
            nameMulti + 
            string(":  ") + 
            valueMulti;

        // Write the line to
        // the response stream.
        WriteLine(writeString.c_str());        
    }

    // Tests an HRESULT for success.
    // hr: the HRESULT value to inspect.
    // throws: a _com_error if the HRESULT
    // parameter is not S_OK.
    static void ThrowOnFail(HRESULT hr)
    {
        if (FAILED(hr))
        {
            _com_error ce(hr);
            throw ce;
        }
    }
private:
    // Specify the IHttpResponse
    // pointer to write to.
    IHttpResponse* m_response;
};

// The CCachePolicyModule is a CHttpModule 
// class that handles response processing
// by updating the IHttpCachePolicy pointer
// and writing that IHttpCachePolicy data
// to the response stream.
class CCachePolicyModule : public CHttpModule
{
public:

    // The CHttpResponseModule method is 
    // the public virtual destructor for 
    // the CHttpResponseModule class.
    virtual ~CCachePolicyModule()
    {

    }

    // The OnSendResponse method is the method 
    // supporting the RQ_SEND_RESPONSE event type.
    // pHttpContext: the current IHttpContext pointer.
    // pProvider: the current IHttpEventProvider pointer.
    // return: RQ_NOTIFICATION_FINISH_REQUEST if the
    // IHttpCachePolicy data is written. Otherwise, 
    // RQ_NOTIFICATION_CONTINUE.
    virtual 
    REQUEST_NOTIFICATION_STATUS
    OnSendResponse
    (
        IN IHttpContext* pHttpContext,
        IN ISendResponseProvider* pProvider
    )
    {
        // Return if the IHttpContext
        // pointer is NULL.
        if (NULL == pHttpContext)
        {
            return RQ_NOTIFICATION_CONTINUE;
        }

        // Get the IHttpResponse pointer 
        // from the IHttpContext pointer.
        IHttpResponse* response = 
            pHttpContext->GetResponse();

        // Return if the IHttpResponse 
        // pointer is NULL.
        if (NULL == response)
        {
            return RQ_NOTIFICATION_CONTINUE;
        }

        // Get the IHttpCachePolicy pointer
        // from the IHttpResponse pointer.
        IHttpCachePolicy* policy =
            response->GetCachePolicy();
    
        // Return if the IHttpCachePolicy
        // pointer is NULL.
        if (NULL == policy)
        {
            return RQ_NOTIFICATION_CONTINUE;
        }

        // Wrap calls in one try-catch statement
        // because a CResponseWriter might throw
        // a _com_error exception.
        try
        {
            // Create a CResponseWriter using
            // the IHttpResponse pointer.
            CResponseWriter writer(response);

            // Clear the existing response stream.
            response->Clear();

            // Set the response header to plain text.
            HRESULT hr = 
                response->SetHeader(HttpHeaderContentType, 
                                    "text/plain", 
                                    (USHORT)strlen("text/plain"), 
                                     TRUE);

            // Throw an excepion if the
            // SetHeader method failed.
            CResponseWriter::ThrowOnFail(hr);

            // Write the IHttpCachePolicy pointer 
            // data to the response stream.
            Write(policy, writer);
        }
        catch(_com_error& ce)
        {
            // Set the error status 
            // using the COM exception.
            response->SetStatus(500,
                                "OnSendResponse",
                                0, ce.Error());
            // Continue processing.
            return RQ_NOTIFICATION_CONTINUE;
        }
    
        // Finish the request because
        // the response is written.
        return RQ_NOTIFICATION_FINISH_REQUEST;
    }
protected:

    // The Write method writes IHttpCachePolicy
    // pointer data to the response stream.
    // httpCachePolicy: the IHttpCachePolicy
    // pointer to write to the stream.
    // writer: the CResponseWriter writer
    // for writing to the response stream.
    // throws: a _com_error exception.
    static void Write
    (
        IHttpCachePolicy* httpCachePolicy,
        CResponseWriter& writer
    ) throw (_com_error)
    {
        // If the IHttpCachePolicy 
        // pointer is NULL, return.
        if (NULL == httpCachePolicy)
        {
            return;
        }

        // Get the vary-by-headers from
        // the IHttpCachePolicy pointer.
        PCSTR varyByHeadersPCSTR = 
            httpCachePolicy->GetVaryByHeaders();

        // Convert the vary-by-headers 
        // to a wstring.
        wstring varyByHeaders = 
            CConvert::ToString(varyByHeadersPCSTR);

        // Write the Vary-by-headers 
        // to the response stream.
        writer.WriteLine(L"Vary-by-Headers", varyByHeaders);        
    }
};

// The CCachePolicyFactory class implements the 
// IHttpModuleFactory interface and creates a new 
// CCachePolicyModule pointer and registers that 
// pointer for request and response events.
class CCachePolicyFactory : public IHttpModuleFactory
{
public:
    // The RegisterCHttpModule method creates a new 
    // CCachePolicyFactory pointer and sets this new 
    // CCachePolicyFactory pointer as the IHttpModuleFactory 
    // pointer on the IHttpModuleRegistrationInfo pointer.
    // dwServerVersion: the current server version.
    // pModuleInfo: the current 
    // IHttpModuleRegistrationInfo pointer.
    // pGlobalInfo: the current IHttpServer pointer.
    // return: the value returned from the the call 
    // to the SetRequestNotifications on the 
    // IHttpModuleRegistrationInfo pointer.
    static HRESULT RegisterCHttpModule
    (
        DWORD dwServerVersion,
        IHttpModuleRegistrationInfo* pModuleInfo,
        IHttpServer* pGlobalInfo
    )
    {
        // Create a new CCachePolicyFactory pointer.
        CCachePolicyFactory* policyFactory = 
            new CCachePolicyFactory;

        // Test for NULL on the new 
        // CCachePolicyFactory pointer.
        if (NULL == policyFactory)
        {
            // Return an out-of-memory error 
            // code if moduleFactory is NULL.
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }

        // Set the request notifications for RQ_SEND_RESPONSE 
        // messages on the new CCachePolicyFactory pointer.
        return pModuleInfo->SetRequestNotifications
                (policyFactory,
                 RQ_SEND_RESPONSE,
                 0); 
    }
    
    // The GetHttpModule method creates a new 
    // CCachePolicyModule pointer and sets the 
    // new CCachePolicyModule on the ppModule parameter.
    // ppModule: the new CHttpResponseModule pointer to return.
    // pAllocator: currently unused.
    // return: ERROR_NOT_ENOUGH_MEMORY if the 
    // heap is exhausted; otherwise, S_OK.
    virtual
    HRESULT
    GetHttpModule
    (
        OUT CHttpModule** ppModule, 
        IN IModuleAllocator* pAllocator
    )
    {
        // Call the UNREFERENCED_PARAMETER macro 
        // with the IModuleAllocator pointer, because 
        // this parameter is currently unused.
        UNREFERENCED_PARAMETER(pAllocator);

        // Set the dereferenced ppModule 
        // to a new CHttpResponseModule pointer.
        (*ppModule) = new CCachePolicyModule;

        // If the new CHttpResponseModule is 
        // NULL, return an error indicating 
        // that heap memory is exhausted.
        if (NULL == (*ppModule))
        {
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }

        // Return S_OK.
        return S_OK;
    }
    
    // The Terminate method 
    // calls delete on this.
    virtual 
    void
    Terminate()
    {
        delete this;
    }    
protected:
    // The CCachePolicyFactory method is 
    // the protected constructor for the 
    // CCachePolicyFactory class.
    CCachePolicyFactory()
    {
        
    }

    // The CCachePolicyFactory method is 
    // the protected virtual destructor for 
    // the CCachePolicyFactory class.
    virtual ~CCachePolicyFactory()
    {
        
    }
private:
};

// The RegisterModule method is the 
// main entry point for the DLL.
// dwServerVersion: the current server version.
// pModuleInfo: the current 
// IHttpModuleRegistrationInfo pointer.
// pGlobalInfo: the current IHttpServer pointer.
// return: the value returned by calling the 
// CCachPolicyFactory::RegisterCHttpModule method.
HRESULT
__stdcall
RegisterModule(
    DWORD dwServerVersion,
    IHttpModuleRegistrationInfo* pModuleInfo,
    IHttpServer* pGlobalInfo
)
{
    return CCachePolicyFactory::RegisterCHttpModule
        (dwServerVersion, 
         pModuleInfo, 
         pGlobalInfo);    
}
// </Snippet7>