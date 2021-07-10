@echo off
iisreset /stop
VER | FINDSTR /C:"6.0" >NUL
IF NOT ERRORLEVEL 1 (
    %windir%\system32\inetsrv\appcmd.exe delete app /app.name:"Default Web Site/ServiceModelSamples"
) ELSE (
    cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs DELETE w3svc/1/root/ServiceModelSamples
)
rd /s /q %SystemDrive%\inetpub\wwwroot\ServiceModelSamples
iisreset /start
