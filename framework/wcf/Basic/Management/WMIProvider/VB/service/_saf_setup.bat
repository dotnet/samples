@echo off
::
:: Installs the service using InstallUtil.exe 
::
%MSSDK%\bin\InstallUtil.exe service.dll 
call setupvroot.bat
copy /y service.svc  %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\
copy /y Web.Config %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\
copy /y service.dll  %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\bin
