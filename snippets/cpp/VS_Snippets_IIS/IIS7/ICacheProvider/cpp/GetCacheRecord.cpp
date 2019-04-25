// <Snippet4>
#pragma warning( disable : 4290 )
#pragma warning( disable : 4530 )

#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <tchar.h>
#include <initguid.h>
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

    // The ToString method converts 
    // a character to a wstring.
    // c: the character to convert.
    // return: c as a wstring.
    static wstring ToString(char c)
    {
        wstring str;
        str += c;
        return str;
    }

    // The ToString method converts
    // a short to a wstring.
    // s: the short to convert.
    // return: s as a wstring.
    static wstring ToString(short s)
    {
        return (ToString((long)s));
    }
};

// The CEventWriter class writes XML 
// documents and strings to the event log.
class CEventWriter
{
public:
    // Creates the CEventWriter class.
    // name: the name of the 
    // event log to open.
    CEventWriter(const wstring& name)
    {                
        #ifdef UNICODE
        m_eventLog = RegisterEventSource(NULL, name.c_str());
        #else
        string multiName = CConvert::ToByteString(name);
        m_eventLog = RegisterEventSource(NULL, multiName.c_str());
        #endif        
    }

    // Creates the destructor for the 
    // CEventWriter class. This destructor
    // closes the HANDLE to the event 
    // log if that HANDLE is open.
    virtual ~CEventWriter()
    {
        // If the HANDLE to the event 
        // log is open, close it.
        if (NULL != m_eventLog)
        {
            // Deregister the event log HANDLE.
            DeregisterEventSource(m_eventLog);
            // Set the HANDLE to NULL.
            m_eventLog = NULL;
        }
    }

    // The ReportInfo method writes the content
    // of the XML document to the event log.
    // doc: the XML document to write.
    // return: true if the event log is written.
    BOOL ReportInfo(MSXML2::IXMLDOMDocument3Ptr doc)
    {
        MSXML2::IXMLDOMElementPtr documentElement = 
            doc->documentElement;
        // Get the XML as a BSTR and place this XML into a 
        // _bstr_t wrapper. The client of the XML document 
        // is responsible for deleting the returned BSTR from 
        // the property, which the _bstr_t wrapper will do
        // automatically.
        _bstr_t bstrXml = documentElement->xml;
        // Convert the _bstr_t to a wstring.
        wstring xmlString = bstrXml;
        // Write the XML to the event writer.
        return ReportInfo(xmlString);        
    }

    // The ReportInfo method writes 
    // a wstring to the event log.
    // info: the wstring to write.
    // return: true if the event log is written.
    BOOL ReportInfo(const wstring& info)
    {
        return ReportEvent(EVENTLOG_INFORMATION_TYPE, info);
    }

    // The ReportError method writes 
    // a wstring to the event log.
    // error: the wstring to write.
    // return: true if the event log is written.
    BOOL ReportError(const wstring& error)
    {
        return ReportEvent(EVENTLOG_ERROR_TYPE, error);
    }
protected:
    // The ReportEvent method accepts an event type
    // and a wstring, and attempts to write that 
    // event to the event log.
    // type: the type of the event.
    // data: the wstring to write to the event log.
    // return: true if the event log is written;
    // otherwise, false.
    BOOL ReportEvent(WORD type, const wstring& data)
    {
        // If the m_eventLog HANDLE 
        // is NULL, return false.
        if (NULL == m_eventLog)
        {
            return FALSE;
        }

        #ifndef _DEBUG
        // If the current build is not debug,
        // return so the event log is not written.
        return TRUE;
        #endif

        #ifdef UNICODE
        // The unicode version of the ReportEvent
        // method requires double-byte strings.
        PCWSTR arr[1];
        arr[0] = data.c_str();
        return ::ReportEvent(m_eventLog,
                             type,
                             0, 0, NULL, 1, 
                             0, arr, (void*)arr);
        #else
        // The non-unicode version of the ReportEvent
        // method requires single-byte strings.
        string multiByte = 
            CConvert::ToByteString(data);
        LPCSTR arr[1];
        arr[0] = multiByte.c_str();
        return ::ReportEvent(m_eventLog,
                             type,
                             0, 0, NULL, 1,
                             0, arr, (void*)arr);
        #endif
    }
private:
    // Specify the HANDLE to the 
    // event log for writing.
    HANDLE m_eventLog;
};

