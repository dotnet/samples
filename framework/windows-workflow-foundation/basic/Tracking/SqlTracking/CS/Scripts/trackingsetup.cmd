echo Create SQL Tracking database...
Osql -S %COMPUTERNAME%\SQLEXPRESS -E -n -i "CreateTrackingSampleDatabase.sql"

echo Creating Tracking database tables...
Osql -S %COMPUTERNAME%\SQLEXPRESS -E -n -d TrackingSample -i "TrackingSample_schema.sql"

::Pause