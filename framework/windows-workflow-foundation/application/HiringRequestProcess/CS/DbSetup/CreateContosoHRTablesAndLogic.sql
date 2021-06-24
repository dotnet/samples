-- Copyright (c) Microsoft Corporation.  All rights reserved.
USE [master]
GO
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'InstanceStore')
DROP DATABASE [InstanceStore]
GO
CREATE DATABASE [InstanceStore]
GO

GO
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'ContosoHR2')
DROP DATABASE [ContosoHR]
GO
CREATE DATABASE [ContosoHR]
GO

USE [ContosoHR]
GO
/****** Object:  StoredProcedure [dbo].[InsertInEmployeeInbox]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[InsertInEmployeeInbox]
GO
/****** Object:  StoredProcedure [dbo].[InsertJobPostingResumee]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[InsertJobPostingResumee]
GO
/****** Object:  StoredProcedure [dbo].[InsertResumee]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[InsertResumee]
GO
/****** Object:  StoredProcedure [dbo].[ArchiveInboxRequest]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[ArchiveInboxRequest]
GO
/****** Object:  StoredProcedure [dbo].[CleanData]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[CleanData]
GO
/****** Object:  StoredProcedure [dbo].[SaveHiringRequest]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SaveHiringRequest]
GO
/****** Object:  StoredProcedure [dbo].[SaveJobPosting]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SaveJobPosting]
GO
/****** Object:  StoredProcedure [dbo].[SaveRequestHistory]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SaveRequestHistory]
GO
/****** Object:  StoredProcedure [dbo].[SelectActiveJobPostings]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectActiveJobPostings]
GO
/****** Object:  StoredProcedure [dbo].[SelectArchivedRequestsStartedBy]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectArchivedRequestsStartedBy]
GO
/****** Object:  StoredProcedure [dbo].[SelectClosedJobPostings]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectClosedJobPostings]
GO
/****** Object:  StoredProcedure [dbo].[SelectHiringRequest]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectHiringRequest]
GO
/****** Object:  StoredProcedure [dbo].[SelectHiringRequestHistory]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectHiringRequestHistory]
GO
/****** Object:  StoredProcedure [dbo].[SelectJobPosting]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectJobPosting]
GO
/****** Object:  StoredProcedure [dbo].[SelectJobPostingResumees]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectJobPostingResumees]
GO
/****** Object:  StoredProcedure [dbo].[SelectNotStartedJobPostings]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectNotStartedJobPostings]
GO
/****** Object:  StoredProcedure [dbo].[SelectRequestsFor]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectRequestsFor]
GO
/****** Object:  StoredProcedure [dbo].[SelectRequestsStartedBy]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[SelectRequestsStartedBy]
GO
/****** Object:  StoredProcedure [dbo].[UpdateResumeesCountInJobPosting]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[UpdateResumeesCountInJobPosting]
GO
/****** Object:  StoredProcedure [dbo].[RemoveFromEmployeeInbox]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[RemoveFromEmployeeInbox]
GO
/****** Object:  StoredProcedure [dbo].[RemoveFromInbox]    Script Date: 11/12/2009 16:17:39 ******/
DROP PROCEDURE [dbo].[RemoveFromInbox]
GO
/****** Object:  Table [dbo].[RequestHistory]    Script Date: 11/12/2009 16:17:42 ******/
DROP TABLE [dbo].[RequestHistory]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 11/12/2009 16:17:41 ******/
DROP TABLE [dbo].[Departments]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 11/12/2009 16:17:42 ******/
DROP TABLE [dbo].[Employees]
GO
/****** Object:  Table [dbo].[HiringRequests]    Script Date: 11/12/2009 16:17:42 ******/
ALTER TABLE [dbo].[HiringRequests] DROP CONSTRAINT [DF_HiringRequests_IsCompleted]
GO
ALTER TABLE [dbo].[HiringRequests] DROP CONSTRAINT [DF_HiringRequests_IsSuccess]
GO
ALTER TABLE [dbo].[HiringRequests] DROP CONSTRAINT [DF_HiringRequests_IsCancelled]
GO
DROP TABLE [dbo].[HiringRequests]
GO
/****** Object:  Table [dbo].[InboxItems]    Script Date: 11/12/2009 16:17:42 ******/
DROP TABLE [dbo].[InboxItems]
GO
/****** Object:  Table [dbo].[InboxItemsArchive]    Script Date: 11/12/2009 16:17:42 ******/
DROP TABLE [dbo].[InboxItemsArchive]
GO
/****** Object:  Table [dbo].[InboxItemsByEmployee]    Script Date: 11/12/2009 16:17:42 ******/
DROP TABLE [dbo].[InboxItemsByEmployee]
GO
/****** Object:  Table [dbo].[JobPostingResumees]    Script Date: 11/12/2009 16:17:42 ******/
DROP TABLE [dbo].[JobPostingResumees]
GO
/****** Object:  Table [dbo].[JobPostings]    Script Date: 11/12/2009 16:17:42 ******/
ALTER TABLE [dbo].[JobPostings] DROP CONSTRAINT [DF_JobPosting_CreationDate]
GO
DROP TABLE [dbo].[JobPostings]
GO
/****** Object:  Table [dbo].[Positions]    Script Date: 11/12/2009 16:17:42 ******/
DROP TABLE [dbo].[Positions]
GO
/****** Object:  Table [dbo].[PositionTypes]    Script Date: 11/12/2009 16:17:42 ******/
DROP TABLE [dbo].[PositionTypes]
GO
/****** Object:  Table [dbo].[PositionTypes]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PositionTypes](
	[Id] [char](8) NOT NULL,
	[Name] [varchar](128) NOT NULL,
 CONSTRAINT [PK_PositionTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[PositionTypes] ([Id], [Name]) VALUES (N'CEO', N'CEO')
INSERT [dbo].[PositionTypes] ([Id], [Name]) VALUES (N'DIR', N'Director')
INSERT [dbo].[PositionTypes] ([Id], [Name]) VALUES (N'IC', N'Individual Contributor')
INSERT [dbo].[PositionTypes] ([Id], [Name]) VALUES (N'MGR', N'Manager')
/****** Object:  Table [dbo].[Positions]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Positions](
	[Id] [char](8) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[PositionType] [char](8) NOT NULL,
 CONSTRAINT [PK_Positions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'AN', N'System Analyst', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'ASSIST', N'Assistant', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'CEO', N'CEO', N'CEO')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'DEVMGR', N'Development Manager', N'MGR')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'HRAN', N'HR Analyst', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'HRDIR', N'HR Director', N'DIR')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'HRMGR', N'HR Manager', N'MGR')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'HRT', N'HR Technician', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'ITDIR', N'Director', N'DIR')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'MKTAN', N'Marketing Analyst', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'MKTDIR', N'Marketing Director', N'DIR')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'MKTECH', N'Marketing Technician', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'MKTMGR', N'Marketing Manager', N'MGR')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'REC', N'Recruiter', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'RECMGR', N'Recruiting Manager', N'MGR')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'SDE', N'Software Engineer', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'SDET', N'Software Engineer in Test', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'SDIR', N'Sales Director', N'DIR')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'SL', N'Salesman', N'IC')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'SMGR', N'Sales Manager', N'MGR')
INSERT [dbo].[Positions] ([Id], [Name], [PositionType]) VALUES (N'SRSL', N'Senior Salesman', N'IC')
/****** Object:  Table [dbo].[JobPostings]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[JobPostings](
	[Id] [uniqueidentifier] NOT NULL,
	[HiringRequestId] [uniqueidentifier] NULL,
	[Title] [varchar](1024) NULL,
	[Description] [text] NULL,
	[ResumeesReceived] [int] NULL,
	[Status] [varchar](64) NULL,
	[CreationDate] [datetime] NULL CONSTRAINT [DF_JobPosting_CreationDate]  DEFAULT (getdate()),
	[LastUpdate] [datetime] NULL,
	[LastResumeeDate] [datetime] NULL,
 CONSTRAINT [PK_JobPosting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[JobPostingResumees]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[JobPostingResumees](
	[JobPostingId] [uniqueidentifier] NOT NULL,
	[CandidateMail] [varchar](512) NOT NULL,
	[CandidateName] [varchar](512) NULL,
	[Resumee] [text] NULL,
	[ReceivedDate] [datetime] NULL,
 CONSTRAINT [PK_JobPostingResumees] PRIMARY KEY CLUSTERED 
(
	[JobPostingId] ASC,
	[CandidateMail] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InboxItemsByEmployee]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InboxItemsByEmployee](
	[RequestId] [uniqueidentifier] NOT NULL,
	[EmployeeId] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InboxItemsArchive]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InboxItemsArchive](
	[RequestId] [uniqueidentifier] NOT NULL,
	[StartedBy] [int] NOT NULL,
	[Title] [varchar](1024) NOT NULL,
	[State] [varchar](512) NOT NULL,
	[InboxEntryDate] [datetime] NOT NULL,
	[ArchivalDate] [datetime] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InboxItems]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InboxItems](
	[RequestId] [uniqueidentifier] NOT NULL,
	[StartedBy] [int] NOT NULL,
	[Title] [varchar](1024) NOT NULL,
	[State] [varchar](512) NOT NULL,
	[InboxEntryDate] [date] NOT NULL,
 CONSTRAINT [PK_InboxItems] PRIMARY KEY CLUSTERED 
(
	[RequestId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HiringRequests]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HiringRequests](
	[Id] [uniqueidentifier] NOT NULL,
	[RequesterId] [varchar](16) NOT NULL,
	[CreationDate] [date] NOT NULL,
	[PositionId] [varchar](16) NOT NULL,
	[DepartmentId] [varchar](16) NOT NULL,
	[Description] [varchar](1024) NULL,
	[Title] [varchar](1024) NOT NULL,
	[WorkflowInstanceId] [uniqueidentifier] NULL,
	[IsCompleted] [bit] NULL CONSTRAINT [DF_HiringRequests_IsCompleted]  DEFAULT ((0)),
	[IsSuccess] [bit] NULL CONSTRAINT [DF_HiringRequests_IsSuccess]  DEFAULT ((0)),
	[IsCancelled] [bit] NULL CONSTRAINT [DF_HiringRequests_IsCancelled]  DEFAULT ((0)),
 CONSTRAINT [PK_HiringRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Employees](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](512) NOT NULL,
	[Position] [char](8) NOT NULL,
	[Department] [char](8) NOT NULL,
	[ManagerId] [int] NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (1, N'Alexander, Michael', N'SDE', N'ENG', 6)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (2, N'Harui, Roger', N'SDE', N'ENG', 6)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (3, N'Petculescu, Cristian', N'SDET', N'ENG', 6)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (4, N'Jacobs, Andy', N'SDET', N'ENG', 6)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (5, N'Potra, Cristina', N'AN', N'ENG', 6)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (6, N'Brehm, Peter', N'DEVMGR', N'ENG', 7)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (7, N'Reiter, Tsvi', N'ITDIR', N'ENG', 25)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (8, N'Poe, Toni', N'SRSL', N'SALES', 9)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (9, N'Pfeiffer, Michael', N'SMGR', N'SALES', 11)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (10, N'Harp, Walter', N'SMGR', N'SALES', 11)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (11, N'Alverca, Luis', N'SDIR', N'SALES', 25)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (12, N'Abercrombie, Kim', N'MKTAN', N'MKT', 13)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (13, N'Xie, Ming-Yang', N'MKTMGR', N'MKT', 15)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (14, N'Yalovsky, David', N'MKTMGR', N'MKT', 15)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (15, N'Allen, Michael', N'MKTDIR', N'MKT', 25)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (16, N'Gonzalez, Howard', N'REC', N'HR', 19)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (17, N'Neves, Paulo', N'REC', N'HR', 19)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (18, N'Williams, Jeff', N'REC', N'HR', 19)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (19, N'Forde, Viggo', N'RECMGR', N'HR', 24)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (20, N'Larsson, Katarina', N'HRT', N'HR', 23)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (21, N'Turner, Olinda', N'HRAN', N'HR', 23)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (22, N'Norred, Chris', N'HRAN', N'HR', 23)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (23, N'Miller, Lisa', N'HRMGR', N'HR', 24)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (24, N'Gonzalez, Nuria', N'HRDIR', N'HR', 25)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (25, N'Goldstein, Brian Richard', N'CEO', N'PRES', NULL)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (26, N'White, Cindy', N'ASSIST', N'PRES', 25)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (27, N'Miller, Ben', N'ASSIST', N'HR', 24)
INSERT [dbo].[Employees] ([Id], [Name], [Position], [Department], [ManagerId]) VALUES (28, N'Truffat, Marcelo', N'ASSIST', N'MKT', 15)
/****** Object:  Table [dbo].[Departments]    Script Date: 11/12/2009 16:17:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Departments](
	[Id] [char](8) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Owner] [int] NOT NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[Departments] ([Id], [Name], [Owner]) VALUES (N'ENG', N'Engineering', 7)
INSERT [dbo].[Departments] ([Id], [Name], [Owner]) VALUES (N'HR', N'Human Resources', 25)
INSERT [dbo].[Departments] ([Id], [Name], [Owner]) VALUES (N'MKT', N'Marketing', 15)
INSERT [dbo].[Departments] ([Id], [Name], [Owner]) VALUES (N'PRES', N'Presidency', 25)
INSERT [dbo].[Departments] ([Id], [Name], [Owner]) VALUES (N'SALES', N'Sales', 11)
/****** Object:  Table [dbo].[RequestHistory]    Script Date: 11/12/2009 16:17:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RequestHistory](
	[RequestId] [uniqueidentifier] NOT NULL,
	[RecordNumber] [int] NULL,
	[SourceState] [varchar](256) NULL,
	[Action] [varchar](256) NULL,
	[Comment] [varchar](256) NULL,
	[EmployeeId] [varchar](16) NULL,
	[EmployeeName] [varchar](512) NULL,
	[Date] [datetime] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[RemoveFromInbox]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RemoveFromInbox]
  (
    @requestId uniqueidentifier   
  )
AS
BEGIN

   DELETE FROM InboxItems
   WHERE RequestId = @requestId    

   DELETE FROM InboxItemsByEmployee
   WHERE RequestId = @requestId    
END
GO
/****** Object:  StoredProcedure [dbo].[RemoveFromEmployeeInbox]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[RemoveFromEmployeeInbox]
  (
    @requestId uniqueidentifier,
    @employeeId int
  )
AS
BEGIN

   DELETE FROM InboxItemsByEmployee
   WHERE RequestId = @requestId
     AND EmployeeId = @employeeId
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateResumeesCountInJobPosting]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateResumeesCountInJobPosting]
	-- Add the parameters for the stored procedure here
	(
		@id uniqueidentifier
	)	
AS

BEGIN
	/* SET NOCOUNT ON */			  
	 	/* UPDATE IT */ 
		UPDATE JobPostings SET			
			ResumeesReceived = ResumeesReceived + 1,
			LastResumeeDate = GetDate()
		WHERE id = @id			  
						
