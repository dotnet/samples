@echo off

echo Creating Rules database...
Osql -S localhost\SQLExpress -E  -n -i "setup.sql" 

Pause