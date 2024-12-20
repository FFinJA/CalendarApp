/****** Object:  Database [calendadb]    Script Date: 2024/11/27 12:24:10 ******/
CREATE DATABASE [calendadb]  (EDITION = 'GeneralPurpose', SERVICE_OBJECTIVE = 'GP_S_Gen5_1', MAXSIZE = 32 GB) WITH CATALOG_COLLATION = SQL_Latin1_General_CP1_CI_AS, LEDGER = OFF;
GO
ALTER DATABASE [calendadb] SET COMPATIBILITY_LEVEL = 160
GO
ALTER DATABASE [calendadb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [calendadb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [calendadb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [calendadb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [calendadb] SET ARITHABORT OFF 
GO
ALTER DATABASE [calendadb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [calendadb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [calendadb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [calendadb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [calendadb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [calendadb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [calendadb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [calendadb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [calendadb] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [calendadb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [calendadb] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [calendadb] SET  MULTI_USER 
GO
ALTER DATABASE [calendadb] SET ENCRYPTION ON
GO
ALTER DATABASE [calendadb] SET QUERY_STORE = ON
GO
ALTER DATABASE [calendadb] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 100, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/*** Azure 中数据库作用域配置的脚本应在目标数据库连接内执行。 ***/
GO
-- ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 8;
GO
/****** Object:  Table [dbo].[category]    Script Date: 2024/11/27 12:24:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[color_code] [nvarchar](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[events]    Script Date: 2024/11/27 12:24:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[events](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[recurrence_id] [int] NULL,
	[category_id] [int] NULL,
	[title] [nvarchar](255) NOT NULL,
	[description] [nvarchar](max) NULL,
	[start_time] [datetime] NOT NULL,
	[end_time] [datetime] NOT NULL,
	[notify_time_before] [int] NULL,
	[notify_status] [nvarchar](10) NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[recurrence]    Script Date: 2024/11/27 12:24:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[recurrence](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[recurrence_pattern] [nvarchar](20) NOT NULL,
	[recurrence_interval] [int] NOT NULL,
	[end_after] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 2024/11/27 12:24:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[password] [nvarchar](255) NOT NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[username] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[events] ADD  DEFAULT ('PENDING') FOR [notify_status]
GO
ALTER TABLE [dbo].[events] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[recurrence] ADD  DEFAULT ((1)) FOR [recurrence_interval]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[events]  WITH CHECK ADD  CONSTRAINT [fk_events_category] FOREIGN KEY([category_id])
REFERENCES [dbo].[category] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[events] CHECK CONSTRAINT [fk_events_category]
GO
ALTER TABLE [dbo].[events]  WITH CHECK ADD  CONSTRAINT [fk_events_recurrence] FOREIGN KEY([recurrence_id])
REFERENCES [dbo].[recurrence] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[events] CHECK CONSTRAINT [fk_events_recurrence]
GO
ALTER TABLE [dbo].[events]  WITH CHECK ADD  CONSTRAINT [fk_events_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[events] CHECK CONSTRAINT [fk_events_users]
GO
ALTER TABLE [dbo].[events]  WITH CHECK ADD  CONSTRAINT [chk_events_time] CHECK  (([start_time]<[end_time]))
GO
ALTER TABLE [dbo].[events] CHECK CONSTRAINT [chk_events_time]
GO
ALTER TABLE [dbo].[events]  WITH CHECK ADD  CONSTRAINT [chk_notify_status] CHECK  (([notify_status]='FAILED' OR [notify_status]='DONE' OR [notify_status]='PENDING'))
GO
ALTER TABLE [dbo].[events] CHECK CONSTRAINT [chk_notify_status]
GO
ALTER DATABASE [calendadb] SET  READ_WRITE 
GO
