USE [master]
GO
/****** Object:  Database [DIMContainerDB_Revised_Dev]    Script Date: 07-01-2019 10:54:12 AM ******/
CREATE DATABASE [DIMContainerDB_Revised_Dev] ON  PRIMARY 
( NAME = N'DIMContainerDB_Revised_Dev', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\DIMContainerDB_Revised_Dev.mdf' , SIZE = 403712KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'DIMContainerDB_Revised_Dev_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\DIMContainerDB_Revised_Dev_log.LDF' , SIZE = 4286528KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DIMContainerDB_Revised_Dev].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET ARITHABORT OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET  MULTI_USER 
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET DB_CHAINING OFF 
GO
USE [DIMContainerDB_Revised_Dev]
GO
/****** Object:  ApplicationRole [cfsagsapprole]    Script Date: 07-01-2019 10:54:13 AM ******/
/* To avoid disclosure of passwords, the password is generated in script. */
declare @idx as int
declare @randomPwd as nvarchar(64)
declare @rnd as float
select @idx = 0
select @randomPwd = N''
select @rnd = rand((@@CPU_BUSY % 100) + ((@@IDLE % 100) * 100) + 
       (DATEPART(ss, GETDATE()) * 10000) + ((cast(DATEPART(ms, GETDATE()) as int) % 100) * 1000000))
while @idx < 64
begin
   select @randomPwd = @randomPwd + char((cast((@rnd * 83) as int) + 43))
   select @idx = @idx + 1
select @rnd = rand()
end
declare @statement nvarchar(4000)
select @statement = N'CREATE APPLICATION ROLE [cfsagsapprole] WITH DEFAULT_SCHEMA = [dbo], ' + N'PASSWORD = N' + QUOTENAME(@randomPwd,'''')
EXEC dbo.sp_executesql @statement
GO
/****** Object:  Schema [DIMContainerDB_Revised_DevModelStoreContainer]    Script Date: 07-01-2019 10:54:13 AM ******/
CREATE SCHEMA [DIMContainerDB_Revised_DevModelStoreContainer]
GO
/****** Object:  UserDefinedTableType [dbo].[ContainerCodesToSearch]    Script Date: 07-01-2019 10:54:13 AM ******/
CREATE TYPE [dbo].[ContainerCodesToSearch] AS TABLE(
	[ContainerCode] [nvarchar](30) NOT NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[ScreenIds]    Script Date: 07-01-2019 10:54:13 AM ******/
CREATE TYPE [dbo].[ScreenIds] AS TABLE(
	[ScreenID] [int] NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[ScreenID] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  UserDefinedTableType [dbo].[TempDmgDetailsTbl]    Script Date: 07-01-2019 10:54:13 AM ******/
CREATE TYPE [dbo].[TempDmgDetailsTbl] AS TABLE(
	[DmgDtlsID] [bigint] NOT NULL,
	[DamageRemark] [nvarchar](200) NOT NULL,
	[DamageTypes] [nvarchar](200) NOT NULL,
	[CamPosID] [int] NULL,
	[IsCommonRemark] [bit] NULL DEFAULT ((0))
)
GO
/****** Object:  Table [dbo].[ApplicationConfig]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationConfig](
	[ShiftStartTime] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationMaster](
	[ID] [int] NOT NULL,
	[ApplicationName] [nvarchar](100) NOT NULL,
	[ApplicationDesc] [nvarchar](100) NULL,
	[AuthToken] [nvarchar](max) NOT NULL,
	[ApplicationTypeID] [int] NOT NULL,
 CONSTRAINT [PK_ApplicationMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationProjectMappingMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationProjectMappingMaster](
	[ID] [int] NOT NULL,
	[ApplicationID] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
 CONSTRAINT [PK_ApplicationProjectMappingMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationTypeDevieTypeMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationTypeDevieTypeMaster](
	[ID] [int] NOT NULL,
	[ApplicationTypeID] [int] NOT NULL,
	[DeviceTypeID] [int] NOT NULL,
 CONSTRAINT [PK_ApplicationTypeDevieTypeMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationTypeMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationTypeMaster](
	[ID] [int] NOT NULL,
	[ApplicationTypeName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ApplicationTypeMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CameraDtlsTbl]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CameraDtlsTbl](
	[CameraID] [int] IDENTITY(1,1) NOT NULL,
	[CameraIP] [varchar](25) NOT NULL,
	[LaneID] [int] NOT NULL,
	[PositionID] [int] NOT NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CameraID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CameraPositionMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CameraPositionMaster](
	[PositionID] [int] IDENTITY(1,1) NOT NULL,
	[PositionName] [nvarchar](200) NOT NULL,
	[PositionDescription] [nvarchar](200) NULL,
	[ContainerVisible] [int] NOT NULL,
	[ImageIndex] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CompanyMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyMaster](
	[ID] [int] NOT NULL,
	[LegalTitle] [nvarchar](100) NOT NULL,
	[CompanyTypeID] [int] NOT NULL,
	[HeadOfficeAddress] [nvarchar](max) NULL,
 CONSTRAINT [PK_CompanyMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CompanyTypeMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyTypeMaster](
	[ID] [int] NOT NULL,
	[CompanyType] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_CompanyTypeMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ContainerTransactions]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContainerTransactions](
	[TransID] [int] IDENTITY(1,1) NOT NULL,
	[ShippingLineID] [int] NULL,
	[TransactionTime] [datetime] NULL,
	[LaneID] [int] NULL,
	[UserId] [int] NULL,
	[DmgDtlsID] [int] NULL,
	[ContainerDmgd] [bit] NULL,
	[ContainerTypeID] [int] NULL,
	[ContainerCode] [nvarchar](50) NULL,
	[IsoCode] [nvarchar](10) NOT NULL,
	[VehicleNo] [nvarchar](50) NULL,
	[DriverName] [nvarchar](50) NULL,
	[BATNo] [nvarchar](50) NULL,
	[Displayed] [bit] NOT NULL,
	[DIRLocation] [nvarchar](500) NULL,
	[TrailerTransID] [int] NOT NULL,
	[SequnceOfContan] [int] NULL,
	[EmailImg] [image] NULL,
	[ContainerType] [varchar](50) NULL,
	[IsRotateImages] [bit] NULL,
	[CancelStatus] [bit] NULL,
	[IsManualTransaction] [bit] NULL,
	[IsReportCreated] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[TransID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ContainerTypeMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContainerTypeMaster](
	[ContainerTypeID] [int] NOT NULL,
	[ContainerTypeName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ContainerTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DamageTransaction]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DamageTransaction](
	[DmgDtlsID] [int] IDENTITY(1,1) NOT NULL,
	[RemarkCam1] [nvarchar](100) NULL,
	[RemarkCam2] [nvarchar](100) NULL,
	[RemarkCam3] [nvarchar](100) NULL,
	[RemarkCam4] [nvarchar](100) NULL,
	[RemarkCam5] [nvarchar](100) NULL,
	[RemarkCam6] [nvarchar](100) NULL,
	[RemarkCam7] [nvarchar](100) NULL,
	[CommonRemark] [nvarchar](100) NULL,
	[DmgdTypeCam1] [nvarchar](100) NULL,
	[DmgdTypeCam2] [nvarchar](100) NULL,
	[DmgdTypeCam3] [nvarchar](100) NULL,
	[DmgdTypeCam4] [nvarchar](100) NULL,
	[DmgdTypeCam5] [nvarchar](100) NULL,
	[DmgdTypeCam6] [nvarchar](100) NULL,
	[DmgdTypeCam7] [nvarchar](100) NULL,
	[CommonDmgTypes] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[DmgDtlsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DamageTransactionDetails]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DamageTransactionDetails](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DmgDtlsID] [int] NOT NULL,
	[DamageRemark] [nvarchar](500) NOT NULL,
	[DamageTypes] [nchar](10) NOT NULL,
	[CamPosID] [int] NULL,
	[IsCommonRemark] [bit] NOT NULL,
 CONSTRAINT [PK_DamageTransactionDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DamageTypeMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DamageTypeMaster](
	[DmgTypeid] [int] IDENTITY(1,1) NOT NULL,
	[DmgTypeName] [varchar](50) NULL,
	[CreatorUserID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[DmgTypeid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DesignationMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DesignationMaster](
	[ID] [int] NOT NULL,
	[DesignationName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_DesignationMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceMaster](
	[ID] [int] NOT NULL,
	[DeviceName] [nvarchar](max) NOT NULL,
	[DeviceTypeID] [int] NOT NULL,
	[DeviceIP] [nvarchar](50) NOT NULL,
	[DeviceMACAddress] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DeviceMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceTypeDetailsMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceTypeDetailsMaster](
	[ID] [bigint] NOT NULL,
	[DeviceTypeID] [int] NOT NULL,
	[PropertyKey] [int] NOT NULL,
	[PropertyValue] [int] NOT NULL,
 CONSTRAINT [PK_DeviceTypeDetailsMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceTypeMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceTypeMaster](
	[ID] [int] NOT NULL,
	[DeviceTypeName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_DeviceTypeMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LaneDeviceMappingMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LaneDeviceMappingMaster](
	[ID] [int] NOT NULL,
	[LaneID] [int] NOT NULL,
	[DeviceID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LaneMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LaneMaster](
	[LaneID] [int] IDENTITY(1,1) NOT NULL,
	[TypeOfGate] [int] NOT NULL,
	[LaneName] [nvarchar](100) NOT NULL,
	[CreatorUserID] [int] NULL,
	[TransactionsEnabled] [bit] NOT NULL,
	[IsBusy] [bit] NULL,
 CONSTRAINT [PK__LaneMast__A5770E2C164452B1] PRIMARY KEY CLUSTERED 
(
	[LaneID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationMaster](
	[ID] [int] NOT NULL,
	[LocationName] [nvarchar](50) NOT NULL,
	[LocationTypeID] [int] NOT NULL,
	[LocationDesc] [nvarchar](100) NULL,
 CONSTRAINT [PK_LocationMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationTypeDetailsMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationTypeDetailsMaster](
	[ID] [int] NOT NULL,
	[LocationTypeID] [int] NOT NULL,
	[PropertyKey] [nvarchar](100) NOT NULL,
	[PropertyValue] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_LocationTypeDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationTypeDeviceTypeMappingMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationTypeDeviceTypeMappingMaster](
	[ID] [int] NOT NULL,
	[LocationTypeID] [int] NOT NULL,
	[DeviceTypeID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationTypeMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationTypeMaster](
	[ID] [int] NOT NULL,
	[LocationTypeName] [nvarchar](50) NOT NULL,
	[IsGate] [bit] NOT NULL,
	[CreatorUserID] [int] NOT NULL,
	[IsTransactionEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_LocationTypeMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationTypeRoleMappingMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationTypeRoleMappingMaster](
	[ID] [int] NOT NULL,
	[LocationTypeID] [int] NOT NULL,
	[RoleID] [int] NOT NULL,
 CONSTRAINT [PK_LocationTypeRoleMappingMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OwnerCodeMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OwnerCodeMaster](
	[OwnerID] [int] IDENTITY(1,1) NOT NULL,
	[OwnerCode] [nvarchar](10) NOT NULL,
	[CreatorUserID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectMaster](
	[ID] [int] NOT NULL,
	[ProjectName] [nvarchar](100) NOT NULL,
	[DeployerFirm] [int] NOT NULL,
	[DeployeeFirm] [int] NOT NULL,
	[TypeID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectTypeMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectTypeMaster](
	[ID] [int] NOT NULL,
	[ProjectType] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ProjectTypeMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleDesignationMappingMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleDesignationMappingMaster](
	[ID] [int] NOT NULL,
	[DesignationID] [int] NOT NULL,
	[RoleID] [int] NOT NULL,
 CONSTRAINT [PK_RoleDesignationMappingMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleMaster](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
	[IsSuperUser] [bit] NULL,
 CONSTRAINT [PK_RoleMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [unique_rolename] UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleScreenMapping]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleScreenMapping](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NOT NULL,
	[ScreenID] [int] NOT NULL,
 CONSTRAINT [PK_RoleScreenMapping] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [unique_rolescreenmap] UNIQUE NONCLUSTERED 
(
	[RoleID] ASC,
	[ScreenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScreenMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScreenMaster](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ScreenName] [nvarchar](50) NOT NULL,
	[ScreenUrl] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ScreenMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [unique_ScreenNameUrl] UNIQUE NONCLUSTERED 
(
	[ScreenName] ASC,
	[ScreenUrl] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingLineMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingLineMaster](
	[ShippingLineID] [int] IDENTITY(1,1) NOT NULL,
	[ShippingLineName] [nvarchar](max) NOT NULL,
	[CompanyAddress] [nvarchar](max) NULL,
	[ContactNo] [nvarchar](max) NULL,
	[faxno] [nvarchar](50) NULL,
	[emailid1] [nvarchar](50) NULL,
	[emailid2] [nvarchar](50) NULL,
	[emailid3] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ShippingLineID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionApplicationTypeMappingMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionApplicationTypeMappingMaster](
	[ID] [int] NOT NULL,
	[TransactionID] [int] NOT NULL,
	[ApplicationTypeID] [int] NOT NULL,
 CONSTRAINT [PK_TransactionApplicationTypeMappingMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionFieldMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionFieldMaster](
	[ID] [int] NOT NULL,
	[TransactionID] [int] NOT NULL,
	[TransactionFieldName] [nvarchar](100) NOT NULL,
	[TransactionFieldType] [nvarchar](100) NOT NULL,
	[IsForeignKey] [bit] NOT NULL,
	[ForeignKeyTableName] [nvarchar](100) NULL,
	[ForeignKeyColumnName] [nvarchar](100) NULL,
 CONSTRAINT [PK_TransactionFieldMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionMaster](
	[ID] [int] NOT NULL,
	[TransactionName] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_TransactionMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserMaster]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMaster](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[ContactNo] [nvarchar](50) NOT NULL,
	[EmailId] [varchar](50) NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsLoggedin] [bit] NOT NULL,
	[DeviceID] [int] NULL,
	[DesignationID] [int] NULL,
	[ReportsTo] [int] NULL,
 CONSTRAINT [PK__UserMast__1788CC4C1DE57479] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [unique_email] UNIQUE NONCLUSTERED 
(
	[EmailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [unique_username] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[DamageTransactionDetails] ADD  CONSTRAINT [DF_DamageTransactionDetails_IsCommonRemark]  DEFAULT ((0)) FOR [IsCommonRemark]
GO
ALTER TABLE [dbo].[DeviceMaster] ADD  CONSTRAINT [DF_DeviceMaster_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[LaneMaster] ADD  CONSTRAINT [DF_LaneMaster_TransactionsEnabled]  DEFAULT ((0)) FOR [TransactionsEnabled]
GO
ALTER TABLE [dbo].[LocationTypeMaster] ADD  CONSTRAINT [DF_LocationTypeMaster_IsGate]  DEFAULT ((0)) FOR [IsGate]
GO
ALTER TABLE [dbo].[LocationTypeMaster] ADD  CONSTRAINT [DF_LocationTypeMaster_IsTransactionEnabled]  DEFAULT ((0)) FOR [IsTransactionEnabled]
GO
ALTER TABLE [dbo].[RoleMaster] ADD  CONSTRAINT [DF_RoleMaster_IsSuperUser]  DEFAULT ((0)) FOR [IsSuperUser]
GO
ALTER TABLE [dbo].[TransactionFieldMaster] ADD  CONSTRAINT [DF_TransactionFieldMaster_IsForeignKey]  DEFAULT ((0)) FOR [IsForeignKey]
GO
ALTER TABLE [dbo].[TransactionMaster] ADD  CONSTRAINT [DF_TransactionMaster_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ApplicationMaster]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationMaster_ApplicationTypeMaster] FOREIGN KEY([ApplicationTypeID])
REFERENCES [dbo].[ApplicationTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[ApplicationMaster] CHECK CONSTRAINT [FK_ApplicationMaster_ApplicationTypeMaster]
GO
ALTER TABLE [dbo].[ApplicationProjectMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationProjectMappingMaster_ApplicationMaster] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[ApplicationMaster] ([ID])
GO
ALTER TABLE [dbo].[ApplicationProjectMappingMaster] CHECK CONSTRAINT [FK_ApplicationProjectMappingMaster_ApplicationMaster]
GO
ALTER TABLE [dbo].[ApplicationProjectMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationProjectMappingMaster_ProjectMaster] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[ProjectMaster] ([ID])
GO
ALTER TABLE [dbo].[ApplicationProjectMappingMaster] CHECK CONSTRAINT [FK_ApplicationProjectMappingMaster_ProjectMaster]
GO
ALTER TABLE [dbo].[ApplicationTypeDevieTypeMaster]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationTypeDevieTypeMaster_ApplicationTypeMaster] FOREIGN KEY([ApplicationTypeID])
REFERENCES [dbo].[ApplicationTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[ApplicationTypeDevieTypeMaster] CHECK CONSTRAINT [FK_ApplicationTypeDevieTypeMaster_ApplicationTypeMaster]
GO
ALTER TABLE [dbo].[ApplicationTypeDevieTypeMaster]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationTypeDevieTypeMaster_DeviceTypeMaster] FOREIGN KEY([DeviceTypeID])
REFERENCES [dbo].[DeviceTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[ApplicationTypeDevieTypeMaster] CHECK CONSTRAINT [FK_ApplicationTypeDevieTypeMaster_DeviceTypeMaster]
GO
ALTER TABLE [dbo].[CameraDtlsTbl]  WITH CHECK ADD  CONSTRAINT [FK_CameraDtlsTbl_CameraPositionMaster] FOREIGN KEY([PositionID])
REFERENCES [dbo].[CameraPositionMaster] ([PositionID])
GO
ALTER TABLE [dbo].[CameraDtlsTbl] CHECK CONSTRAINT [FK_CameraDtlsTbl_CameraPositionMaster]
GO
ALTER TABLE [dbo].[CameraDtlsTbl]  WITH CHECK ADD  CONSTRAINT [FK_CameraIPDetails_GateDetails] FOREIGN KEY([LaneID])
REFERENCES [dbo].[LaneMaster] ([LaneID])
GO
ALTER TABLE [dbo].[CameraDtlsTbl] CHECK CONSTRAINT [FK_CameraIPDetails_GateDetails]
GO
ALTER TABLE [dbo].[CompanyMaster]  WITH CHECK ADD  CONSTRAINT [FK_CompanyMaster_CompanyTypeMaster] FOREIGN KEY([CompanyTypeID])
REFERENCES [dbo].[CompanyTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[CompanyMaster] CHECK CONSTRAINT [FK_CompanyMaster_CompanyTypeMaster]
GO
ALTER TABLE [dbo].[ContainerTransactions]  WITH CHECK ADD  CONSTRAINT [FK_ContainerTransactions_ContainerTypeMaster] FOREIGN KEY([ContainerTypeID])
REFERENCES [dbo].[ContainerTypeMaster] ([ContainerTypeID])
GO
ALTER TABLE [dbo].[ContainerTransactions] CHECK CONSTRAINT [FK_ContainerTransactions_ContainerTypeMaster]
GO
ALTER TABLE [dbo].[ContainerTransactions]  WITH CHECK ADD  CONSTRAINT [FK_ContainerTransactions_DamageTransaction] FOREIGN KEY([DmgDtlsID])
REFERENCES [dbo].[DamageTransaction] ([DmgDtlsID])
GO
ALTER TABLE [dbo].[ContainerTransactions] CHECK CONSTRAINT [FK_ContainerTransactions_DamageTransaction]
GO
ALTER TABLE [dbo].[ContainerTransactions]  WITH CHECK ADD  CONSTRAINT [FK_ContainerTransactions_LaneMaster] FOREIGN KEY([LaneID])
REFERENCES [dbo].[LaneMaster] ([LaneID])
GO
ALTER TABLE [dbo].[ContainerTransactions] CHECK CONSTRAINT [FK_ContainerTransactions_LaneMaster]
GO
ALTER TABLE [dbo].[ContainerTransactions]  WITH CHECK ADD  CONSTRAINT [FK_ContainerTransactions_ShippingLineMaster] FOREIGN KEY([ShippingLineID])
REFERENCES [dbo].[ShippingLineMaster] ([ShippingLineID])
GO
ALTER TABLE [dbo].[ContainerTransactions] CHECK CONSTRAINT [FK_ContainerTransactions_ShippingLineMaster]
GO
ALTER TABLE [dbo].[ContainerTransactions]  WITH CHECK ADD  CONSTRAINT [FK_ContainerTransactions_UserMaster] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserMaster] ([UserId])
GO
ALTER TABLE [dbo].[ContainerTransactions] CHECK CONSTRAINT [FK_ContainerTransactions_UserMaster]
GO
ALTER TABLE [dbo].[DamageTransactionDetails]  WITH CHECK ADD  CONSTRAINT [FK_DamageTransactionDetails_CameraPositionMaster] FOREIGN KEY([CamPosID])
REFERENCES [dbo].[CameraPositionMaster] ([PositionID])
GO
ALTER TABLE [dbo].[DamageTransactionDetails] CHECK CONSTRAINT [FK_DamageTransactionDetails_CameraPositionMaster]
GO
ALTER TABLE [dbo].[DamageTransactionDetails]  WITH CHECK ADD  CONSTRAINT [FK_DamageTransactionDetails_DamageTransaction] FOREIGN KEY([DmgDtlsID])
REFERENCES [dbo].[DamageTransaction] ([DmgDtlsID])
GO
ALTER TABLE [dbo].[DamageTransactionDetails] CHECK CONSTRAINT [FK_DamageTransactionDetails_DamageTransaction]
GO
ALTER TABLE [dbo].[DeviceMaster]  WITH CHECK ADD  CONSTRAINT [FK_DeviceMaster_DeviceTypeMaster] FOREIGN KEY([DeviceTypeID])
REFERENCES [dbo].[DeviceTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[DeviceMaster] CHECK CONSTRAINT [FK_DeviceMaster_DeviceTypeMaster]
GO
ALTER TABLE [dbo].[DeviceTypeDetailsMaster]  WITH CHECK ADD  CONSTRAINT [FK_DeviceTypeDetailsMaster_DeviceTypeMaster] FOREIGN KEY([DeviceTypeID])
REFERENCES [dbo].[DeviceTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[DeviceTypeDetailsMaster] CHECK CONSTRAINT [FK_DeviceTypeDetailsMaster_DeviceTypeMaster]
GO
ALTER TABLE [dbo].[LaneDeviceMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_LaneDeviceMappingMaster_DeviceMaster] FOREIGN KEY([DeviceID])
REFERENCES [dbo].[DeviceMaster] ([ID])
GO
ALTER TABLE [dbo].[LaneDeviceMappingMaster] CHECK CONSTRAINT [FK_LaneDeviceMappingMaster_DeviceMaster]
GO
ALTER TABLE [dbo].[LaneDeviceMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_LaneDeviceMappingMaster_LaneMaster] FOREIGN KEY([LaneID])
REFERENCES [dbo].[LaneMaster] ([LaneID])
GO
ALTER TABLE [dbo].[LaneDeviceMappingMaster] CHECK CONSTRAINT [FK_LaneDeviceMappingMaster_LaneMaster]
GO
ALTER TABLE [dbo].[LocationMaster]  WITH CHECK ADD  CONSTRAINT [FK_LocationMaster_LocationTypeMaster] FOREIGN KEY([LocationTypeID])
REFERENCES [dbo].[LocationTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[LocationMaster] CHECK CONSTRAINT [FK_LocationMaster_LocationTypeMaster]
GO
ALTER TABLE [dbo].[LocationTypeDetailsMaster]  WITH CHECK ADD  CONSTRAINT [FK_LocationTypeDetailsMaster_LocationTypeMaster] FOREIGN KEY([LocationTypeID])
REFERENCES [dbo].[LocationTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[LocationTypeDetailsMaster] CHECK CONSTRAINT [FK_LocationTypeDetailsMaster_LocationTypeMaster]
GO
ALTER TABLE [dbo].[LocationTypeDeviceTypeMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_LocationTypeDeviceTypeMappingMaster_DeviceTypeMaster] FOREIGN KEY([DeviceTypeID])
REFERENCES [dbo].[DeviceTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[LocationTypeDeviceTypeMappingMaster] CHECK CONSTRAINT [FK_LocationTypeDeviceTypeMappingMaster_DeviceTypeMaster]
GO
ALTER TABLE [dbo].[LocationTypeDeviceTypeMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_LocationTypeDeviceTypeMappingMaster_LocationTypeMaster] FOREIGN KEY([LocationTypeID])
REFERENCES [dbo].[LocationTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[LocationTypeDeviceTypeMappingMaster] CHECK CONSTRAINT [FK_LocationTypeDeviceTypeMappingMaster_LocationTypeMaster]
GO
ALTER TABLE [dbo].[LocationTypeRoleMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_LocationTypeRoleMappingMaster_LocationTypeMaster] FOREIGN KEY([LocationTypeID])
REFERENCES [dbo].[LocationTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[LocationTypeRoleMappingMaster] CHECK CONSTRAINT [FK_LocationTypeRoleMappingMaster_LocationTypeMaster]
GO
ALTER TABLE [dbo].[LocationTypeRoleMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_LocationTypeRoleMappingMaster_RoleMaster] FOREIGN KEY([RoleID])
REFERENCES [dbo].[RoleMaster] ([ID])
GO
ALTER TABLE [dbo].[LocationTypeRoleMappingMaster] CHECK CONSTRAINT [FK_LocationTypeRoleMappingMaster_RoleMaster]
GO
ALTER TABLE [dbo].[ProjectMaster]  WITH CHECK ADD  CONSTRAINT [FK_ProjectMaster_DeployeeFirm] FOREIGN KEY([DeployeeFirm])
REFERENCES [dbo].[CompanyMaster] ([ID])
GO
ALTER TABLE [dbo].[ProjectMaster] CHECK CONSTRAINT [FK_ProjectMaster_DeployeeFirm]
GO
ALTER TABLE [dbo].[ProjectMaster]  WITH CHECK ADD  CONSTRAINT [FK_ProjectMaster_DeployerFirm] FOREIGN KEY([DeployerFirm])
REFERENCES [dbo].[CompanyMaster] ([ID])
GO
ALTER TABLE [dbo].[ProjectMaster] CHECK CONSTRAINT [FK_ProjectMaster_DeployerFirm]
GO
ALTER TABLE [dbo].[ProjectMaster]  WITH CHECK ADD  CONSTRAINT [FK_ProjectMaster_ProjectTypeMaster] FOREIGN KEY([TypeID])
REFERENCES [dbo].[ProjectTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[ProjectMaster] CHECK CONSTRAINT [FK_ProjectMaster_ProjectTypeMaster]
GO
ALTER TABLE [dbo].[RoleDesignationMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_RoleDesignationMappingMaster_DesignationMaster] FOREIGN KEY([DesignationID])
REFERENCES [dbo].[DesignationMaster] ([ID])
GO
ALTER TABLE [dbo].[RoleDesignationMappingMaster] CHECK CONSTRAINT [FK_RoleDesignationMappingMaster_DesignationMaster]
GO
ALTER TABLE [dbo].[RoleDesignationMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_RoleDesignationMappingMaster_RoleMaster] FOREIGN KEY([RoleID])
REFERENCES [dbo].[RoleMaster] ([ID])
GO
ALTER TABLE [dbo].[RoleDesignationMappingMaster] CHECK CONSTRAINT [FK_RoleDesignationMappingMaster_RoleMaster]
GO
ALTER TABLE [dbo].[RoleScreenMapping]  WITH CHECK ADD  CONSTRAINT [FK_RoleScreenMapping_RoleID] FOREIGN KEY([RoleID])
REFERENCES [dbo].[RoleMaster] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoleScreenMapping] CHECK CONSTRAINT [FK_RoleScreenMapping_RoleID]
GO
ALTER TABLE [dbo].[RoleScreenMapping]  WITH CHECK ADD  CONSTRAINT [FK_RoleScreenMapping_ScreenMaster] FOREIGN KEY([ScreenID])
REFERENCES [dbo].[ScreenMaster] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoleScreenMapping] CHECK CONSTRAINT [FK_RoleScreenMapping_ScreenMaster]
GO
ALTER TABLE [dbo].[TransactionApplicationTypeMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_TransactionApplicationTypeMappingMaster_ApplicationTypeMaster] FOREIGN KEY([ApplicationTypeID])
REFERENCES [dbo].[ApplicationTypeMaster] ([ID])
GO
ALTER TABLE [dbo].[TransactionApplicationTypeMappingMaster] CHECK CONSTRAINT [FK_TransactionApplicationTypeMappingMaster_ApplicationTypeMaster]
GO
ALTER TABLE [dbo].[TransactionApplicationTypeMappingMaster]  WITH CHECK ADD  CONSTRAINT [FK_TransactionApplicationTypeMappingMaster_TransactionMaster] FOREIGN KEY([TransactionID])
REFERENCES [dbo].[TransactionMaster] ([ID])
GO
ALTER TABLE [dbo].[TransactionApplicationTypeMappingMaster] CHECK CONSTRAINT [FK_TransactionApplicationTypeMappingMaster_TransactionMaster]
GO
ALTER TABLE [dbo].[TransactionFieldMaster]  WITH CHECK ADD  CONSTRAINT [FK_TransactionFieldMaster_TransactionMaster] FOREIGN KEY([TransactionID])
REFERENCES [dbo].[TransactionMaster] ([ID])
GO
ALTER TABLE [dbo].[TransactionFieldMaster] CHECK CONSTRAINT [FK_TransactionFieldMaster_TransactionMaster]
GO
/****** Object:  StoredProcedure [dbo].[CameraPositionID]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CameraPositionID]
AS
BEGIN
	declare @PositionIndex int
	declare @TempPostion  int
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	set @PositionIndex = 1;

		IF CURSOR_STATUS('global','cur_trn')>=-1
		BEGIN
		 DEALLOCATE cur_trn
		END

	declare cur_trn CURSOR

	static for
		select [PositionID] from [dbo].[CameraPositionMaster] order by [PositionID] 		
	open cur_trn
	if @@CURSOR_ROWS>0
	begin
		fetch next from cur_trn into @TempPostion
		while @@FETCH_STATUS = 0
		begin
			if(@PositionIndex != @TempPostion)
				break;

			else
				set @PositionIndex = @PositionIndex + 1;

			fetch next from cur_trn into @TempPostion
		end
	end

	close cur_trn
	deallocate cur_trn

	select @PositionIndex
	return @PositionIndex;
END







GO
/****** Object:  StoredProcedure [dbo].[fetch_image_details]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[fetch_image_details]
(@TransactionID int
)
AS
declare @ContainerType		  int	--1 = 20 Ft
									--2 = 40 Ft
declare @MinTransactionID int
declare @TransactionCount	  int
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT @ContainerType = ContainerTypeID from [dbo].[ContainerTransactions] where TransID=@TransactionID

	if(@ContainerType = 2)
	begin
		select PositionID from [dbo].[CameraPositionMaster]
	end
	
	else
	begin
		select @TransactionCount=count(*) from ContainerTransactions where TrailerTransID = (select TrailerTransID from ContainerTransactions where TransID = @TransactionID)

		if(@TransactionCount=1)
		begin
			select PositionID from [dbo].[CameraPositionMaster]
		end

		else
		begin
			select @MinTransactionID = min(TransID) from ContainerTransactions where TrailerTransID = (select TrailerTransID from ContainerTransactions where TransID = @TransactionID)

			if(@MinTransactionID=@TransactionID)
			begin
				select PositionID from [dbo].[CameraPositionMaster] where ContainerVisible in (1,3)
			end

			else
			begin
				select PositionID from [dbo].[CameraPositionMaster] where ContainerVisible in (2,3)
			end
		end
	end
END

GO
/****** Object:  StoredProcedure [dbo].[savecamerapositionngdata]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[savecamerapositionngdata]
(
@positiondescription varchar(250) ,
@userdescriptionposition varchar(250),
@containervisible int
)
as 
DECLARE @positionID int;
EXEC @positionID=[dbo].[CameraPositionID];
insert into CameraPositionMaster(PositionID,PositionName,PositionDescription,ContainerVisible) values(@positionID,@positiondescription,@userdescriptionposition,@containervisible);





GO
/****** Object:  StoredProcedure [dbo].[sp_GetMissingTransactions]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetMissingTransactions] 
	-- Add the parameters for the stored procedure here
	@ContainerTransReport ContainerCodesToSearch readonly,
	@StartTime datetime,
	@EndTime datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ContainerCode, TransactionTime as InsertionTime from ContainerTransactions where ContainerCode not in (SELECT ContainerCode FROM @ContainerTransReport) AND TransactionTime between @StartTime and @EndTime
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetScreensFromRoleID]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetScreensFromRoleID] 
	-- Add the parameters for the stored procedure here
	@RoleID int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
		rm.RoleName,
	    sm.ScreenName,
	    sm.ScreenUrl
    FROM 
		dbo.RoleScreenMapping rsm
	INNER JOIN 
		dbo.ScreenMaster sm on rsm.ScreenID = sm.ID
	INNER JOIN
		dbo.RoleMaster rm ON rsm.RoleID = rm.ID
	WHERE rsm.RoleID = @RoleID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_MapScreensToRoles]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_MapScreensToRoles]
	-- Add the parameters for the stored procedure here
	@ScreenIDList ScreenIds readonly,
	@RoleID int=0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @ScreenId int = 0
	DECLARE screen_cursor CURSOR FOR SELECT * FROM @ScreenIDList ORDER BY ScreenID
	OPEN screen_cursor
	FETCH NEXT FROM screen_cursor INTO @ScreenId
	WHILE @@FETCH_STATUS=0
	BEGIN
		INSERT INTO RoleScreenMapping (RoleID, ScreenID) values (@RoleID, @ScreenId)
	END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_StoreDamageDetails]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_StoreDamageDetails] 
	-- Add the parameters for the stored procedure here
	@DmgDetails TempDmgDetailsTbl readonly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @DmgDtlsID bigint, @DamageRemark nvarchar(200), @DmgTypes nvarchar(200), @CamPosID int, @IsCommonRemark bit 
	DECLARE dmg_dtls_cursor CURSOR FOR select DmgDtlsID, DamageRemark, DamageTypes, CamPosID, IsCommonRemark from @DmgDetails
	OPEN dmg_dtls_cursor 
	FETCH NEXT FROM dmg_dtls_cursor INTO @DmgDtlsID, @DamageRemark, @DmgTypes, @CamPosID, @IsCommonRemark
	WHILE @@FETCH_STATUS=0
	BEGIN
		INSERT INTO [dbo].[DamageTransactionDetails] (DmgDtlsID, DamageRemark, DamageTypes, CamPosID, IsCommonRemark) values (@DmgDtlsID, @DamageRemark, @DmgTypes, @CamPosID, @IsCommonRemark)
		FETCH NEXT FROM dmg_dtls_cursor INTO @DmgDtlsID, @DamageRemark, @DmgTypes, @CamPosID, @IsCommonRemark 
	END
	CLOSE dmg_dtls_cursor
	DEALLOCATE dmg_dtls_cursor
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UserLogout]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UserLogout]
	-- Add the parameters for the stored procedure here
	@UserID int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE UserMaster set IsLoggedin = 0 WHERE UserId=@UserID
END
GO
/****** Object:  StoredProcedure [dbo].[spsavecameradata]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[spsavecameradata]
(
@cameraip varchar(50) ,
@laneid varchar(50),
@positiondesc varchar(250)
)
as
DECLARE @ID int;
EXEC @ID=[dbo].[CameraPositionID];
insert into   CameraDtlsTbl(CameraIP , LaneID , PositionID) 
values (@cameraip , @laneid , @positiondesc);





GO
/****** Object:  StoredProcedure [dbo].[spsavelanedata]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[spsavelanedata]
(
@ingate varchar(50) ,
@lanename varchar(50) ,
@busy varchar(50) ,
@ipaddress varchar(50) 

)
as
DECLARE @Ipadd varchar(50);
select @Ipadd=SystemIP from LaneMaster where SystemIP=@ipaddress;
if(@Ipadd=@ipaddress)
begin
RAISERROR ('System IP Already Assigned', 16, 1);
end
else
begin
insert into   LaneMaster(TypeOfGate , LaneName , IsBusy , SystemIP) 
values (@ingate ,@lanename ,@busy ,@ipaddress);

end







GO
/****** Object:  StoredProcedure [dbo].[spsaveshippinglinedata]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[spsaveshippinglinedata] 
(
@shippinglinename varchar(50),
@companyaddr varchar(50) ,
@phoneno varchar(50),
@faxno varchar(50),
@emailid1 varchar(50) ,
@emailid2 varchar(50) ,
@emailid3 varchar(50) 
)
as
insert into ShippingLineMaster(ShippingLineName , CompanyAddress , ContactNo , faxno , emailid1 , emailid2 , emailid3) values (@shippinglinename ,@companyaddr ,@phoneno,@faxno,@emailid1 , @emailid2 , @emailid3 );









GO
/****** Object:  StoredProcedure [dbo].[spupdatecamerapositioningdata]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[spupdatecamerapositioningdata]
(
@primarykey varchar(200) ,
@positiondescription varchar(300),
@userdescriptionposition varchar(300),
@containervisible int
)
as 
begin
update CameraPositionMaster set PositionName=@positiondescription,PositionDescription= @userdescriptionposition,ContainerVisible=@containervisible where PositionID=@primarykey ;
end








GO
/****** Object:  StoredProcedure [dbo].[updatecamera]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[updatecamera] 
(
@ipofcamera varchar(50) ,
@cameraid int,
@laneid varchar(50),
@positiondesc int
)
as
declare @cameraidid  int; declare @cnt  int  ;
select @cameraidid=CameraID from CameraDtlsTbl where CameraIP= @ipofcamera ;
select @cnt=count(1)  from CameraDtlsTbl where LaneID=@laneid and PositionID=@positiondesc ;

if(@cnt=1)
begin
--update CameraDtlsTbl set PositionID=(select PositionID from CameraDtlsTbl where CameraIP= @ipofcamera) where CameraIP=(select CameraIP  from CameraDtlsTbl where laneid=@laneid and PositionID=@positiondesc );
update CameraDtlsTbl set CameraIP=@ipofcamera ,laneid=@laneid,PositionID=@positiondesc where cameraid=@cameraid;
end

if( @cameraidid is not null )
begin
update CameraDtlsTbl set CameraIP=@ipofcamera ,laneid=@laneid,PositionID=@positiondesc where cameraid=@cameraid;
end








GO
/****** Object:  StoredProcedure [dbo].[UpdateEmptytransaction]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[UpdateEmptytransaction](
	-- Add the parameters for the stored procedure here
	@TransiD int)
AS
BEGIN
update ContainerTransactions set Displayed='True',CancelStatus='True' where TransID=@TransiD
END
GO
/****** Object:  StoredProcedure [dbo].[updatelanedata]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[updatelanedata]
(
@id int ,
@ingate varchar(50) ,
@lanenameold varchar(50) ,
@lanenamenew varchar(50),
@busy int ,
@ipaddress varchar(50)
)
as

declare @laneid  int ;
declare @ipaddr varchar(50) ;
declare @ipaddssr varchar(50) ;

select @laneid=LaneID from LaneMaster where LaneName= @lanenamenew and @ipaddr= @ipaddress;
select @ipaddssr=SystemIP from LaneMaster where SystemIP=@ipaddress;
if(@laneid=@id or @laneid is null )
begin
update LaneMaster set TypeOfGate=@ingate , LaneName=@lanenamenew , IsBusy = @busy ,SystemIP=@ipaddress  where LaneID = @id
end
else
begin
RAISERROR ('lane name Exists', 16, 1);
end








GO
/****** Object:  StoredProcedure [dbo].[updateshippingline]    Script Date: 07-01-2019 10:54:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[updateshippingline]
(
@id int , 
@shippinglinenamenew varchar(50),
@shippinglinenameold varchar(50),
@companyaddress varchar(50),
@companyphoneno varchar(50) ,
@faxno varchar(50),
@emailid1 varchar(50),
@emailid2 varchar(50),
@emailid3 varchar(50)
)
as

declare @shippinglineid  int ;

if(@shippinglinenameold!=@shippinglinenamenew)
begin
select @shippinglineid=ShippingLineID from ShippingLineMaster where ShippingLineName= @shippinglinenamenew ;
if(@shippinglineid is null)
begin
update ShippingLineMaster set ShippingLineName=@shippinglinenamenew , CompanyAddress=@companyaddress ,ContactNo=@companyphoneno ,faxno=@faxno , emailid1=@emailid1 , emailid2=@emailid2 , emailid3=@emailid3 where ShippingLineID=@id;
end
else
begin
RAISERROR ('lane name Exists', 16, 1);
end
end

else
begin
update ShippingLineMaster set ShippingLineName=@shippinglinenamenew , CompanyAddress=@companyaddress ,ContactNo=@companyphoneno ,faxno=@faxno , emailid1=@emailid1 , emailid2=@emailid2 , emailid3=@emailid3 where ShippingLineID=@id;

end








GO
USE [master]
GO
ALTER DATABASE [DIMContainerDB_Revised_Dev] SET  READ_WRITE 
GO
