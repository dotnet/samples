@echo off

echo Creating Sample Database ...
sqlcmd -S localhost\SQLExpress -E -i "DatabaseCreation.sql"

echo Creating Persistence Schema ...
sqlcmd -S localhost\SQLExpress -E -d DefaultSampleStore -i %FrameworkDir%%FrameworkVersion%\sql\en\SqlWorkflowInstanceStoreSchema.sql

echo Creating Persistence Logic ...
sqlcmd -S localhost\SQLExpress -E -d DefaultSampleStore -i %FrameworkDir%%FrameworkVersion%\sql\en\SqlWorkflowInstanceStoreLogic.sql

echo Adding Instance Store Users ...
sqlcmd -S localhost\SQLExpress -E -d DefaultSampleStore -i "AddInstanceStoreUsers.sql"

echo Done

Pause