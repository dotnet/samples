// <Snippet1>
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

// The CGlobalTraceModule class creates the CGlobalModule 
// class and registers for GL_TRACE_EVENT events.
class CGlobalTraceModule : public CGlobalModule
{
public:
    // Creates the destructor for the 
    // CGlobalTraceModule class.
    virtual ~CGlobalTraceModule()
    {

    }

    // The RegisterGlobalModule method creates and registers 
    // a new CGlobalTraceModule for GL_TRACE_EVENT events.
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
        // The IHttpModuleRegistrationInfo and 
        // IHttpServer pointers must not be NULL.
        if ((NULL == pModuleInfo) || (NULL == pGlobalInfo))
        {
            return E_INVALIDARG;
        }

        // Get the IHttpTraceContext pointer 
        // from the IHttpServer pointer.
        IHttpTraceContext* traceContext =
            pGlobalInfo->GetTraceContext();

        // Get the HTTP_MODULE_ID from the 
        // IHttpModuleRegistrationInfo pointer.
        HTTP_MODULE_ID moduleId = 
            pModuleInfo->GetId();

        // The IHttpTraceContext pointer and 
        // HTTP_MODULE_ID both must not be NULL.
        if ((NULL == traceContext) || (NULL == moduleId))
        {
            return E_INVALIDARG;
        }

        // Create a new CGlobalTraceModule pointer
        // using the HTTP_MODULE_ID from the 
        // IHttpModuleRegistrationInfo pointer.
        CGlobalTraceModule* traceModule = 
            new CGlobalTraceModule(moduleId);

        // Return an out-of-memory error if the traceModule 
        // is NULL after the call to the new operator.
        if (NULL == traceModule)
        {            
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }    

        // Declare an HTTP_TRACE_CONFIGURATION structure 
        // to pass to the SetTraceConfiguration method.
        HTTP_TRACE_CONFIGURATION traceConfiguration;

        // Set the GUID on the HTTP_TRACE_CONFIGURATION
        // to GUID_IIS_ALL_TRACE_PROVIDERS.
        traceConfiguration.pProviderGuid = 
                                    &GUID_IIS_ALL_TRACE_PROVIDERS;
        // Register for all areas.
        traceConfiguration.dwAreas = 0xffffe;
        // Register for the maximum verbosity.
        traceConfiguration.dwVerbosity = 5;
        // Enable the provider.
        traceConfiguration.fProviderEnabled = TRUE;

        // Set the trace configuration on 
        // the IHttpTraceContext pointer.
        traceContext->SetTraceConfiguration(moduleId, &traceConfiguration);                                                  

        // Attempt to set global notification 
        // for an GL_TRACE_EVENT event by using 
        // the traceModule as a listener.
        HRESULT hr = pModuleInfo->SetGlobalNotifications
            (traceModule, GL_TRACE_EVENT);