END
GO
/****** Object:  StoredProcedure [dbo].[SelectRequestsStartedBy]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[SelectRequestsStartedBy]
	(
		@employeeId int
	)	
AS

BEGIN
	
    SELECT * 
	 	FROM InboxItems
		WHERE StartedBy = @employeeId		  
		ORDER BY InboxEntryDate
			  
END
GO
/****** Object:  StoredProcedure [dbo].[SelectRequestsFor]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[SelectRequestsFor]
	(
		@employeeId int
	)	
AS

BEGIN	
	
    SELECT i.* 
	 	FROM InboxItems i, InboxItemsByEmployee ie
		WHERE i.RequestId = ie.RequestId
		  AND ie.EmployeeId = @employeeId
		ORDER BY i.InboxEntryDate
			 
END
GO
/****** Object:  StoredProcedure [dbo].[SelectNotStartedJobPostings]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SelectNotStartedJobPostings]
AS

BEGIN
	/* SET NOCOUNT ON */
	
    SELECT * 
	FROM JobPostings	
	WHERE Status = 'WaitingData'
	ORDER BY CreationDate
			  
END
GO
/****** Object:  StoredProcedure [dbo].[SelectJobPostingResumees]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SelectJobPostingResumees]
	(
		@id uniqueidentifier
	)	
AS

BEGIN
	/* SET NOCOUNT ON */
	
    SELECT * 
	 	FROM JobPostings, JobPostingResumees
		WHERE JobPostings.id = @id 
		AND JobPostings.Id = JobPostingResumees.JobPostingId
			  
