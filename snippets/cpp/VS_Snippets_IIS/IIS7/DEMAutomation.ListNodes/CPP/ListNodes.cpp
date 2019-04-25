// ListNodes.cpp : Defines the entry point for the console application.
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

        //<Snippet3>
        // For each of the four nodes in the Device Emulator Manager window
        // (Datastore, My Device Emulators, All Device Emulators, and Others)  
        for (; SUCCEEDED(hr); (hr = pDeviceEmulatorManager->MoveNext()))
        //</Snippet3>
        {
            //<Snippet4>
            // Output the name of node
            CComBSTR node;
            hr = pDeviceEmulatorManager->get_Name(&node);
            if (FAILED(hr)) {
                wprintf_s(L"Error: Failed to get current SDK category name. ErrorCode=0x%08X\n", hr);
                return false;
            }
            wprintf_s(L"- %s\n", node);
            //</Snippet4>

            //<Snippet2>
            // Get a list of SDKs/platforms in this node
            CComPtr<IEnumManagerSDKs> pSDKEnumerator;
            hr = pDeviceEmulatorManager->EnumerateSDKs(&pSDKEnumerator);
            if (FAILED(hr)) {
                wprintf_s(L"Error: Failed to get enumerator for the SDK Category[%s]. ErrorCode=0x%08X\n", node, hr);
                return false;
            }
            //</Snippet2>

            //<Snippet5>
            // For every SDK/platform in the list
            for (; SUCCEEDED(hr); (hr = pSDKEnumerator->MoveNext()))
            //</Snippet5>
            {
                //<Snippet7>
                // Output its name
                CComBSTR sdkName;
                hr = pSDKEnumerator->get_Name(&sdkName);
                if (hr == E_ENUMSDK_NOT_LOADED ) {
                    continue;
                } else if (FAILED(hr)) {
                    wprintf_s(L"Error: Failed to get SDK details. ErrorCode=0x%08X\n", hr);
                    return false;
                }
                wprintf_s(L"\t- %s\n", sdkName);
                //</Snippet7>

                //<Snippet6>
                // Get the list of emulators in the SDK/platform
                CComPtr<IEnumVMIDs> pDeviceEnumerator;
                hr = pSDKEnumerator->EnumerateVMIDs(&pDeviceEnumerator);
                if (FAILED(hr)) {
                    wprintf_s(L"Error: Failed to get enumerator for VM devices. ErrorCode=0x%08X\n", hr);
                    return false;
                }
                //</Snippet6>

                //<Snippet9>
                // For every emulator in the list
                for (; SUCCEEDED(hr); (hr = pDeviceEnumerator->MoveNext()))
                //</Snippet9>
                {
                    //<Snippet8>
                    // Get the IDeviceEmulatorManagerVMID object.
                    CComPtr<IDeviceEmulatorManagerVMID> pDevice;
                    hr = pDeviceEnumerator->GetVMID(&pDevice);
                    if (FAILED(hr)) {
                        continue;
                    }
                    //</Snippet8>

                    // Output the name of the emulator.
                    CComBSTR deviceName;
                    hr = pDevice->get_Name(&deviceName);
                    if (FAILED(hr)){
                        continue;
                    }
                    wprintf_s(L"\t\t%s\n", deviceName);
                }
            }
        }
        return true;
        CoUninitialize();
    }
    return false;
}
//</Snippet1>