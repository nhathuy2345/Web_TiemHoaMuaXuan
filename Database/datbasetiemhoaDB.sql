-- 1. TẠO DATABASE NẾU CHƯA CÓ
USE [master]
GO
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TiemHoaDB')
BEGIN
    CREATE DATABASE [TiemHoaDB]
END
GO

USE [TiemHoaDB]
GO

-- 2. TẠO CẤU TRÚC BẢNG
/****** Object:  Table [dbo].[__MigrationHistory] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC, [ContextKey] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[AppUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[FullName] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[Address] [nvarchar](255) NULL,
	[IsAdmin] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- ⭐ BẢNG NGÂN HÀNG ĐÃ CÓ CỘT SePayApiKey
CREATE TABLE [dbo].[BankSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BankName] [nvarchar](100) NULL,
	[AccountNumber] [nvarchar](50) NULL,
	[AccountHolder] [nvarchar](200) NULL,
	[QRImageUrl] [nvarchar](500) NULL,
	[SePayApiKey] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.BankSettings] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ChatMessages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserEmail] [nvarchar](max) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[SentAt] [datetime] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ChatMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContactMessages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](200) NULL,
	[Phone] [nvarchar](20) NULL,
	[Message] [nvarchar](2000) NOT NULL,
	[SentAt] [datetime] NOT NULL,
	[IsRead] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ContactMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[OrderItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[ProductName] [nvarchar](200) NULL,
	[Image] [nvarchar](500) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_dbo.OrderItems] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderCode] [nvarchar](50) NULL,
	[UserEmail] [nvarchar](200) NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Address] [nvarchar](500) NOT NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NULL,
	[OrderDate] [datetime] NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_dbo.Orders] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[OldPrice] [decimal](18, 2) NULL,
	[DiscountPercent] [int] NOT NULL,
	[ImageUrl] [nvarchar](500) NULL,
	[Stock] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[IsFeatured] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Products] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Reviews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[UserEmail] [nvarchar](255) NULL,
	[UserName] [nvarchar](255) NULL,
	[Rating] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 3. CHÈN DỮ LIỆU MẪU (DATA)
-- Lưu ý: Tài khoản Admin mật khẩu là: admin123 (Bạn nhớ ghi vào README cho cô nhé)

SET IDENTITY_INSERT [dbo].[AppUsers] ON 
INSERT [dbo].[AppUsers] ([Id], [Email], [Password], [FullName], [Phone], [Address], [IsAdmin], [IsActive]) VALUES (1, N'admin@gmail.com', N'admin123', N'Quản Trị Viên', N'0901234567', N'TP. Hồ Chí Minh', 1, 1)
SET IDENTITY_INSERT [dbo].[AppUsers] OFF
GO

SET IDENTITY_INSERT [dbo].[BankSettings] ON 
INSERT [dbo].[BankSettings] ([Id], [BankName], [AccountNumber], [AccountHolder], [QRImageUrl], [SePayApiKey]) VALUES (1, N'Vietcombank', N'0123456789', N'TIEM HOA MUA XUAN', N'', N'Apikey huy_sepay_webhook_2024!@#')
SET IDENTITY_INSERT [dbo].[BankSettings] OFF
GO

SET IDENTITY_INSERT [dbo].[Categories] ON 
INSERT [dbo].[Categories] ([Id], [Name], [Description], [IsActive]) VALUES (1, N'Hoa Tình Yêu', N'Hoa cho người yêu', 1)
INSERT [dbo].[Categories] ([Id], [Name], [Description], [IsActive]) VALUES (2, N'Hoa Sinh Nhật', N'Hoa chúc mừng sinh nhật', 1)
INSERT [dbo].[Categories] ([Id], [Name], [Description], [IsActive]) VALUES (3, N'Hoa Khai Trương', N'Hoa chúc mừng khai trương', 1)
INSERT [dbo].[Categories] ([Id], [Name], [Description], [IsActive]) VALUES (4, N'Hoa Cưới', N'Hoa cầm tay cô dâu', 1)
INSERT [dbo].[Categories] ([Id], [Name], [Description], [IsActive]) VALUES (5, N'Lan Hồ Điệp', N'Lan hồ điệp cao cấp', 1)
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO

SET IDENTITY_INSERT [dbo].[Products] ON 
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (1, N'Bó Hoa Hướng Dương', N'Hoa hướng dương tươi 20 cành', CAST(450000.00 AS Decimal(18, 2)), CAST(500000.00 AS Decimal(18, 2)), 10, N'https://shophoatuoanh.com/wp-content/uploads/2024/11/y_nghia_cua_hoa_huong_duong_trong_ngay__1-600x590.webp', 50, 2, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 1)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (2, N'Lẵng Hoa Hồng Đỏ', N'99 bông hồng đỏ nhập khẩu', CAST(650000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://tse2.mm.bing.net/th/id/OIP.29qSOz-hUghAyl8f1vsHbwHaJ4?r=0&w=720&h=960&rs=1&pid=ImgDetMain&o=7&rm=3', 29, 1, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 1)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (3, N'Chậu Lan Hồ Điệp Trắng', N'Lan hồ điệp trắng 5 cành', CAST(1200000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://dallasflower.com.vn/wp-content/uploads/2025/06/z6670731024941_61d7823bbe84242aa753e781077508db.jpg', 20, 5, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 1)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (4, N'Hoa Cưới Cầm Tay', N'Hoa cầm tay cô dâu', CAST(850000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://tailocwedding.vn/wp-content/uploads/2023/05/Hoa-cuoi-cam-tay-bang-hoa-hong.jpg', 15, 4, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 1)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (5, N'Kệ Hoa Khai Trương', N'Kệ hoa 2 tầng', CAST(2500000.00 AS Decimal(18, 2)), CAST(2800000.00 AS Decimal(18, 2)), 11, N'https://i.pinimg.com/736x/8f/b1/34/8fb13491038a6fe96f9638fa76b757d5.jpg', 10, 3, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 1)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (6, N'Bó Hồng Pastel', N'Hồng pastel nhẹ nhàng', CAST(520000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://th.bing.com/th/id/R.ed4ee0b395e6b915e3e5f2536563847a?rik=C0wswuwDWyytUA&pid=ImgRaw&r=0', 40, 1, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (7, N'Giỏ Hoa Baby', N'Hoa baby trắng', CAST(380000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://4tfloral.vn/wp-content/uploads/2025/10/gio-hoa-baby-hong-mini.jpg', 60, 2, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 1)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (8, N'Hộp Hoa Hồng Vàng', N'Hộp quà hoa hồng vàng', CAST(720000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://tse4.mm.bing.net/th/id/OIP.Ni8JddyVWhO6RRuPy_P-eQHaJ4?r=0&rs=1&pid=ImgDetMain&o=7&rm=3', 25, 1, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 1)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (9, N'Lan Hồ Điệp Tím', N'Lan hồ điệp tím 7 cành', CAST(1500000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://www.flowercorner.vn/image/catalog/products/August%202023/chau-lan-ho-diep-tim-2-canh.jpg', 20, 5, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (10, N'Hoa Cưới Tone Xanh', N'Hoa cầm tay tone xanh mint', CAST(950000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://product.hstatic.net/1000086616/product/z2515775216738_2bb920736291f39102bda343d3a8c0f3_3dd4944d7e0043b4b387742e7fbd3c4e_master.jpg', 12, 4, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (11, N'Bó Tulip Hà Lan', N'Tulip Hà Lan tươi 15 cành', CAST(680000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://hoathangtu.com/wp-content/uploads/2024/10/IMG_2271.jpg', 35, 2, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [OldPrice], [DiscountPercent], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsFeatured]) VALUES (12, N'Chậu Hoa Cẩm Tú Cầu', N'Cẩm tú cầu xanh pastel', CAST(890000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, N'https://storage.googleapis.com/cdn_dlhf_vn/public/products/HFDC/HFDCP14017/1713258799_661e412ff2889.jpg', 18, 3, CAST(N'2026-10-13T09:47:01.860' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Products] OFF
GO

-- CHÈN ĐƠN HÀNG MẪU
SET IDENTITY_INSERT [dbo].[Orders] ON 
INSERT [dbo].[Orders] ([Id], [OrderCode], [UserEmail], [FullName], [Phone], [Address], [PaymentMethod], [Status], [OrderDate], [TotalAmount]) VALUES (1, N'DH1001', N'www.khotienich247@gmail.com', N'khotienich247', N'0123456789', N'thành phố hồ chí minh', N'COD', N'Đã xác nhận', CAST(N'2026-10-13T09:58:18.413' AS DateTime), CAST(450000.00 AS Decimal(18, 2)))
INSERT [dbo].[Orders] ([Id], [OrderCode], [UserEmail], [FullName], [Phone], [Address], [PaymentMethod], [Status], [OrderDate], [TotalAmount]) VALUES (2, N'DH1002', N'admin@gmail.com', N'Quản Trị Viên', N'0901234567', N'TP. Hồ Chí Minh', N'COD', N'Chờ xử lý', CAST(N'2026-10-31T10:36:54.027' AS DateTime), CAST(600000.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO

SET IDENTITY_INSERT [dbo].[OrderItems] ON 
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [ProductName], [Image], [Price], [Quantity]) VALUES (1, 1, 1, N'Bó Hoa Hướng Dương', N'/Content/images/1.jpg', CAST(450000.00 AS Decimal(18, 2)), 1)
INSERT [dbo].[OrderItems] ([Id], [OrderId], [ProductId], [ProductName], [Image], [Price], [Quantity]) VALUES (2, 2, 2, N'Lẵng Hoa Hồng Đỏ', N'https://tse2.mm.bing.net/th/id/OIP.29qSOz-hUghAyl8f1vsHbwHaJ4?r=0&w=720&h=960&rs=1&pid=ImgDetMain&o=7&rm=3', CAST(650000.00 AS Decimal(18, 2)), 1)
SET IDENTITY_INSERT [dbo].[OrderItems] OFF
GO

-- 4. TẠO CÁC RÀNG BUỘC (KHÓA NGOẠI)
ALTER TABLE [dbo].[AppUsers] ADD  DEFAULT ((0)) FOR [IsAdmin]
GO
ALTER TABLE [dbo].[AppUsers] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Reviews] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderItems_dbo.Orders_OrderId] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_dbo.OrderItems_dbo.Orders_OrderId]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Products_dbo.Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_dbo.Products_dbo.Categories_CategoryId]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Reviews_dbo.Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK_dbo.Reviews_dbo.Products_ProductId]
GO