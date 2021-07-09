@echo off
rem Batch file to remove certificate with supplied name from CurrentUser\TrustedPeople store
echo Deleting certificate for %1
certmgr -del -r CurrentUser -s TrustedPeople -c -n %1
