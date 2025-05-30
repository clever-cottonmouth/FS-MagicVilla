USE [MagicVillaDb]
GO
/****** Object:  Table [dbo].[VillaNumbers]    Script Date: 20-04-2025 10:40:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VillaNumbers](
	[VillaNo] [int] NOT NULL,
	[SpecialDetails] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
	[VillaID] [int] NOT NULL,
 CONSTRAINT [PK_VillaNumbers] PRIMARY KEY CLUSTERED 
(
	[VillaNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[VillaNumbers] ([VillaNo], [SpecialDetails], [CreatedDate], [UpdatedDate], [VillaID]) VALUES (29, N'Vel inventore maxime', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[VillaNumbers] ([VillaNo], [SpecialDetails], [CreatedDate], [UpdatedDate], [VillaID]) VALUES (210, N'Est omnis nostrud cu', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 2)
INSERT [dbo].[VillaNumbers] ([VillaNo], [SpecialDetails], [CreatedDate], [UpdatedDate], [VillaID]) VALUES (882, N'Quam voluptatibus ex', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[VillaNumbers] ([VillaNo], [SpecialDetails], [CreatedDate], [UpdatedDate], [VillaID]) VALUES (945, N'Id ullam necessitati', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[VillaNumbers] ([VillaNo], [SpecialDetails], [CreatedDate], [UpdatedDate], [VillaID]) VALUES (961, N'Maiores consequatur ', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[VillaNumbers] ([VillaNo], [SpecialDetails], [CreatedDate], [UpdatedDate], [VillaID]) VALUES (966, N'Rxcs', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2024-07-08T10:35:07.0346948' AS DateTime2), 1)
GO
/****** Object:  Index [IX_VillaNumbers_VillaID]    Script Date: 20-04-2025 10:40:06 PM ******/
CREATE NONCLUSTERED INDEX [IX_VillaNumbers_VillaID] ON [dbo].[VillaNumbers]
(
	[VillaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[VillaNumbers] ADD  DEFAULT ((0)) FOR [VillaID]
GO
ALTER TABLE [dbo].[VillaNumbers]  WITH CHECK ADD  CONSTRAINT [FK_VillaNumbers_Villas_VillaID] FOREIGN KEY([VillaID])
REFERENCES [dbo].[Villas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[VillaNumbers] CHECK CONSTRAINT [FK_VillaNumbers_Villas_VillaID]
GO
