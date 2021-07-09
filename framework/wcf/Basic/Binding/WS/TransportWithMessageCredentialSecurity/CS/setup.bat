@echo off
REM -------- Make sure Certmgr.exe and makecert.exe is in your path --------
@echo Cleaning up certs from previous runs
certmgr.exe -del -r LocalMachine -s My -c -n ServiceModelSamples-HTTPS-Server
makecert.exe -sr LocalMachine -ss My -n CN=ServiceModelSamples-HTTPS-Server -sky exchange -sk ServiceModelSamples-HTTPS-Key
