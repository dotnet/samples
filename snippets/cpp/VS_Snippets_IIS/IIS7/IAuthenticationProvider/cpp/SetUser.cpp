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

// The CHttpUser class implements the interface IHttpUser
// that emulates a user. This class creates a valid user 
// with the user name "ValidUser".
class CHttpUser : public IHttpUser
{
public:
    // PRE: none.
    // POST: sets the internal reference count to 1.
    // The CHttpUser method is the public 
    // constructor for the CHttpUser class.
    CHttpUser()
    {
        m_refs = 1;
    }

    // The GetRemoteUserName method 
    // returns the remote name of the user.    
    // return: L"ValidUser".
    virtual PCWSTR GetRemoteUserName(VOID)
    {
        return L"ValidUser";
    }
  
    // The GetUserName method 
    // returns the name of the user.
    // return: L"ValidUser".
    virtual PCWSTR GetUserName(VOID)
    {
        return L"ValidUser";
    }
    
    // The GetAuthenticationType method 
    // returns the authentication type 
    // for the user.
    // return: L"Anonymous".
    virtual PCWSTR GetAuthenticationType(VOID)
    {
        return L"Anonymous";
    }
    
    // The GetPassword method returns 
    // the password for the user. This 
    // password is empty because Anonymous
    // authentication only requires only a
    // non-NULL password value.
    // return: L"".
    virtual PCWSTR GetPassword(VOID)
    {
        return L"";
    }
 
    // The GetImpersonationToken method returns 
    // the impersonation token for the user.
    // return: NULL.
    virtual HANDLE GetImpersonationToken(VOID)
    {
        return NULL;
    }
   
    // The GetPrimaryToken method returns 
    // the primary token for the user.
    // return: NULL.
    virtual HANDLE GetPrimaryToken(VOID)
    {
        return NULL;
    }
    
    // PRE: none.
    // POST: the internal reference 
    // count is incremented by 1.
    // The ReferenceUser method should be called 
    // when a new reference to a user is accessed.    
    virtual VOID ReferenceUser(VOID)
    {
        InterlockedIncrement(&m_refs);
    }
   
    // PRE: the internal reference 
    // count is at least 1.
    // POST: decrements the internal reference count 
    // and deletes this if that count goes to 0.
    virtual VOID DereferenceUser(VOID)
    {
        if (0 == InterlockedDecrement(&m_refs))
        {
            delete this;
        }
    }

    // The SupportsIsInRole method returns a BOOL 
    // indicating whether this user supports roles.
    // return: FALSE.
    virtual BOOL SupportsIsInRole(VOID)
    {
        return FALSE;
    }
       
    // The IsInRole method returns E_NOTIMPL.
    // return: E_NOTIMPL.
    virtual HRESULT IsInRole
    (
        IN PCWSTR pszRoleName,
        OUT BOOL* pfInRole
    )
    {
        return E_NOTIMPL;
    }

    // The GetUserVariable method returns NULL.
    // return: NULL.
    virtual PVOID GetUserVariable
    (
        IN PCSTR pszVariableName
    )
    {
        return NULL;        
    }

private:
    // The CHttpUser method is the private virtual
    // destructor for the CHttpUser class.
    virtual ~CHttpUser()
    {
        
    }

    // Specify an internal value 
    // for reference counting.
    LONG m_refs;
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

        // Call the GetAuthenticationType 
        // method on the IHttpUser pointer.
        PCWSTR authTypePCWSTR = 
            m_user->GetAuthenticationType();

        // Convert the PCWSTR to a wstring.
        wstring authType = 
            CConvert::ToString(authTypePCWSTR);

        // Add an attribute to the
        // userElement element.
        AddAttribute(L"authType", authType, 
                     userElement, doc);

        // Call the GetImpersonationToken 
        // method on the IHttpUser pointer.
        HANDLE impersonationTokenHANDLE = 
            m_user->GetImpersonationToken();

