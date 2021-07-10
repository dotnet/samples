@echo off
::
:: Unnstalls the service using InstallUtil.exe 
::
call cleanupvroot.bat
%MSSDK%\bin\InstallUtil.exe /u service.dll 
