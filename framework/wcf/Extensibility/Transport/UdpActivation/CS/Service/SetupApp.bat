@echo off
set ISVISTA=0
ver | findstr /c:" 6." && set ISVISTA=1

echo Create bin directory
if not exist %SystemDrive%\inetpub\wwwroot\servicemodelsamples\bin mkdir %SystemDrive%\inetpub\wwwroot\servicemodelsamples\bin

echo Create protocol binding for "net.udp".
if "%ISVISTA%" == "1" (
	%windir%\system32\inetsrv\AppCmd.exe set site "Default Web Site" /-bindings.[protocol='net.udp'] > NUL:
	%windir%\system32\inetsrv\AppCmd.exe set site "Default Web Site" /+bindings.[protocol='net.udp',bindingInformation='8080']
)

echo Create virtual directory.
if "%ISVISTA%" == "1" (
	%windir%\system32\inetsrv\AppCmd.exe delete app /app.name:"Default Web Site/servicemodelsamples" > NUL:
        %windir%\system32\inetsrv\AppCmd.exe add app /site.name:"Default Web Site" /path:/servicemodelsamples /physicalPath:%SystemDrive%\inetpub\wwwroot\servicemodelsamples /enabledProtocols:http,net.udp
)

@echo on
