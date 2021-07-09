@echo off
rem Batch file to remove certs for sample
rem Remove STS certificate
echo Removing STS certificate.
certmgr.exe -del -c -n STS -s -r localMachine My
certmgr.exe -del -c -n STS -s -r localMachine TrustedPeople

rem Remove service certificate
echo Removing Service certificate.
certmgr.exe -del -c -n localhost -s -r localMachine My
certmgr.exe -del -c -n localhost -s -r localMachine TrustedPeople 

echo Sample Certificates removed.