END
GO
/****** Object:  StoredProcedure [dbo].[SelectJobPosting]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SelectJobPosting]
	(
		@id uniqueidentifier
	)	
AS

BEGIN
	/* SET NOCOUNT ON */
	
    SELECT * 
	 	FROM JobPostings
		WHERE id = @id 
			  
END
GO
/****** Object:  StoredProcedure [dbo].[SelectHiringRequestHistory]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[SelectHiringRequestHistory]
	(
		@id uniqueidentifier
	)	
AS

BEGIN
	/* SET NOCOUNT ON */
	
    SELECT * 
	 	FROM RequestHistory
		WHERE RequestId = @id 
		ORDER BY [Date]
			  
END
GO
/****** Object:  StoredProcedure [dbo].[SelectHiringRequest]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[SelectHiringRequest]
	(
		@id uniqueidentifier
	)	
AS

BEGIN
	/* SET NOCOUNT ON */
	
    SELECT * 
	 	FROM HiringRequests
		WHERE id = @id 
			  
END
GO
/****** Object:  StoredProcedure [dbo].[SelectClosedJobPostings]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SelectClosedJobPostings]
AS
	SELECT * 
	FROM JobPostings	
	WHERE Status = 'Closed'
GO
/****** Object:  StoredProcedure [dbo].[SelectArchivedRequestsStartedBy]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[SelectArchivedRequestsStartedBy]
	(
		@employeeId int
	)	
AS

BEGIN
	
	
    SELECT * 
	 	FROM InboxItemsArchive
		WHERE StartedBy = @employeeId		  
		ORDER BY InboxEntryDate
			  
END
GO
/****** Object:  StoredProcedure [dbo].[SelectActiveJobPostings]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SelectActiveJobPostings]
AS

BEGIN
	/* SET NOCOUNT ON */
	
    SELECT * 
	FROM JobPostings	
	WHERE Status = 'Receiving Resumes'
			  
