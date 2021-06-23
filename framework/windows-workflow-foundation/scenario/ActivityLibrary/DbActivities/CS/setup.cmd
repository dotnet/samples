@echo off

echo Creating DbActivitiesSample Database ...
Osql -S localhost\SQLExpress -E  -n -i "createdb.sql"

Pause