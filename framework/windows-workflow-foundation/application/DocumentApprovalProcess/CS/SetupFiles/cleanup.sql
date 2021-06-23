Use Master
Go
IF EXISTS (SELECT * 
	   FROM   master..sysdatabases 
	   WHERE  name = N'DocApprovalSample')
	DROP DATABASE DocApprovalSample
GO
