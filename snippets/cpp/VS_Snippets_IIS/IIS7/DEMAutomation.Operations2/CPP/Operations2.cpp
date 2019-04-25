// Operations2.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

//<Snippet1>
BOOL FindDevice(const CComBSTR& deviceIdentifier, IDeviceEmulatorManagerVMID** pDeviceVMID);

int _tmain(int argc, _TCHAR* argv[])
{
    if (SUCCEEDED(CoInitializeEx(NULL, COINIT_MULTITHREADED)))
    {
        // HRESULT is used to determine whether method calls are successful
        HRESULT hr;
        CComPtr<IDeviceEmulatorManagerVMID> pDevice = NULL;

        // Get the emulator by calling a helper function
        hr = FindDevice("Pocket PC 2003 SE Emulator", &pDevice);

        // If the emulator is found
        if (SUCCEEDED(hr))
        {
            // Connect to the emulator
            hr = pDevice->Connect();
            if (SUCCEEDED(hr)) wprintf_s(L"Emulator is connected\n");

            system("pause");

            // Output the Device Emulator's configuration.
            BSTR bstrConfig;
            hr = pDevice->GetConfiguration(&bstrConfig);
            if (SUCCEEDED(hr)) wprintf_s(L"Get the device's configuration\n");
            wprintf_s(bstrConfig);

            system("pause");

            // Insert code here to modify the Device Emulator's configuration XML.

            //<Snippet2>
            // Set the Device Emulator's configuration
            hr = pDevice->SetConfiguration(bstrConfig);
            if (!SUCCEEDED(hr)) wprintf_s(L"Set the device's configuration\n");
            system("pause");
            //</Snippet2>
            
            //<Snippet3>
            // Perform a soft reset 
            hr = pDevice->Reset(true);
            if (SUCCEEDED(hr)) wprintf_s(L"Resetting the device\n");
            //</Snippet3>
        }
    }
    return 0;
}

// Helper method to find a device given name or VMID
BOOL FindDevice(const CComBSTR& deviceIdentifier, IDeviceEmulatorManagerVMID** pDeviceVMID)
{
    HRESULT hr;

    // Instantiate DeviceEmulatorManager (DEM) object.
    //  This starts DvcEmuManager.exe in silent mode
    CComPtr<IDeviceEmulatorManager> pDeviceEmulatorManager;
    hr = pDeviceEmulatorManager.CoCreateInstance(__uuidof(DeviceEmulatorManager));
    if (FAILED(hr)) {
        wprintf_s(L"Error: Unable to instantiate DeviceEmulatorManager. ErrorCode=0x%08X\n", hr);
        return FALSE;
    }

    // For each of the four nodes in the Device Emulator Manager window
    // (Datastore, My Device Emulators, All Device Emulators, and Others)
    for (; SUCCEEDED(hr); (hr = pDeviceEmulatorManager->MoveNext()))
    {
        CComPtr<IEnumManagerSDKs> pSDKEnumerator;

        // Get a list of SDKs/platforms in this node
        hr = pDeviceEmulatorManager->EnumerateSDKs(&pSDKEnumerator);
        if (FAILED(hr)) {
            continue;
        }
        // For every SDK/platform in the list
        for (; SUCCEEDED(hr); (hr = pSDKEnumerator->MoveNext()))
        {
            // Get the list of emulators in the SDK/platform
            CComPtr<IEnumVMIDs> pDeviceEnumerator;
            hr = pSDKEnumerator->EnumerateVMIDs(&pDeviceEnumerator);
            if (FAILED(hr)) {
                continue;
            }
            // For every emulator in the list
            for (; SUCCEEDED(hr); (hr = pDeviceEnumerator->MoveNext()))
            {
                CComBSTR deviceName;
                CComPtr<IDeviceEmulatorManagerVMID> pDevice;

                // Get the IDeviceEmulatorManagerVMID object.
                hr = pDeviceEnumerator->GetVMID(&pDevice);
                if (FAILED(hr)) {
                    continue;
                }

                // Get the name of the emulator
                hr = pDevice->get_Name(&deviceName);
                if (FAILED(hr)){
                    continue;
                }

                // If the name of the device matches the supplied name, 
                // then this is the device we are looking for. 
                if (deviceIdentifier == deviceName){
                    *pDeviceVMID = pDevice;
                    (*pDeviceVMID)->AddRef();
                    return TRUE;
                }
            }
        }
    }
    wprintf_s(L"Error: Unable to locate the device '%s'", deviceIdentifier);
    return FALSE;
}
//</Snippet1>