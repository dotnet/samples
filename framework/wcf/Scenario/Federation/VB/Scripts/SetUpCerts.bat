@echo off
setlocal

makecert.exe -a sha1 -n CN=BookStoreService.com -sr LocalMachine -ss My -sky exchange -sk BookStoreService
certmgr.exe -add -c -n BookStoreService -s -r localMachine My -s -r localMachine TrustedPeople

makecert.exe -a sha1 -n CN=BookStoreSTS.com -sr LocalMachine -ss My -sky exchange -sk BookStoreSTS
certmgr.exe -add -c -n BookStoreSTS.com -s -r localMachine My -s -r localMachine TrustedPeople

makecert.exe -a sha1 -n CN=HomeRealmSTS.com -sr LocalMachine -ss My -sky exchange -sk HomeRealmSTS
certmgr.exe -add -c -n HomeRealmSTS.com -s -r localMachine My -s -r localMachine TrustedPeople

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


for /F "delims=" %%i in ('"%ProgramFiles%\ServiceModelSampleTools\FindPrivateKey.exe" My LocalMachine -n CN^=BookStoreService.com -a') do set PRIVATE_KEY_FILE=%%i
echo %PRIVATE_KEY_FILE%
(ver | findstr /C:"5.1") && set WP_ACCOUNT=%COMPUTERNAME%\ASPNET
echo Y|cacls.exe "%PRIVATE_KEY_FILE%" /E /G "%WP_ACCOUNT%":R

for /F "delims=" %%i in ('"%ProgramFiles%\ServiceModelSampleTools\FindPrivateKey.exe" My LocalMachine -n CN^=BookStoreSTS.com -a') do set PRIVATE_KEY_FILE=%%i
echo %PRIVATE_KEY_FILE%
(ver | findstr /C:"5.1") && set WP_ACCOUNT=%COMPUTERNAME%\ASPNET
echo Y|cacls.exe "%PRIVATE_KEY_FILE%" /E /G "%WP_ACCOUNT%":R

for /F "delims=" %%i in ('"%ProgramFiles%\ServiceModelSampleTools\FindPrivateKey.exe" My LocalMachine -n CN^=HomeRealmSTS.com -a') do set PRIVATE_KEY_FILE=%%i
echo %PRIVATE_KEY_FILE%
(ver | findstr /C:"5.1") && set WP_ACCOUNT=%COMPUTERNAME%\ASPNET
echo Y|cacls.exe "%PRIVATE_KEY_FILE%" /E /G "%WP_ACCOUNT%":R