        // Convert the HANDLE to a wstring.
        wstring impersonationToken =
            CConvert::ToString(impersonationTokenHANDLE);

        // Write an attribute to the 
        // userElement element.
        AddAttribute(L"impersonationToken", 
                     impersonationToken, 
                     userElement, 
                     doc);
                
        // Call the GetPassword method 
        // on the IHttpUser pointer.
        PCWSTR passWordPCWSTR = 
            m_user->GetPassword();

        // Convert the PCWSTR to a wstring.
        wstring passWord =
            CConvert::ToString(passWordPCWSTR);

        // Add an attribute to the
        // userElement element.
        AddAttribute(L"passWord", L"[hidden]", 
                     userElement, doc);
                
        // Call the GetPrimaryToken method
        // on the IHttpUser pointer.
        HANDLE primaryTokenHANDLE = 
            m_user->GetPrimaryToken();

        // Convert the HANDLE to a wstring.
        wstring primaryToken =
            CConvert::ToString(primaryTokenHANDLE);

        // Add an attribute to the
        // userElement element.
        AddAttribute(L"primaryToken", 
                     primaryToken, 
                     userElement, 
                     doc);            

        // Call the GetRemoteUserName method
        // on the IHttpUser pointer.
        PCWSTR remoteNamePCWSTR = 
            m_user->GetRemoteUserName();

        // Convert the PCWSTR to a wstring.
        wstring remoteName =
            CConvert::ToString(remoteNamePCWSTR);

        // Add an attribute to
        // the userElement element.
        AddAttribute(L"remoteName", remoteName, 
                     userElement, doc);

        // Call the GetUserName method
        // on the IHttpUser pointer.
        PCWSTR userNamePCWSTR = 
            m_user->GetUserName();

        // Convert the PCWSTR to a wstring.
        wstring userName =
            CConvert::ToString(userNamePCWSTR);

        // Add an attribute to
        // the userElement attribute.
        AddAttribute(L"userName", userName, 
                     userElement, doc);

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

        // Call the SupportsIsInRole method
        // on the IHttpUser pointer.
        BOOL supportsRolesBOOL = 
            m_user->SupportsIsInRole();

        // Convert the BOOL to a wstring.
        wstring supportsRoles =
            CConvert::ToString(supportsRolesBOOL);

        // Add an attribute to the
        // userElement element.
        AddAttribute(L"supportsRoles", 
                     supportsRoles, 
                     userElement, 
                     doc);

        // If the user supports roles, create 
        // an Unknown role that the user 
        // should not be a member of.
        if (supportsRolesBOOL)
        {
            // Declare a BOOL for a 
            // role that is unknown.
            BOOL isInRoleBOOL = FALSE;

            // Call the IsInRole method 
            // on the IHttpUser pointer.
            HRESULT hr = 
                m_user->IsInRole(L"Unknown", &isInRoleBOOL);

            // Convert the BOOL to a wstring.
            wstring isInRole = 
                CConvert::ToString(isInRoleBOOL);

            // Add an attribute to
            // the userElement element.
            AddAttribute(L"isInRole", 
                         isInRole, 
                         userElement, 
                         doc);

            // Convert the HRESULT to a wstring.
            wstring isInRoleHresult =
                CConvert::ToString(hr);

            // Add an attribute to the
            // userElement element.
            AddAttribute(L"isInRoleCode",
                         isInRoleHresult,
                         userElement,
                         doc);
        }
                
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

// The CAuthenticateModule extends the CHttpModule 
// class and authenticates requests. The CAuthenticateModule 
// class implements the OnAuthenticateRequest and 
// OnPostAuthenticateRequest methods and returns
// RQ_NOTIFICATION_FINISH_REQUEST from those methods 
// if the user cannot be verified.
class CAuthenticateModule : public CHttpModule
{
public:
    // The CAuthenticateModule method is the public 
    // constructor for the CAuthenticateModule class.
    CAuthenticateModule()
    {

    }

