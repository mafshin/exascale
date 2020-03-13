/**** Object:  Table [dbo].[Customers]    Script Date: 3/13/2020 8:28:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerId] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Birthdate] [date] NOT NULL,
	[Address] [nvarchar](100) NULL,
	[Job] [nvarchar](100) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 3/13/2020 8:28:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Count] [int] NOT NULL,
	[Price] [int] NOT NULL,
	[Description] [nvarchar](1000) NOT NULL,
	[OrderEntryDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Customers]
GO
/****** Object:  StoredProcedure [dbo].[AddCustomer]    Script Date: 3/13/2020 8:28:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[AddCustomer]
@customerId int,
@title nvarchar(100),
@birthdate date,
@address nvarchar(100),
@job nvarchar(100)
AS
BEGIN
insert into Customers
(CustomerId, Title, Birthdate, Address, Job)
VALUES
(@customerId, @title, @birthdate, @address, @job)
END
GO
/****** Object:  StoredProcedure [dbo].[AddOrder]    Script Date: 3/13/2020 8:28:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  create proc [dbo].[AddOrder]
  @customerId int,
  @count int,
  @price decimal(18, 0),
  @description nvarchar(1000),
  @orderEntryDate datetime
  AS
  BEGIn

  insert into Orders
  VALUES
  (@customerId, @count, @price, @description, @orderEntryDate)
  END
GO
/****** Object:  StoredProcedure [dbo].[ClearAll]    Script Date: 3/13/2020 8:28:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[ClearAll]
AS
DELETE FROM Orders
DELETE FROM Customers
GO
/****** Object:  StoredProcedure [dbo].[GetShardItemsCount]    Script Date: 3/13/2020 8:28:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[GetShardItemsCount]
AS
BEGIn
Select count(1) as Count from Customers
END
GO
