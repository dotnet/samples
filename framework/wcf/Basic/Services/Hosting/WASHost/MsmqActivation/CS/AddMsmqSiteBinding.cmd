%windir%\system32\inetsrv\appcmd.exe set site "Default Web Site" -+bindings.[protocol='net.msmq',bindingInformation='localhost']
%windir%\system32\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:/servicemodelsamples /physicalPath:%SystemDrive%\inetpub\wwwroot\servicemodelsamples
%windir%\system32\inetsrv\appcmd.exe set app "Default Web Site/servicemodelsamples" /enabledProtocols:http,net.msmq
%windir%\system32\inetsrv\appcmd.exe set apppool "DefaultAppPool" /managedRuntimeVersion:v4.0



