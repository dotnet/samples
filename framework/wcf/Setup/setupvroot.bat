@echo off
iisreset /stop
mkdir %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\bin
VER | FINDSTR /c:" 6.0." >NUL
IF NOT ERRORLEVEL 1 (
    IF EXIST %windir%\system32\inetsrv\appcmd.exe (
        %windir%\system32\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:/ServiceModelSamples /physicalPath:%systemdrive%\inetpub\wwwroot\ServiceModelSamples
    ) ELSE (
        Echo "Could not find %windir%\system32\inetsrv\appcmd.exe.  Please ensure IIS is installed.  See 'Internet Information Service (IIS) Hosting Instructions' in the WCF samples Setup Instructions."
    )
) ELSE (
    IF EXIST %SystemDrive%\inetpub\adminscripts\adsutil.vbs (
        cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs CREATE w3svc/1/root/ServiceModelSamples "IIsWebVirtualDir"
        cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs SET w3svc/1/root/ServiceModelSamples/Path %SystemDrive%\inetpub\wwwroot\ServiceModelSamples
        cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs SET w3svc/1/root/ServiceModelSamples/AppRoot "w3svc/1/Root/ServiceModelSamples"
        cscript.exe %SystemDrive%\inetpub\adminscripts\adsutil.vbs APPCREATEPOOLPROC w3svc/1/root/ServiceModelSamples
    ) ELSE (
        Echo "Could not find %SystemDrive%\inetpub\adminscripts\adsutil.vbs.  Please ensure IIS is installed.  See 'Internet Information Service (IIS) Hosting Instructions' in the WCF samples Setup Instructions."
    )
    
)
iisreset /start