// The CGlobalCacheModule class creates the CGlobalModule 
// class and registers for GL_CACHE_OPERATION and 
// GL_CACHE_CLEANUP events.
class CGlobalCacheModule : public CGlobalModule
{
public:
    // Creates the destructor for the 
    // CGlobalCacheModule class.
    virtual ~CGlobalCacheModule()
    {

    }

    // The RegisterGlobalModule method creates and registers 
    // a new CGlobalCacheModule for GL_CACHE_OPERATION and 
    // GL_CACHE_CLEANUP events.
    // dwServerVersion: the current server version.
    // pModuleInfo: the current IHttpModuleRegistrationInfo pointer.
    // pGlobalInfo: the current IHttpServer pointer.
    // return: ERROR_NOT_ENOUGH_MEMORY if the heap is out of 
    // memory; otherwise, the value from the call to the 
    // SetGlobalNotifications method on the pModuleInfo pointer.
    static HRESULT RegisterGlobalModule
    (
        DWORD dwServerVersion,
        IHttpModuleRegistrationInfo* pModuleInfo,
        IHttpServer* pGlobalInfo
    )
    {        
        // The pGlobalInfo parmeter must be non-NULL because
        // the constructor for the CGlobalCacheModule class
        // requires a non-NULL pointer to a valid IHttpServer 
        // pointer.
        if (NULL == pGlobalInfo)
        {
            return E_INVALIDARG;
        }

        // Create a new CGlobalCacheModule pointer.
        CGlobalCacheModule* traceModule = 
            new CGlobalCacheModule();

        // Return an out-of-memory error if the traceModule 
        // is NULL after the call to the new operator.
        if (NULL == traceModule)
        {            
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }        

        // Attempt to set global notification for both 
        // GL_CACHE_OPERATION and GL_CACHE_CLEANUP events 
        // by using the traceModule as a listener.
        HRESULT hr = pModuleInfo->SetGlobalNotifications
            (traceModule, GL_CACHE_OPERATION | GL_CACHE_CLEANUP);
                        
        // If the SetGlobalNotifications method 
        // fails, return the HRESULT.
        if (FAILED(hr))
        {
            return hr;
        }

        // Set the priority to PRIORITY_ALIAS_FIRST, 
        // which will populate the data as much as possible.
        hr = pModuleInfo->SetPriorityForGlobalNotification(
            GL_CACHE_OPERATION, PRIORITY_ALIAS_FIRST);

        // Return the HRESULT from the call to 
        // the SetGlobalNotifications method.
        return hr;
    }
    // The OnGlobalCacheOperation method is called 
    // when GL_CACHE_OPERATION operations occur.
    // pProvider: the current ICacheProvider pointer.
    // return: GL_NOTIFICATION_CONTINUE.
    virtual GLOBAL_NOTIFICATION_STATUS OnGlobalCacheOperation
    (
        IN ICacheProvider* pProvider
    )
    {
        // The OnGlobalCacheOperation must return if the 
        // pProvider parameter is NULL because this pointer 
        // is needed for data to write to the event log.
        if (NULL == pProvider)
        {
            return GL_NOTIFICATION_CONTINUE;
        }

        // The following code uses COM smart pointers. Wrap 
        // the code in one try/catch statement for _com_error
        // exceptions. Note: it is not necessary to call 
        // CoInitialize and CoUninitialize on this thread 
        // because IIS does this automatically.
        try
        {
            // Create an XML document in memory.
            MSXML2::IXMLDOMDocument3Ptr doc(__uuidof(MSXML2::DOMDocument60));

            // Create a cacheProvider root element and 
            // append this root element to the XML document.
            MSXML2::IXMLDOMElementPtr cacheProviderElement =
                doc->createElement(L"cacheProvider");
            doc->appendChild(cacheProviderElement);

            // Get the IHttpCacheSpecificData pointer 
            // from the ICacheProvider element.
            IHttpCacheSpecificData* cacheSpecificData = 
                pProvider->GetCacheRecord();

            // Create an element for the IHttpCacheSpecificData
            // pointer and add that element to the 
            // cacheProviderElement.
            MSXML2::IXMLDOMElementPtr recordElement =
                CreateElement(cacheSpecificData, doc);    
            cacheProviderElement->appendChild(recordElement);
            // Write the XML using the writer.            
            m_eventWriter.ReportInfo(doc);    
        }
        // Catch any _com_error that occurs while you are 
        // writing to the XML document in memory.
        catch (_com_error& ce)
        {            
            // Get the description for the error.
            wstring description = ce.Description();
            // Write the error to the event writer.
            m_eventWriter.ReportError(description);
        }

        // Return GL_NOTIFICATION_CONTINUE so that 
        // other listeners will receive the event.
        return GL_NOTIFICATION_CONTINUE;
    }

