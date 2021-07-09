echo off
setlocal
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
DEL service.cer > NUL 2>&1
GOTO end

:cleancerts
REM cleans up certs from previous runs.
echo ****************
echo Cleanup starting
echo ****************

set SERVER_NAME=identity.com

echo -------------------------
echo del client certs
echo -------------------------
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n %SERVER_NAME%

echo -------------------------
echo del service certs
echo -------------------------
certmgr.exe -del -r LocalMachine -s My -c -n %SERVER_NAME%
certmgr.exe -put -r LocalMachine -s My -c -n %COMPUTER_NAME% computer.cer

IF %ERRORLEVEL% NEQ 0 GOTO cleanupcompleted

REM delete the machine cert 
certmgr.exe -del -r LocalMachine -s My -c -n %COMPUTER_NAME%
DEL computer.cer

:cleanupcompleted
echo *****************
echo Cleanup completed
echo *****************

GOTO :EOF


:end

