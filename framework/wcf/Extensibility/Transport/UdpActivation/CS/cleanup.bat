@echo off
set ISVISTA=0
ver | findstr /c:" 6." && set ISVISTA=1

echo Remove protocol binding for "net.udp".
if "%ISVISTA%" == "1" (
	%windir%\system32\inetsrv\AppCmd.exe set site "Default Web Site" /-bindings.[protocol='net.udp'] > NUL:
)

@echo on
