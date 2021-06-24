@echo off
rem Batch file to create certs for sample
rem Create SecurityTokenService certificate
echo Setting up STS Certificate
makecert.exe -sr LocalMachine -ss My -a sha1 -n CN=STS -sky exchange -pe -sk 974877a3-6154-40e4-b688-25fe8962dc17
certmgr.exe -add -c -n STS -s -r localMachine My -s -r localMachine TrustedPeople

rem Create service certificate
echo Setting up Service Certificate
makecert.exe -sr LocalMachine -ss My -a sha1 -n CN=localhost -sky exchange -pe -sk 974877a3-6154-40e4-b688-25fe8962dc18
certmgr.exe -add -c -n localhost -s -r localMachine My -s -r localMachine TrustedPeople

echo Sample certificates setup successfully
