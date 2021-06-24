echo off
echo ************
echo Client cert import starting
echo ************
echo copying client cert to server's LocalMachine store
echo ************
certmgr.exe -add client.cer -r LocalMachine -s TrustedPeople
