USE [MagicVillaDb]
GO
/****** Object:  Table [dbo].[RefreshTokens]    Script Date: 20-04-2025 10:40:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshTokens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](max) NOT NULL,
	[JwtTokenId] [nvarchar](max) NOT NULL,
	[Refresh_Token] [nvarchar](max) NOT NULL,
	[IsValid] [bit] NOT NULL,
	[ExpiresAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[RefreshTokens] ON 

INSERT [dbo].[RefreshTokens] ([Id], [UserId], [JwtTokenId], [Refresh_Token], [IsValid], [ExpiresAt]) VALUES (1007, N'bf6a8f80-79ef-4fc5-90a0-52d56797d3a1', N'JTIc47a723d-a8e8-4b91-b1d2-03242e9b7a93', N'e629706b-f50d-4dae-a882-39a6df22445a-c0050bbe-b534-4f41-8f22-a976ab41d26f', 0, CAST(N'2024-08-10T05:37:13.0197801' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([Id], [UserId], [JwtTokenId], [Refresh_Token], [IsValid], [ExpiresAt]) VALUES (2007, N'bf6a8f80-79ef-4fc5-90a0-52d56797d3a1', N'JTI8d31d2af-6272-45d7-99de-42c66a6ce0fe', N'3134ad5c-c01d-4dc3-b0c2-6c4246e6ca6d-6434e347-3fd8-446f-84a2-9e48ce8a6387', 0, CAST(N'2024-08-11T04:57:46.9367957' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([Id], [UserId], [JwtTokenId], [Refresh_Token], [IsValid], [ExpiresAt]) VALUES (2008, N'bf6a8f80-79ef-4fc5-90a0-52d56797d3a1', N'JTIeffe7e5e-b47b-4b7b-8c46-582575b20831', N'3ebd0d6c-8419-4bf0-b093-bbbdab7ffb65-2fee3747-b47e-4d6e-8481-f133cc3174b7', 0, CAST(N'2024-08-25T12:03:12.7234182' AS DateTime2))
SET IDENTITY_INSERT [dbo].[RefreshTokens] OFF
GO
