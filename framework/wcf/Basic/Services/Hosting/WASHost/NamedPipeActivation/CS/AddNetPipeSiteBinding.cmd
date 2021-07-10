%windir%\system32\inetsrv\appcmd.exe set site "Default Web Site" -+bindings.[protocol='net.pipe',bindingInformation='*']
%windir%\system32\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:/servicemodelsamples /physicalPath:C:\inetpub\wwwroot\servicemodelsamples
%windir%\system32\inetsrv\appcmd.exe set app "Default Web Site/servicemodelsamples" /enabledProtocols:http,net.pipe
