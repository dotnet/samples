echo off

setlocal
echo ************
echo cert setup starting
echo ************

call :setscriptvariables %1
IF NOT DEFINED SUPPORTED_MODE call :displayusage
IF DEFINED SETUP_SERVICE call :setupservice
GOTO end

:setupservice

set PFX_FILE=identity.pfx
set PFX_PASSWORD=xyz
set LocalMachine_My=0
set CN=identity
set SERIAL=30 3e 60 f8 cb 96 95 81 48 33 6f da f2 09 0b b7

REM Determine which certificates are installed on the machine.
for /f "delims=" %%l in ('certmgr.exe -all -s -r LocalMachine My') do (

       if /i "%%l" == "   %SERIAL%" (
           set LocalMachine_My=1
       )
)


if %LocalMachine_My% == 1 (
	goto copycert
)

REM If this is Windows XP show help text indicating the certificate needs to be imported manually.
	set XP=0
	(ver | findstr /C:"5.1") && set XP=1
	if "%XP%"=="1" (
		goto displaywinxpusage
	)

REM Import server certificates on Windows 2003 - certutil is only on Windows 2003
if NOT %LocalMachine_My% == 1 (
	echo ************
	echo Server cert setup starting
	echo Installing %SERVER_NAME% certificate into the LocalMachine/My store
	echo ************
	echo Importing %PFX_FILE% to LocalMachine/My store ...
	echo ************
   	certutil -importpfx -p %PFX_PASSWORD% %PFX_FILE%
)

:copycert

IF DEFINED EXPORT_SERVICE GOTO exportservice

echo ************
echo copying server cert to client's CurrentUser store
echo ************
certmgr.exe -add -r LocalMachine -s My -c -n %SERVER_NAME% -r CurrentUser -s TrustedPeople
GOTO :EOF

set XP=0
(ver | findstr /C:"5.1") && set XP=1
if NOT "%XP%"=="1" (
	goto end
)

REM Show instructions for adding server certificates
:displaywinxpusage
    echo.
    echo ********************************
    echo MANUAL SET UP INSTRUCTIONS:
    echo ********************************
    echo Use the MMC Console Certificates Snap-in to install the 
    echo required server certificates into the 
    echo LocalMachine/My ^(Personal^) certificate store.
    echo.
    echo The password for the PFX files is "%PFX_PASSWORD%".
    echo.
    echo Then run this script again copy the server certificate
    echo to the currentuser trusted people store.
    echo.
    echo SEE ALSO:
    echo See this sample's ReadMe topic in the documentation for detailed instructions.
    echo.
    pause
GOTO :EOF

:cleancerts
REM cleans up certs from previous runs.
echo ****************
echo Cleanup starting
echo ****************

echo -------------------------
echo del client certs
echo -------------------------
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n localhost
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n %SERVER_NAME%

echo -------------------------
echo del service certs
echo -------------------------
certmgr.exe -del -r LocalMachine -s My -c -n localhost
certmgr.exe -del -r LocalMachine -s My -c -n %SERVER_NAME%

:cleanupcompleted
echo *****************
echo Cleanup completed
echo *****************
GOTO :EOF

:exportservice

echo ************
echo exporting service cert to service.cer
echo ************
certmgr.exe -put -r LocalMachine -s My -c -n identity.com service.cer
GOTO :EOF

:setscriptvariables
REM Parses the input to determine if we are setting this up for a single machine, client, or server
REM sets the appropriate name variables
IF [%1]==[] CALL :singlemachine
IF [%1]==[service] CALL :service
GOTO :EOF

:cleancerts
REM cleans up certs from previous runs.
echo ****************
echo Cleanup starting
echo ****************

echo -------------------------
echo del client certs
echo -------------------------
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n localhost
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n %SERVER_NAME%

echo -------------------------
echo del service certs
echo -------------------------
certmgr.exe -del -r LocalMachine -s My -c -n localhost
certmgr.exe -del -r LocalMachine -s My -c -n %SERVER_NAME%

:cleanupcompleted
echo *****************
echo Cleanup completed
echo *****************
GOTO :EOF


:singlemachine
echo ************
echo Running setup script for Single Machine
echo ************
SET SUPPORTED_MODE=1
SET SETUP_SERVICE=1
SET SERVER_NAME=identity.com
GOTO :EOF

:service
echo ************
echo Running setup script for Service
echo ************
SET SUPPORTED_MODE=1
SET SETUP_SERVICE=1
SET EXPORT_SERVICE=1
SET SERVER_NAME=identity.com
GOTO :EOF

:displayusage
ECHO Correct usage:
ECHO     Single Machine - Setup.bat
ECHO     Service Machine - Setup.bat service

:end
