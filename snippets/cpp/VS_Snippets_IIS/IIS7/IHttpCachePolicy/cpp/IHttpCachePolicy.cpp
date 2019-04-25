// <Snippet1>
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

// The CCachePolicyModule is a CHttpModule 
// class that handles response processing
// by updating the IHttpCachePolicy pointer
// and writing that IHttpCachePolicy data
// to the response stream.
class CCachePolicyModule : public CHttpModule
{
public:
    // The CCachePolicyModule method 
    // is the public constructor for 
    // the CHttpResponseModule class.
    CCachePolicyModule()
    {
        
    }

    // The CHttpResponseModule method is 
    // the public virtual destructor for 
    // the CHttpResponseModule class.
    virtual ~CCachePolicyModule()
    {

    }

    // The OnBeginRequest method handles 
    // request processing when the request 
    // is first placed into the pipeline.
    // This method sets values on the 
    // IHttpCachePolicy pointer.
    // pHttpContext: the IHttpContext pointer.
    // pProvider: the IHttpEventProvider pointer.
    // return:  RQ_NOTIFICATION_CONTINUE.
    virtual 
    REQUEST_NOTIFICATION_STATUS
    OnBeginRequest
    (
        IN IHttpContext* pHttpContext,
        IN IHttpEventProvider* pProvider
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

        // Append the first header value.
        policy->AppendVaryByHeader("header1");

        // Append the second header value.
        policy->AppendVaryByHeader("header2");

        // Append the first query string.
        policy->AppendVaryByQueryString("query1");

        // Append the second query string.
        policy->AppendVaryByQueryString("query2");

        // Disable the user cache.
        policy->DisableUserCache();

        // Get the HTTP_CACHE_POLICY for the kernel 
        // from the IHttpCachePolicy pointer.
        HTTP_CACHE_POLICY* kernelPolicy =
            policy->GetKernelCachePolicy();

        // If the kernelPolicy is not 
        // NULL then set its values.
        if (NULL != kernelPolicy)
        {
            // Set the kernelPolicy to
            // HttpCachePolicyTimeToLive.
            kernelPolicy->Policy = HttpCachePolicyTimeToLive;

            // Set the seconds to live.
            kernelPolicy->SecondsToLive = 2;
        }

        // Get the HTTP_CACHE_POLICY for the 
        // user from the IHttpCachePolicy pointer.
        HTTP_CACHE_POLICY* userPolicy =
            policy->GetUserCachePolicy();

        // If the userPolicy is not
        // NULL then set its values.
        if (NULL != userPolicy)
        {
            // Set the userPolicy to
            // HttpCachePolicyMaximum.
            userPolicy->Policy = HttpCachePolicyMaximum;

            // Set the seconds to live.
            userPolicy->SecondsToLive = 5;
        }

        // Set the data as cached.
        policy->SetIsCached();

        // Invalidate the kernel cache.
        policy->SetKernelCacheInvalidatorSet();

        // Set the VaryByValue value.
        policy->SetVaryByValue("vary");

        // Return RQ_NOTIFICATION_CONTINUE so that
        // other handlers will receive the event.
        return RQ_NOTIFICATION_CONTINUE;
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

        try
        {
            // Create an XML document in memory.
            MSXML2::IXMLDOMDocument3Ptr doc
                (__uuidof(MSXML2::DOMDocument60));
            
            // Create a response element 
            // for the root XML element.
            MSXML2::IXMLDOMElementPtr responseElement =
                doc->createElement(L"response");

            // Append the responseElement 
            // to the XML document.
            doc->appendChild(responseElement);

            // Create a policyElement element.
            MSXML2::IXMLDOMElementPtr policyElement =
                CreateElement(policy, doc);

            // Append the policyElement 
            // to the responseElement.
            responseElement->appendChild(policyElement);

            // Clear the existing response.
            response->Clear();

            // Set the MIME type to text/xml.
            response->SetHeader(
                HttpHeaderContentType,
                "text/xml",
                (USHORT)strlen("text/xml"),
                TRUE);

            // Get the XML from the document.
            _bstr_t xml = doc->xml;

            // Add version information 
            // to the XML string.
            string xmlString = 
                "<?xml version=\"1.0\"?>" + xml;

            // Create a writer to write
            // the response data to.
            CResponseWriter writer(response);

            // Write the XML to the
            // response stream.
            writer.Write(xmlString.c_str());
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
    // The AddAttribute method creates a new 
    // XML attribute and adds it to an XML element.
    // name: the name of the attribute.
    // text: the text of the attribute.
    // element: the XML element to 
    // append the attribute to.
    // doc: the XML document for creating 
    // elements and attributes.
    // return: a new MSXML2::IXMLDOMAttributePtr.
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
        // Set the text of the 
        // attribute to the text value.
        attribute->text = text.c_str();
        // Add the attribute 
        // to the passed element.
        element->setAttributeNode(attribute);
        // Return the new attribute 
        // to the caller.
        return attribute;
    }

    // The CreateAttribute method creates an 
    // XML attribute representing the data
    // in the HTTP_CACHE_POLICY_TYPE parameter.
    // policyType: the HTTP_CACHE_POLICY_TYPE.
    // name: the name to give the XML attribute.
    // doc: the XML document to use for
    // creating elements and attributes.
    // return: a new MSXML2::IXMLDOMAttributePtr.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMAttributePtr CreateAttribute
    (
        HTTP_CACHE_POLICY_TYPE policyType,
        const wstring& name,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create an attribute for the CACHE_OPERATION value.
        MSXML2::IXMLDOMAttributePtr policyTypeAttribute =
            doc->createAttribute(name.c_str());        

        // Create an empty string to write the 
        // value of the HTTP_CACHE_POLICY_TYPE to.
        wstring value;

        // Write to the value string dependent
        // upon the value of the HTTP_CACHE_POLICY_TYPE.
        switch (policyType)
        {
            case HttpCachePolicyNocache:
            {
                value = L"HttpCachePolicyNocache";
                break;
            }
            case HttpCachePolicyUserInvalidates:
            {
                value = L"HttpCachePolicyUserInvalidates";
                break;
            }
            case HttpCachePolicyTimeToLive:
            {
                value = L"HttpCachePolicyTimeToLive";
                break;
            }
            case HttpCachePolicyMaximum:
            {
                value = L"HttpCachePolicyMaximum";
                break;
            }
            default:
            {
                value = L"Error";
                break;
            }
        }

        // Set the attribute text 
        // using the value string.
        policyTypeAttribute->text = value.c_str();
        
        // Return the new attribute to the caller.
        return policyTypeAttribute;        
    }    

    // The CreateElement method writes the value
    // of an HTTP_CACHE_POLICY to a new XML element.
    // cachePolicy: the HTTP_CACHE_POLICY
    // pointer to write.
    // name: the name to use for the 
    // HTTP_CACHE_POLICY_TYPE attribute.
    // doc: the XML document to use for 
    // creating elements and attributes.
    // return: a new MSXML2::IXMLDOMElementPtr.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMElementPtr CreateElement
    (
        HTTP_CACHE_POLICY* cachePolicy,
        const wstring& name,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create a new XML element to return.
        MSXML2::IXMLDOMElementPtr cachePolicyElement =
            doc->createElement(L"cachePolicy");

        // If the HTTP_CACHE_POLICY pointer 
        // is NULL then return.
        if (NULL == cachePolicy)
        {
            return cachePolicyElement;
        }

        // Get the HTTP_CACHE_POLICY_TYPE from
        // the HTTP_CACHE_POLICY pointer.
        HTTP_CACHE_POLICY_TYPE policyType =
            cachePolicy->Policy;

        // Create an XML attribute for the
        // HTTP_CACHE_POLICY_TYPE.
        MSXML2::IXMLDOMAttributePtr policyTypeAttribute =
            CreateAttribute(policyType, name, doc);

        // Add the policyTypeAttribute 
        // to the cachePolicyElement.
        cachePolicyElement->setAttributeNode(policyTypeAttribute);

        // Get the seconds to live from
        // the HTTP_CACHE_POLICY pointer.
        ULONG secondsToLiveULONG =
            cachePolicy->SecondsToLive;

        // Convert the ULONG to a wstring.
        wstring secondsToLive =
            CConvert::ToString(secondsToLiveULONG);

        // Add a secondsToLive attribute 
        // to the cachePolicyElement.
        AddAttribute(L"secondsToLive", secondsToLive, 
                     cachePolicyElement, doc);

        // Return the cachePolicyElement 
        // to the caller.
        return cachePolicyElement;
    }

    // The CreateElement method converts an
    // IHttpCachePolicy pointer to an XML element.
    // httpCachePolicy: the IHttpCachePolicy
    // pointer to convert.
    // doc: the XML document to use for
    // creating elements and attributes.
    static MSXML2::IXMLDOMElementPtr CreateElement
    (
        IHttpCachePolicy* httpCachePolicy,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create an element for the 
        // IHttpCachePolicy pointer.
        MSXML2::IXMLDOMElementPtr httpCachePolicyElement =
            doc->createElement(L"httpCachePolicy");

        // If the IHttpCachePolicy 
        // pointer is NULL, return.
        if (NULL == httpCachePolicy)
        {
            return httpCachePolicyElement;
        }

        // Get the HTTP_CACHE_POLICY 
        // pointer for the kernel from 
        // the IHttpCachePolicy pointer.
        HTTP_CACHE_POLICY* kernelCachePolicy =
            httpCachePolicy->GetKernelCachePolicy();

        // Create an XML element for the
        // HTTP_CACHE_POLICY pointer.
        MSXML2::IXMLDOMElementPtr kernelCacheElement =
            CreateElement(kernelCachePolicy, L"kernelPolicy", doc);

        // Append the kernelCacheElement to
        // the httpCachePolicyElement.
        httpCachePolicyElement->appendChild(kernelCacheElement);

        // Get the HTTP_CACHE_POLICY 
        // pointer from the IHttpCachePolicy 
        // pointer for the user.
        HTTP_CACHE_POLICY* userCachePolicy =
            httpCachePolicy->GetUserCachePolicy();

        // Create an XML element for the 
        // HTTP_CACHE_POLICY pointer for the user.
        MSXML2::IXMLDOMElementPtr userCacheElement =
            CreateElement(userCachePolicy, L"userPolicy", doc);

        // Append the userCacheElement to
        // the httpCachePolicyElement.
        httpCachePolicyElement->appendChild(userCacheElement);

        // Get the vary-by-headers from
        // the IHttpCachePolicy pointer.
        PCSTR varyByHeadersPCSTR = 
            httpCachePolicy->GetVaryByHeaders();

        // Convert the vary-by-headers 
        // to a wstring.
        wstring varyByHeaders = 
            CConvert::ToString(varyByHeadersPCSTR);

        // Add a varyByHeaders attribute 
        // to the httpCachePolicyElement.
        AddAttribute(L"varyByHeaders", varyByHeaders, 
                     httpCachePolicyElement, doc);

        // Get the vary-by-query string data
        // from the IHttpCachePolicy pointer.
        PCSTR varyByQueryPCSTR =
            httpCachePolicy->GetVaryByQueryStrings();

        // Convert the vary-by-query 
        // string to a wstring.
        wstring varyByQuery = 
            CConvert::ToString(varyByQueryPCSTR);

        // Add a varyByQuery attribute to
        // the httpCachePolicyElement.
        AddAttribute(L"varyByQuery", varyByQuery, 
                     httpCachePolicyElement, doc);

        // Get the vary-by-value value from
        // the IHttpCachePolicy pointer.
        PCSTR varyByValuePCSTR = 
            httpCachePolicy->GetVaryByValue();

        // Convert the vary-by-value
        // value to a wstring.
        wstring varyByValue = 
            CConvert::ToString(varyByValuePCSTR);

        // Add a varyByValue attribute
        // to the httpCachePolicyElement.
        AddAttribute(L"varyByValue", varyByValue, 
                     httpCachePolicyElement, doc);

        // Get a value indicating if 
        // the user data is cached.
        BOOL isCachedBOOL = 
            httpCachePolicy->IsCached();

        // Convert the BOOL to a wstring.
        wstring isCached = 
            CConvert::ToString(isCachedBOOL);

        // Add an isCached attribute to
        // the httpCachePolicyElement.
        AddAttribute(L"isCached", isCached, 
                     httpCachePolicyElement, doc);
        
        // Get a value indicating if the user cache 
        // is enabled from the IHttpCachePolicy pointer.
        BOOL isUserCacheEnabledBOOL =
            httpCachePolicy->IsUserCacheEnabled();

        // Convert the BOOL to a wstring.
        wstring isUserCacheEnabled = 
            CConvert::ToString(isUserCacheEnabledBOOL);

        // Add an isUserCacheEnabled attribute
        // to the httpCachePolicyElement.
        AddAttribute(L"isUserCacheEnabled", isUserCacheEnabled, 
                     httpCachePolicyElement, doc);

        // Return the httpCachePolicyElement.
        return httpCachePolicyElement;
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
                 RQ_BEGIN_REQUEST | RQ_SEND_RESPONSE,
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
// </Snippet1>