    // The CAuthenticateModule method is the public
    // destructor for the CAuthenticateModule class.
    virtual ~CAuthenticateModule()
    {

    }

    // The OnAuthenticateRequest method authenticates all
    // users by simulating Anonymous authentication.
    // pHttpContext: the current IHttpContext pointer.
    // pProvider: the current IAuthenticationProvider pointer.
    // return: RQ_NOTIFICATION_CONTINUE.
    virtual 
    REQUEST_NOTIFICATION_STATUS
    OnAuthenticateRequest
    (
        IN IHttpContext* pHttpContext,
        IN OUT IAuthenticationProvider* pProvider
    )
    {        
        // Attempt to get the IHttpUser pointer 
        // from the IHttpContext pointer.
        IHttpUser* currentUser =
            pHttpContext->GetUser();

        // The IHttpUser pointer will be NULL if the 
        // user has not yet been authenticated.
        if (NULL == currentUser)
        {
            // Create a new custom anonymous user
            // that is always authenticated.
            CHttpUser* httpUser = 
                new CHttpUser;
            // Set the authenticated user on the 
            // provider to signal that this user 
            // is authentated.
            pProvider->SetUser(httpUser);            
        }        
                
        // The user should now be authenticated.
        return RQ_NOTIFICATION_CONTINUE;
    }

    // The OnPostAuthenticateRequest method 
    // overrides the CHttpModule functionality.
    // pHttpContext: the current IHttpContext pointer.
    // pProvider: the current IHttpEventProvider pointer.
    // return: RQ_NOTIFICATION_CONTINUE.
    virtual 
    REQUEST_NOTIFICATION_STATUS
    OnPostAuthenticateRequest
    (
        IN IHttpContext* pHttpContext,
        IN OUT IHttpEventProvider* pProvider
    )
    {
        UNREFERENCED_PARAMETER(pHttpContext);
        UNREFERENCED_PARAMETER(pProvider);        
                
        return RQ_NOTIFICATION_CONTINUE;
    }

    // The OnSendResponse method handles 
    // the RQ_SEND_RESPONSE events.
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
        
        // Retrieve the IHttpUser pointer 
        // from the IHttpContext pointer.
        IHttpUser* user = pHttpContext->GetUser();        
        
        // Retrieve the IHttpResponse pointer 
        // from the IHttpContext pointer.
        IHttpResponse* response = 
            pHttpContext->GetResponse();        
        
        // Both the user and response must not
        // be NULL. The response must be cleared,
        // the response's header must be changed,
        // and the CUser constructor precondition
        // requires a non-NULL IHttpUser pointer.
        if ((NULL == user) || (NULL == response))
        {
            return RQ_NOTIFICATION_CONTINUE;
        }

        // Create a writer to write to
        // the response stream.
        CResponseWriter writer(response);
        
        try
        {            
            // Create an IHttpUser pointer wrapper 
            // that will increment and decrement the 
            // reference count, and provide writing
            // user information to the response.
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

            // If the SetHeader method call failed, 
            // ignore the failure for now. The user
            // can run View/Source in a browser for the
            // returned text. It is also possible to
            // return here, if necessary.

            // Clear the response before writing
            // the XML data to the response.
            response->Clear();

            // Write the XML to 
            // the response stream.
            writer.Write(xmlString.c_str());
        }
        catch (_com_error& ce)
        {
            // Write the error description
            // to the response stream.
            _bstr_t description =
                ce.Description();

            writer.Write(description);
        }        
        
        // Return the RQ_NOTIFICATION_CONTINUE message.
        return RQ_NOTIFICATION_CONTINUE;            
    }
};

