@echo off
setlocal

echo Deleting sample virtual directories from IIS
echo.

(reg query HKLM\Software\Microsoft\InetStp /v MajorVersion | findstr /C:"0x7") && set IsIIS7=true

@if "%IsIIS7%" == "true" (

   for /f  %%p in (Scripts\ServiceNames.txt) do (
       "%systemroot%\system32\inetsrv\AppCmd.exe" delete app "Default Web Site/FederationSample/%%p"
   )

   "%systemroot%\system32\inetsrv\AppCmd.exe" delete app "Default Web Site/FederationSample"
    %windir%\system32\inetsrv\appcmd.exe delete apppool FederationSampleAppPool

)  else (
   
   for /f  %%p in (Scripts\ServiceNames.txt) do (
      Scripts\CleanUpVdir.vbs FederationSample/%%p
   )

   Scripts\CleanUpVdir.vbs FederationSample
) 


REM Remove certs from store
echo.
call Scripts/CleanUpCerts.bat

iisreset

echo.
pause