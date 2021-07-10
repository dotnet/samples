%windir%\system32\inetsrv\appcmd.exe set app "Default Web Site/servicemodelsamples" /enabledProtocols:http
%windir%\system32\inetsrv\appcmd.exe delete app /app.name:"Default Web Site/servicemodelsamples"
%windir%\system32\inetsrv\appcmd.exe set site "Default Web Site" --bindings.[protocol='net.tcp',bindingInformation='808:*']
%windir%\system32\inetsrv\appcmd.exe set apppool "DefaultAppPool" /managedRuntimeVersion:v2.0
rmdir /S /Q %SystemDrive%\inetpub\wwwroot\servicemodelsamples