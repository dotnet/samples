#include <iostream>
#include <iomanip>

#include <Contract_h.h>
#include <Contract_i.c>

int main()
{
    ::SetConsoleOutputCP(CP_UTF8);

    HRESULT hr;
    hr = ::CoInitializeEx(0, COINITBASE_MULTITHREADED);
    if (FAILED(hr))
    {
        std::cout << "CoInitializeEx failure: " << std::hex << std::showbase << hr << std::endl;
        return EXIT_FAILURE;
    }

    IServer* server;
    hr = ::CoCreateInstance(CLSID_Server, nullptr, CLSCTX_LOCAL_SERVER, __uuidof(IServer), (void **)&server);
    if (FAILED(hr))
    {
        std::cout << "CoCreateInstance failure: " << std::hex << std::showbase << hr << std::endl;
        return EXIT_FAILURE;
    }

    double pi;
    hr = server->ComputePi(&pi);
    server->Release();
    server = NULL;
    if (FAILED(hr))
    {
        std::cout << "Failure: " << std::hex << std::showbase << hr << std::endl;
        return EXIT_FAILURE;
    }

    std::cout << u8"\u03C0 = " << std::setprecision(16) << pi << std::endl;

    ::CoUninitialize();

    return EXIT_SUCCESS;
}
