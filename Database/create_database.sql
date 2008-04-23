USE [SutekiShop]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 04/23/2008 09:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 04/23/2008 09:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ParentId] [int] NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [category_name_unique] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Image]    Script Date: 04/23/2008 09:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Image](
	[ImageId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[ImageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 04/23/2008 09:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[RoleId] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [user_email_unique] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 04/23/2008 09:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [text] NOT NULL,
	[Price] [money] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [product_name_unique] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductImage]    Script Date: 04/23/2008 09:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductImage](
	[ProductImageId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[ImageId] [int] NOT NULL,
 CONSTRAINT [PK_ProductImage] PRIMARY KEY CLUSTERED 
(
	[ProductImageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [productImage_productId_imageId_unique] UNIQUE NONCLUSTERED 
(
	[ProductId] ASC,
	[ImageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 04/23/2008 09:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[OrderDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItem]    Script Date: 04/23/2008 09:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItem](
	[OrderItemId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[SizeId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Size]    Script Date: 04/23/2008 09:48:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Size](
	[SizeId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsInStock] [bit] NOT NULL,
 CONSTRAINT [PK_Size] PRIMARY KEY CLUSTERED 
(
	[SizeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_Category_Category]    Script Date: 04/23/2008 09:48:36 ******/
ALTER TABLE [dbo].[Category]  WITH CHECK ADD  CONSTRAINT [FK_Category_Category] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_Category]
GO
/****** Object:  ForeignKey [FK_Order_User]    Script Date: 04/23/2008 09:48:36 ******/
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_User]
GO
/****** Object:  ForeignKey [FK_OrderItem_Order]    Script Date: 04/23/2008 09:48:36 ******/
ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([OrderId])
GO
ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_Order]
GO
/****** Object:  ForeignKey [FK_OrderItem_Size]    Script Date: 04/23/2008 09:48:36 ******/
ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Size] FOREIGN KEY([SizeId])
REFERENCES [dbo].[Size] ([SizeId])
GO
ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_Size]
GO
/****** Object:  ForeignKey [FK_Product_Category]    Script Date: 04/23/2008 09:48:36 ******/
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Category]
GO
/****** Object:  ForeignKey [FK_ProductImage_Image]    Script Date: 04/23/2008 09:48:36 ******/
ALTER TABLE [dbo].[ProductImage]  WITH CHECK ADD  CONSTRAINT [FK_ProductImage_Image] FOREIGN KEY([ImageId])
REFERENCES [dbo].[Image] ([ImageId])
GO
ALTER TABLE [dbo].[ProductImage] CHECK CONSTRAINT [FK_ProductImage_Image]
GO
/****** Object:  ForeignKey [FK_ProductImage_Product]    Script Date: 04/23/2008 09:48:36 ******/
ALTER TABLE [dbo].[ProductImage]  WITH CHECK ADD  CONSTRAINT [FK_ProductImage_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[ProductImage] CHECK CONSTRAINT [FK_ProductImage_Product]
GO
/****** Object:  ForeignKey [FK_Size_Product]    Script Date: 04/23/2008 09:48:36 ******/
ALTER TABLE [dbo].[Size]  WITH CHECK ADD  CONSTRAINT [FK_Size_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[Size] CHECK CONSTRAINT [FK_Size_Product]
GO
/****** Object:  ForeignKey [FK_User_Role]    Script Date: 04/23/2008 09:48:36 ******/
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Role]
GO
