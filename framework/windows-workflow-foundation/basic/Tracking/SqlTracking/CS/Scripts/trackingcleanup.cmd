echo Deleting Tracking database...
Osql -S %COMPUTERNAME%\SQLExpress -E  -n -i "DeleteTrackingSampleDatabase.sql" 

::Pause