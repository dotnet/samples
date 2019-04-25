#include "stdafx.h"

//<Snippet1>
// Function declarations
DWORD WINAPI workerThread(HANDLE);
void UnRegisterRefreshEvent();
HANDLE RegisterRefreshEvent(BSTR);

// Global DEM COM pointer object
CComPtr<IDeviceEmulatorManager> pDeviceEmulatorManager;


int _tmain(int argc, TCHAR* argv[], TCHAR* envp[])
{
    int nRetCode = 0;
    HRESULT hr;

    // Name of Event
    BSTR Named_Event = L"__MyEvent1";

    // Start Device Emulator Manager (DEM)
    CoInitialize(NULL);    
    hr = pDeviceEmulatorManager.CoCreateInstance(__uuidof(DeviceEmulatorManager));
    if (FAILED(hr)) 
    {
        wprintf_s(L"Error: Unable to instantiate DeviceEmulatorManager. ErrorCode=0x%08X\n", hr);
        nRetCode = -1;
    }

    // Register DEM Refresh Event
    HANDLE Named_Handle = RegisterRefreshEvent(Named_Event);

    // Create a worker thread to wait for Refresh Event
    HANDLE h = CreateThread(NULL, 0, workerThread, (HANDLE) Named_Handle, 0, NULL);
    if (h == NULL)
    {
         // Not Able to create worker Thread.
         // Add code to handle error.
    }
    
    //
    //
    // Add code to perform program's main functions here.
    //
    system("pause");
    //
    // Wait for the worker thread to complete (or post a message to workerthread
    // to terminate)
    //
    
    //  Finally UnRegister the DEM Refresh Event
    UnRegisterRefreshEvent();

    // Close all handles
    CloseHandle(h);
    CloseHandle(Named_Handle);
    
    // Uninitialize DEM
    CoUninitialize ();

    return nRetCode;
}


HANDLE RegisterRefreshEvent(BSTR Named_Event)
{    
    HRESULT hr;
    // Create the Event Handle for the Named Event you are interested
    HANDLE Named_EventHandle = CreateEvent( (LPSECURITY_ATTRIBUTES)0, false, false, Named_Event);
    if (Named_EventHandle == NULL)
    {
         // Not Able to create the Named Event.
         // Add code to handle error. 
    }

    // Register for the refresh event. 
    hr = pDeviceEmulatorManager->RegisterRefreshEvent(Named_Event);
    // Add code to handle possible errors.

    return Named_EventHandle;
}

void UnRegisterRefreshEvent()
{
    // UnRegister the refresh event.
    pDeviceEmulatorManager->UnRegisterRefreshEvent();
    // Add code to handle possible errors.
}


DWORD WINAPI workerThread(HANDLE Named_Handle)
{
    HRESULT hr; 
    int Infinite = 0;
    int WAIT_Timeout = 0X00000102;

    // Timeout after 60 seconds.
    int MY_TIMEOUT =60*1000;

    // Block the worker thread until event is signaled or timeout occurs.
    // Named_Handle has been registered to fire when there is a change in the DataStore, 
    // the My Device Emulators, or the All Device Emulators folder.
    int ret = WaitForSingleObject(Named_Handle, MY_TIMEOUT );
    if(ret == 0)
    {
        // The event Event was signled. Refresh DEM
        hr = pDeviceEmulatorManager->Refresh();
        wprintf_s(L"Refreshing the Device Emulator Manager");
    }
    else if(ret == WAIT_Timeout )
    {
        // Event was not signled in timeout period
        wprintf_s(L"Worker Thread Timeout. No refresh required.");
    }
    else
    {
        // Unknown Error
    }
    return 0;
}
//</Snippet1>