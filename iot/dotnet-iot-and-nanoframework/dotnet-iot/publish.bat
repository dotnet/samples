dotnet publish -r linux-arm /p:ShowLinkerSizeComparison=true 
pushd .\bin\Debug\net5.0\linux-arm\publish
pscp -pw 1234 -v -r .\* pi@192.168.1.24:DNSensorAzureIoTHub
popd