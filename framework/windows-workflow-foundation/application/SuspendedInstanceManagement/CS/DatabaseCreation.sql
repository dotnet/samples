-- Copyright (c) Microsoft Corporation.  All rights reserved.
USE [master]
GO
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'DefaultSampleStore')
DROP DATABASE [DefaultSampleStore]
GO
CREATE DATABASE [DefaultSampleStore]
GO