    // The OnGlobalCacheCleanup method is called when 
    // GL_CACHE_CLEANUP events occur.
    // return: GL_NOTIFICATION_CONTINUE.
    virtual GLOBAL_NOTIFICATION_STATUS OnGlobalCacheCleanup(VOID)
    {
        // Return GL_NOTIFICATION_CONTINUE so that 
        // other listeners will receive this event.
        return GL_NOTIFICATION_CONTINUE;
    }

    // PRE: none.
    // POST: the Terminate method calls delete on this,
    // which releases the memory for the current 
    // CGlobalCacheModule pointer on the heap.
    virtual VOID Terminate(VOID)
    {
        delete this;
    }
protected:
    // Creates the constructor for 
    // the CGlobalCacheModule class.
    CGlobalCacheModule() : m_eventWriter(L"IISADMIN")
    {

    }

    // The AddAttribute creates a new attribute using 
    // the doc value, sets the new attribute's name and 
    // text, adds the new attribute to the element value 
    // and, finally, returns the attribute.
    // PRE: neither the element nor the doc 
    // parameters are NULL.
    // name: the name of the attribute.
    // text: the text of the attribute.
    // element: the element to add the new attribute to.
    // doc: the XML document for creating a new attribute.
    // return: the new IXMLDOMAttributePtr 
    // that is added to the element.
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

    // The CreateElement method converts an IHttpStoredContext
    // pointer and its descendant data to an XML element.
    // PRE: the doc parameter must not be NULL.    
    // storedContext: the IHttpStoredContext 
    // pointer to convert to an XML element.
    // doc: the XML document for creating elements.
    // return: a new MSXML2::IXMLDOMElementPtr representing the 
    // data in the storedContext parameter and its descendants.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMElementPtr CreateElement
    (
        IHttpStoredContext* storedContext,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create a new element to return.
        MSXML2::IXMLDOMElementPtr storedContextElement =
            doc->createElement(L"storedContext");

        // If the storedContext pointer is NULL, 
        // return the storedContextElement.
        if (NULL == storedContext)
        {
            return storedContextElement;
        }

        // Return the storedContextElement to the caller.
        return storedContextElement;
    }

    // The CreateElement method converts an IHttpModuleContextContainer
    // and its descendant data into an XML element.
    // PRE: the doc parameter must not be NULL.    
    // contextContainer: the IHttpModuleContextContainer 
    // pointer to convert to an XML element.
    // doc: the XML document used for creating elements.
    // return: a new MSXML2::IXMLDOMElementPtr representing the 
    // data in the contextContainer parameter and its descendants.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMElementPtr CreateElement
    (
        IHttpModuleContextContainer* contextContainer,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create a new element to return.
        MSXML2::IXMLDOMElementPtr contextContainerElement =
            doc->createElement(L"contextContainer");

        // If the contextContainer pointer is NULL, return the 
        // contextContainerElement.
        if (NULL == contextContainer)
        {
            return contextContainerElement;
        }

        // Get the IHttpStoredContext pointer from 
        // the IHttpModuleContextContainer pointer.
        IHttpStoredContext* storedContext = 
            contextContainer->GetModuleContext(NULL);

        // Add a new element from the IHttpStoredContext 
        // pointer to the contextContainerElement.
        MSXML2::IXMLDOMElementPtr storedContextElement = 
            CreateElement(storedContext, doc);
        contextContainerElement->appendChild(storedContextElement);

        // Return the contextContainerElement.
        return contextContainerElement;
    }

