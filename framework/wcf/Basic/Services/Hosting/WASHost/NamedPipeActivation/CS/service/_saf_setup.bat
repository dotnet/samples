@echo off
:: Setup the named pipe activation sample
:: Make sure the net pipe listener is started
net start NetPipeActivator
::
:: Create virtual directory 
call setupvroot.bat 

:: Configure WAS to support NamedPipe Activation
call AddNetPipeSiteBinding.cmd

:: Copy the service files
copy service.dll /y %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\bin\
copy service.svc /y %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\
copy Web.config /y %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\