        // Return the HRESULT from the call to 
        // the SetGlobalNotifications method.
        return hr;
    }

    // The OnGlobalTraceEvent method is the callback
    // method for GL_TRACE_EVENT events in the pipeline.
    // pProvider: the IGlobalTraceEventProvider pointer.
    // return: GL_NOTIFICATION_CONTINUE.
    virtual 
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalTraceEvent
    (
        IN IGlobalTraceEventProvider*  pProvider
    )
    {
        // If the IGlobalTraceEventProvider 
        // pointer is NULL, return.
        if (NULL == pProvider)
        {
            return GL_NOTIFICATION_CONTINUE;
        }

        // Check if the this module has a 
        // subscription to the event.
        BOOL subscription = 
            pProvider->CheckSubscription(m_moduleId);

        // If the module is not subscribed,
        // return GL_NOTIFICATION_CONTINUE.
        if (!subscription)
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
            MSXML2::IXMLDOMDocument3Ptr doc
                (__uuidof(MSXML2::DOMDocument60));
            
            // Create a eventProviderElement root element.            
            MSXML2::IXMLDOMElementPtr eventProviderElement =
                doc->createElement(L"eventProvider");

            // Append the eventProviderElement 
            // element to the XML document.
            doc->appendChild(eventProviderElement);

            // Convert the Boolean to a wstring.
            wstring subscriptionString = 
                CConvert::ToString(subscription);

            // Add a subscription attribute to the
            // eventProviderElement element.
            AddAttribute(L"subscription", subscriptionString,
                         eventProviderElement, doc);

            // Declare an HRESULT and initialize
            // that HRESULT to E_FAIL.
            HRESULT hr = E_FAIL;

            // Declare an HTTP_TRACE_EVENT pointer
            // and initialize that pointer to NULL.
            HTTP_TRACE_EVENT* traceEvent = NULL;

            // Call the GetTraceEvent method on the
            // IGlobalTraceEventProvider pointer.
            hr = pProvider->GetTraceEvent(&traceEvent);        

            // Create an XML element for 
            // the HTTP_TRACE_EVENT pointer.
            MSXML2::IXMLDOMElementPtr traceEventElement =
                CreateElement(traceEvent, hr, doc);

            // Append the traceEventElement 
            // to the eventProviderElement.
            eventProviderElement->appendChild(traceEventElement);
                            
            // Write the XML using the writer.        
            m_eventWriter.ReportInfo(doc);
        }
        // Catch any _com_error that occurs while 
        // writing to the XML document in memory.
        catch (_com_error& ce)
        {            
            // Get the description for the error.
            wstring description = ce.Description();
            // Write the error to the event writer.
            m_eventWriter.ReportError(description);
            // Set the error status on the
            // IGlobalTraceEventProvider pointer.
            pProvider->SetErrorStatus(ce.Error());
        }

        // Return GL_NOTIFICATION_CONTINUE.
        return GL_NOTIFICATION_CONTINUE;
    }

    // The Terminate method is required for
    // non-abstract CGlobalTraceModule classes.
    // This method calls delete on this.
    virtual VOID Terminate(VOID)
    {
        delete this;
    }
