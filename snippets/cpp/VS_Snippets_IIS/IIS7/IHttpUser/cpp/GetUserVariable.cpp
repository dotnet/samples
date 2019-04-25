// <Snippet8>
#pragma warning( disable : 4290 )
#pragma warning( disable : 4530 )

#define _WINSOCKAPI_
#include <windows.h>
#include <io.h>
#include <sal.h>
#include <httpserv.h>

#include <string>
using namespace std;

#import "msxml6.dll"
using namespace MSXML2;

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

// The CUser class is the wrapper 
// class for an IHttpUser pointer.
class CUser
{
public:
    // Creates the CUser class.
    // user: the IHttpUser pointer to reference.
    // throws: a _com_error exception if 
    // the IHttpUser parameter is NULL.
    CUser(IHttpUser* user)
        throw (_com_error)
    {
        // Throw an exception if the 
        // IHttpUser parameter is NULL.
        if (NULL == user)
        {
            _com_error ce(E_INVALIDARG);
            throw ce;
        }

        // Call ReferenceUser, which increments
        // the reference count for the user.
        user->ReferenceUser();
        // Assign the user to m_user.
        m_user = user;
    }

    // The CUser method is the public 
    // destructor for the CUser class.
    // The destructor calls the DereferenceUser
    // method on the IHttpUser pointer, and
    // sets that pointer to NULL.
    virtual ~CUser()
    {
        // Call DereferenceUser on the m_user private 
        // data, which decrements the reference count
        // on m_user. An IHttpUser will be deleted when 
        // its reference count goes to 0.
        m_user->DereferenceUser();
        // Set the m_user data to NULL.
        m_user = NULL;
    }

    // The CreateElement method converts 
    // the internal IHttpUser pointer and 
    // descendant data into an XML element.
    // doc: the MSXML2::IXMLDOMDocument3Ptr to use 
    // for creating elements and attributes.
    // return: a new IXMLDOMElementPtr pointer.
    // throws: a _com_error exception.
    MSXML2::IXMLDOMElementPtr CreateElement
    (
        MSXML2::IXMLDOMDocument3Ptr doc
    ) const throw (_com_error)
    {
        // Create a new XML element to return.
        MSXML2::IXMLDOMElementPtr userElement = 
            doc->createElement(L"user");

        // Call the GetUserVariable method 
        // on the IHttpUser pointer.
        PVOID userVariablePVOID = 
            m_user->GetUserVariable(HTTP_USER_VARIABLE_SID);

        // Convert the PVOID to a wstring.
        wstring userVariable =
            CConvert::ToString(userVariablePVOID);

        // Add an attribute to the
        // userElement element.
        AddAttribute(L"userVariable", 
                     userVariable, 
                     userElement, 
                     doc);

        // Return the XML element.
        return userElement;
    }
protected:
    // The AddAttribute method creates a new XML 
    // attribute and appends it to the element
    // parameter.
    // name: the name of the attribute.
    // text: the text value of the attribute.
    // element: the XML element to 
    // append the new attribute to.
    // doc: the XML document to use for
    // creating attributes.
    // return: a new IXMLDOMAttributePtr.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMAttributePtr AddAttribute
    (
        const wstring& name,
        const wstring& text,
        MSXML2::IXMLDOMElementPtr element,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create an attribute with the name.
        MSXML2::IXMLDOMAttributePtr attribute = 
            doc->createAttribute(name.c_str());
        // Set the text of the attribute to the text value.
        attribute->text = text.c_str();
        // Add the attribute to the passed element.
        element->setAttributeNode(attribute);
        // Return the new attribute to the caller.
        return attribute;
    }
private:
    // Specify the IHttpUser pointer 
    // to authenticate and authorize.
    IHttpUser* m_user;
};

// The CHttpResponseModule is a CHttpModule 
// class that handles response processing
// by creating a custom user and writing that
// user information to the response stream.
class CHttpResponseModule : public CHttpModule
{
public:
    // The CHttpResponseModule method is the public
    // constructor for the CHttpResponseModule class.
    CHttpResponseModule()
    {

    }

    // The CHttpResponseModule method is 
    // the public virtual destructor for 
    // the CHttpResponseModule class.
    virtual ~CHttpResponseModule()
    {

    }

