@echo off

echo Removing Sample Database ...
sqlcmd -S localhost\SQLExpress -E -i "cleanup.sql"
echo Database remove successfully

Pause