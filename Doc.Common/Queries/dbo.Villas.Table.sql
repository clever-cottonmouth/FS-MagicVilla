USE [MagicVillaDb]
GO
/****** Object:  Table [dbo].[Villas]    Script Date: 20-04-2025 10:40:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Villas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Details] [nvarchar](max) NOT NULL,
	[Rate] [int] NOT NULL,
	[Sqft] [int] NOT NULL,
	[Occupancy] [int] NOT NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[Amenity] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
	[ImageLocalPath] [nvarchar](max) NULL,
 CONSTRAINT [PK_Villas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Villas] ON 

INSERT [dbo].[Villas] ([Id], [Name], [Details], [Rate], [Sqft], [Occupancy], [ImageUrl], [Amenity], [CreatedDate], [UpdatedDate], [ImageLocalPath]) VALUES (1, N'Royal Villa', N'Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.', 200, 550, 4, N'https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg', N'', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL)
INSERT [dbo].[Villas] ([Id], [Name], [Details], [Rate], [Sqft], [Occupancy], [ImageUrl], [Amenity], [CreatedDate], [UpdatedDate], [ImageLocalPath]) VALUES (2, N'Premium Pool Villa', N'Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.', 300, 550, 4, N'https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg', N'', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL)
INSERT [dbo].[Villas] ([Id], [Name], [Details], [Rate], [Sqft], [Occupancy], [ImageUrl], [Amenity], [CreatedDate], [UpdatedDate], [ImageLocalPath]) VALUES (3, N'Luxury Pool Villa', N'Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.', 400, 750, 4, N'https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg', N'', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL)
INSERT [dbo].[Villas] ([Id], [Name], [Details], [Rate], [Sqft], [Occupancy], [ImageUrl], [Amenity], [CreatedDate], [UpdatedDate], [ImageLocalPath]) VALUES (4, N'Diamond Villa', N'Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.', 550, 900, 4, N'https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg', N'', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL)
SET IDENTITY_INSERT [dbo].[Villas] OFF
GO
