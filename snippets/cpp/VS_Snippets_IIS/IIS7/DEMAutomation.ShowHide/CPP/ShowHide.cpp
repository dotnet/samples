// ShowHide.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

//<Snippet1>
int _tmain(int argc, _TCHAR* argv[])
{
    if (SUCCEEDED(CoInitializeEx(NULL, COINIT_MULTITHREADED)))
    {
        // HRESULT is used to determine whether method calls are successful
        HRESULT hr;

        // Instantiate DeviceEmulatorManager (DEM) object.
        // This starts DvcEmuManager.exe in silent mode

        CComPtr<IDeviceEmulatorManager> pDeviceEmulatorManager;
        hr = pDeviceEmulatorManager.CoCreateInstance(__uuidof(DeviceEmulatorManager));
        if (FAILED(hr)) {
            wprintf_s(L"Error: Unable to instantiate DeviceEmulatorManager. ErrorCode=0x%08X\n", hr);
            return false;
        }

        // Show the window.
        hr = pDeviceEmulatorManager->ShowManagerUI(true);
        system("pause");

        // Hide the window.
        pDeviceEmulatorManager->ShowManagerUI(false);
        system("pause");
       
        return true;
        CoUninitialize();
    }
    return 0;
}
//</Snippet1>

