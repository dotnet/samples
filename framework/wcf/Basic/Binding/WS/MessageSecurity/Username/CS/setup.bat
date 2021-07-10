echo off
setlocal
echo ************
echo cert setup starting
echo ************

call :setscriptvariables %1
IF NOT DEFINED SUPPORTED_MODE call :displayusage
IF DEFINED SUPPORTED_MODE call :cleancerts
IF DEFINED SETUP_SERVICE call :setupservice
IF DEFINED SETUP_SERVICE call :setcertpermissions
GOTO end

:cleancerts
REM cleans up certs from previous runs.
echo ****************
echo Cleanup starting
echo ****************

echo -------------------------
echo del client certs
echo -------------------------
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n localhost

echo -------------------------
echo del service certs
echo -------------------------
certmgr.exe -del -r LocalMachine -s My -c -n localhost
certmgr.exe -put -r LocalMachine -s My -c -n %COMPUTER_NAME% computer.cer
IF %ERRORLEVEL% EQU 0 (
   DEL computer.cer
   echo ****************
   echo "You have a certificate with a Subject name matching your Machine name: %COMPUTER_NAME%"
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

:setupservice

echo ************
echo Server cert setup starting
echo %SERVER_NAME%
echo ************
echo making server cert
echo ************
makecert.exe -sr LocalMachine -ss MY -a sha1 -n CN=%SERVER_NAME% -sky exchange -pe

IF DEFINED EXPORT_SERVICE (
    echo ************
    echo exporting service cert to service.cer
    echo ************
    certmgr.exe -put -r LocalMachine -s My -c -n %SERVER_NAME% service.cer
) ELSE (
    echo ************
    echo copying server cert to client's CurrentUser store
    echo ************
    certmgr.exe -add -r LocalMachine -s My -c -n %SERVER_NAME% -r CurrentUser -s TrustedPeople
)
GOTO :EOF

:setcertpermissions

echo ************
echo setting privileges on server certificates
echo ************
for /F "delims=" %%i in ('"%ProgramFiles%\ServiceModelSampleTools\FindPrivateKey.exe" My LocalMachine -n CN^=%SERVER_NAME% -a') do set PRIVATE_KEY_FILE=%%i

set OS_VERSION=
set OS_MAJOR=
set OS_MINOR=
set OS_BUILD=
set IsWin7OrHigher=
set WP_ACCOUNT=

for /f "skip=1" %%i in ( 'wmic os get version' ) do ( 
    set OS_VERSION=%%i 
    goto:__ver_done
)
:__ver_done

for /f "delims=. tokens=1,2,3" %%i in ("%OS_VERSION%") do ( 
    set OS_MAJOR=%%i&set OS_MINOR=%%j&set OS_BUILD=%%k  
    goto :__ver_split_done
)
:__ver_split_done

if "%OS_MAJOR%" GEQ "6" (
    if "%OS_MINOR%" == "1" (
        set IsWin7OrHigher=true
        goto :__ver_set_done
    )
    if "%OS_MAJOR%" GTR "6" (
        set IsWin7OrHigher=true
        goto :__ver_set_done
    )
)

:__ver_set_done

if "%IsWin7OrHigher%" == "true" ( 
     set WP_ACCOUNT=IIS_IUSRS
) else (
     set WP_ACCOUNT=NT AUTHORITY\NETWORK SERVICE
)
(ver | findstr /C:"5.1") && set WP_ACCOUNT=%COMPUTERNAME%\ASPNET
echo Y|cacls.exe "%PRIVATE_KEY_FILE%" /E /G "%WP_ACCOUNT%":R
iisreset

GOTO :EOF

:setscriptvariables
REM Parses the input to determine if we are setting this up for a single machine, client, or server
REM sets the appropriate name variables
call :setcomputername
IF [%1]==[] CALL :singlemachine
IF [%1]==[service] CALL :service

GOTO :EOF

:singlemachine
echo ************
echo Running setup script for Single Machine
echo ************
SET SUPPORTED_MODE=1
SET SETUP_SERVICE=1
SET SERVER_NAME=localhost
GOTO :EOF

:service
echo ************
echo Running setup script for Service
echo ************
SET SUPPORTED_MODE=1
SET SETUP_SERVICE=1
SET EXPORT_SERVICE=1
SET SERVER_NAME=%COMPUTER_NAME%
GOTO :EOF

:setcomputername
REM Puts the Fully Qualified Name of the Computer into a variable named COMPUTER_NAME
for /F "delims=" %%i in ('cscript /nologo GetComputerName.vbs') do set COMPUTER_NAME=%%i
GOTO :EOF

:displayusage
ECHO Correct usage:
ECHO     Single Machine - Setup.bat
ECHO     Service Machine - Setup.bat service
:end
