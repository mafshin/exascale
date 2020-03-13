/****** Object:  Table [dbo].[ShardKeyMap]    Script Date: 3/13/2020 8:25:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShardKeyMap](
	[ShardKey] [nvarchar](100) NULL,
	[ShardId] [int] NULL,
 CONSTRAINT [IX_ShardKeyMap] UNIQUE NONCLUSTERED 
(
	[ShardId] ASC,
	[ShardKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Shards]    Script Date: 3/13/2020 8:25:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shards](
	[ShardId] [int] NOT NULL,
 CONSTRAINT [PK_Shards] PRIMARY KEY CLUSTERED 
(
	[ShardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShardKeyMap]  WITH CHECK ADD  CONSTRAINT [FK_ShardKeyMap_Shards] FOREIGN KEY([ShardId])
REFERENCES [dbo].[Shards] ([ShardId])
GO
ALTER TABLE [dbo].[ShardKeyMap] CHECK CONSTRAINT [FK_ShardKeyMap_Shards]
GO
/****** Object:  StoredProcedure [dbo].[AddShard]    Script Date: 3/13/2020 8:25:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[AddShard]
@shardId int
AS
BEGIN
insert into Shards
(ShardId)
VALUES
(@shardId)
END
GO
/****** Object:  StoredProcedure [dbo].[AddShardKey]    Script Date: 3/13/2020 8:25:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[AddShardKey]
@shardId int,
@shardKey nvarchar(100)
AS
BEGIN
INSERT INTO dbo.ShardKeyMap
(ShardKey, ShardId)
VALUES
(@shardKey, @shardId)
END
GO
/****** Object:  StoredProcedure [dbo].[ClearAll]    Script Date: 3/13/2020 8:25:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[ClearAll]
AS
Truncate Table ShardKeyMap
DELETE FROM Shards
GO
/****** Object:  StoredProcedure [dbo].[GetShardMap]    Script Date: 3/13/2020 8:25:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetShardMap]
as
begin
select * from ShardKeyMap
SELECT * FROM shards
END
GO
/****** Object:  StoredProcedure [dbo].[GetShards]    Script Date: 3/13/2020 8:25:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetShards]
AS
SELECT * FROM dbo.Shards
GO