    // The CreateElement method converts an IHttpTokenEntry 
    // pointer and its descendant data into an XML element.
    // PRE: the doc parameter must not be NULL.    
    // tokenEntry: the IHttpTokenEntry 
    // pointer to convert to an XML element.
    // doc: the XML document used for creating elements.
    // return: a new MSXML2::IXMLDOMElementPtr representing the 
    // data in the tokenEntry parameter and its descendants.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMElementPtr CreateElement
    (
        IHttpTokenEntry* tokenEntry,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create a new element to return.
        MSXML2::IXMLDOMElementPtr tokenEntryElement =
            doc->createElement(L"tokenEntry");

        // If the tokenEntry parameter is NULL, 
        // return the empty tokenEntryElement.
        if (NULL == tokenEntry)
        {
            return tokenEntryElement;
        }

        // Get the impersonation token from
        // the IHttpTokenEntry pointer.
        HANDLE impersonationTokenHANDLE =
            tokenEntry->GetImpersonationToken();
        // Convert the token to a wstring.
        wstring impersonationToken =
            CConvert::ToString(impersonationTokenHANDLE);

        // Add an impersonationToken attribute
        // to the tokenEntryElement.
        AddAttribute(L"impersonationToken", impersonationToken,                      
                     tokenEntryElement, doc);

        // Get the primary token from the
        // IHttpTokenEntry pointer.
        HANDLE primaryTokenHANDLE =
            tokenEntry->GetPrimaryToken();
        // Convert the primary token to
        // a wstring.
        wstring primaryToken =
            CConvert::ToString(primaryTokenHANDLE);

        // Add a primaryToken attribute to
        // the tokenEntryElement.
        AddAttribute(L"primaryToken", primaryToken,                     
                     tokenEntryElement, doc);

        // Get the SID from the 
        // IHttpTokenEntry pointer.
        PSID sidPSID = tokenEntry->GetSid();
        // Convert the SID to a wstring.
        wstring sid = CConvert::ToString(sidPSID);

        // Add an attribute to the tokenEntryElement 
        // for the SID.
        AddAttribute(L"sid", sid,                      
                     tokenEntryElement, doc);

        // Return the tokenEntryElement to the caller.
        return tokenEntryElement;
    }

