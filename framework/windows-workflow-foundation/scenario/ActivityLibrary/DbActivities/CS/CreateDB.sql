CREATE DATABASE DbActivitiesSample
GO

USE DbActivitiesSample
GO

CREATE TABLE Roles
(
   Code     varchar(4),
   Name     varchar(256)
)
GO

INSERT INTO Roles (code, name) VALUES ('PM', 'Program Manager')
GO

INSERT INTO Roles (code, name) VALUES ('SDE', 'Software Development Engineer')
GO

INSERT INTO Roles (code, name) VALUES ('SDET', 'Software Development Engineer in Test')
GO
