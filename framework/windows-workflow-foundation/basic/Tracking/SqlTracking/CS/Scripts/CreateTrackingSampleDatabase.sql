-- Copyright (c) Microsoft Corporation.  All rights reserved.
Use Master
Go
IF EXISTS (SELECT * 
	   FROM   master..sysdatabases 
	   WHERE  name = N'TrackingSample')
	DROP DATABASE TrackingSample
GO
CREATE DATABASE TrackingSample
GO

