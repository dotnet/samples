// <Snippet10>
#pragma warning( disable : 4290 )
#pragma warning( disable : 4530 )

#define _WINSOCKAPI_
#include <windows.h>
#include <io.h>
#include <sal.h>
#include <httpserv.h>

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

// The MyHttpModule class extends the CHttpModule 
// class and handles response processing.  This 
// class handles retrieving an IHttpFileInfo pointer 
// from the IHttpContext pointer during the overridden 
// OnSendResponse method, and writes the IHttpFileInfo 
// data to the response stream.
class MyHttpModule : public CHttpModule
{
public:
    // The MyHttpModule method is the public 
    // constructor for the MyHttpModule class.
    MyHttpModule()
    {

    }

    // The OnSendResponse method 
    // handles respose processing.
    // pHttpContext: an IHttpContext pointer.
    // pProvider: an ISendResponseProvider pointer.
    // return: RQ_NOTIFICATION_FINISH_REQUEST if the 
    // IHttpFileInfo pointer can be retrieved from the 
    // pHttpContext parameter; otherwise, 
    // RQ_NOTIFICATION_CONTINUE.
    REQUEST_NOTIFICATION_STATUS OnSendResponse
    (
        IN IHttpContext* pHttpContext,
        IN OUT ISendResponseProvider* pProvider
    )
    {
        // Use the UNREFERENCED_PARAMETER macro since
        // the pProvider parameter is never used.
        UNREFERENCED_PARAMETER(pProvider);

        if (NULL == pHttpContext)
        {
            return RQ_NOTIFICATION_CONTINUE;
        }

        // Get an IHttpFileInfo pointer 
        // from the IHttpContext pointer.
        IHttpFileInfo* fileInfo = 
            pHttpContext->GetFileInfo();

        // Get the IHttpResponse pointer 
        // from the IHttpContext pointer.
        IHttpResponse* response = 
            pHttpContext->GetResponse();

        // Both the IHttpFileInfo pointer
        // and IHttpResponse pointer are
        // required for processing.
        if ((NULL == fileInfo) ||
            (NULL == response))
        {
            return RQ_NOTIFICATION_CONTINUE;
        }

        // Clear the existing response stream.        
        response->Clear();

        // Set the response header to plain text.
        HRESULT hr = 
            response->SetHeader(HttpHeaderContentType, 
                                "text/plain", 
                                (USHORT)strlen("text/plain"), 
                                 TRUE);

        // If the SetHeader method fails,
        // call the SetStatus with an error code.
        if (FAILED(hr))
        {
            response->SetStatus(500, 
                                "IHttpResponse::SetHeader", 
                                0, hr);
            return RQ_NOTIFICATION_CONTINUE;
        }

        // The CResponseWriter will throw a 
        // com_exception if it cannot be created, 
        // or if the response stream cannot be written. 
        // Create a new CResponseWriter, and wrap 
        // all calls into that CResponseWriter,
        // with a try-catch statement.
        try
        {
            // Declare a writer for 
            // the response stream.
            CResponseWriter writer(response);

            // Call the GetLastModifiedString
            // method on the IHttpFileInfo pointer.
            PCSTR lastModifiedPCSTR = 
                fileInfo->GetLastModifiedString();

            // Convert the modified 
            // information to a wstring.
            wstring lastModifiedString =
                CConvert::ToString(lastModifiedPCSTR);

            // Write the modification information
            // to the response stream.
            writer.WriteLine(L"Last Modified String",
                             lastModifiedString);
        }
        catch (_com_error& ce)
        {
            // Attempt to get the 
            // description of the error.
            _bstr_t description = 
                ce.Description();

            // Print the description if 
            // it is not empty.
            if (0 == description.length())
            {
                description = ce.ErrorMessage();            
            }
            
            // Set an error status on the 
            // IHttpResponse pointer.
            response->SetStatus(500, description, 0, ce.Error());            
        }

        // Return RQ_NOTIFICATION_FINISH_REQUEST, which 
        // will terminate any additional request processing.
        return RQ_NOTIFICATION_FINISH_REQUEST;
    }
protected:    
    // The MyHttpModule is the protected virtual
    // constructor for the MyHttpModule class.
    virtual ~MyHttpModule()
    {

    }
};

