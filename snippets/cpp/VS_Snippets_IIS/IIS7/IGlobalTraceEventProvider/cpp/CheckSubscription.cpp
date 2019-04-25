// <Snippet2>
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
// </Snippet2>