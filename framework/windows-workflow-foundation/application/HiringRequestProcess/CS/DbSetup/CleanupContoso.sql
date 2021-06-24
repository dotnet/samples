Use Master
Go
IF EXISTS (SELECT * 
	   FROM   master..sysdatabases 
	   WHERE  name = N'ContosoHr')
	DROP DATABASE ContosoHR
GO