// The MyHttpModuleFactory class implements the IHttpModuleFactory 
// interface, and creates a new MyHttpModule pointer, where this
// MyHttpModule listens for RQ_SEND_RESPONSE events.
class MyHttpModuleFactory : public IHttpModuleFactory
{
public:
    // The RegisterGlobalModule method creates a new 
    // MyHttpModuleFactory pointer, assigns that pointer 
    // as the factory on the pModuleInfo parameter, and
    // registers for RQ_SEND_RESPONSE events.
    // dwServerVersion: the current server version.
    // pModuleInfo: the IHttpModuleRegistrationInfo 
    // used for requesting notifications.
    // pGlobalInfo: the IHttpServer pointer.
    static HRESULT RegisterGlobalModule
    (
        DWORD dwServerVersion,
        IHttpModuleRegistrationInfo* pModuleInfo,
        IHttpServer* pGlobalInfo
    )
    {
        // Create a new MyHttpModuleFactory pointer.
        MyHttpModuleFactory* factory =
            new MyHttpModuleFactory;

        // If the factory is NULL, return a
        // return a ERROR_NOT_ENOUGH_MEMORY 
        // errror.
        if (NULL == factory)
        {
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }

        // Return the value from calling the 
        // SetRequestNotifications method by 
        // passing the factory for RQ_SEND_RESPONSE
        // events.
        return pModuleInfo->SetRequestNotifications(
            factory,
            RQ_SEND_RESPONSE,
            0);
    }

    // The GetHttpModule method creates a new MyHttpModule 
    // pointer and sets the ppModule with this pointer.
    // ppModule: the OUT parameter which 
    // accepts a new CHttpModule subclass.
    // pAllocator: the IModuleAllocator pointer, 
    // which is unused in this implementation.
    virtual HRESULT GetHttpModule
    (
        OUT CHttpModule** ppModule, 
        IN IModuleAllocator* pAllocator
    )
    {
        // Call the UNREFERENCED_PARAMETER with 
        // the IModuleAllocator pointer, because
        // this parameter is never used.
        UNREFERENCED_PARAMETER(pAllocator);        

        // Create a new MyHttpModule pointer and assign
        // it to the dereferenced ppModule pointer.
        *ppModule = new MyHttpModule;

        // If the dereferenced ppModule is NULL 
        // then heap memory is exhausted. Return
        // an ERROR_NOT_ENOUGH_MEMORY error.
        if (NULL == *ppModule)
        {
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }

        // Creating a new MyHttpModule pointer 
        // was successful; return S_OK.
        return S_OK;       
    }

    // The Terminate method must be implemented
    // by IHttpModuleFactory implementers, 
    // and calls delete on this.
    virtual void Terminate()
    {
        delete this;
    }

private:
    // The MyHttpModuleFactory is the 
    // protected constructor for the 
    // MyHttpModuleFactory class.
    MyHttpModuleFactory()
    {

    }

    // The MyHttpModuleFactory method is the 
    // protected virtual destructor for the 
    // MyHttpModuleFactory class.
    virtual ~MyHttpModuleFactory()
    {

    }
};

// The RegisterModule method is the 
// main entry point for the DLL.
// dwServerVersion: the current server version.
// pModuleInfo: the current 
// IHttpModuleRegistrationInfo pointer.
// pGlobalInfo: the current IHttpServer pointer.
// return: the HRESULT value returned from 
// the MyHttpModuleFactory::RegisterGlobalModule
// static method.
HRESULT
__stdcall
RegisterModule
(
    DWORD dwServerVersion,
    IHttpModuleRegistrationInfo* pModuleInfo,
    IHttpServer* pGlobalInfo
)
{    
    // Register for global events using the MyHttpModuleFactory 
    // class' static RegisterGlobalModule method.
    return MyHttpModuleFactory::RegisterGlobalModule
        (dwServerVersion, pModuleInfo, pGlobalInfo);  
}
// </Snippet10>