    // The CreateElement method converts an IHttpFileInfo
    // pointer and its descendant data into an XML element.
    // PRE: the doc parameter must not be NULL.
    // fileInfo: the IHttpFileInfo pointer 
    // to convert to an XML element.
    // return: a new MSXML2::IXMLDOMElementPtr representing 
    // the data in the fileInfo parameter and its descendants.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMElementPtr CreateElement
    (
        IHttpFileInfo* fileInfo,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create a new element to return.
        MSXML2::IXMLDOMElementPtr fileInfoElement =
            doc->createElement(L"fileInfo");

        // If the fileInfo pointer is NULL, 
        // return the empty fileInfoElement.
        if (NULL == fileInfo)
        {
            return fileInfoElement;
        }

        // Get file-changed information from
        // the IHttpFileInfo pointer.
        BOOL changedBOOL = 
            fileInfo->CheckIfFileHasChanged(NULL);
        // Covert the Boolean to a wstring.
        wstring changed = CConvert::ToString(changedBOOL);

        // Add a changed attribute to
        // the fileInfoElement.
        AddAttribute(L"changed", changed,                      
                     fileInfoElement, doc);

        // Get attribute information from
        // the IHttpFileInfo pointer.
        DWORD attributesDWORD =
            fileInfo->GetAttributes();
        // Convert the attributes to a wstring.
        wstring attributes = 
            CConvert::ToString(attributesDWORD);

        // Add an attributes attribute to
        // the fileInfoElement.
        AddAttribute(L"attributes", attributes, 
                     fileInfoElement, doc);

        // Get eTag information from the
        // IHttpFileInfo pointer.
        PCSTR eTagPCSTR = fileInfo->GetETag();
        // Convert the eTag to a wstring.
        wstring eTag = CConvert::ToString(eTagPCSTR);

        // Add an eTag attribute to 
        // the fileInfoElement.
        AddAttribute(L"eTag", eTag,                     
                     fileInfoElement, doc);

        // Get the file buffer as a raw BYTE 
        // pointer from the IHttpFileInfo pointer.
        const BYTE* fileBuffer = fileInfo->GetFileBuffer();
        // Convert the buffer to a string.
        wstring buffer = 
            CConvert::ToString(NULL != fileBuffer);

        // Add a buffer attribute to
        // the fileInfoElement.
        AddAttribute(L"buffer", buffer,                      
                     fileInfoElement, doc);

        // Get the file HANDLE from the
        // IHttpFileInfo pointer.
        HANDLE handleHANDLE =
            fileInfo->GetFileHandle();
        // Convert the HANDLE to a wstring.
        wstring handle = CConvert::ToString(handleHANDLE);

        // Add an attribute to the fileInfoElement 
        // for file handle information.
        AddAttribute(L"handle", handle,                     
                     fileInfoElement, doc);

        // Get the path from the 
        // IHttpFileInfo pointer.
        wstring path = fileInfo->GetFilePath();

        // Add a path attribute to
        // the fileInfoElement.
        AddAttribute(L"path", path, 
                     fileInfoElement, doc);

        // Declare a DWORD for seconds to live.
        DWORD secondsToLiveDWORD = 0;

        // Get cache allowed information from the
        // IHttpFileInfo pointer.
        BOOL cacheAllowedBOOL =
            fileInfo->GetHttpCacheAllowed(&secondsToLiveDWORD);
        // Convert the Boolean to a wstring.
        wstring cacheAllowed = 
            CConvert::ToString(cacheAllowedBOOL);
        // Convert the seconds-to-live
        // to a wstring.
        wstring secondsToLive = 
            CConvert::ToString(secondsToLiveDWORD);

        // Add an attribute to the fileInfoElement
        // indicating if caching is allowed.
        AddAttribute(L"cacheAllowed", cacheAllowed, 
                     fileInfoElement, doc);

        // Add an attribute to the fileInfoElement
        // for the number of seconds to live.
        AddAttribute(L"secondsToLive", secondsToLive,
                     fileInfoElement, doc);

        // Get last modified information from
        // the IHttpFileInfo pointer.
        PCSTR lastModifiedPCSTR =
            fileInfo->GetLastModifiedString();
        // Convert the PCSTR string to
        // a wstring.
        wstring lastModifiedString =
            CConvert::ToString(lastModifiedPCSTR);

        // Add an attribute to the fileInfoElement 
        // for last-modified information.
        AddAttribute(L"lastModifiedString", lastModifiedString,                    
                     fileInfoElement, doc);

        // Get the last modified time from
        // the IHttpFileInfo pointer.
        FILETIME lastModifiedTimeFILETIME;
        fileInfo->GetLastModifiedTime(&lastModifiedTimeFILETIME);
        // Convert the last modified time
        // to a wstring.
        wstring lastModifiedTime =
            CConvert::ToString(&lastModifiedTime);

        // Add a lastModifiedTime attribute to
        // the fileInfoElement.
        AddAttribute(L"lastModifiedTime", lastModifiedTime,                     
                     fileInfoElement, doc);

        // Get the IHttpModuleContextContainer pointer
        // from the IHttpFileInfo pointer.
        IHttpModuleContextContainer* contextContainer = 
            fileInfo->GetModuleContextContainer();

        // Add a container element to the
        // fileInfoElement element.
        MSXML2::IXMLDOMElementPtr contextContainerElement =
            CreateElement(contextContainer, doc);
        fileInfoElement->appendChild(contextContainerElement);

        // Get the file size from the
        // IHttpFileInfo pointer.
        ULARGE_INTEGER fileSizeInt;
        fileInfo->GetSize(&fileSizeInt);
        // Convert the ULARGE_INTEGER to
        // a wstring.
        wstring size = CConvert::ToString(&fileSizeInt);

        // Add a size attribute to 
        // the fileInfoElement.
        AddAttribute(L"size", size, 
                     fileInfoElement, doc);

        // Get the VR path from the 
        // IHttpFileInfo pointer.
        wstring vrPath = fileInfo->GetVrPath();
        
        // Add a vrPath attribute to 
        // the fileInfoElement.
        AddAttribute(L"vrPath", vrPath, 
                     fileInfoElement, doc);

        // Get the VR token from the
        // IHttpFileInfo pointer.
        HANDLE vrTokenHANDLE =
            fileInfo->GetVrToken();
        // Convert the HANDLE to
        // a wstring.
        wstring vrToken = CConvert::ToString(vrTokenHANDLE);

        // A a vrToken attribute to
        // the fileInfoElement.
        AddAttribute(L"vrToken", vrToken,                      
                     fileInfoElement, doc);
        
        // Return the fileInfoElement.
        return fileInfoElement;
    }

