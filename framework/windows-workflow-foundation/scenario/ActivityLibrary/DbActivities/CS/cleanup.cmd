@echo off

echo Removing Sample Databases ...
Osql -S localhost\SQLExpress -E  -n -i "cleanup.sql"

Pause