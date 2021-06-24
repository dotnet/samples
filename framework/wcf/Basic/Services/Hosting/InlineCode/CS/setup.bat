@if "%_echo%"=="" echo off
@setlocal
if "%_echo%"=="" set REDIR=^>nul 2^>^&1

iisreset /stop

VER | FINDSTR /c:" 6." >NUL
IF NOT ERRORLEVEL 1 (
%windir%\system32\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:/servicemodelsamples /physicalPath:%SystemDrive%\inetpub\wwwroot\servicemodelsamples
%windir%\system32\inetsrv\appcmd.exe set apppool "DefaultAppPool" /managedRuntimeVersion:v4.0
) else (
cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs CREATE w3svc/1/root/servicemodelsamples "IIsWebVirtualDir"
cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs SET w3svc/1/root/servicemodelsamples/Path %SystemDrive%\inetpub\wwwroot\servicemodelsamples
cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs SET w3svc/1/root/servicemodelsamples/AppRoot "w3svc/1/Root/servicemodelsamples"
cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs APPCREATEPOOLPROC w3svc/1/root/servicemodelsamples
)

iisreset /start