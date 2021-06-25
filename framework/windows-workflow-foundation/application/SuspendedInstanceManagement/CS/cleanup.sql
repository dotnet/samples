Use Master
Go
IF EXISTS (SELECT * 
	   FROM   master..sysdatabases 
	   WHERE  name = N'DefaultSampleStore')
	DROP DATABASE DefaultSampleStore
GO