END
GO
/****** Object:  StoredProcedure [dbo].[SaveRequestHistory]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[SaveRequestHistory]
	(
		@requestId uniqueidentifier,
		@sourceState varchar(256),
		@actionName varchar(256),
		@comment varchar(256),
		@employeeId varchar(16),
		@employeeName varchar(512)
	)	
AS

BEGIN
	/* SET NOCOUNT ON */
	
		INSERT INTO RequestHistory (RequestId, SourceState, [action], comment, employeeId, employeeName, [date])
		VALUES (@requestId, @sourceState, @actionName, @comment, @employeeId, @employeeName, GETDATE())
		
END
GO
/****** Object:  StoredProcedure [dbo].[SaveJobPosting]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaveJobPosting]
	(
		@id uniqueidentifier,
		@hiringRequestId uniqueidentifier,
		@title varchar(1024),
		@description text,
		@status varchar(64)
	)	
AS

BEGIN
	/* SET NOCOUNT ON */
	
	/* if the HiringRequest exists... */
	IF exists(SELECT * 
	 		  FROM JobPostings
			  WHERE id = @id) 
			  
	 	/* UPDATE IT */ 
		UPDATE JobPostings SET			
			Title = @title,
			Description = @description,			
			Status = @status,
			LastUpdate = GetDate()
		WHERE id = @id			  
				
	ELSE /* if it does not exist, insert it */ 
		
		INSERT INTO JobPostings (Id, HiringRequestId, Title, Description, CreationDate, LastUpdate, ResumeesReceived, Status)
		VALUES (@id, @hiringRequestId, @title, @description, GetDate(), GetDate(), 0, @status)
		
