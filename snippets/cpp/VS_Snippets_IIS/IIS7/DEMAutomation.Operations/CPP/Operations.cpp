// Operations.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

//<Snippet1>
BOOL FindDevice(const CComBSTR& deviceIdentifier, IDeviceEmulatorManagerVMID** pDeviceVMID);

int _tmain(int argc, _TCHAR* argv[])
{
    if (SUCCEEDED(CoInitializeEx(NULL, COINIT_MULTITHREADED)))
    {
        HRESULT hr;
        CComPtr<IDeviceEmulatorManagerVMID> pDevice = NULL;

        // Get the emulator by calling a helper function
        hr = FindDevice("Pocket PC 2003 SE Emulator", &pDevice);

        // If the emulator is found
        if (SUCCEEDED(hr))
        {
            //<Snippet5>
            // Output the name of the emulator
            CComBSTR deviceName;
            hr = pDevice->get_Name(&deviceName);
            if (SUCCEEDED(hr)) wprintf_s(L"Device Name: %s\n", deviceName);
            //</Snippet5>

            // Output the emulator's VMID
            CComBSTR VMID;
            hr = pDevice->get_VMID(&VMID);
            if (SUCCEEDED(hr)) wprintf_s(L"Device VMID: %s\n", VMID);

            //<Snippet7>
            // Output the emulator's current state
            EMULATOR_STATE deviceState = EMU_NOT_RUNNING;
            hr = pDevice->get_State(&deviceState);
            if (SUCCEEDED(hr))
            {
                if (deviceState == EMU_CRADLED) wprintf_s(L"Emulator is Cradled\n");
                else if (deviceState == EMU_RUNNING) wprintf_s(L"Emulator is Running\n");
                else wprintf_s(L"Emulator is Not Running\n");
            }
            //</Snippet7>
            
            //<Snippet6>
            // Connect to the emulator
            hr = pDevice->Connect();
            if (SUCCEEDED(hr)) wprintf_s(L"Emulator is connected\n");
            //</Snippet6>

            //<Snippet2>
            // Make the Device Emulator window visible
            hr = pDevice->BringToFront();
            if (SUCCEEDED(hr)) wprintf_s(L"Device Emulator window is on top\n");
            //</Snippet2>

            //<Snippet4>
            // Cradle the emulator
            hr = pDevice->Cradle();
            if (SUCCEEDED(hr)) wprintf_s(L"Emulator is cradled\n");
            system("pause");
            //</Snippet4>

            //<Snippet3>
            // Uncradle the emulator
            hr = pDevice->UnCradle();
            if (SUCCEEDED(hr)) wprintf_s(L"Emulator is uncradled\n");
            //</Snippet3>

            //<Snippet10>
            // Save state and shutdown the emulator
            hr = pDevice->Shutdown(true);
            if (SUCCEEDED(hr)) wprintf_s(L"Emulator is shutdown\n");
            //</Snippet10>

            //<Snippet8>
            // Delete the .dess save state file for this device
            hr = pDevice->ClearSaveState();
            if (SUCCEEDED(hr)) wprintf_s(L"Save state file is deleted");
            system("pause");
            //</Snippet8>
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

            //<Snippet9>
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
            //</Snippet9>
        }
    }
    wprintf_s(L"Error: Unable to locate the device '%s'", deviceIdentifier);
    return FALSE;
}
//</Snippet1>