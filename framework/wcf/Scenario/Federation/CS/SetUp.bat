
@echo off
setlocal

set SamplesVRootDir=%SystemDrive%\inetpub\wwwroot\FederationSample
mkdir %SamplesVRootDir%

echo Creating virtual directories...

echo.
echo FederationSample

(reg query HKLM\Software\Microsoft\InetStp /v MajorVersion | findstr /C:"0x7") && set IsIIS7=true

@if "%IsIIS7%" == "true" (

   %windir%\system32\inetsrv\appcmd.exe add apppool /name:FederationSampleAppPool /managedRuntimeVersion:v4.0 /processModel.loadUserProfile:true

   "%systemroot%\system32\inetsrv\AppCmd.exe" add app /site.name:"Default Web Site" /path:/FederationSample /physicalPath:%SamplesVRootDir% /applicationPool:FederationSampleAppPool

   for /f %%p in (Scripts\ServiceNames.txt) do (
       "%systemroot%\system32\inetsrv\AppCmd.exe" add app /site.name:"Default Web Site" /path:/FederationSample/%%p /physicalPath:%SamplesVRootDir%\%%p /applicationPool:FederationSampleAppPool
   )

) else (

  Scripts\SetUpVDir.vbs FederationSample %SamplesVRootDir%
  
  for /f %%p in (Scripts\ServiceNames.txt) do (
      Scripts\SetUpVDir.vbs FederationSample/%%p %SamplesVRootDir%\%%p
  )

)  

call Scripts\SetUpCerts.bat

iisreset

echo Done.
pause