END
GO
/****** Object:  StoredProcedure [dbo].[SaveHiringRequest]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaveHiringRequest]
	(
		@id uniqueidentifier,
		@requesterId int,
		@creationDate datetime,
		@positionId varchar(16),
		@departmentId varchar(16),
		@description varchar(1024),
		@title varchar(1024),
		@workflowInstanceId uniqueidentifier,
		@isCompleted bit,
		@isSuccess bit,
		@isCancelled bit	
	)	
AS

BEGIN
	/* SET NOCOUNT ON */
	
	/* if the HiringRequest exists... */
	IF exists(SELECT * 
	 		  FROM HiringRequests
			  WHERE id = @id) 
			  
	 	/* UPDATE IT */ 
		UPDATE HiringRequests SET
			PositionId = @positionId,
			DepartmentId = @departmentId,
			Description = @description,
			Title = @title,
			WorkflowInstanceId = @workflowInstanceId,
			IsCompleted = @isCompleted,
			IsSuccess = @isSuccess,
			IsCancelled = @isCancelled
		WHERE id = @id			  
				
	ELSE /* if it does not exist, insert it */ 
		
		INSERT INTO HiringRequests (Id, RequesterId, CreationDate, PositionId, DepartmentId, Description, Title, WorkflowInstanceId, IsCompleted, IsSuccess, IsCancelled)
		VALUES (@id, @requesterId, @creationDate, @positionId, @departmentId, @description, @title, @workflowInstanceId, @isCompleted, @isSuccess, @isCancelled)
		
