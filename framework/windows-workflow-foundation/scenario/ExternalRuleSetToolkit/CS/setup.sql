Use Master
Go
IF EXISTS (SELECT * 
	   FROM   master..sysdatabases 
	   WHERE  name = N'Rules')
	DROP DATABASE Rules
GO
CREATE DATABASE Rules
GO
USE Rules
GO


IF EXISTS (SELECT * from dbo.sysobjects WHERE id = object_id(N'[dbo].[RuleSet]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[RuleSet]
GO

CREATE TABLE [dbo].[RuleSet]
(
	[Name]		nvarchar(128) NOT NULL,
	[MajorVersion]	int NOT NULL,
	[MinorVersion] int NOT NULL,
	[RuleSet] ntext NOT NULL,
	[Status] smallint,
	[AssemblyPath] nvarchar(256),
	[ActivityName] nvarchar(128),
	[ModifiedDate] datetime
	CONSTRAINT pk_RuleSet PRIMARY KEY([Name],[MajorVersion],[MinorVersion])
) ON [PRIMARY]
GO

CREATE UNIQUE INDEX [idx_RuleSet_NameMajorMinor] ON [dbo].[RuleSet]([Name],[MajorVersion],[MinorVersion])
GO