echo off
set SERVER_NAME=localhost
echo ---------------------------------------------------------------------
echo cleaning up the certificates from previous run
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n %SERVER_NAME%
certmgr.exe -del -r LocalMachine -s My -c -n %SERVER_NAME%
echo cleanup completed
echo ---------------------------------------------------------------------

