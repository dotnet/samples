// <Snippet2>
#define _WINSOCKAPI_
#include <windows.h>
#include <sal.h>
#include <tchar.h>
#include <httpserv.h>

#include <string>
using namespace std;

// The CConvert class mirrors the Convert class that is 
// defined in the .NET Framework. It converts primitives 
// and other data types to wstring types.
class CConvert
{
public:
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

    // The ReportInfo method writes 
    // a wstring to the event log.
    // info: the wstring to write.
    // return: true if the event log is written.
    BOOL ReportInfo(const wstring& info)
    {
        return ReportEvent(EVENTLOG_INFORMATION_TYPE, info);
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

// The CRSCAGlobalModule class creates the CGlobalModule 
// class and registers for GL_RSCA_QUERY and events.
class CRSCAGlobalModule : public CGlobalModule
{
public:
    // Creates the destructor for the 
    // CGlobalCacheModule class.
    virtual ~CRSCAGlobalModule()
    {

    }

    // The RegisterGlobalModule method creates and registers 
    // a new CRSCAGlobalModule for GL_RSCA_QUERY events.
    // dwServerVersion: the current server version.
    // pModuleInfo: the current IHttpModuleRegistrationInfo pointer.
    // pGlobalInfo: the current IHttpServer pointer.
    // return: E_INVALIDARG if the IHttpServer pointer
    // is NULL; ERROR_NOT_ENOUGH_MEMORY if the heap is out of 
    // memory; otherwise, the value from the call to the 
    // SetGlobalNotifications method on the pModuleInfo pointer.
    static HRESULT RegisterGlobalModule
    (
        DWORD dwServerVersion,
        IHttpModuleRegistrationInfo* pModuleInfo,
        IHttpServer* pGlobalInfo
    )
    {        
        // The pGlobalInfo parmeter must be 
        // non-NULL because the constructor 
        // for the CGlobalCacheModule class 
        // requires a non-NULL pointer to a 
        // valid IHttpServer pointer.
        if (NULL == pGlobalInfo)
        {
            return E_INVALIDARG;
        }

        // Create a new CGlobalCacheModule pointer.
        CRSCAGlobalModule* rscaModule = 
            new CRSCAGlobalModule();

        // Return an out-of-memory error 
        // if the traceModule is NULL 
        // after the call to the new operator.
        if (NULL == rscaModule)
        {            
            return HRESULT_FROM_WIN32(ERROR_NOT_ENOUGH_MEMORY);
        }        

        // Attempt to set global notification 
        // for GL_RSCA_QUERY events by using 
        // the rscaModule as a listener.
        HRESULT hr = pModuleInfo->SetGlobalNotifications
            (rscaModule, GL_RSCA_QUERY);

        // Return the HRESULT from the call to 
        // the SetGlobalNotifications method.
        return hr;
    }

    // The OnGlobalRSCAQuery method is the event 
    // handler method for the GL_RSCA_QUERY event.
    // pProvider: the IGlobalRSCAQueryProvider pointer.
    // return: GL_NOTIFICATION_CONTINUE.
    virtual GLOBAL_NOTIFICATION_STATUS OnGlobalRSCAQuery
    (
        IN IGlobalRSCAQueryProvider* pProvider
    )
    {
        // Return GL_NOTIFICATION_CONTINUE if the
        // IGlobalRSCAQueryProvider pointer is NULL.
        if (NULL == pProvider)
        {
            return GL_NOTIFICATION_CONTINUE;
        }

        // Get the name of the function from
        // the IGlobalRSCAQueryProvider pointer.
        wstring functionName = 
            pProvider->GetFunctionName();

        // Write the function name 
        // information to the event writer.
        m_eventWriter.ReportInfo
            (L"Function Name:  " + functionName);

        // Return GL_NOTIFICATION_CONTINUE so
        // other listeners will receive this event.
        return GL_NOTIFICATION_CONTINUE;
    }

    // The Terminate method is a pure virtual 
    // method that all non-abstract CGlobalModule
    // classes must implement. Calls delete on this.
    virtual VOID Terminate(VOID)
    {
        delete this;
    }
protected:
    // Creates the CRSCAGlobalModule class.
    // Initializes the CEventWriter to write
    // to the event log using the IISADMIN key.
    CRSCAGlobalModule() : m_eventWriter(L"IISADMIN")
    {

    }
private:
    // Specify the CEventWriter writer.
    CEventWriter m_eventWriter;
};

// The RegisterModule method is the 
// main entry point for the DLL.
// dwServerVersion: the current server version.
// pModuleInfo: the current 
// IHttpModuleRegistrationInfo pointer.
// pGlobalInfo: the current IHttpServer pointer.
// return: the value returned by calling the
// CRSCAGlobalModule::RegisterGlobalModule
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
    return CRSCAGlobalModule::RegisterGlobalModule            
        (dwServerVersion, 
         pModuleInfo, 
         pGlobalInfo);             
}
// </Snippet2>