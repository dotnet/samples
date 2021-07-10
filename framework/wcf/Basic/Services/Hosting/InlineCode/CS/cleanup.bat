@if "%_echo%"=="" echo off
@setlocal
if "%_echo%"=="" set REDIR=^>nul 2^>^&1

iisreset /stop

VER | FINDSTR /c:" 6." >NUL
IF NOT ERRORLEVEL 1 (
%windir%\system32\inetsrv\appcmd.exe delete app /app.name:"Default Web Site/servicemodelsamples"
%windir%\system32\inetsrv\appcmd.exe set apppool "DefaultAppPool" /managedRuntimeVersion:v2.0
) else (
cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs APPDELETE w3svc/1/root/servicemodelsamples
cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs DELETE w3svc/1/root/servicemodelsamples 
)

rmdir /S /Q %SystemDrive%\inetpub\wwwroot\servicemodelsamples

iisreset /start



