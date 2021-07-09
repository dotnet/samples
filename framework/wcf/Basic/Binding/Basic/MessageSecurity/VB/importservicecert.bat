echo off
echo ************
echo Server cert import starting
echo ************
echo copying server cert to client's CurrentUser store
echo ************
certmgr.exe -add service.cer -r CurrentUser -s TrustedPeople
