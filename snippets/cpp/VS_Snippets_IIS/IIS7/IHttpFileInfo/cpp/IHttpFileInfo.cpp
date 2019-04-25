// <Snippet1>
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
    // The ToString method converts a HANDLE to a wstring.
    // h: the HANDLE to convert to a wstring.
    // return: the HANDLE as a wstring.
    static wstring ToString(HANDLE h)
    {
        // If the HANDLE is NULL, return the "NULL" string.
        if (NULL == h)
        {
            return L"NULL";
        }

        // If the HANDLE is not valid, return 
        // the INVALID_HANDLE_VALUE as a string.
        if (INVALID_HANDLE_VALUE == h)
        {
            return L"INVALID_HANDLE_VALUE";
        }

        // The HANDLE is valid.
        return L"valid";
    }
    
    // The ToString method converts a FILETIME pointer to a wstring.
    // fileTime: the FILETIME pointer to convert to a wstring.
    // return: the FILETIME pointer as a wstring.
    static wstring ToString(FILETIME* fileTime)
    {
        // If fileTime is NULL, return the empty string.
        if (NULL == fileTime)
        {
            return L"NULL";
        }

        // Convert the FILETIME to a local time, and 
        // then convert that local time to a wstring.
        SYSTEMTIME stUTC;
        SYSTEMTIME stLocal;
        FileTimeToSystemTime(fileTime, &stUTC);
        SystemTimeToTzSpecificLocalTime(NULL, &stUTC, &stLocal);        

        // Create a wstring to return.  Note: wsprintf 
        // can be also used. However, it is more difficult 
        // to handle both UNICODE and non-UNICODE correctly.
        wstring timeString =
            ToString(stLocal.wMonth) +
            wstring(L"/")            +
            ToString(stLocal.wDay)   +
            wstring(L"/")            +
            ToString(stLocal.wYear)  +
            wstring(L"  ")           +
            ToString(stLocal.wHour)  +
            wstring(L":")            +
            ToString(stLocal.wMinute);

        // Return the FILETIME data as a wstring.
        return timeString;
    }
    
    // The ToString method converts a 
    // ULARGE_INTEGER pointer to a wstring.
    // ui: the ULARGE_INTEGER pointer to convert to a string.
    // return: the ULARGE_INTEGER pointer as a string.
    static wstring ToString(ULARGE_INTEGER* ui)
    {
        // Return the empty string if the 
        // ULARGE_INTEGER pointer is NULL.
        if (NULL == ui)
        {
            return L"NULL";
        }
        
        // Return the low-order part to a wstring.
        return (ToString(ui->LowPart));
    }
    
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
        
    // The ToString method converts a long to a wstring.
    // l: the long value to convert to a wstring.
    // return: the long as a wstring.
    static wstring ToString(long l)
    {
        WCHAR str[256];
        str[0] = '\0';        
        _ltow_s(l, str, 256, 10);
        return str;
    }
        
    // The ToString method converts a DWORD to a wstring.
    // d: the DWORD value to convert to a wstring.
    // return: the DWORD as a wstring.
    static wstring ToString(DWORD d)
    {
        return ToString((long)d);
    }
    
    // The ToString method converts an LPCGUID to a wstring.
    // guid: the LPCGUID value to convert to a wstring.
    // return: The LPCGUID as a wstring; otherwise, L"" if
    // guid is NULL.
    static wstring ToString(LPCGUID guid)
    {
        // If the GUID is NULL, return the empty string.
        if (NULL == guid)
        {
            return L"NULL";
        }

        // Create a WCHAR array to write the GUID to.
        WCHAR guidString[256];
        // Initialize the zero-based index of the 
        // guidString to the null-terminating character 
        // because the StringFromGUID2 may fail.
        guidString[0] = '\0';
        // Convert the GUID to a string of the form "{...}".
        int characters = StringFromGUID2(*guid, guidString, 256);
        // Return the guidString as a wstring.
        return guidString;
    }

    // The ToString method converts a BOOL to a wstring.
    // b: the BOOL value to convert to a wstring.
    // return: L"true" for true; otherwise, L"false" for false.
    static wstring ToString(BOOL b)
    {
        return (b) ? L"true" : L"false";
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

    // The ToString method converts a USHORT to a wstring.
    // u: the USHORT value to convert to a wstring.
    // return: the value of u as a wstring.
    static wstring ToString(USHORT u)
    {
        return (ToString((long)u));
    }

    // The ToString method converts a 
    // const BYTE pointer to a wstring.
    // bytes: the BYTE pointer to convert.
    // return: the value of bytes as a wstring.
    static wstring ToString(const BYTE* bytes)
    {
        return (ToString((PCSTR)bytes));    
    }

    // The ToString method converts 
    // a PCWSTR to a wstring.
    // pcwstr: the PCWSTR to convert.
    // return: L"NULL" if the pcwstr
    // parameter is NULL; otherwise, 
    // pcwstr converted to a wstring.
    static wstring ToString(PCWSTR pcwstr)
    {
        // If the pcwstr parameter
        // is NULL, return L"NULL".
        if (NULL == pcwstr)
        {
            return L"NULL";
        }

        // Implicitly convert the 
        // PCWSTR to a wstring.
        return pcwstr;
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

            // Declare a HANDLE for 
            // the user and initialize 
            // that HANDLE to NULL.
            HANDLE userHandle = NULL;

            // Get the IHttpUser pointer from 
            // the IHttpContext pointer.
            IHttpUser* user =
                pHttpContext->GetUser();

            // If the IHttpUser pointer 
            // is not NULL, reset the 
            // primary token HANDLE.
            if (NULL != user)
            {
                userHandle = 
                    user->GetPrimaryToken();
            }            

            // Call the AccessCheck method
            // on the IHttpFileInfo pointer.
            hr = fileInfo->AccessCheck(userHandle, NULL);

            // Convert the HRESULT to a wstring.
            wstring validAccess = CConvert::ToString(hr);

            // Write access information 
            // to the response stream.
            writer.WriteLine(L"Access", validAccess);

            // Call the CheckIfFileHasChanged method
            // on the IHttpFileInfo pointer.
            BOOL fileChanged = 
                fileInfo->CheckIfFileHasChanged(userHandle);

            // Convert the Boolean to a wstring.
            wstring changed = 
                CConvert::ToString(fileChanged);

            // Write file changed information
            // to the response stream.
            writer.WriteLine(L"File Changed", changed);

            // Call the GetAttributes method 
            // on the IHttpFileInfo pointer.
            DWORD attributes = 
                fileInfo->GetAttributes();

            // Convert the attribute
            // to a wstring.
            wstring attributesString = 
                CConvert::ToString(attributes);

            // Write attribute information 
            // to the response stream.
            writer.WriteLine(L"Attributes", attributesString);        

            // Convert a readonly file to a wstring.
            wstring readOnlyString =
                CConvert::ToString((BOOL)attributes & _A_RDONLY);

            // Write read-only information to
            // the response stream.
            writer.WriteLine(L"Read only", readOnlyString);

            // Convert a hidden file to a wstring.
            wstring hiddenFileString =
                CConvert::ToString((BOOL)attributes & _A_HIDDEN);

            // Write hidden information
            // to the response stream.
            writer.WriteLine(L"Hidden", hiddenFileString);

            // Convert archive information
            // to a wstring.
            wstring archiveString =
                CConvert::ToString((BOOL)attributes & _A_ARCH);

            // Write the archive information
            // to the response stream.
            writer.WriteLine(L"Archive", archiveString);

            // Declare a USHORT and 
            // initialize it to 0 for a 
            // call to the GetETag method.
            USHORT cchETag = 0;
            // Call the GetETag metohd on
            // the IHttpFileInfo pointer.
            PCSTR pszETag = fileInfo->GetETag(&cchETag);

            // Convert the etag information
            // to a wstring.
            wstring etagString =
                CConvert::ToString(pszETag);

            // Write the etag information
            // to the response stream.
            writer.WriteLine(L"ETag", etagString);        

            // Convert the USHORT to a wstring.
            wstring cchString = 
                CConvert::ToString(cchETag);

            // Write the etag information
            // to the response stream.
            writer.WriteLine(L"ETag Value", cchString);

            // Call the GetFileBuffer method
            // on the IHttpFileInfo pointer.
            const BYTE* fileBuffer = 
                fileInfo->GetFileBuffer();

            // Convert the const BYTE 
            // pointer to a wstring.
            wstring fileString =
                CConvert::ToString(fileBuffer);

            // Write the file buffer to
            // the response stream.
            writer.WriteLine(L"File Buffer", fileString);            

            // Call the GetFileHandle method
            // on the IHttpFileInfo pointer.
            HANDLE fileHandle = 
                fileInfo->GetFileHandle();

            // Convert the file 
            // HANDLE to a wstring.
            wstring fileHandleString = 
                CConvert::ToString(fileHandle);

            // Write the file information
            // to the response stream.
            writer.WriteLine(L"File Handle", fileHandleString);

            // Call the GetFilePath method
            // on the IHttpFileInfo pointer.
            PCWSTR filePathPCWSTR = 
                fileInfo->GetFilePath();

            // Convert the file 
            // path to a wstring.
            wstring filePath = 
                CConvert::ToString(filePathPCWSTR);

            // Write the file path to
            // the response stream.
            writer.WriteLine(L"File Path", filePath);        

            // Call the GetHttpCacheAllowed method
            // on the IHttpFileInfo pointer.
            DWORD timeToLive = 0;
            BOOL cacheAllowed = 
                fileInfo->GetHttpCacheAllowed(&timeToLive);

            // Convert the Boolean 
            // to a wstring.
            wstring cacheAllowedString = 
                CConvert::ToString(cacheAllowed);

            // Write the Boolean to
            // the response stream.
            writer.WriteLine(L"Cache Allowed",
                             cacheAllowedString);

            // Convert the time-to-live
            // to a wstring.
            wstring timeToLiveString = 
                CConvert::ToString(timeToLive);

            // Write the time-to-live information
            // to the response stream.
            writer.WriteLine(L"Time-to-Live",
                             timeToLiveString);

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

            // Call the GetLastModifiedTime method
            // on the IHttpFileInfo pointer.
            FILETIME fileTime;
            fileInfo->GetLastModifiedTime(&fileTime);

            // Convert the FILETIME 
            // structure to a wstring.
            wstring lastModifiedTimeString = 
                CConvert::ToString(&fileTime);

            // Write the last modified time
            // information to the response stream.
            writer.WriteLine(L"Last Modified Time",
                             lastModifiedTimeString);

            // Get the IHttpModuleContextContainer
            // from the IHttpFileInfo pointer.
            IHttpModuleContextContainer* contextContainer =
                fileInfo->GetModuleContextContainer();        

            // Convert the IHttpModuleContextContainer
            // pointer to a wstring.
            wstring contextString =
                CConvert::ToString(NULL != contextContainer);

            // Write the context data to
            // the response stream.
            writer.WriteLine(L"Context", contextString);

            // Call the GetSize method on
            // the IHttpFileInfo pointer.
            ULARGE_INTEGER sizeInteger;
            fileInfo->GetSize(&sizeInteger);

            // Convert the size to a wstring.
            wstring size = 
                CConvert::ToString(&sizeInteger);

            // Write the size information
            // to the response stream.
            writer.WriteLine(L"Size", size);

            // Call the GetVrPath method on 
            // the IHttpFileInfo pointer.
            PCWSTR vrPathPCWSTR = 
                fileInfo->GetVrPath();

            // Convert the VR path to a wstring.
            wstring vrPath = 
                CConvert::ToString(vrPathPCWSTR);

            // Write the VR path information
            // to the response stream.
            writer.WriteLine(L"VR Path", vrPath);        

            // Call the GetVrToken method 
            // on the IHttpFileInfo pointer.
            HANDLE vrToken = 
                fileInfo->GetVrToken();

            // Convert the VR token to a wstring.
            wstring vrTokenString = 
                CConvert::ToString(vrToken);

            // Write the token information 
            // to the response stream.
            writer.WriteLine(L"VR Token", vrTokenString);
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
// </Snippet1>