    // The OnSendResponse method is the method 
    // supporting the RQ_SEND_RESPONSE event type.
    // pHttpContext: the current IHttpContext pointer.
    // pProvider: the current IHttpEventProvider pointer.
    // return: RQ_NOTIFICATION_CONTINUE.
    virtual 
    REQUEST_NOTIFICATION_STATUS
    OnSendResponse
    (
        IN IHttpContext* pHttpContext,
        IN OUT ISendResponseProvider* pProvider
    )
    {
        // Call the UNREFERENCED_PARAMETER macro because
        // the ISendResponseProvider is never used.
        UNREFERENCED_PARAMETER(pProvider);

        // If the IHttpContext is NULL, 
        // return RQ_NOTIFICATION_CONTINUE.
        if (NULL == pHttpContext)
        {
            return RQ_NOTIFICATION_CONTINUE;
        }

        // Retrieve the IHttpUser pointer 
        // from the IHttpContext pointer.
        IHttpUser* user = 
            pHttpContext->GetUser();

        // Retrieve the IHttpResponse pointer 
        // from the IHttpContext pointer.
        IHttpResponse* response = 
            pHttpContext->GetResponse();

        // Both the IHttpUser and 
        // IHttpResponse pointers must 
        // not be NUL to continue
        // response processing.
        if ((NULL == user) || (NULL == response))
        {
            return RQ_NOTIFICATION_CONTINUE;
        }

        // Wrap calls in a try-catch 
        // statement as a _com_error 
        // exception might be thrown.
        try
        {
            // Create a CUser wrapper
            // for the IHttpUser pointer.
            CUser userWrapper(user);

            // Create a new XML document.
            MSXML2::IXMLDOMDocument3Ptr 
                doc(__uuidof(MSXML2::DOMDocument60));

            // Get the user XML element from
            // the CUser class, and append 
            // this element to the XML document.
            MSXML2::IXMLDOMElementPtr userElement =
                userWrapper.CreateElement(doc);
            doc->appendChild(userElement);
                        
            // Create the string for writing 
            // the XML to the response stream.
            _bstr_t bstrXml = doc->xml;
            string xmlString = 
                "<?xml version=\"1.0\"?>" + bstrXml;

            // Reset the header for text/xml so that
            // browsers will display the XML correctly.
            HRESULT hr = 
                response->SetHeader(HttpHeaderContentType, 
                                    "text/xml", 
                                    (USHORT)strlen("text/xml"), 
                                     TRUE);

            // Do not throw an exception here.
            // If the header cannot be set, then
            // the user should be able to view
            // the XML by selecting the
            // "View\Source" menu item.

            // Clear the response before writing
            // the XML data to the response.
            response->Clear();

            // Create a writer.
            CResponseWriter writer(response);

            // Write the XML to the 
            // response stream.
            writer.Write(xmlString.c_str());
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

        // Return the RQ_NOTIFICATION_CONTINUE message.
        return RQ_NOTIFICATION_CONTINUE;
    }
};

// The CHttpResponseFactory class implements the 
// IHttpModuleFactory interface and creates and 
// sets a new CHttpResponseModule listener for 
// the RQ_SEND_RESPONSE event on the 
// IHttpModuleRegistrationInfo pointer.
class CHttpResponseFactory : public IHttpModuleFactory
{
public:
    // The RegisterCHttpModule method creates a new 
    // CHttpResponseFactory pointer and sets this new 
    // CHttpResponseFactory pointer as the IHttpModuleFactory 
    // pointer on the IHttpModuleRegistrationInfo pointer.
    // dwServerVersion: the current server version.
    // pModuleInfo: the current 
    // IHttpModuleRegistrationInfo pointer.
    // pGlobalInfo: the current IHttpServer pointer.
    // return: the value returned from the the call to 
    // the SetRequestNotifications on the 
    // IHttpModuleRegistrationInfo pointer.
    static HRESULT RegisterCHttpModule
    (
        DWORD dwServerVersion,
        IHttpModuleRegistrationInfo* pModuleInfo,
        IHttpServer* pGlobalInfo
    )
    {
        // Create a new CHttpResponseFactory pointer.
        CHttpResponseFactory* moduleFactory = 
            new CHttpResponseFactory;

        // Test for NULL on the new 
        // CHttpResponseFactory pointer.
        if (NULL == moduleFactory)
        {
            // Return an out-of-memory error 
            // code if moduleFactory is NULL.
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }

        // Set the request notifications for RQ_SEND_RESPONSE 
        // messages on the new CHttpResponseFactory pointer.
        return pModuleInfo->SetRequestNotifications
                (moduleFactory,
                 RQ_SEND_RESPONSE,
                 0); 
    }

    // The GetHttpModule method creates a new 
    // CHttpResponseModule pointer and sets the 
    // new CHttpResponseModule on the ppModule parameter.
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
        // Call the UNREFERENCED_PARAMETER macro with 
        // the IModuleAllocator pointer, because this
        // parameter is currently unused.
        UNREFERENCED_PARAMETER(pAllocator);

        // Set the dereferenced ppModule 
        // to a new CHttpResponseModule pointer.
        (*ppModule) = new CHttpResponseModule;

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

    // The Terminate method calls delete on this.
    virtual 
    void
    Terminate()
    {
        delete this;
    }

protected:
    // The CHttpResponseFactory method is the
    // protected constructor for the 
    // CHttpResponseFactory class.
    CHttpResponseFactory()
    {

    }

    // The CHttpResponseFactory method is the 
    // protected virtual destructor for the
    // CHttpResponseFactory class.
    virtual ~CHttpResponseFactory()
    {

    }
};

// The RegisterModule method is the 
// main entry point for the DLL.
// dwServerVersion: the current server version.
// pModuleInfo: the current 
// IHttpModuleRegistrationInfo pointer.
// pGlobalInfo: the current IHttpServer pointer.
// return: the value returned by calling the static
// CHttpResponseFactory::RegisterCHttpModule method.
HRESULT
__stdcall
RegisterModule
(
    DWORD dwServerVersion,
    IHttpModuleRegistrationInfo* pModuleInfo,
    IHttpServer* pGlobalInfo
)
{
    return CHttpResponseFactory::RegisterCHttpModule
        (dwServerVersion,
         pModuleInfo,
         pGlobalInfo);
}
// </Snippet8>