    // The CreateElement method converts an IHttpCacheSpecificData
    // and its descendant data into an XML element.
    // PRE: the doc parameter must not be NULL.    
    // cacheSpecificData: the IHttpCacheSpecificData 
    // pointer to convert to an XML element.
    // doc: the XML document to use for creating elements.
    // return: a new MSXML2::IXMLDOMElementPtr.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMElementPtr CreateElement
    (
        IHttpCacheSpecificData* cacheSpecificData,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create a default cacheSpecificData element.
        MSXML2::IXMLDOMElementPtr cacheSpecificDataElement =
            doc->createElement(L"cacheSpecificData");

        // If the cacheSpecificData is NULL, 
        // return the empty cacheSpecificDataElement.
        if (NULL == cacheSpecificData)
        {
            return cacheSpecificDataElement;
        }

        // Get the IHttpCacheKey pointer from the 
        // IHttpCacheSpecificData pointer.
        IHttpCacheKey* cacheKey = 
            cacheSpecificData->GetCacheKey();

        // If the cacheKey is non-NULL, get its name.
        // This may allow downcasting to a more specific 
        // IHttpCacheSpecificData pointer type.
        if (NULL != cacheKey)
        {
            // Get the cache name from the cacheKey.
            wstring cacheKeyName = cacheKey->GetCacheName();

            // If the cacheKeyName is FILE_CACHE_NAME, the
            // IHttpCacheSpecificData pointer can be downcast 
            // to an IHttpFileInfo pointer.
            if (FILE_CACHE_NAME == cacheKeyName)
            {
                // Attempt to cast the IHttpCacheSpecificData 
                // pointer to an IHttpFileInfo pointer.                
                IHttpFileInfo* fileInfo =
                    dynamic_cast<IHttpFileInfo*>(cacheSpecificData);
                // Reset the cacheSpecificDataElement 
                // to the new element.
                cacheSpecificDataElement = 
                    CreateElement(fileInfo, doc);
            }

            // If the cacheKeyName is TOKEN_CACHE_NAME, the
            // IHttpCacheSpecificData pointer can be 
            // downcast to an IHttpTokenEntry pointer.
            if (TOKEN_CACHE_NAME == cacheKeyName)
            {
                // Attempt to cast the IHttpCacheSpecificData 
                // pointer to an IHttpTokenEntry pointer.                
                IHttpTokenEntry* tokenEntry =
                    dynamic_cast<IHttpTokenEntry*>(cacheSpecificData);
                // Reset the cacheSpecificDataElement 
                // to the new element.
                cacheSpecificDataElement = 
                    CreateElement(tokenEntry, doc);
            }
        }

        // Get flushed information from the
        // IHttpCacheSpecificData pointer.
        BOOL flushedBOOL =
            cacheSpecificData->GetFlushed();
        // Convert the Boolean to
        // a wstring.
        wstring flushed = CConvert::ToString(flushedBOOL);

        // Add a flushed attribute to the
        // cacheSpecificDataElement.
        AddAttribute(L"flushed", flushed,                      
                     cacheSpecificDataElement, doc);    

        // Return the new cacheSpecificDataElement to the caller.
        return cacheSpecificDataElement;
    }
private:
    // Specify the event writer.
    CEventWriter m_eventWriter;
};

// The RegisterModule method is the 
// main entry point for the DLL.
// dwServerVersion: the current server version.
// pModuleInfo: the current 
// IHttpModuleRegistrationInfo pointer.
// pGlobalInfo: the current IHttpServer pointer.
// return: the value returned by calling the
// CGlobalCacheModule::RegisterGlobalModule
// method.
HRESULT
__stdcall
RegisterModule(
    DWORD dwServerVersion,
    IHttpModuleRegistrationInfo* pModuleInfo,
    IHttpServer* pGlobalInfo
)
{        
    // Call the static method for initialization.
    return CGlobalCacheModule::RegisterGlobalModule            
        (dwServerVersion, 
         pModuleInfo, 
         pGlobalInfo);             
}
// </Snippet4>