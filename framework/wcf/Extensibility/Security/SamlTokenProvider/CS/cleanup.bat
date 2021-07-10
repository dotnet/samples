echo off
set SERVER_NAME=localhost
set USER_NAME=Alice
echo ---------------------------------------------------------------------
echo cleaning up the certificates from previous run
certmgr.exe -del -r LocalMachine -s My -c -n %SERVER_NAME%
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n %SERVER_NAME%
certmgr.exe -del -r CurrentUser -s My -c -n %USER_NAME%
certmgr.exe -del -r LocalMachine -s TrustedPeople -c -n %USER_NAME%
echo cleanup completed
echo ---------------------------------------------------------------------