// The CAuthenticateFactory class implements the 
// IHttpModuleFactory interface and creates a new 
// CAuthenticateModule pointer and registers that 
// pointer for authentication events.
class CAuthenticateFactory : public IHttpModuleFactory
{
public:
    // The RegisterCHttpModule static method creates 
    // a new CAuthenticateFactory pointer and sets
    // that pointer on the pModuleInfo as a class factory
    // by using the SetRequestNotifications method.
    // dwServerVersion: the current server version.
    // pModuleInfo: the current IHttpModuleRegistrationInfo 
    // pointer to call the SetRequestNotifications method.
    // pGlobalInfo: the current IHttpServer pointer.
    // return: the value returned from the 
    // SetRequestNotifications call on the pModuleInfo pointer.
    static HRESULT RegisterCHttpModule
    (
        DWORD dwServerVersion,
        IHttpModuleRegistrationInfo* pModuleInfo,
        IHttpServer* pGlobalInfo
    )
    {
        // Create a new CAuthenticateFactory pointer.    
        CAuthenticateFactory* moduleFactory =
            new CAuthenticateFactory;

        // Return an out-of-memory error if the 
        // CAuthenticateFactory pointer is NULL.
        if (NULL == moduleFactory)
        {
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }        

        // Set the new CAuthenticateFactory pointer to 
        // receive authentication requests, and the 
        // response to send out a final response.
        HRESULT hr = 
            pModuleInfo->SetRequestNotifications
                (moduleFactory,
                 RQ_AUTHENTICATE_REQUEST | RQ_SEND_RESPONSE,
                 0);

        // If the call to the SetRequestNotifications method 
        // failed, return the HRESULT from that call.
        if (FAILED(hr))
        {
            return hr;                
        }

        // Set the priority to the highest so that 
        // the user will be authenticated immediately.
        return pModuleInfo->SetPriorityForRequestNotification
            (RQ_AUTHENTICATE_REQUEST, 
             PRIORITY_ALIAS_FIRST);        
    }

    // The GetHttpModule method creates a new 
    // CAuthenticateModule pointer and assigns this pointer 
    // on the dereferenced CHttpModule pointer.
    // ppModule: the dereferenced CHttpModule pointer 
    // that will be a new CAuthenticateModule pointer.
    // pAllocator: the optional IModuleAllocator pointer.
    // return: ERROR_NOT_ENOUGH_MEMORY if the heap is 
    // out of memory; otherwise, S_OK.
    virtual
    HRESULT
    GetHttpModule
    (
        OUT CHttpModule** ppModule, 
        IN IModuleAllocator* pAllocator
    )
    {
        // Call the UNREFERENCED_PARAMETER macro 
        // with the IModuleAllocator pointer because 
        // this parameter is never used.
        UNREFERENCED_PARAMETER(pAllocator);

        // Create a new CAuthenticateModule pointer 
        // and set that pointer on the dereferenced 
        // CHttpModule parameter.
        (*ppModule) = new CAuthenticateModule;

        // If the dereferenced ppModule pointer 
        // is NULL, the heap has been exhausted.
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
    // The CAuthenticateFactory method is the protected
    // constructor for the CAuthenticateFactory class.
    CAuthenticateFactory()
    {

    }

    // The CAuthenticateFactory method is 
    // the protected virtual destructor for 
    // the CAuthenticateFactory class.
    virtual ~CAuthenticateFactory()
    {

    }
};

// The RegisterModule method is the 
// main entry point for the DLL.
// dwServerVersion: the current server version.
// pModuleInfo: the current 
// IHttpModuleRegistrationInfo pointer.
// pGlobalInfo: the current IHttpServer pointer.
// return: the value returned by calling the 
// CAuthenticateFactory::RegisterCHttpModule method.
HRESULT
__stdcall
RegisterModule(
    DWORD dwServerVersion,
    IHttpModuleRegistrationInfo* pModuleInfo,
    IHttpServer* pGlobalInfo
)
{
    return CAuthenticateFactory::RegisterCHttpModule
        (dwServerVersion, 
         pModuleInfo, 
         pGlobalInfo);    
}
// </Snippet1>