Use Master
Go
IF EXISTS (SELECT * 
	   FROM   master..sysdatabases 
	   WHERE  name = N'DbActivitiesSample')
	DROP DATABASE DbActivitiesSample
GO