protected:
    // Creates the constructor for the CGlobalTraceModule 
    // class. This constructor initializes the CEventWriter
    // to write to the application event log.
    // moduleId: the current module identifier.
    CGlobalTraceModule(HTTP_MODULE_ID moduleId): 
         m_eventWriter(L"IISADMIN")
    {
        m_moduleId = moduleId;
    }
    
    // The AddAttribute method creates a 
    // new XML attribute and appends that 
    // attribute to an existing XML element.
    // name: the name of the attribute to create.
    // text: the text to set on the new attribute.
    // element: the XML element to 
    // append the new attribute to.
    // doc: the XML document to use for
    // creating XML elements and attributes.
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
        // Set the text of the attribute to the text value.
        attribute->text = text.c_str();
        // Add the attribute to the passed element.
        element->setAttributeNode(attribute);
        // Return the new attribute to the caller.
        return attribute;
    }
    
    // The CreateElement accepts an 
    // HTTP_TRACE_EVENT_ITEM pointer, and writes 
    // that pointer to a new XML element.
    // traceEventItem: the HTTP_TRACE_EVENT_ITEM 
    // pointer to write.
    // doc: the XML document for creating
    // elements and attributes.
    // return: a new MSXML2::IXMLDOMElementPtr.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMElementPtr CreateElement
    (
        HTTP_TRACE_EVENT_ITEM* traceEventItem,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)    
    {
        // Create a new element to return.
        MSXML2::IXMLDOMElementPtr traceEventItemElement =
            doc->createElement(L"traceEventItem");

        // If the HTTP_TRACE_EVENT_ITEM pointer is
        // NULL, return the XML element.
        if (NULL == traceEventItem)
        {
            return traceEventItemElement;
        }

        // Get the cbData element from
        // the HTTP_TRACE_EVENT_ITEM pointer.
        DWORD data = traceEventItem->cbData;

        // Convert the DWORD to a wstring.
        wstring dataString =
            CConvert::ToString(data);

        // Add a data attribute to
        // the traceEventItemElement.
        AddAttribute(L"data", dataString,
                     traceEventItemElement, doc);

        // Get the HTTP_TRACE_TYPE from the
        // HTTP_TRACE_EVENT_ITEM pointer.
        HTTP_TRACE_TYPE traceType =
            traceEventItem->dwDataType;

        // Declare an empty string for 
        // the type of the request.
        wstring traceTypeString;

        // Declare a string that defaults
        // to NULL for the data type.        
        wstring pbDataString(L"NULL");

        // Get the data from the 
        // HTTP_TRACE_EVENT_ITEM pointer.
        PBYTE pbData =
            traceEventItem->pbData; 

        // A switch statement is necesary for
        // displaying the event type, but also
        // for downcasting the pbData to the
        // correct type based upon that type.
        switch (traceType)
        {
        case HTTP_TRACE_TYPE_BYTE:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_BYTE";
                // Cast the data to a character
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    char pbDataByte = 
                        reinterpret_cast<char>(pbData);    
                    pbDataString = 
                        CConvert::ToString(pbDataByte);
                }
                break;
            }
        case HTTP_TRACE_TYPE_USHORT:
            {
                // Set the type sring.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_USHORT";
                // Cast the data to a USHORT
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    USHORT uShort =                        
                        reinterpret_cast<USHORT>(pbData);
                    pbDataString =
                        CConvert::ToString(uShort);
                }
                break;
            }
        case HTTP_TRACE_TYPE_ULONG:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_ULONG";
                // Cast the data to a ULONG
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    ULONG uLong =                        
                        static_cast<ULONG>(*pbData);
                    pbDataString =
                        CConvert::ToString(uLong);
                }
                break;
            }
        case HTTP_TRACE_TYPE_ULONGLONG:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_ULONGLONG";
                // Cast the data to a long
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    long l =                        
                        static_cast<long>(*pbData);
                    pbDataString =
                        CConvert::ToString(l);
                }
                break;
            }
        case HTTP_TRACE_TYPE_CHAR:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_CHAR";
                // Cast the data to a char
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    char c =                        
                        reinterpret_cast<char>(pbData);
                    pbDataString =
                        CConvert::ToString(c);
                }
                break;
            }
        case HTTP_TRACE_TYPE_SHORT:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_SHORT";
                // Cast the data to a short 
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    short s =
                        reinterpret_cast<short>(pbData);                        
                    pbDataString =
                        CConvert::ToString(s);
                }
                break;
            }

        case HTTP_TRACE_TYPE_LONG:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_LONG";
                // Cast the data to a long
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    long l =                        
                        static_cast<long>(*pbData);
                    pbDataString =
                        CConvert::ToString(l);
                }
                break;
            }
        case HTTP_TRACE_TYPE_LONGLONG:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_LONGLONG";
                // Cast the data to a long
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    long l =                        
                        static_cast<long>(*pbData);
                    pbDataString =
                        CConvert::ToString(l);
                }
                break;
            }
        case HTTP_TRACE_TYPE_LPCWSTR:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_LPCWSTR";
                // Cast the data to a LPCWSTR
                // if the data is not NULL.
                if (NULL != pbData)
                {    
                    LPCWSTR str =
                        (LPCWSTR)(pbData);                        
                    pbDataString =
                        CConvert::ToString(str);    
                }        
                break;
            }
        case HTTP_TRACE_TYPE_LPCSTR:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_LPCSTR";
                // Cast the data to an LPCSTR
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    LPCSTR str =
                        (LPCSTR)(pbData);
                    pbDataString =
                        CConvert::ToString(str);
                }
                break;
            }
        case HTTP_TRACE_TYPE_LPCGUID:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_LPCGUID";
                // Cast the data to an LPCGUID
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    LPCGUID guid =                        
                        reinterpret_cast<LPCGUID>(pbData);
                    pbDataString =
                        CConvert::ToString(guid);
                }            
                break;
            }
        case HTTP_TRACE_TYPE_BOOL:
            {
                // Set the type string.
                traceTypeString = 
                    L"HTTP_TRACE_TYPE_BOOL";
                // Cast the data to a BOOL
                // if the data is not NULL.
                if (NULL != pbData)
                {
                    BOOL b =                                        
                        static_cast<BOOL>(*pbData);
                    pbDataString =
                        CConvert::ToString(b);
                }
                break;
            }
        default:
            {
                // Set the type string to
                // Unknown. The data string
                // is NULL which should be correct.
                traceTypeString = 
                    L"Unknown";
                break;
            }
        }

        // Add a traceType attribute to the
        // traceEventItemElement.
        AddAttribute(L"traceType", traceTypeString,
                     traceEventItemElement, doc);

        // Add a pbData attribute to the 
        // traceEventItemElement.
        AddAttribute(L"pbData", pbDataString,
                     traceEventItemElement, doc);

        // Get the description from the 
        // HTTP_TRACE_EVENT_ITEM pointer.
        LPCWSTR description = 
            traceEventItem->pszDataDescription;

        // Convert the LPCWSTR to a wstring.
        wstring descriptionString = 
            CConvert::ToString(description);

        // Add a description attribute 
        // to the traceEventItemElement.
        AddAttribute(L"description", descriptionString,
                     traceEventItemElement, doc);

        // Get the name from the 
        // HTTP_TRACE_EVENT_ITEM pointer.
        LPCWSTR name = traceEventItem->pszName;

        // Convert the LPCWSTR to a wstring.
        wstring nameString =
            CConvert::ToString(name);

        // Add a name attribute to
        // the traceEventItemElement.
        AddAttribute(L"name", nameString, 
                     traceEventItemElement, doc);

        // Return the XML element.
        return traceEventItemElement;
    }

    // The CreateElement method converts an 
    // HTTP_TRACE_EVENT to an IXMLDOMElementPtr.
    // traceEvent: the HTTP_TRACE_EVENT pointer 
    // to convert.
    // hr: the HRESULT returned from 
    // retrieving the HTTP_TRACE_EVENT pointer.
    // doc: the XML document to use for
    // creating elements and attributes.
    // throws: a _com_error exception.
    static MSXML2::IXMLDOMElementPtr CreateElement
    (
        HTTP_TRACE_EVENT* traceEvent,
        HRESULT hr,
        MSXML2::IXMLDOMDocument3Ptr doc
    ) throw (_com_error)
    {
        // Create a new element to return.
        MSXML2::IXMLDOMElementPtr traceEventElement =
            doc->createElement(L"traceEvent");

        // If the HTTP_TRACE_EVENT pointer is NULL, 
        // or the call to retrieve the HTTP_TRACE_EVENT
        // pointer failed, return an empty XML element.
        if ((NULL == traceEvent) ||
            (FAILED(hr)))
        {
            return traceEventElement;
        }

        // Get the dwArea from the 
        // HTTP_TRACE_EVENT pointer.
        DWORD area =
            traceEvent->dwArea;

        // Convert the DWORD to a wstring.
        wstring areaString = 
            CConvert::ToString(area);

        // Add an area attribute to
        // the traceEventElement.
        AddAttribute(L"area", areaString,
                     traceEventElement, doc);

        // Get the dwEvent from the 
        // HTTP_TRACE_EVENT pointer.
        DWORD eventDWORD =
            traceEvent->dwEvent;

        // Convert the DWORD to a wstring.
        wstring eventString =
            CConvert::ToString(eventDWORD);

        // Add an event attribute to
        // the traceEventElement.
        AddAttribute(L"event", eventString,
                     traceEventElement, doc);

        // Get the dwEventVersion from 
        // the HTTP_TRACE_EVENT pointer.
        DWORD eventVersion = 
            traceEvent->dwEventVersion;

        // Convert the DWORD to a wstring.
        wstring eventVersionString =
            CConvert::ToString(eventVersion);

        // Add an eventVersion attribute
        // to the traceEventElement.
        AddAttribute(L"eventVersion", eventVersionString,
                     traceEventElement, doc);

        // Get the dwFlags from the
        // HTTP_TRACE_EVENT pointer.
        DWORD flags = 
            traceEvent->dwFlags;

        // Convert the DWORD to a wstring.
        wstring flagsString =
            CConvert::ToString(flags);

        // Add a flags attribute to 
        // the traceEventElement.
        AddAttribute(L"flags", flagsString,
                     traceEventElement, doc);

        // Get the dwTimeStamp from
        // the HTTP_TRACE_EVENT pointer.
        DWORD timeStamp =
            traceEvent->dwTimeStamp;

        // Convert the DWORD to a wstring.
        wstring timeStampString = 
            CConvert::ToString(timeStamp);

        // Add a timeStamp attribute to
        // the traceEventElement.
        AddAttribute(L"timeStamp", timeStampString,
                     traceEventElement, doc);

        // Get the dwVerbosity from 
        // the HTTP_TRACE_EVENT pointer.
        DWORD verbosity = 
            traceEvent->dwVerbosity;

        // Convert the DWORD to a wstring.
        wstring verbosityString = 
            CConvert::ToString(verbosity);

        // Add a verbosity attribute to
        // the traceEventElement.
        AddAttribute(L"verbosity", verbosityString,
                     traceEventElement, doc);

        // Get the pActivityGuid from the
        // HTTP_TRACE_EVENT pointer.
        LPCGUID activityGuid =
            traceEvent->pActivityGuid;

        // Convert the LPCGUID to a wstring.
        wstring activityGuidString = 
            CConvert::ToString(activityGuid);

        // Add an activityGuid attribute 
        // to the traceEventElement.
        AddAttribute(L"activityGuid", activityGuidString,
                     traceEventElement, doc);

        // Get the pAreaGuid from the 
        // HTTP_TRACE_EVENT pointer.
        LPCGUID areaGuid = 
            traceEvent->pAreaGuid;

        // Convert the LPCGUID to a wstring.
        wstring areaGuidString =
            CConvert::ToString(areaGuid);

        // Add an areaGuid attribute to
        // the traceEventElement.
        AddAttribute(L"areaGuid", areaGuidString,
                     traceEventElement, doc);

        // Get the pProviderGuid from the
        // HTTP_TRACE_EVENT pointer.
        LPCGUID providerGuid =
            traceEvent->pProviderGuid;

        // Convert the LPCGUID to a wstring.
        wstring providerGuidString =
            CConvert::ToString(providerGuid);

        // Add a providerGuid attribute to
        // the traceEventElement.
        AddAttribute(L"providerGuid", providerGuidString,
                     traceEventElement, doc);

        // Get the pRelatedActivityGuid from
        // the HTTP_TRACE_EVENT pointer.
        LPCGUID relatedActivityGuid =
            traceEvent->pRelatedActivityGuid;

        // Convert the LPCGUID to a wstring.
        wstring relatedActivityGuidString =
            CConvert::ToString(relatedActivityGuid);

        // Add a relatedActivityGuid attribute
        // to the traceEventElement.
        AddAttribute(L"relatedActivityGuid", 
                     relatedActivityGuidString,
                     traceEventElement, doc);

        // Get the pszEventName from the
        // HTTP_TRACE_EVENT pointer.
        LPCWSTR eventName = traceEvent->pszEventName;

        // Convert the LPCWSTR to a wstring.
        wstring eventNameString =
            CConvert::ToString(eventName);

        // Add an eventName attribute to
        // the traceEventElement.
        AddAttribute(L"eventName", eventNameString,
                     traceEventElement, doc);
        
        // Get the number of event 
        // items in the array from the
        // HTTP_TRACE_EVENT pointer.
        DWORD eventItems = 
            traceEvent->cEventItems;

        // Convert the DWORD to a wstring.
        wstring eventItemsString =
            CConvert::ToString(eventItems);

        // Add an eventItems attribute to
        // the traceEventElement.
        AddAttribute(L"eventItems", eventItemsString, 
                     traceEventElement, doc);

        // Create a new traceEventItems element 
        // for placing the returned items in.
        MSXML2::IXMLDOMElementPtr traceEventItemsElement =
            doc->createElement(L"traceEventItems");

        // Append the traceEventItemsElement 
        // to the traceEventElement.
        traceEventElement->appendChild(traceEventItemsElement);

        // Get the HTTP_TRACE_EVENT_ITEM pointer 
        // array from the HTTP_TRACE_EVENT pointer.
        HTTP_TRACE_EVENT_ITEM* eventItemsArray =
            traceEvent->pEventItems;

        // Enumerate the array and add an XML element
        // for each HTTP_TRACE_EVENT_ITEM pointer.
        for (DWORD i = 0; i < eventItems; ++i)
        {
            // Get the current HTTP_TRACE_EVENT_ITEM
            // pointer from the array.
            HTTP_TRACE_EVENT_ITEM* traceEventItem =
                (&eventItemsArray[i]);

            // Create an IXMLDOMElementPtr for the
            // HTTP_TRACE_EVENT_ITEM pointer.
            MSXML2::IXMLDOMElementPtr traceEventItemElement =
                CreateElement(traceEventItem, doc);

            // Append the traceEventItemElement to
            // the traceEventItemsElement.
            traceEventItemsElement->appendChild(traceEventItemElement);
        }

        // Return the XML element.
        return traceEventElement;
    }

private:
    // Specify the private CEventWriter
    // for writing events.
    CEventWriter m_eventWriter;

    // Specify the HTTP_MODULE_ID
    // for this module.
    HTTP_MODULE_ID m_moduleId;
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
    return CGlobalTraceModule::RegisterGlobalModule            
        (dwServerVersion, 
         pModuleInfo, 
         pGlobalInfo);             
}
// </Snippet1>