@echo off
REM -------- Make sure certmgr.exe is in your path --------
REM makecert.exe -sr LocalMachine -ss My -n CN=ServiceModelSamples-HTTPS-Server -sky exchange -sk ServiceModelSamples-HTTPS-Key
certmgr.exe -del -r LocalMachine -s My -c -n ServiceModelSamples-HTTPS-Server
