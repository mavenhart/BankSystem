USE master

CREATE DATABASE [BankSystem]
GO

ALTER DATABASE [BankSystem] SET ALLOW_SNAPSHOT_ISOLATION ON
GO

USE [BankSystem]
GO

/****** Object:  Table [dbo].[Account]    Script Date: 8/9/2018 3:32:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Account](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoginName] [varchar](50) NOT NULL,
	[AccountNumber] [varchar](50) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[Balance] [money] NOT NULL,
	[CreatedDate] [datetime] NOT NULL
) ON [PRIMARY]

GO


