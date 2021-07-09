echo off
echo ************
echo Client cert import starting
echo ************
echo copying client cert to server's CurrentUser store
echo ************
certmgr.exe -add client.cer -r CurrentUser -s TrustedPeople
