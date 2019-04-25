// <Snippet3>
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

// The CConvert class mirrors the Convert class that is 
// defined in the .NET Framework. It converts primitives 
// and other data types to wstring types.
class CConvert
{
public:
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

// The CEventException class is 
// an exception that can be thrown 
// when writing an event fails.
class CEventException
{
public:
    // Creates the CEventException class.
    // str: the wstring that could 
    // not be written to a log.
    CEventException(const wstring& str)
        : m_string(str)
    {
        
    }

    // Creates the destructor for 
    // the CEventException class.
    virtual ~CEventException()
    {

    }

private:
    // Specify the wstring that could
    // not be written to an event viewer.
    wstring m_string;
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
    // return: GL_NOTIFICATION_CONTINUE if the event
    // log is written; otherwise, GL_NOTIFICATION_HANDLED.
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

        try
        {
            // Get the IHttpCacheKey pointer 
            // from the ICacheProvider pointer.
            IHttpCacheKey* cacheKey = 
                pProvider->GetCacheKey();

            // Write the IHttpCacheKey pointer
            // information to the event log.
            Write(cacheKey);            
        }
        // A CEventException is thrown 
        // if any Write method cannot 
        // write to the event log.
        catch (CEventException)
        {
            return GL_NOTIFICATION_HANDLED;
        }

        // Return GL_NOTIFICATION_CONTINUE so that 
        // other listeners will receive the event.
        return GL_NOTIFICATION_CONTINUE;
    }

    // The OnGlobalCacheCleanup method is called 
    // when GL_CACHE_CLEANUP events occur.
    // return: GL_NOTIFICATION_CONTINUE.
    virtual GLOBAL_NOTIFICATION_STATUS OnGlobalCacheCleanup(VOID)
    {
        // Return GL_NOTIFICATION_CONTINUE so that 
        // other listeners will receive this event.
        return GL_NOTIFICATION_CONTINUE;
    }

    // PRE: none.
    // POST: the Terminate method calls delete on 
    // this, which releases the memory for the current 
    // CGlobalCacheModule pointer on the heap.
    virtual VOID Terminate(VOID)
    {
        delete this;
    }
protected:
    // Creates the constructor for 
    // the CGlobalCacheModule class.
    // The constructor initializes the 
    // private m_eventWriter to write 
    // to the IISADMIN event log.
    CGlobalCacheModule() : m_eventWriter(L"IISADMIN")
    {

    }

    // The ReportInfo method writes the 
    // formatted name and value of a method 
    // call to the event log.
    // name: the name of the method or property.
    // value: the value of the 
    // method or the property.
    // throws: a CEventException exception.
    void ReportInfo
    (
        const wstring& name,
        const wstring& value
    ) throw (CEventException)
    {
        // Create a formatted string to
        // write to the event log.
        wstring infoString =
            name + wstring(L":  ") + value;
        // Attempt to write the formatted 
        // string to the event log. If the 
        // ReportInfo method call fails,
        // throw a CEventException exception.
        if (!m_eventWriter.ReportInfo(infoString))
        {
            throw CEventException(infoString);
        }        
    }
    // The Write method writes IHttpCacheKey 
    // pointer information to the event log.
    // cacheKey: the IHttpCacheKey to write.
    // throws: a CEventException exception.
    void Write
    (
        IHttpCacheKey* cacheKey        
    ) throw (CEventException)
    {
        // If the cacheKey parameter is NULL, 
        // throw a CEventException exception.
        if (NULL == cacheKey)
        {            
            CEventException ce
                (L"NULL IHttpCacheKey pointer");
            throw ce;            
        }

        // Get the hash from the 
        // IHttpCacheKey pointer.
        DWORD hashDWORD = cacheKey->GetHash();

        // Convert the DWORD to a wstring.
        wstring hash = CConvert::ToString(hashDWORD);

        // Write the hash information
        // to the event log.
        ReportInfo(L"IHttpCacheKey::GetHash", hash);
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
// </Snippet3>