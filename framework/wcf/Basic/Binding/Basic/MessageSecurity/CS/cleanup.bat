echo off
setlocal
set CLIENT_NAME=client.com
call :setcomputername
ECHO ****************************************************************
ECHO WARNING:  This script will not remove service certificates on a 
ECHO           client machine from a cross machine run of this
ECHO           sample.

ECHO If you have run WCF samples that use Certs across machines, 
ECHO be sure to clear the service certs that have been installed in
ECHO the CurrentUser - TrustedPeople store.
ECHO To do this, use the following command:

ECHO "certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n <Fully Qualified Server Machine Name>"

ECHO For example:

ECHO "certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n server1.contoso.com"
call :cleancerts
DEL client.cer > NUL 2>&1
DEL service.cer > NUL 2>&1
GOTO end

:cleancerts
REM cleans up certs from previous runs.
echo ****************
echo Cleanup starting
echo ****************

echo -------------------------
echo del client certs
echo -------------------------
certmgr.exe -del -r CurrentUser -s My -c -n %CLIENT_NAME%
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n localhost

echo -------------------------
echo del service certs
echo -------------------------
certmgr.exe -del -r LocalMachine -s My -c -n localhost
certmgr.exe -del -r LocalMachine -s TrustedPeople -c -n %CLIENT_NAME%
certmgr.exe -put -r LocalMachine -s My -c -n %COMPUTER_NAME% computer.cer
IF %ERRORLEVEL% EQU 0 (
   DEL computer.cer
   echo ****************
   echo "You have a certificate with a Subject name matching your Machine name."
   echo "If this certificate is from a cross machine run of WCF samples press any key to delete it."
   echo "Otherwise press Ctrl + C to abort this script."
   pause
   certmgr.exe -del -r LocalMachine -s My -c -n %COMPUTER_NAME%
)

:cleanupcompleted
echo *****************
echo Cleanup completed
echo *****************

GOTO :EOF


:setcomputername
REM Puts the Fully Qualified Name of the Computer into a variable named COMPUTER_NAME
for /F "delims=" %%i in ('cscript /nologo GetComputerName.vbs') do set COMPUTER_NAME=%%i
GOTO :EOF

:end

