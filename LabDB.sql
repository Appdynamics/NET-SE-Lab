USE [master]
GO

IF NOT EXISTS 
    (SELECT name  
     FROM master.sys.server_principals
     WHERE name = 'NT AUTHORITY\SYSTEM')
BEGIN
    CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
END


/****** Object:  Database [LabTrainingDatabase1]    Script Date: 5/21/2014 4:27:30 PM ******/
CREATE DATABASE [LabTrainingDatabase]
GO

ALTER DATABASE [LabTrainingDatabase] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [LabTrainingDatabase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [LabTrainingDatabase] SET RECOVERY SIMPLE 
GO



USE [LabTrainingDatabase]
GO


CREATE TABLE [dbo].[OperationLog](
	[Date] [datetime] NOT NULL,
	[Query] [nvarchar](255) NULL
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[SearchData](
	[Operation] [nchar](255) NOT NULL,
	[Fast] [int] NULL,
	[Slow] [int] NULL,
	[Error] [int] NULL,
	[Number] [int] NULL,
	[Total] [int] NULL,
	[Date] [datetime] NULL,
	[Data] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO



IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'system')
BEGIN
    CREATE USER [system] FOR LOGIN [NT AUTHORITY\SYSTEM] WITH DEFAULT_SCHEMA=[db_owner]
END

GO


EXEC sp_addrolemember 'db_owner', 'system'
GO

