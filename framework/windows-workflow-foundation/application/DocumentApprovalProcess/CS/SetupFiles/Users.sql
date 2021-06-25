Use Master
Go
IF EXISTS (SELECT * 
	   FROM   master..sysdatabases 
	   WHERE  name = N'DocApprovalSample')
	DROP DATABASE DocApprovalSample
GO
CREATE DATABASE DocApprovalSample
GO
USE DocApprovalSample
GO

 
CREATE TABLE [dbo].[users]
(
	[username]	varchar(255) NOT NULL,
	[usertype]	varchar(255) NOT NULL,
	[addressrequest]	varchar(255) NOT NULL,
	[guid]		varchar(255) NOT NULL,
	[addressresponse] varchar(255) NOT NULL,
)
GO
