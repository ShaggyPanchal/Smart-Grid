USE [SmartGridSQL]
GO

/****** Object: Table [dbo].[GridInfo] Script Date: 10/31/2015 15:56:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GridInfo] (
    [GridID]       INT            NOT NULL,
    [Lat1]         DECIMAL (8, 6) NOT NULL,
    [Long1]        DECIMAL (8, 6) NOT NULL,
    [PublicKey]    NVARCHAR (50)  NOT NULL,
    [Lat2]         DECIMAL (8, 6) NOT NULL,
    [Long2]        DECIMAL (8, 6) NOT NULL,
    [Status]       BIT            NOT NULL,
    [LastPingTime] DATETIME       NULL
);

GO

/****** Object: Table [dbo].[GridLocationMapping] Script Date: 10/31/2015 15:57:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GridLocationMapping] (
    [MappingID]   BIGINT NOT NULL,
    [LocationID1] INT    NOT NULL,
    [LocationID2] INT    NOT NULL,
    [GridID]      INT    NOT NULL
);

GO

/****** Object: Table [dbo].[LocationInfo] Script Date: 10/31/2015 15:57:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LocationInfo] (
    [LocationID] INT            NOT NULL,
    [Lat]        DECIMAL (8, 6) NOT NULL,
    [Long]       DECIMAL (8, 6) NOT NULL
);


