@echo off
:: Setup the TCP activation sample
:: Make sure the TCP listeners are started
net start NetTcpActivator
net start NetTcpPortSharing
::
:: Create virtual directory 
call setupvroot.bat 

:: Configure WAS to support TCP Activation
call AddNetTcpSiteBinding.cmd

:: Copy the service files
copy service.dll /y %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\bin\
copy service.svc /y %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\
copy Web.config /y %SystemDrive%\inetpub\wwwroot\ServiceModelSamples\