END
GO
/****** Object:  StoredProcedure [dbo].[CleanData]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CleanData]
AS
BEGIN
    DELETE FROM InboxItemsByEmployee

    DELETE FROM InboxItemsArchive

    DELETE FROM InboxItems

    DELETE FROM RequestHistory

    DELETE FROM HiringRequests
END
GO
/****** Object:  StoredProcedure [dbo].[ArchiveInboxRequest]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ArchiveInboxRequest]
  (
    @requestId uniqueidentifier,
    @state varchar(512)   
  )
AS
BEGIN
   			  
   INSERT INTO InboxItemsArchive (RequestId, StartedBy, Title, State, InboxEntryDate, ArchivalDate)
   SELECT i.RequestId, i.StartedBy, i.Title, @state, i.InboxEntryDate, GETDATE()
   FROM InboxItems i
   WHERE i.RequestId = @requestId   
   
   DELETE FROM InboxItems
   WHERE RequestId = @requestId    

   DELETE FROM InboxItemsByEmployee
   WHERE RequestId = @requestId  
      
END
GO
/****** Object:  StoredProcedure [dbo].[InsertResumee]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertResumee]
	(
		@id uniqueidentifier,
		@candidateMail varchar(512),		
		@candidateName varchar(512),
		@resumee text
	)	
AS
BEGIN
	/* SET NOCOUNT ON */
	INSERT INTO JobPostingResumees (JobPostingId, CandidateMail, CandidateName, Resumee, ReceivedDate)
	VALUES (@id, @candidateMail, @candidateName, @resumee, GetDate())
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertJobPostingResumee]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertJobPostingResumee]
	(
		@jobPostingId uniqueidentifier,
		@candidateMail varchar(512),
		@candidateName varchar(512),
		@resumee text		
	)	
AS

BEGIN	  
		
	INSERT INTO JobPostingResumees (JobPostingId, CandidateMail, CandidateName, ReceivedDate, Resumee)
	VALUES (@jobPostingId, @candidateMail, @candidateName, GETDATE(), @resumee)   
		
END
GO
/****** Object:  StoredProcedure [dbo].[InsertInEmployeeInbox]    Script Date: 11/12/2009 16:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertInEmployeeInbox]
	(
		@requestId uniqueidentifier,
		@startedBy int,
		@title varchar(1024),
		@state varchar(512),
		@employeeId int
	)	
AS

BEGIN	  
		
   	IF NOT exists(SELECT * 
        		       FROM InboxItems
    		           WHERE RequestId = @requestId
				   AND State = @state) 			  
	BEGIN				   	    
		EXEC RemoveFromInbox @requestId
		
        INSERT INTO InboxItems (RequestId, StartedBy, Title, State, InboxEntryDate)
        VALUES (@requestId, @startedBy, @title, @state, GETDATE())
	END

    INSERT INTO InboxItemsByEmployee (RequestId, EmployeeId)
    VALUES (@requestId, @employeeId)
		
END
GO
