CREATE DATABASE DBDistCache;

GO

USE [DBDistCache]
GO

/****** Object:  Table [dbo].[TiGammaCaches]    Script Date: 2/1/2019 4:34:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DbCaches](
	[Id] [nvarchar](256) NOT NULL PRIMARY KEY,
	[Value] [varbinary](max) NOT NULL,
	[ExpiresAtTime] [datetimeoffset](7) NULL,
	[SlidingExpirationInSeconds] [bigint] NULL,
	[AbsoluteExpiration] [datetimeoffset](7) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO




