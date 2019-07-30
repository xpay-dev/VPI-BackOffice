USE [VeritasPay]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[AccountId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[PIN] [nvarchar](50) NOT NULL,
	[ParentId] [int] NOT NULL,
	[ParentTypeId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[LogTries] [int] NOT NULL,
	[IPAddress] [nvarchar](max) NULL,
	[AccountAvailableDate] [datetime] NULL,
	[LastLoggedIn] [datetime] NULL,
	[PasswordExpirationDate] [datetime] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[NeedsUpdate] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Accounts] PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccountType]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountType](
	[AccountTypeId] [int] IDENTITY(1,1) NOT NULL,
	[AccountName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AccountType] PRIMARY KEY CLUSTERED 
(
	[AccountTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActionLogs]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActionLogs](
	[ActionLogId] [int] IDENTITY(1,1) NOT NULL,
	[TargetId] [int] NOT NULL,
	[TargetTable] [nvarchar](50) NOT NULL,
	[Action] [nvarchar](50) NOT NULL,
	[Details] [nvarchar](max) NULL,
	[LoggedByUserId] [int] NOT NULL,
	[DateLogged] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.ActionLogs] PRIMARY KEY CLUSTERED 
(
	[ActionLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Agreements]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agreements](
	[AgreementsId] [int] IDENTITY(1,1) NOT NULL,
	[TermsOfService] [nvarchar](max) NULL,
	[Disclaimer] [nvarchar](max) NULL,
	[LanguageCode] [nvarchar](50) NULL,
	[IsCustom] [bit] NOT NULL,
	[MerchantId] [int] NULL,
 CONSTRAINT [PK_dbo.Agreements] PRIMARY KEY CLUSTERED 
(
	[AgreementsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AndroidAppVersion]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AndroidAppVersion](
	[AndroidAppVersionId] [int] IDENTITY(1,1) NOT NULL,
	[AppName] [nvarchar](max) NULL,
	[PackageName] [nvarchar](max) NULL,
	[VersionName] [nvarchar](max) NULL,
	[VersionBuild] [nvarchar](max) NULL,
	[VersionCode] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.AndroidAppVersion] PRIMARY KEY CLUSTERED 
(
	[AndroidAppVersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Batch]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Batch](
	[BatchId] [int] IDENTITY(1,1) NOT NULL,
	[BatchNumber] [nvarchar](max) NULL,
	[PaymentTypeId] [int] NOT NULL,
	[MerchantId] [int] NOT NULL,
	[MobileAppId] [int] NOT NULL,
	[Currency] [nvarchar](max) NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[TotalCount] [int] NOT NULL,
	[BatchDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Batch] PRIMARY KEY CLUSTERED 
(
	[BatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BillingCycles]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BillingCycles](
	[BillingCycleId] [int] IDENTITY(1,1) NOT NULL,
	[CycleType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.BillingCycles] PRIMARY KEY CLUSTERED 
(
	[BillingCycleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CardTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CardTypes](
	[CardTypeId] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](100) NOT NULL,
	[IsoCode] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.CardTypes] PRIMARY KEY CLUSTERED 
(
	[CardTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ContactInformation]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContactInformation](
	[ContactInformationId] [int] IDENTITY(1,1) NOT NULL,
	[Address] [nvarchar](100) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[StateProvince] [nvarchar](max) NULL,
	[ProvIsoCode] [nvarchar](max) NULL,
	[CountryId] [int] NOT NULL,
	[ZipCode] [nvarchar](10) NOT NULL,
	[PrimaryContactNumber] [nvarchar](30) NOT NULL,
	[Fax] [nvarchar](30) NULL,
	[MobileNumber] [nvarchar](30) NULL,
	[NeedsUpdate] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ContactInformation] PRIMARY KEY CLUSTERED 
(
	[ContactInformationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Countries](
	[CountryId] [int] IDENTITY(1,1) NOT NULL,
	[CountryName] [nvarchar](50) NULL,
	[CountryCode] [nvarchar](5) NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Countries] PRIMARY KEY CLUSTERED 
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CountryIPs]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountryIPs](
	[CountryIPId] [int] IDENTITY(1,1) NOT NULL,
	[CountryIPFrom] [int] NOT NULL,
	[CountryIPTo] [int] NOT NULL,
	[CountryId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.CountryIPs] PRIMARY KEY CLUSTERED 
(
	[CountryIPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Currencies]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currencies](
	[CurrencyId] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyCode] [nvarchar](10) NOT NULL,
	[CurrencyName] [nvarchar](100) NOT NULL,
	[IsoCode] [nvarchar](max) NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Currencies] PRIMARY KEY CLUSTERED 
(
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbSettings]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbSettings](
	[DbSettingsId] [int] IDENTITY(1,1) NOT NULL,
	[SettingCode] [nvarchar](max) NULL,
	[SettingValue] [nvarchar](max) NULL,
	[SettingDescription] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.DbSettings] PRIMARY KEY CLUSTERED 
(
	[DbSettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceFlowTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceFlowTypes](
	[DeviceFlowTypeId] [int] IDENTITY(1,1) NOT NULL,
	[FlowType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.DeviceFlowTypes] PRIMARY KEY CLUSTERED 
(
	[DeviceFlowTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceMerchantLink]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceMerchantLink](
	[DeviceMerchantLinkId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantId] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[AssignedDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.DeviceMerchantLink] PRIMARY KEY CLUSTERED 
(
	[DeviceMerchantLinkId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DevicePlatforms]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DevicePlatforms](
	[DevicePlatformId] [int] IDENTITY(1,1) NOT NULL,
	[Platform] [nvarchar](50) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.DevicePlatforms] PRIMARY KEY CLUSTERED 
(
	[DevicePlatformId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DevicePOSLink]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DevicePOSLink](
	[DevicePOSLinkId] [int] IDENTITY(1,1) NOT NULL,
	[MasterDeviceId] [int] NOT NULL,
	[MerchantPOSId] [int] NOT NULL,
	[AssignedDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.DevicePOSLink] PRIMARY KEY CLUSTERED 
(
	[DevicePOSLinkId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Devices]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Devices](
	[DeviceId] [int] IDENTITY(1,1) NOT NULL,
	[MasterDeviceId] [int] NOT NULL,
	[SerialNumber] [nvarchar](100) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsBlackListed] [bit] NOT NULL,
	[KeyInjectedId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Devices] PRIMARY KEY CLUSTERED 
(
	[DeviceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceTypes](
	[DeviceTypeId] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_dbo.DeviceTypes] PRIMARY KEY CLUSTERED 
(
	[DeviceTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmailServers]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailServers](
	[EmailServerId] [int] IDENTITY(1,1) NOT NULL,
	[EmailServerName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Host] [nvarchar](max) NULL,
	[Port] [int] NOT NULL,
	[Username] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[UseSSL] [bit] NOT NULL,
	[DefaultCredential] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[PartnerId] [int] NOT NULL,
	[IsPartnerDefaultEmail] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.EmailServers] PRIMARY KEY CLUSTERED 
(
	[EmailServerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmvAmexParameters]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmvAmexParameters](
	[EmvAmexParameterId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationID] [nvarchar](max) NULL,
	[AppSelectionIndicator] [nvarchar](max) NULL,
	[AppAccountSelection] [nvarchar](max) NULL,
	[AppVersionNumber] [nvarchar](max) NULL,
	[TerminalActionDefault] [nvarchar](max) NULL,
	[TerminalActionDenial] [nvarchar](max) NULL,
	[TerminalActionOnline] [nvarchar](max) NULL,
	[MaxTarget] [nvarchar](max) NULL,
	[TagPercent] [nvarchar](max) NULL,
	[ThresholdValue] [nvarchar](max) NULL,
	[ContactlessTerminalDefault] [nvarchar](max) NULL,
	[ContactlessTerminalDenial] [nvarchar](max) NULL,
	[ContactlessTerminalOnline] [nvarchar](max) NULL,
	[TerminalRiskManagement] [nvarchar](max) NULL,
	[ReservedFutureUse] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.EmvAmexParameters] PRIMARY KEY CLUSTERED 
(
	[EmvAmexParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmvDiscoverParameters]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmvDiscoverParameters](
	[EmvDiscoverParameterId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationID] [nvarchar](max) NULL,
	[AppSelectionIndicator] [nvarchar](max) NULL,
	[AppAccountSelection] [nvarchar](max) NULL,
	[AppVersionNumber] [nvarchar](max) NULL,
	[TerminalActionDefault] [nvarchar](max) NULL,
	[TerminalActionDenial] [nvarchar](max) NULL,
	[TerminalActionOnline] [nvarchar](max) NULL,
	[MaxTarget] [nvarchar](max) NULL,
	[TagPercent] [nvarchar](max) NULL,
	[ThresholdValue] [nvarchar](max) NULL,
	[ContactlessTerminalDefault] [nvarchar](max) NULL,
	[ContactlessTerminalDenial] [nvarchar](max) NULL,
	[ContactlessTerminalOnline] [nvarchar](max) NULL,
	[TerminalRiskManagement] [nvarchar](max) NULL,
	[ReservedFutureUse] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.EmvDiscoverParameters] PRIMARY KEY CLUSTERED 
(
	[EmvDiscoverParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmvHostParameters]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmvHostParameters](
	[EmvHostParameterId] [int] IDENTITY(1,1) NOT NULL,
	[TransResponseTimer] [nvarchar](max) NULL,
	[DialActTime] [nvarchar](max) NULL,
	[PrimaryPhoneNumber] [nvarchar](max) NULL,
	[IPAddress] [nvarchar](max) NULL,
	[SecondaryPhoneNumber] [nvarchar](max) NULL,
	[SecondaryIPAddress] [nvarchar](max) NULL,
	[MerchantName] [nvarchar](max) NULL,
	[MerchantAddress] [nvarchar](max) NULL,
	[MerchantCityProv] [nvarchar](max) NULL,
	[TerminalAppSoftwareVer] [nvarchar](max) NULL,
	[HostDateTime] [nvarchar](max) NULL,
	[ExtraReceiptDisplay] [nvarchar](max) NULL,
	[DiscoverCLCvmLimit] [nvarchar](max) NULL,
	[VisaCLCvmLimit] [nvarchar](max) NULL,
	[MasterCardCLCvmLimit] [nvarchar](max) NULL,
	[MerchantTypeIndicator] [nvarchar](max) NULL,
	[Future1] [nvarchar](max) NULL,
	[DiscoverEVMFloorLimit] [nvarchar](max) NULL,
	[VISAEMVFloorLimit] [nvarchar](max) NULL,
	[MasterCardEMVFloorLimit] [nvarchar](max) NULL,
	[AmexJCBEMVFloorLimit] [nvarchar](max) NULL,
	[Future2] [nvarchar](max) NULL,
	[IDPRetailSubcharge] [nvarchar](max) NULL,
	[IDPCashbackSubcharge] [nvarchar](max) NULL,
	[CCRetailSubcharge] [nvarchar](max) NULL,
	[CCCashbackSubcharge] [nvarchar](max) NULL,
	[RetailSubchargeLimit] [nvarchar](max) NULL,
	[CashbackSubchargeLimit] [nvarchar](max) NULL,
	[VisaDebitSupport] [nvarchar](max) NULL,
	[AmexJCBCLCVMLimit] [nvarchar](max) NULL,
	[InteracCLReceiptReqLimit] [nvarchar](max) NULL,
	[PredialSetting] [nvarchar](max) NULL,
	[DiscoverCLFloorLimit] [nvarchar](max) NULL,
	[VisaCLFloorLimit] [nvarchar](max) NULL,
	[MasterCardCLFloorLimit] [nvarchar](max) NULL,
	[AmexJCBCLFloorLimit] [nvarchar](max) NULL,
	[DiscoverCLTxnLimit] [nvarchar](max) NULL,
	[VisaCLTxnLimit] [nvarchar](max) NULL,
	[MasterCardCLTxnLimit] [nvarchar](max) NULL,
	[AmexJCBCLTxnLimit] [nvarchar](max) NULL,
	[TerminalTransInfo] [nvarchar](max) NULL,
	[TerminalOptionStat] [nvarchar](max) NULL,
	[Future3] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.EmvHostParameters] PRIMARY KEY CLUSTERED 
(
	[EmvHostParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmvInteracParameters]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmvInteracParameters](
	[EmvInteracParameterId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationID] [nvarchar](max) NULL,
	[AppSelectionIndicator] [nvarchar](max) NULL,
	[AppAccountSelection] [nvarchar](max) NULL,
	[AppVersionNumber] [nvarchar](max) NULL,
	[TerminalActionDefault] [nvarchar](max) NULL,
	[TerminalActionDenial] [nvarchar](max) NULL,
	[TerminalActionOnline] [nvarchar](max) NULL,
	[MaxTarget] [nvarchar](max) NULL,
	[TagPercent] [nvarchar](max) NULL,
	[ThresholdValue] [nvarchar](max) NULL,
	[ContactlessTerminalDefault] [nvarchar](max) NULL,
	[ContactlessTerminalDenial] [nvarchar](max) NULL,
	[ContactlessTerminalOnline] [nvarchar](max) NULL,
	[TerminalRiskManagement] [nvarchar](max) NULL,
	[ReservedFutureUse] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.EmvInteracParameters] PRIMARY KEY CLUSTERED 
(
	[EmvInteracParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmvJcbParameters]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmvJcbParameters](
	[EmvJcbParameterId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationID] [nvarchar](max) NULL,
	[AppSelectionIndicator] [nvarchar](max) NULL,
	[AppAccountSelection] [nvarchar](max) NULL,
	[AppVersionNumber] [nvarchar](max) NULL,
	[TerminalActionDefault] [nvarchar](max) NULL,
	[TerminalActionDenial] [nvarchar](max) NULL,
	[TerminalActionOnline] [nvarchar](max) NULL,
	[MaxTarget] [nvarchar](max) NULL,
	[TagPercent] [nvarchar](max) NULL,
	[ThresholdValue] [nvarchar](max) NULL,
	[ContactlessTerminalDefault] [nvarchar](max) NULL,
	[ContactlessTerminalDenial] [nvarchar](max) NULL,
	[ContactlessTerminalOnline] [nvarchar](max) NULL,
	[TerminalRiskManagement] [nvarchar](max) NULL,
	[ReservedFutureUse] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.EmvJcbParameters] PRIMARY KEY CLUSTERED 
(
	[EmvJcbParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmvMastercardParameters]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmvMastercardParameters](
	[EmvMastercardParameterId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationID] [nvarchar](max) NULL,
	[AppSelectionIndicator] [nvarchar](max) NULL,
	[AppAccountSelection] [nvarchar](max) NULL,
	[AppVersionNumber] [nvarchar](max) NULL,
	[TerminalActionDefault] [nvarchar](max) NULL,
	[TerminalActionDenial] [nvarchar](max) NULL,
	[TerminalActionOnline] [nvarchar](max) NULL,
	[MaxTarget] [nvarchar](max) NULL,
	[TagPercent] [nvarchar](max) NULL,
	[ThresholdValue] [nvarchar](max) NULL,
	[ContactlessTerminalDefault] [nvarchar](max) NULL,
	[ContactlessTerminalDenial] [nvarchar](max) NULL,
	[ContactlessTerminalOnline] [nvarchar](max) NULL,
	[TerminalRiskManagement] [nvarchar](max) NULL,
	[ReservedFutureUse] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.EmvMastercardParameters] PRIMARY KEY CLUSTERED 
(
	[EmvMastercardParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmvTerminalParameters]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmvTerminalParameters](
	[EmvTerminalParameterId] [int] IDENTITY(1,1) NOT NULL,
	[TerminalCapabilities] [nvarchar](max) NULL,
	[AddTerminlaCapablities] [nvarchar](max) NULL,
	[TerminalCountryCode] [nvarchar](max) NULL,
	[TerminalType] [nvarchar](max) NULL,
	[TransCurrCode] [nvarchar](max) NULL,
	[TransCurrExponent] [nvarchar](max) NULL,
	[TransRefCurrCode] [nvarchar](max) NULL,
	[TransRefCurrExponent] [nvarchar](max) NULL,
	[TransRefCurrConversion] [nvarchar](max) NULL,
	[Reserved] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.EmvTerminalParameters] PRIMARY KEY CLUSTERED 
(
	[EmvTerminalParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmvVisaParameters]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmvVisaParameters](
	[EmvVisaParameterId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationID] [nvarchar](max) NULL,
	[AppSelectionIndicator] [nvarchar](max) NULL,
	[AppAccountSelection] [nvarchar](max) NULL,
	[AppVersionNumber] [nvarchar](max) NULL,
	[TerminalActionDefault] [nvarchar](max) NULL,
	[TerminalActionDenial] [nvarchar](max) NULL,
	[TerminalActionOnline] [nvarchar](max) NULL,
	[MaxTarget] [nvarchar](max) NULL,
	[TagPercent] [nvarchar](max) NULL,
	[ThresholdValue] [nvarchar](max) NULL,
	[ContactlessTerminalDefault] [nvarchar](max) NULL,
	[ContactlessTerminalDenial] [nvarchar](max) NULL,
	[ContactlessTerminalOnline] [nvarchar](max) NULL,
	[TerminalRiskManagement] [nvarchar](max) NULL,
	[ReservedFutureUse] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.EmvVisaParameters] PRIMARY KEY CLUSTERED 
(
	[EmvVisaParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ErrorLogs]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLogs](
	[ErrorLogId] [int] IDENTITY(1,1) NOT NULL,
	[Method] [nvarchar](100) NULL,
	[Action] [nvarchar](100) NULL,
	[ErrText] [nvarchar](max) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[InnerException] [nvarchar](max) NULL,
	[InnerExceptionStackTrace] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.ErrorLogs] PRIMARY KEY CLUSTERED 
(
	[ErrorLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HeaderResponse]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HeaderResponse](
	[HeaderResponseId] [int] IDENTITY(1,1) NOT NULL,
	[TerminalId] [nvarchar](max) NULL,
	[EmvHostParameterId] [int] NULL,
	[EmvTerminalParameterId] [int] NULL,
	[EmvMastercardParameterId] [int] NULL,
	[EmvVisaParameterId] [int] NULL,
	[EmvAmexParameterId] [int] NULL,
	[EmvInteracParameterId] [int] NULL,
	[EmvJcbParametersId] [int] NULL,
	[EmvDiscoverParameterId] [int] NULL,
	[ResultMessage] [nvarchar](max) NULL,
	[ReturnCode] [nvarchar](max) NULL,
	[MessageClass] [nvarchar](max) NULL,
	[SequenceNumber] [nvarchar](max) NULL,
	[TransType] [nvarchar](max) NULL,
	[POSEntryMode] [nvarchar](max) NULL,
	[MessageVersion] [nvarchar](max) NULL,
	[POSResultCode] [nvarchar](max) NULL,
	[POSStatIndicator] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.HeaderResponse] PRIMARY KEY CLUSTERED 
(
	[HeaderResponseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KeyInjected]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KeyInjected](
	[KeyInjectedId] [int] IDENTITY(1,1) NOT NULL,
	[KeyDetail1] [nvarchar](50) NOT NULL,
	[KeyDetail2] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.KeyInjected] PRIMARY KEY CLUSTERED 
(
	[KeyInjectedId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Keys]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Keys](
	[KeyId] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](2000) NOT NULL,
	[IV] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_dbo.Keys] PRIMARY KEY CLUSTERED 
(
	[KeyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Languages]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Languages](
	[LanguageId] [int] IDENTITY(1,1) NOT NULL,
	[LanguageCode] [nvarchar](50) NOT NULL,
	[LanguageName] [nvarchar](50) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Languages] PRIMARY KEY CLUSTERED 
(
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MasterDevices]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MasterDevices](
	[MasterDeviceId] [int] IDENTITY(1,1) NOT NULL,
	[DeviceFlowTypeId] [int] NOT NULL,
	[DeviceTypeId] [int] NOT NULL,
	[DeviceName] [nvarchar](100) NOT NULL,
	[Manufacturer] [nvarchar](100) NOT NULL,
	[Warranty] [nvarchar](30) NOT NULL,
	[ExternalData] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.MasterDevices] PRIMARY KEY CLUSTERED 
(
	[MasterDeviceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantBranches]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantBranches](
	[MerchantBranchId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantBranchName] [nvarchar](100) NOT NULL,
	[ContactInformationId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[MerchantId] [int] NOT NULL,
	[Merchant_MerchantId] [int] NULL,
 CONSTRAINT [PK_dbo.MerchantBranches] PRIMARY KEY CLUSTERED 
(
	[MerchantBranchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantBranchPOSs]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantBranchPOSs](
	[MerchantPOSId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantPOSName] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[MerchantBranchId] [int] NOT NULL,
	[MidsMerchantBranchPOSs_Id] [int] NULL,
 CONSTRAINT [PK_dbo.MerchantBranchPOSs] PRIMARY KEY CLUSTERED 
(
	[MerchantPOSId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantFeatures]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantFeatures](
	[MerchantFeaturesId] [int] IDENTITY(1,1) NOT NULL,
	[BillingCycleId] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[LanguageCode] [nvarchar](50) NULL,
	[TermsOfServiceEnabled] [bit] NOT NULL,
	[DisclaimerEnabled] [bit] NOT NULL,
	[UseDefaultAgreements] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.MerchantFeatures] PRIMARY KEY CLUSTERED 
(
	[MerchantFeaturesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantMobileAppPricePoints]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantMobileAppPricePoints](
	[PricePointId] [int] IDENTITY(1,1) NOT NULL,
	[PricePointStart] [int] NOT NULL,
	[PricePointEnd] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[MerchantId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.MerchantMobileAppPricePoints] PRIMARY KEY CLUSTERED 
(
	[PricePointId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantOnBoardRequest]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantOnBoardRequest](
	[MerchantOnBoardRequestId] [int] IDENTITY(1,1) NOT NULL,
	[RequestFileName] [nvarchar](max) NULL,
	[MiscText] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsUpdated] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.MerchantOnBoardRequest] PRIMARY KEY CLUSTERED 
(
	[MerchantOnBoardRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantOnBoardResponse]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantOnBoardResponse](
	[MerchantOnBoardResponseId] [int] IDENTITY(1,1) NOT NULL,
	[ResponseFileName] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[DateReceived] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.MerchantOnBoardResponse] PRIMARY KEY CLUSTERED 
(
	[MerchantOnBoardResponseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantOnBoardResponseLink]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantOnBoardResponseLink](
	[MerchantOnBoardResponseLinkId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantId] [nvarchar](max) NULL,
	[MerchantOnBoardRequestId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.MerchantOnBoardResponseLink] PRIMARY KEY CLUSTERED 
(
	[MerchantOnBoardResponseLinkId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Merchants]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Merchants](
	[MerchantId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantName] [nvarchar](100) NOT NULL,
	[MerchantEmail] [nvarchar](100) NOT NULL,
	[ContactInformationId] [int] NOT NULL,
	[MerchantFeaturesId] [int] NULL,
	[CurrencyId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[CanCreateSubMerchants] [bit] NOT NULL,
	[ParentMerchantId] [int] NULL,
	[ResellerId] [int] NULL,
	[PartnerId] [int] NOT NULL,
	[EmailServerId] [int] NULL,
	[NeedAddToCT] [bit] NOT NULL,
	[NeedUpdateToCT] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Merchants] PRIMARY KEY CLUSTERED 
(
	[MerchantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantUserPricePoints]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantUserPricePoints](
	[PricePointId] [int] IDENTITY(1,1) NOT NULL,
	[PricePointStart] [int] NOT NULL,
	[PricePointEnd] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[MerchantId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.MerchantUserPricePoints] PRIMARY KEY CLUSTERED 
(
	[PricePointId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Mids]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mids](
	[MidId] [int] IDENTITY(1,1) NOT NULL,
	[MidName] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[SwitchId] [int] NOT NULL,
	[CardTypeId] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[MidsPricingId] [int] NOT NULL,
	[TransactionChargesId] [int] NOT NULL,
	[MerchantId] [int] NOT NULL,
	[SetLikeMerchantId] [nvarchar](50) NULL,
	[SetLikeTerminalId] [nvarchar](50) NULL,
	[Param_1] [nvarchar](50) NULL,
	[Param_2] [nvarchar](50) NULL,
	[Param_3] [nvarchar](max) NULL,
	[Param_4] [nvarchar](50) NULL,
	[Param_5] [nvarchar](50) NULL,
	[Param_6] [nvarchar](50) NULL,
	[Param_7] [nvarchar](50) NULL,
	[Param_8] [nvarchar](50) NULL,
	[Param_9] [nvarchar](50) NULL,
	[Param_10] [nvarchar](50) NULL,
	[Param_11] [nvarchar](50) NULL,
	[Param_12] [nvarchar](50) NULL,
	[Param_13] [nvarchar](50) NULL,
	[Param_14] [nvarchar](50) NULL,
	[Param_15] [nvarchar](50) NULL,
	[Param_16] [nvarchar](50) NULL,
	[Param_17] [nvarchar](50) NULL,
	[Param_18] [nvarchar](50) NULL,
	[Param_19] [nvarchar](50) NULL,
	[Param_20] [nvarchar](50) NULL,
	[Param_21] [nvarchar](50) NULL,
	[Param_22] [nvarchar](50) NULL,
	[Param_23] [nvarchar](50) NULL,
	[Param_24] [nvarchar](50) NULL,
	[NeedAddBulk] [bit] NOT NULL,
	[NeedUpdateBulk] [bit] NOT NULL,
	[NeedDeleteBulk] [bit] NOT NULL,
	[NeedAddTerminal] [bit] NOT NULL,
	[AcquiringBin] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[Merchant_MerchantId] [int] NULL,
 CONSTRAINT [PK_dbo.Mids] PRIMARY KEY CLUSTERED 
(
	[MidId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MidsGroups]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MidsGroups](
	[MidsGroupId] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](max) NULL,
	[MidsGroupTypeId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.MidsGroups] PRIMARY KEY CLUSTERED 
(
	[MidsGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MidsGroupTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MidsGroupTypes](
	[MidsGroupTypeId] [int] IDENTITY(1,1) NOT NULL,
	[GroupType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.MidsGroupTypes] PRIMARY KEY CLUSTERED 
(
	[MidsGroupTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MidsMerchantBranches]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MidsMerchantBranches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MidId] [int] NOT NULL,
	[MerchantBranchId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.MidsMerchantBranches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MidsMerchantBranchPOSs]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MidsMerchantBranchPOSs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MidId] [int] NOT NULL,
	[MerchantBranchPOSId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
	[MerchantBranchPOS_MerchantPOSId] [int] NULL,
 CONSTRAINT [PK_dbo.MidsMerchantBranchPOSs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MobileAppFeatures]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MobileAppFeatures](
	[MobileAppFeaturesId] [int] IDENTITY(1,1) NOT NULL,
	[SystemMode] [nvarchar](50) NULL,
	[CurrencyId] [int] NOT NULL,
	[LanguageCode] [nvarchar](50) NULL,
	[GPSEnabled] [bit] NOT NULL,
	[SMSEnabled] [bit] NOT NULL,
	[GiftAllowed] [bit] NOT NULL,
	[EmailAllowed] [bit] NOT NULL,
	[EmailLimit] [int] NOT NULL,
	[CheckForEmailDuplicates] [bit] NOT NULL,
	[BillingCyclesCheckEmail] [int] NOT NULL,
	[PrintAllowed] [bit] NOT NULL,
	[PrintLimit] [int] NOT NULL,
	[CheckForPrintDuplicates] [bit] NOT NULL,
	[BillingCyclesCheckPrint] [int] NOT NULL,
	[ReferenceNumber] [bit] NOT NULL,
	[CreditSignature] [bit] NOT NULL,
	[DebitSignature] [bit] NOT NULL,
	[ChequeSignature] [bit] NOT NULL,
	[CashSignature] [bit] NOT NULL,
	[CreditTransaction] [bit] NOT NULL,
	[DebitTransaction] [bit] NOT NULL,
	[ChequeTransaction] [bit] NOT NULL,
	[CashTransaction] [bit] NOT NULL,
	[BalanceInquiry] [bit] NOT NULL,
	[BillsPayment] [bit] NOT NULL,
	[ProofId] [bit] NOT NULL,
	[ChequeType] [bit] NOT NULL,
	[DebitRefund] [bit] NOT NULL,
	[TOSRequired] [bit] NOT NULL,
	[DisclaimerRequired] [bit] NOT NULL,
	[TipsEnabled] [bit] NOT NULL,
	[Amount1] [decimal](18, 2) NOT NULL,
	[Amount2] [decimal](18, 2) NOT NULL,
	[Amount3] [decimal](18, 2) NOT NULL,
	[Percentage1] [decimal](18, 2) NOT NULL,
	[Percentage2] [decimal](18, 2) NOT NULL,
	[Percentage3] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_dbo.MobileAppFeatures] PRIMARY KEY CLUSTERED 
(
	[MobileAppFeaturesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MobileAppLogs]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MobileAppLogs](
	[MobileAppLogId] [int] IDENTITY(1,1) NOT NULL,
	[MobileAppId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[LogDetails] [nvarchar](500) NULL,
	[Notes] [nvarchar](500) NULL,
	[DateLogged] [datetime] NOT NULL,
	[GPSLat] [decimal](18, 2) NOT NULL,
	[GPSLong] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_dbo.MobileAppLogs] PRIMARY KEY CLUSTERED 
(
	[MobileAppLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MobileApps]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MobileApps](
	[MobileAppId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantBranchPOSId] [int] NOT NULL,
	[MobileDeviceInfoId] [int] NULL,
	[MobileAppFeaturesId] [int] NULL,
	[ActivationCode] [nvarchar](30) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateActivated] [datetime] NULL,
	[ExpirationDate] [datetime] NOT NULL,
	[UpdatePending] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TOS_Acknowledged] [bit] NOT NULL,
	[GPSLat] [decimal](18, 2) NOT NULL,
	[GPSLong] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_dbo.MobileApps] PRIMARY KEY CLUSTERED 
(
	[MobileAppId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MobileAppTokenLogs]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MobileAppTokenLogs](
	[MobileAppTokenLogId] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NOT NULL,
	[MobileAppId] [int] NOT NULL,
	[RequestToken] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[ExpirationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.MobileAppTokenLogs] PRIMARY KEY CLUSTERED 
(
	[MobileAppTokenLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MobileDeviceInfo]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MobileDeviceInfo](
	[MobileDeviceInfoId] [int] IDENTITY(1,1) NOT NULL,
	[Platform] [nvarchar](50) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[Manufacturer] [nvarchar](50) NULL,
	[Model] [nvarchar](50) NULL,
	[OS] [nvarchar](50) NULL,
	[IMEI] [nvarchar](50) NULL,
	[IP] [nvarchar](50) NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.MobileDeviceInfo] PRIMARY KEY CLUSTERED 
(
	[MobileDeviceInfoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParentTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParentTypes](
	[ParentTypeId] [int] IDENTITY(1,1) NOT NULL,
	[ParentTypeName] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.ParentTypes] PRIMARY KEY CLUSTERED 
(
	[ParentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Partners]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Partners](
	[PartnerId] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](100) NOT NULL,
	[LogoUrl] [nvarchar](max) NULL,
	[CompanyEmail] [nvarchar](100) NOT NULL,
	[MerchantDiscountRate] [decimal](18, 2) NOT NULL,
	[ContactInformationId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[CanCreateSubPartners] [bit] NOT NULL,
	[ParentPartnerId] [int] NULL,
 CONSTRAINT [PK_dbo.Partners] PRIMARY KEY CLUSTERED 
(
	[PartnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentAccountTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentAccountTypes](
	[PaymentAccountTypeId] [int] IDENTITY(1,1) NOT NULL,
	[AccountType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.PaymentAccountTypes] PRIMARY KEY CLUSTERED 
(
	[PaymentAccountTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentTypes](
	[PaymentTypeId] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.PaymentTypes] PRIMARY KEY CLUSTERED 
(
	[PaymentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[POSTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[POSTypes](
	[POSTypeId] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_dbo.POSTypes] PRIMARY KEY CLUSTERED 
(
	[POSTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Provinces]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Provinces](
	[ProvinceId] [int] IDENTITY(1,1) NOT NULL,
	[ProvinceName] [nvarchar](100) NOT NULL,
	[IsoCode] [nvarchar](max) NULL,
	[CountryId] [int] NULL,
 CONSTRAINT [PK_dbo.Provinces] PRIMARY KEY CLUSTERED 
(
	[ProvinceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequestedMerchants]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestedMerchants](
	[RequestedMerchantId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantName] [nvarchar](100) NOT NULL,
	[MerchantEmail] [nvarchar](100) NOT NULL,
	[ParentId] [int] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](max) NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Address] [nvarchar](100) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[StateProvince] [nvarchar](max) NULL,
	[CountryId] [int] NOT NULL,
	[ZipCode] [nvarchar](10) NOT NULL,
	[PrimaryContactNumber] [nvarchar](30) NOT NULL,
	[Fax] [nvarchar](30) NULL,
	[MobileNumber] [nvarchar](30) NULL,
	[ProvinceId] [int] NOT NULL,
	[ProvIsoCode] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[MID] [nvarchar](max) NULL,
	[CardTypeId] [int] NOT NULL,
	[Currency] [nvarchar](max) NULL,
	[SecureHash] [nvarchar](max) NULL,
	[AccessCode] [nvarchar](max) NULL,
	[Username] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[RequestNote] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.RequestedMerchants] PRIMARY KEY CLUSTERED 
(
	[RequestedMerchantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Resellers]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resellers](
	[ResellerId] [int] IDENTITY(1,1) NOT NULL,
	[ResellerName] [nvarchar](100) NOT NULL,
	[ResellerEmail] [nvarchar](100) NOT NULL,
	[ContactInformationId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[CanCreateSubResellers] [bit] NOT NULL,
	[PartnerId] [int] NOT NULL,
	[ParentResellerId] [int] NULL,
 CONSTRAINT [PK_dbo.Resellers] PRIMARY KEY CLUSTERED 
(
	[ResellerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwitchAPITypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwitchAPITypes](
	[SwitchAPITypeId] [int] IDENTITY(1,1) NOT NULL,
	[APIName] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.SwitchAPITypes] PRIMARY KEY CLUSTERED 
(
	[SwitchAPITypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Switches]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Switches](
	[SwitchId] [int] IDENTITY(1,1) NOT NULL,
	[SwitchName] [nvarchar](100) NOT NULL,
	[SwitchCode] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsAddressRequired] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Switches] PRIMARY KEY CLUSTERED 
(
	[SwitchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwitchParameters]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwitchParameters](
	[SwitchParameterId] [int] IDENTITY(1,1) NOT NULL,
	[SwitchId] [int] NOT NULL,
	[ParameterId] [int] NOT NULL,
	[ParameterName] [nvarchar](100) NOT NULL,
	[ParameterTypeId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SwitchParameters] PRIMARY KEY CLUSTERED 
(
	[SwitchParameterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwitchParameterTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwitchParameterTypes](
	[SwitchParameterTypeId] [int] IDENTITY(1,1) NOT NULL,
	[ParameterType] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.SwitchParameterTypes] PRIMARY KEY CLUSTERED 
(
	[SwitchParameterTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwitchPartnerLink]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwitchPartnerLink](
	[SwitchPartnerLinkId] [int] IDENTITY(1,1) NOT NULL,
	[SwitchId] [int] NOT NULL,
	[PartnerId] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[Switch_SwitchId] [int] NULL,
 CONSTRAINT [PK_dbo.SwitchPartnerLink] PRIMARY KEY CLUSTERED 
(
	[SwitchPartnerLinkId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaxTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxTypes](
	[TaxTypeId] [int] IDENTITY(1,1) NOT NULL,
	[TaxTypeName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.TaxTypes] PRIMARY KEY CLUSTERED 
(
	[TaxTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TempSystemTraceNumber]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempSystemTraceNumber](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TraceNumber] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.TempSystemTraceNumber] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TempTransactions]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempTransactions](
	[TransactionId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantPOSId] [int] NOT NULL,
	[TransactionEntryTypeId] [int] NOT NULL,
	[CardTypeId] [int] NOT NULL,
	[CardNumber] [nvarchar](max) NULL,
	[NameOnCard] [nvarchar](max) NULL,
	[ExpYear] [nvarchar](max) NULL,
	[ExpMonth] [nvarchar](max) NULL,
	[CSC] [nvarchar](max) NULL,
	[Track1] [nvarchar](max) NULL,
	[Track2] [nvarchar](max) NULL,
	[Track3] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[RefNumSales] [nvarchar](max) NULL,
	[RefNumApp] [nvarchar](max) NULL,
	[OriginalAmount] [decimal](18, 2) NOT NULL,
	[TaxAmount] [decimal](18, 2) NOT NULL,
	[FinalAmount] [decimal](18, 2) NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[KeyId] [int] NOT NULL,
	[MidId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.TempTransactions] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionAttemptCash]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionAttemptCash](
	[TransactionAttemptCashId] [int] IDENTITY(1,1) NOT NULL,
	[TransactionTypeId] [int] NOT NULL,
	[TransactionCashId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[MobileAppId] [int] NOT NULL,
	[TransactionChargesId] [int] NOT NULL,
	[DateSent] [datetime] NOT NULL,
	[DateReceived] [datetime] NOT NULL,
	[DepositDate] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[ReturnCode] [nvarchar](50) NULL,
	[AuthNumber] [nvarchar](50) NULL,
	[TransNumber] [nvarchar](50) NULL,
	[BatchNumber] [nvarchar](50) NULL,
	[SeqNumber] [nvarchar](50) NULL,
	[DisplayReceipt] [nvarchar](50) NULL,
	[DisplayTerminal] [nvarchar](50) NULL,
	[Notes] [nvarchar](500) NULL,
	[GPSLat] [decimal](18, 2) NOT NULL,
	[GPSLong] [decimal](18, 2) NOT NULL,
	[Reference] [nvarchar](max) NULL,
	[TransactionSignatureId] [int] NULL,
	[PosEntryMode] [int] NULL,
	[TransactionVoidReasonId] [int] NULL,
	[TransactionVoidNote] [nvarchar](max) NULL,
	[TransactionCash_TransactionCashId] [int] NULL,
 CONSTRAINT [PK_dbo.TransactionAttemptCash] PRIMARY KEY CLUSTERED 
(
	[TransactionAttemptCashId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionAttemptDebit]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionAttemptDebit](
	[TransactionAttemptDebitId] [int] IDENTITY(1,1) NOT NULL,
	[TransactionTypeId] [int] NOT NULL,
	[TransactionDebitId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[MobileAppId] [int] NOT NULL,
	[TransactionChargesId] [int] NOT NULL,
	[DateSent] [datetime] NOT NULL,
	[DateReceived] [datetime] NOT NULL,
	[DepositDate] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[ReturnCode] [nvarchar](50) NULL,
	[AuthNumber] [nvarchar](50) NULL,
	[TraceNumber] [nvarchar](50) NULL,
	[RetrievalRefNumber] [nvarchar](50) NULL,
	[BatchNumber] [nvarchar](50) NULL,
	[InvoiceNumber] [nvarchar](50) NULL,
	[SeqNumber] [nvarchar](50) NULL,
	[ReferenceNumber] [nvarchar](50) NULL,
	[DisplayReceipt] [nvarchar](50) NULL,
	[DisplayTerminal] [nvarchar](50) NULL,
	[Notes] [nvarchar](500) NULL,
	[GPSLat] [decimal](18, 2) NOT NULL,
	[GPSLong] [decimal](18, 2) NOT NULL,
	[TransactionSignatureId] [int] NULL,
	[TransactionDebit_TransactionDebitId] [int] NULL,
 CONSTRAINT [PK_dbo.TransactionAttemptDebit] PRIMARY KEY CLUSTERED 
(
	[TransactionAttemptDebitId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionAttempts]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionAttempts](
	[TransactionAttemptId] [int] IDENTITY(1,1) NOT NULL,
	[TransactionTypeId] [int] NOT NULL,
	[TransactionId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[MobileAppId] [int] NOT NULL,
	[TransactionChargesId] [int] NOT NULL,
	[DateSent] [datetime] NOT NULL,
	[DateReceived] [datetime] NOT NULL,
	[DepositDate] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[ReturnCode] [nvarchar](50) NULL,
	[AuthNumber] [nvarchar](50) NULL,
	[TransNumber] [nvarchar](50) NULL,
	[BatchNumber] [nvarchar](50) NULL,
	[SeqNumber] [nvarchar](50) NULL,
	[DisplayReceipt] [nvarchar](50) NULL,
	[DisplayTerminal] [nvarchar](50) NULL,
	[Notes] [nvarchar](500) NULL,
	[GPSLat] [decimal](18, 2) NOT NULL,
	[GPSLong] [decimal](18, 2) NOT NULL,
	[Reference] [nvarchar](max) NULL,
	[TransactionSignatureId] [int] NULL,
	[PosEntryMode] [int] NULL,
	[TransactionVoidReasonId] [int] NULL,
	[TransactionVoidNote] [nvarchar](max) NULL,
	[Transaction_TransactionId] [int] NULL,
 CONSTRAINT [PK_dbo.TransactionAttempts] PRIMARY KEY CLUSTERED 
(
	[TransactionAttemptId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionCash]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionCash](
	[TransactionCashId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantPOSId] [int] NOT NULL,
	[TransactionEntryTypeId] [int] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[RefNumSales] [nvarchar](max) NULL,
	[RefNumApp] [nvarchar](max) NULL,
	[OriginalAmount] [decimal](18, 2) NOT NULL,
	[TaxAmount] [decimal](18, 2) NOT NULL,
	[FinalAmount] [decimal](18, 2) NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[MidId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.TransactionCash] PRIMARY KEY CLUSTERED 
(
	[TransactionCashId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionCharges]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionCharges](
	[TransactionChargeId] [int] IDENTITY(1,1) NOT NULL,
	[DiscountRate] [decimal](18, 2) NOT NULL,
	[CardNotPresent] [decimal](18, 2) NOT NULL,
	[eCommerce] [decimal](18, 2) NOT NULL,
	[PreAuth] [decimal](18, 2) NOT NULL,
	[Capture] [decimal](18, 2) NOT NULL,
	[Purchased] [decimal](18, 2) NOT NULL,
	[Declined] [decimal](18, 2) NOT NULL,
	[Refund] [decimal](18, 2) NOT NULL,
	[Void] [decimal](18, 2) NOT NULL,
	[CashBack] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_dbo.TransactionCharges] PRIMARY KEY CLUSTERED 
(
	[TransactionChargeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionDebit]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionDebit](
	[TransactionDebitId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantPOSId] [int] NOT NULL,
	[TransactionEntryTypeId] [int] NOT NULL,
	[AccountTypeId] [int] NOT NULL,
	[CardNumber] [nvarchar](max) NULL,
	[NameOnCard] [nvarchar](max) NULL,
	[ExpYear] [nvarchar](max) NULL,
	[ExpMonth] [nvarchar](max) NULL,
	[EPB] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[RefNumSales] [nvarchar](max) NULL,
	[RefNumApp] [nvarchar](max) NULL,
	[OriginalAmount] [decimal](18, 2) NOT NULL,
	[TaxAmount] [decimal](18, 2) NOT NULL,
	[FinalAmount] [decimal](18, 2) NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[KeyId] [int] NOT NULL,
	[MidId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.TransactionDebit] PRIMARY KEY CLUSTERED 
(
	[TransactionDebitId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionEntryTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionEntryTypes](
	[TransactionEntryTypeId] [int] IDENTITY(1,1) NOT NULL,
	[EntryType] [nvarchar](100) NULL,
 CONSTRAINT [PK_dbo.TransactionEntryTypes] PRIMARY KEY CLUSTERED 
(
	[TransactionEntryTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[TransactionId] [int] IDENTITY(1,1) NOT NULL,
	[MerchantPOSId] [int] NOT NULL,
	[TransactionEntryTypeId] [int] NOT NULL,
	[CardTypeId] [int] NOT NULL,
	[CardNumber] [nvarchar](max) NULL,
	[NameOnCard] [nvarchar](max) NULL,
	[ExpYear] [nvarchar](max) NULL,
	[ExpMonth] [nvarchar](max) NULL,
	[CSC] [nvarchar](max) NULL,
	[Track1] [nvarchar](max) NULL,
	[Track2] [nvarchar](max) NULL,
	[Track3] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[RefNumSales] [nvarchar](max) NULL,
	[RefNumApp] [nvarchar](max) NULL,
	[OriginalAmount] [decimal](18, 2) NOT NULL,
	[TaxAmount] [decimal](18, 2) NOT NULL,
	[FinalAmount] [decimal](18, 2) NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[KeyId] [int] NOT NULL,
	[MidId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Transactions] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionSignature]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionSignature](
	[TransactionSignatureId] [int] IDENTITY(1,1) NOT NULL,
	[Path] [nvarchar](max) NULL,
	[FileName] [nvarchar](max) NULL,
	[FileSize] [int] NOT NULL,
	[FileData] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.TransactionSignature] PRIMARY KEY CLUSTERED 
(
	[TransactionSignatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionTypes]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionTypes](
	[TransactionTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
 CONSTRAINT [PK_dbo.TransactionTypes] PRIMARY KEY CLUSTERED 
(
	[TransactionTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionVoidReason]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionVoidReason](
	[TransactionVoidReasonId] [int] IDENTITY(1,1) NOT NULL,
	[VoidReason] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.TransactionVoidReason] PRIMARY KEY CLUSTERED 
(
	[TransactionVoidReasonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 7/31/2019 7:40:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[EmailAddress] [nvarchar](100) NOT NULL,
	[PhotoUrl] [nvarchar](500) NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](max) NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[ContactInformationId] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__MigrationHistory] ([MigrationId], [ContextKey], [Model], [ProductVersion]) VALUES (N'201907221337208_InitialCreate', N'SDGDAL.DataContext', 0x1F8B0800000000000400ECBDDB72E4B8922DF83E66F30F69F93473AC4F6529AB77F7DEDBAAE6986E59A5DD522A2C43993DA75F64540425B1154146910CA5D463F365F3309F34BF3000AF20E0B85FC850D2CA2C2B44B83B1C8E05C0E1B8FD7FFFCFFFFBEBFF78D96EDE3DC7799164E96FEF8F7EFAF9FDBB385D65EB247DF8EDFDBEBCFFEF7F7DFF3FFE8FFFF57FF9F57CBD7D79F7ADA5FB05D321CEB4F8EDFD6359EEFEFEE143B17A8CB751F1D33659E55991DD973FADB2ED87689D7DF8F8F3CF7FFB7074F4214622DE2359EFDEFDFA659F96C936AEFE407F9E66E92ADE95FB687395ADE34DD17C4729CB4AEABBCFD1362E76D12AFEEDFDF2ECF7B3E3CBF7EF8E374984725FC69BFBF7EFA234CDCAA844BAFDFD6B112FCB3C4B1F963BF421DADCBCEE6244771F6D8AB8D1F9EF3DB9AAFA3F7FC4EA7FE8195B51AB7D51665B4D8147BF34F6F840B31B59F57D672F64B17364D9F21597BAB2DA6FEF8F57AB0C99FBFD3B3AAFBF9F6E724CD7DAF4A78A35898B9F1A967F7A5727FC5357F1081FF8BF7F7A77BADF94FB3CFE2D8DF7651E6DFEE9DD627FB74956FF16BFDE644F71FA5BBADF6C48B59062286DF0017D5AE4D92ECECBD72FF1FD50D98BF5FB771F86EC1F68FE8E9B65AD4B759196BF7C7CFFEE335225BADBC41D06080B2CCB2C8F7F8FD3388FCA78BD88CA32CE51155EACE3CA8A8C125496086B798A7EB53922E0A176F3FEDD55F47219A70FE5E36FEFFF821ACAA7E4255EB71F1A25BEA6096A6588A7CCF731A0A438E3455414DFB37C1D3EE38BCF2314368F55EA55450A96602BE94BB6B19671511CAFCAE4B943CE49868446A9819CB3781323ECDA0ABACC1E6E72D4F62D8BB5385EAFF3B8280420413F955022CEA969EDC7CF51522978861A709B29FE7D936CE5658E8A1295FB215E5FA4BABC6D0B3C7FD92579D599C01A681A10339EE67144D4A8B12CDC3BD9E2F4731CAF8BAFBB35513665847D8E9E9387CA3440037AFFEE4BBCA9128BC764578FE3EDB8735B137CCAB32DFED55576F5FD7699EDF315D62603126FA2FC212ED555C13612AA5213B0AAE0EF5C55AA4448955F3FF4C3B370D0AE2DA03E6263FA1186EBB62BD41DABD5BA50770335CEEFB378A0FEE865EC326FCFCA50A911AA0E154C3F0254DADE48172A6ABD983BA89C6FD188221FC58E7EF6E3EB3C6665F635DF089D2CB59CC5197D4AF2A29434094F65BC4AD66B697374E325E0117E9C42A2296D19ADCA8BF43ECBB71530AD1DD83C59F50E46BC4AB61182C92247BF9AC9FB5FDFBF5BAE222C50413A774864350707C86A8C8368FBB19243C20C9B3C3AAB1114524DBD9364B947E832610CE976A0264874D79D8ED6939E56CA85CE7589AC16A36FCF49BAF2DFBDE18C2E8AEC143178CFEB147BB6F9AB6D2FF61FC94EA2EE919F813547FD65FEDA3486CFFBED1DF69AB84AFCE245894FB8266CF3940CAED95D82065747E51B6172D8E00C1C738031A223EF871D3E1533F208482D079F462B9D11A762196598E95AB6FED8A2D829B81B509A2C255E9D622452292B497FF517073959857D644DE96251705A53930C37222AB16B107DDBA129DAD665D3642E16068DE6623162B359D8349CC5084DE76281ABD96E18EF44DD646E04E9BA1586E38716E2D9D182D7268C10DF440BF11783C5399C38DE025DBB8E62B848A7B60CE370165267EB21E6A051DBB82097D9835E5D374CA3D47493B7593D13CCA16AB96E8DB633945ACA0DE608BED4599B2D78B6677119251BFF8B76F562DBC9AB8B9529EC1FD5F27C05D68F1FF238DE22E8155A4DB6E31AA3CD76991B35DA0177B0561BE7DBE2FA7E19E7CF498098C95952AC36118289687AEC2AFA9C3EECA387583681703157B9284E2B88DAEE03B88AF3D563046CB750F5BF5A01A003D626DE926DAB77C1806466DA01D1584D3C7A85D51B79CB334213272B48B781732BD763F36EF31C69ADA9C9BD5AD57B0BAB406D813EC5118617DB534B14DAE7799CAED8F9D6816D5772B94FE6344A6B51CBFD5D6BDEC256C17ABB996A6F4A6F57888B78B3017C24699E656AEF59556D050FC7DA0AE080F0F17A7D939DDED8DA0F8BAA43CB46D2B8C31339F0B819A0E82881681073BC06DB65255E871590F1B5575C8F1596A2E96C24BA775490C64DA240CF964257BB0AE6450573B18203424047229DAF2649A4AB692BE4248FD2D5632C412E4B0DE84C13F11567284DB56F472C35ED7B6A81F62D915CFB8E525BFB642DD3B8A280B4440902CD70AAB636D5EADEF16E576D06596489AC1F03183A9B404A8BE8B9AEB89009F2CD45451C0E9C62D3D3B440250C49F8D541D1E9564C33F44AD5AD89603D719A50C18A4057B3D69700556B13BBEA1A20994D65200090E856F8D0EB725BDDDCC92307163ADB86559BE09056D8FA38A4DC62F0E8ED16E2BAF158631DAEE11963198E98D168AFC2F166433E17E19A3C47D905D3663ECE643CD456A58BE23CC56A69CE32955B083141D2692404DB08ED849AD5E93615D1A4D0F76EEC3ADB71303B52E0E88FAC28FD6F1FCCF2D22E54A07000D291AAF2038F6E32C2C7879797D6E1A9F83E42EDF8348F2B9C471B6B810EE35D930BE6398A5B5D148DA0C6FA83A6EB780818CE8A4D960E6ACE1117106A056C96117A09A11713EA9CDFCAC98EC9354897BD8DFA9A93A7E0688D15C5102997983B31E7739806CA544A239A45522432CD8D631E2DE3E27A299E01D71921B25BBADBE429CFD272E7C05C06DD393D3FD287129828A8A0247272B6300A3C4E16B93B6B990F598879C4510BE56E336435ECA1C72B94AD9B0DEBF3C862EE86982DB7F8EFD9C4FDB3A02B74B68641E4C459CC1852A8A86CB4BCC174833A630B875956189647B17800A3F17A0E5CC636F516E8BE8962F1C9D88146406B37C070ACAF31CA801246186A4CC697A0834AB276B58BA8B3F33C8591C8AAF7A2B83C9D05B46925E7B3EA68C41D82220FDBCFA9321A74ECBCD2B1D20BAA2C0005E4404364B65D9A66FF35865F5C7706DAFEB04A1FE2B4CB9AFDDF6AB1FB7B521AF8AACC8EC57CAD7684CA7037A8FE8D2E055E8746956A7DFA0635DE22AA8EC19C3EE246AE7016C153008AAEBAB8BC4C9E62569CAF2DFD4D86F87C4492461BFF192EA23CDADE1E85C9E663986C7E09B03485F3F9E730C5F94B986CFE254C36FF1A269BBF86C9E66F81DAE7CF81F209D4111C05EA098E445D81CB7C0275054781FA82A3409DC151A0DEE02850777014A83FF818A83FF818CA3108D41F7C0CD41F7CF4DE1F34C76E4EF69B2777C76E5C49AB274EAEA4E1D3458D3F6C2BEE78F5E73EC1157292884EDC3BBA3D4D7C1B9DD32BDADCE7C35F8A6F26A2DCE04E4F300CE9B4DFA110549FE8F6C011960C9D3522BEC3EA989E30122FE813112B5EC00B54C77ABD5E7C8C085C89C69130D90A37524D6D591B116AAFCD2B2E0A9986134576B658DEA9A33D5C55DBE4A17AF55750A52649570D36A6C25509221DAAC75280AA02647637EA751D89C6A98486678C530944844EFB54827274CFE1251C289FB77428C062DD32B65CB58CE735CB006B96F382E5C80B96A1B6CAC9D728F9DBEB1C2C4F9A3B436A8A135E9351FFD5FA10EA3D56CD31421FD5AF7DE9F654AAAB66EEFAAB3AC77146C43A6FE9F9C0892F983617BF7F89F12C7BE4EE51E2A037C7462E93F409F68C5B971CA0EE3B022E11D313F0291D7405843CFD5E81601EAD83207430EF2B282161BB0D074FD5B9390F65740C56D45E44D71C308687EE3BE012711A0944E97016CE66C2CEC979340A1A0BE6EBCAADBA33A27A5B6E584668C1047275DBAD32E85DDE87BDDD45E94887E12FB307C9434FAE82C25521473AC7DC3ABCF8D64D1C9DFE423E9268FD86D17C32CF669648DE7BD7B43E47D7DEF1DAB2E3137EED80203EDAC7A7627A7001A9C18540BD2184DA5394ACE20302AECE432AD35B8260FFB7BBE507D0924EE35F1144974079FC26E1E9CE90B49E62731B8DDDFDDD4B1A2F79363C63BCE6495C3BA93B7C73AFACF4387EB7798E3380B7B9BF99AB657F98C18EE8ECAC473B1713B5BAC751BDF4D576D4ECFA63F1B0292063C62011ADE92AB5642452BDB08EAB2A7BA79DDED0AE76AF1E4D0B8F9B04095F618ACEE50D855AE3BB4841B3F9F9A045BAB5A6C81981AC6E34CC438BE6EA033ECB3DC2D0CF2861E203804242390361669851BEFE9C95485C11A7A5B76C623469DFC6B9CBE77EE9DE208F8FF7E5A34743ED304EFDE9BFC7BD77413806AE7340023749EA3103D4F8F6A93FF1DFB2C49FF0D3A8783C89564F8E3250DFADD19F97D7D8A2D1328D713CB3CDDBE89026C91C6CA786EBA3E17529CE62FCBA0F760C35DF14E88C60F8E24535A7A92C235951F5F3E4AFEB6D1D4D71206962E6F3975D92577638234748535DEAED258B385D5776B45E2D9ED6CCF3E67A797BBC7A4AB3EF9B78FD602FEFF7C5F232F2E72F60F1595F0DF6F26D2F2DB0BFB884D93DA370C9897221E82E455608961E2C044D262A04436B5C88CBEC41520044714B8C9C80EE030AFEA5314332ED4DDCD440A06475929C6FF49E4A6A7382D46A5D160085814BD2328FE99A9003ABB18BC21D9D7DEEB6782DCA787B15E0753D571725847C11100D0AA6FB3F683B5FB992F47B725F1E6F36D9777B5155D8DDA5ACCB649B585E877DFA18AF9E3E65F592C0D97E879A2902B67598F924D9A0A9E8C3E9EB6A1317551E833507C3A033429DABBAA86439B45F25CFA7FDAA0CEC74451D698CFB84F8F37E7BD73F266AAA23BE2A3C2997C9431A918113F30BC8EF1C4A4336FB731FBB1317158FEE84558623428F4E4CE7505E6D3C970291F91C8A3B8936C891461E11DEA8FBEAA2AD158BE8754BC447CDBB952CBBBFB0EE9D9A0AA80E8F3980C6306E6731C374B635BA7BD1D895C49B6467BC77940EBF6C7150BEBB3AC0F9D4B796FFD1B3FC5FBCC95FE0307F5A229FD09F8DFA3CFCD9A9CFC3A1AD0C0F7A33736BF0D837978A3F856449DD4C21D1BCDA68F688F8C69C38A2ECADE68C0D7FB0C8B67A385D1652AE561AAD2783D9C3595CE2674185534127378464841BED2D171C4C46857A7010EA7E3B0153B51B9E754374DC2E0A8EE459744F64004EB78BEA7947EBA686EB4E665D9560EDCA6377B54068C1DBA87CC789168F594A4D627D657515A5FB7B346B41D5ED3F2F4429DA96E92413BC9EE137878BABF30BEF792C7CE760BE0AAAFDC28951389EE21DF17D13AB603C2023546F45C6D8A6722D71C8683BBE95ACB8BEC74F55A2B1C2D1E4B99FD93B12F8B5889BC7F18E1FF238C6411AFDE82AD7D72131207CB4A39B470D39D8973A4042EEF31C30B5DB4BC4E8ACC0A9258F48AEB99B89E5D0ACEAFD20C937421F48F721BAFD9F5E1FE4F01C29CE908C2F7ABADF5E7B2CEC2602FD23E126C32220668CB3C55DE646C78B07DCC17CF82ED76519D93EB6DB0B3B4F2D47C64A94B7D9B5FB271F959E4604602A7C275144CFEDA9854C76D3ED460A7E53D9AEBD0E25CC4D756EAA536BAA43840A5B298794DB4079F4566DF3385DE759B246CDBE69433ACD92611EA145323A98344C5048A8F68972C53F042E96ABE73C564F68CE1824AFC68C21F33AD9271BD143324E332327DE66BDDF595CACF26447EE73F0A6B4CBADFA663BDAD5277891E6A57F15C31853BAC8F0CABF8E3158000B67280D83BBEA63AAFD313897A9BC6EE56C71B40F887836E20DAAF84DBD3DC39B7B54E5714A666166940A5C66E76ECC1CAD2A43D0AF1AA6306E1495ECF641DB4638B4CE4925F1F412AE6C8A14239A9C40B50115AD1C91C8518FA4B0BCF48DD043E7E2B78E6D94CBDF06BD9AF6B44FA7530C7AF3B99FB729C7773720394607E894717D76B78CCB12D9576B79AEE71A01D57DE626A01E7287C27493A7877BF5B9797D8B36FB60991DC6AC40BD5554FB4BB45A44C531466BA8B7C298B4848E33D87EBFA8400CAAF9CA809727D1463A3DF074D7D71B1A2A2041279B68F57499140E8421145EA4FF19AF902887214D0248A0F35827DD0EE97AF71148661C4888C66E3961A08CC62202C137C63E18AAD16AEF81D16AF40EAF1FAA72FCB4C9BEBB98D5D7D2DC499238B8BE2EFF55DBEEE729F77F8FF21C4D614581083F577E9CBF60E8441BD4454713764FA6383470BBE0BE41C0EB4A64B749D212EB493009BB8EC4A1D30D38B45D815CDF9E92A36D4B20D6B5A3B21A3648E3E9FAA323C51D869DA5995F3AC1A8838BCE49B3DA7B24EA567DCB395AF59323AF1904D4C76E7730E80D3E8D8D63B525DAE8AFEE1B322CF7687020953087042D25D8CCD5D1A28E9BB9EF7151240F69BC76726F9837DF40303123ABF196373D038838933488D2E9D3C04036D04A8E804C4573373B636AC1FD5925DDDEA2E51CADA7681530EF254809C176AB393B1CF6E386A274317EBD341B101BC6F1105EE76F01F05EC021C66EDB8ECEC15DA6531F0A1523954D854A0296101567600149EDDEA7E7DD2D496528BA5F52422A2B8BD23D93CA9DC8F9F6F9781BBF2CA21CFD59EABD0943F38ED095D02A98F426908C80DB47ABDBC4F0AED533EF5139941B026B5CDDCF7491AE71C699FF6D6528D7E66A8A2EF31079B6BB58C3EC9DC3072D9334DA1C57E56B4E3306CF354D22FF4FF70D33BD4EF17DF4DE334552BAFB5F3D172F7A58D4B706F9CFEA318F8BC76CB30EB34DA17906671317455B87A1800A661D04AD40CE8120DB66F725299EAEA2347A88B72140851F8FC99FE3F5A73D1E71BF16FE0BEA6AC214604F0B1AEBAB17599EE3DCD4E561F8C7717B18350C5D1F50CEECFECCEECFECFEA8643ABB3FB3FB33BB3FA27632BB3F93727FFEC88AD2D4F519F08EE3F60C543074791819C1B657E0EBB1114077595A54F5EA7F603E4BAA91AA0691E7BC1679B28DF257B59BFB1CB5BDC5F17A8D4611D12DA2AEF6F8AFB2741DB8785DA6E1CAD9C69725DB80DC6616BA70A7A8B1A2A4E770CE229A7A64F7E5F7288FBF05C00DEEE1FAB1C37366E72FA893FF12AFE26457A2D9E46E13F93F65DBCE5A4F2F4F9FB783273EFC5D1E901451C0ECEA352BFCB26BC84C9BF6516D380B3643AE5DB5A360A039FF76F56993657920DC5C2C8FCFAFBE05CCB1874ED87CF12ADA3F4E4FC2665A83E7A37F4FE36CF1A5BA167DB9BF5B55CF4887C8123FAA7217AD9EC2657A7A1ABA98A7A7E14B4995310C549962861BB8EA8797F6BB5D9607EB07D0B8F5ED2A4C11D1D40D4DC956A7978D1BF225FE334CC60BFCCC53B4690E0007747B420E6095E333CAF81534DB0EB601F3EC2BF4E6250D599DC1B2232B3358A65D5506CBB19DE855219EFA598A40395E57170E2CCB2894AFF58BFF5631FA6E5E9D886A33F898065569F671E2AAB41686A15548CCBC983C2F26CF8BC92A99CE8BC9F362F2BC982C6A27F362F2A41693FFB1BA33757B48D6715C1E52034377871631BB3AB3AB33BB3A2A99CEAECEECEACCAE8EA89DCCAECEA45C9D3A8EB98AF2B5A9C7034818C7F1011431F47F3892663768768366374825D3D90D9ADDA0D90D12B593D90D9A941BD4568BA913C4F08FE302316A183A40A09C60C7099ACC4FA35D74976C2A1BFBF70DD6EB3ADF4D54E51B28DBAEACD813CA5F4FB3006F08746BFAAFC2AB101D6586370EE0578AC214ADCDEDFC6597A54146EBFAECCB7DD82236198E54CAF4B97D9631D0A835EFCC180C5678AB93E94035E01D67901AA86038403132E6B9F93C379FE7E62A99CE73F3796E3ECFCD45ED649E9B4F686E9EE7597E993D68B9390DCF18EE4D93B5915B43F0867267AEE2F23113F9D7AACFC54846779913E1261B64C19BF8C57F8B5DA20EEA09CD8956019A6A8AAAF6FC651587797F6F985DC07206E84AFE88A3759CB7F754E8742843CE11BA95A102269D0B2B217438EFC2FF345E7A098A945D1EF8948A50593C960A11CF6FA5ECC7C28B7FA5ECD2E30E5209E40E42F6315629BBFCF63EA94B855AE2157221A330A76EF7791A24FAD714E9741305B91BE6CF7D9CAE42DD0A5305198304A517D7CB731C6EBF0A5865DF02C54C51D96AF407C123CA0D1FD10B179B1A3F52FB4E70A93E7BED3C70A7FE7028BE6579FA3BF525A4CC9DFA327ADD4702E07B6595CA04F089CAC5902B948DE531281F75719C52D9281E51B906A40A651AD21B94873DB6A95424964D542A9A5AA1600C8B41D986E73294CA3564119589A45428CF80DCA02CE0C64BA522819CA292010C0A0584B80CCA09ECAC502A25C0272A2343AE504296C7A07CD4629C52D9281E51B906A40A651AD25BBDA142BCEEAD335926D8469829534F92EB4E93B55E3477374746D99E5557F7886E0BF3F3C45897B5E8AE293F59D77E9CC1EB4ECA10BE8CD2877D350554C76FCB330278DBAC4D904BF286826D9BA764AAE1073D6DE6F8AFE099070857B697165EA727191A84BFE0C97851EA2019963002AE61454C50CE97140AF34D969F928D0C798EC2076892136451C3EDCCFAEB6E1DF97B239281817E4C9F23620AADC33CCA2F1015AE7DD479066B202E17B3AB7BF69E03F6E9B5AD745F3A1588990E7E4D5F4195880BB7324EBF0AEEFD7A62DD414DD62EDC3F78CA71498049AFA0166F7962FA79B03E373335361061355BBE4AD6C5EF79B6DF6935E496698C66DBE66DD44849E6504DB2CA3090D3D5140FE763DF10477D785BD8A0C972C2EDB8A5B8A56889C60A93B02D9243E7A6D9D55A19343D9C3C66F36B2166DC04D530EAB8194AD6695DCCBAD53190DD2167F378B7AB6A4473B320C33C0616681D8CF0000909762CA23E2F60FD787C5B065B41CD905E99E2A066CEE72FBB24AFAAC4ECD57B6E57DFD410DCC9D3D0B9EDA8896E9E47C476F45C4AED47EA5B498A6A13F422C5BB2F0AAAF7B456E3D422CAE3FA01129DCEA9E71AA157EA3337E98E86DCA1FAA13ED7207E628030F0227AC5A7109A16A40F209A7B1420D14A98010A921278809B92DBB3B85E6AC3A16619030375CE4615DFB306DBB32CEF403E06ADEA3C7B4ED215BE9040A3B25BA631AABBC9DBA8BE09DE60E34693A7A4D2558FA968CFECB320BB359B5B26649BA9B9EE58C30F3A631DDA6E3BAADE05631219C78BA5B072B71AFF3F5EB7A13E9D86C3308FD080181D4C5A12282474007D9C26D5E67EBE8D12D199554FD9D77EA8ED2CF653921763D92F59AF032DE05D46631552FE3EABA78C4FABF6133A57BC533F6E3BDAD1061B4DA5FF23D949C6C6233F2DB87EF5B9398D2E3D02F38B17253EE19AB0CD5325E4E6AA7C6A5E962D26B09C503ED3F8AB371EA38D5701EEB0C16FA4B95846C3774FC5E9CAFF23CCCB78853CBA3FA2E2D1FFAD372BE4F1164170FCB540DE5B88E17C1115C5F72CF7BF6DA2F16E3F67A5EB42294F3396DF9372F578BCB8D00DC40C1847985E0CF237995A30028205E2161713DE56A6891CA35BDC28D6D1D06375871B20221482EAACAD7D1075D515058D3307EAB2371AA3B9A1A2815830604421E096E2E883474242269024A6B60A2A51A2CDFAFC01FBF86DD7AEFFD7048FD3753EB212C619FD6FA2175D0C342C23D47B93B3495D13ACC11660EA2C039CED51AFED78BB5BBE1665BCAD6E446A67CC1A750F091801092610085AF7A4790F7F011F577B75B549D45C82A6871882758C5EA3CFDDA8E718B2875E7A585C2F6DBD33A204D595314E2219AE2222484EA07682FFBD4E7186FE2FF17AD9FDCF38F25F2494CF559696FE833CA7CBD3107727AD9E4487C71D66233A28EE309B5FFC833A2B033CF1803A53D44897D126585ED5FE47CF395DE7C94375A7F4B6DE27DA8C9DF12AD9E23B821739FA550D51EF8FFEFAFEDD7215E15ECDA0FB8D5E3CE7F0294029DAD0B56D878F6FA7B0DD619D285C6CE1710182BFB7A41914C15801E508DDF6C47D988047C34408B884BABB93FB150915953B6281CA0D8D5CE5965057658420256D2B3ABEA22859AA23A6D1DEEFDD3B6E4A6A0EE8F9EA126452B5495A6DF593B59ADA984EA06EB296AB896874D5837C59257D61467E01207A69894026AB785D8D62F5E916A61FE9CA22C3AB8AC25E5124DC0DEB6967E43771A621E33F6023D098CC03FCE3CEE807135A8BA9BDC6C4D81D1E893AB05E32B1C5C331D27DBB2B4FAB2D03468820248C8B0942114B54509202460F5B0D5CC45808716A2539AC739264F11EF158CB5ED66D303B5812CFA618EF4DB2B88F051016EFB22229CDCE54D275E877B2A8749BB8E2CA87A420FBF2511A3F74925305B330599D44E52A50A996F19F61323A4B8ADD267AAD5AC34E741398CBDCDADB517D67278BC2FDC5C9D330BF2F969791BF468BC56758716F9DC27D8C67FCFEF73D1123C232794823EC7068BEBEB0C80AE67A7D254622EF6F59B2FE124705B070A425C3C3EE40D1145B74CA1EF68DA0A3F6624A76262D26777BE89E931978F25E46AB5A12F1197CC59047ED9DAB97886195968BE2502D1DCD6653C6DA87332D66CBAD55D29AC9A4B00DA74579BB1ECAB0C404BF4E993B368352F7BC16E5EE7B47C3829302744ADEF319149D60B68AEB312DD368CA3DFE5CDBC1243BF4EC7AE23B2BE615DD7945F7A05674A7BB1C2B5CDBA43C16706D9343231A378684CE170FA98C788B877C32A9EE5E160FE93CA8C5433659AEA6DDE2E120DCADA0318F955F088E2F212B17CF05F1B64ECACF1F5E2755A0D729A4A37552D8213672AA3AFE713DAB41E4C2C2BDE24740BC9E6D08B02931D8F5EC38A365F25F7420487B4CDFE08762A2D18E7AC05327A336D20B18B7910C436C16AD4410ABF3D84CC87A181D14CD707316DF255A570D71444C620DB8D2C4CD227027EAF05781158B126419F82C7E4EEC6FD3981793E7C5E4035B4C56381FE6242B64BE3C899FA34D1D86795BCBD717E9739684B264B0B5F26EA5745E9A9F97E6432CCD9B2C983B5A3FAE7C11B505E401A9C2DACD905E3782523B265AA56859E485A82995CBD090FB5E05AF33535C06A78895CBE266215C6B91B8CE58779598C7A55C52B7EBC4CDBCCCACC00DB35671EB0F06856D3E845B126734505C13E7F399145BBC2A6E3223B7998A4F600EEE62F21D7CD63DF1D561ADA703E6A3F76FE0E8FDF9E2643E453EEF3998F71CFC28A7C807CFB488FD9FC10C47B052CBD089FC1B96D8EDA1723A27C9CE8B21915C71F7E7CAE91CE883E540BA5C4D2F47CBE94CE4DB43183AB9E69E3788349970778874E90A9A3AD923A234F16266034366414978330B69E9B85392005B45180D647B45F80C5A0575BF5BA4319DDD82E724D63ADD2C73BE8D15CE79715362A07971735EDC9C4FCA4E72F56F5E8EFBD197E3E693B28632267D52566D9153637D33CCD2A6D2AAA6FA8266A8B54CD5654CAD154C378B97AA2518B008CB407C522905491E6C11567BFDD56CE975E4D3B906AB90A60B90E39FC835398D6B7C12D7F3295CC300C07C9FF9DB58579DEF33FFA11655E7FBCC8DB299EF339F57A2E795E81F66255A7C9F39318900EF3207D2456E9EAF3BCCC91CC42BCD4A8BCC5ED797054BCBB25565DF0BCA0A6BC9AACBC8BE5790058BC7B27563474BC6D23931B078CB9B0D43A4C22240F421D684759683B55782FD2E02D79A1A4D0031C1B893C076DA623111549BF9B89B0CE27F05EE90CB3BAF8F8B225B2595D6C3A8ED2DC61865FAF374FDAE861E49D4E3B25618E3BE8BE55EA10A4E76A84A51DEBFBDFF6F4CC939225BE412222B7D287947EF69285CA7F51BECEF8EAB9AC38374B18AD6ECA88EECB11E7E699615CA24DA9CA2468CF098A4250BB5245D25BB6823D09AE201018AE9595462BDBA1CE894B37817A71841823AB0CBBACB813295CC32BF7E20602446D72956397FBD58DC36BFB8106328219C75443A48632503706BD59B1CE2B8DAABD47DCB6C873C6ED558AB1004816989BAF58BF43ECBB751EDB34AA1C86381314953EB81939BD761A154568C90709555DF21E0F66B11E7B700B67840E2D04388C5A43A18E58906012A6B0C474C46468093E8A456C334B725F02455E052A700006CFD8D0A2D322FAE2ABA5BC7B01209400C42EFF81D20A4B54A85637A478E21590776590740571BF4B83D7EC8E3788B84165C9001B410D65A321A1C3FFFF413DBE988A503B023F49482D908418252AAD466CB6E09268135ECB508892B8DA153C0A3833351A726CA62CC615441AF518652852A39B0E1B42F511B7697638509C0BB02211DB427A1D7AD0A28F4A29698A3D450AA5162A1CA09BE281BDBEB10124BE7DB28D9A061FC59E0A241C4CE1145CA064055252F6B3DFDE30A5046A55A09255DA10B30B9134D0262EC248FD2D5A3C970CAE514A1AF6632C1203FBB290CB052ED461D66A55575A0836D53AEAE6B53C450D77E7D02B5CB048027AF2FF6034A5A9311A61B1CC31FD494A32E035EC5A730A2060896510EBF6AF7812902810C055084F13E7EE045B54C3A50AA399D429B5FBBAE140B81F4645D30C52A6E594C7201A82800443EC8AB057FD5DCA5CD0068771368099AC5D3015EB34FDDA6396856BD7EB3E0EA18A665F4DBFE04F803F6FE0D10AE0967769720E9E376DB102788544673250F933896600745A61EECB30F853269F888207285328378912B5715C83E649C0830E661848890E2F2690F41E4082A63CE6A80EC434E6500631EC6FC851999637A5CD670E8185E355F2EB6F4E4D86C0F712EA35EACF0D319F59A3EE8190D2E15EAF6F490476EA3F78D72624BBEB8AB9E2A9EFB022801A53E0EE41CB97D9559691178282726BE2AA33031D1521BDE9D6246AA138C63A359BEB5C7C09A2A183A05A6993A3617515EA67A1B15F92C10461B6A9D6E5390C1982B4172B5942616AE9780E4D5E152AD809044FF47025BFCC8C032A0760CC4A16C00831CD9EE96C6859AA8D46FC3E2086BA0B115D5E8194743D897B888371BBD5E4FC003A1AD25D7819B288B313B3E05BD46E9F914AAE4C0BABEAE44AD43C1F71259521D184AB6D402C235C2418E9C3B7E09556AB5E5768531C612F64A840454DDF176709056FC90DE790F4789878E8FEA20D70E5FB03263800CB6BAFAC03A29BC09DD369AD007C234BDB5F1C32D3CE5C33978BC6AB1D62000F696DF93124DBE6B2D2E93F4490A422E07844686580796FC9C74F0E9A4DB93AA120E6D52FB1F26ECEA2F1AA8ABBF04005D931180B946E750901B2AA252CD358773C00D4D6FAB4830B8DD3225E1CF14B81C7CC0694E17F8397091A60E69CD40BBA4F1F5248ED6CBF59BD4D85E06AB75B0F6C75AFF005A5CB5DEC55EF62C0285E8AA672BB4096E8326044357534F1185FCD2A8C082E176004E7EC51969544C694F7BB2969FD01E921BED5F573CAD4DE5E46A25D1DA443A0BB58A02421C03803336B62A5B893A86ED5E15D0D858CEE701CDD7BF86A06136411E07BA635C5E22957ECBE126717935EA2834817DE16D813EC5D5830485C2FE5D2E8B10C92DB511A2D9ACB476FA4E06C7DC72282DE438DA322CAD3F7B654222F7968598420FC918C1432FCCE401F5C2B2F6E1F0FCB95C33A5CE8B6676D7A7F22AC5A15A219179993D10EF1A4901332017E211511A41729883088D13EE43C1426821C41560C10A73A0C908BD67F3F4697A9F29779E3D8BB7BE93C8820B5642F3603D27AB977AADF7BC8EFB4DB63EDC2915F2CE186647BA6C9AC870F8B87C8DC943FBF886FD4CBE1B164F92CD26491F4E5F57823BB1855C22131979EAC2CC005B0D8A30BDC146A5382ACD8BE473745E5F589F6E740AD8DC35E69D3C0EEF581EF378A94C9790134759051CC6BC91EED1E5B3461E87F35BCA980C04834C80F9A24C2D9D5097ABC9A2AC2EDCE914148B2A8B17D2350B63DF86BF42E1DE89E9DC5654E7AB78912544654A4D2062165906E03369A0C2ECB50E944FC5C15129914E9372D4BC55EAD95EAD902DBCDEC0AA0C748ADEF9484389D7C0AE87F105566604D871ACAEA2C99073127813EE0DA5097D20ECE03628F394570480832DA3BC6AB1D62020F6F0BDFD26833B874F84CC218B094E79991EE4682E29CC083DAAA44E0F620C3F894A953B5A8764106A2B0A1D9052220F0A93B0EE012108D7C721214EBA7448D1B9C49CE12AA113C790A382FA7286FD1A1FC7B20E5408069F45F48A9F4711DEF8C8503A8410291574027BF526DA79012550F3C33A36272804AAC7891A215CC2A828E3BC5E5DBCADFF27BE8114A6075D408254CBF1E36401409450D853745FAC8B4A2DF75CB61E9BD8F42E74090DB84F9BECBB3ADC5A6A6F60EB32E042AD533800DC686DD42BB8E5740938DAF8AEB40900B9A6040388F0F000D04270D3071A2418F2DC04381E7FD4151442C9F122F82CB129A827379A04C3653B97A94E832AA113E0E0639424D6C72B9415B7739C2A5E058550EFC59CE0555073B69A8C835769CC45C01304B387199A512848C0388D421D1E44D0A62EC7E27A591741DD1D8058F8E86DA8F5810B6673A03E82A82CA3B80AA23A3C308FA12B8AF2E146099F4F2CBF91838E8AA5D2E907ED0F3B2A56AB13A502A0FB8F185560FE252E7648507C7BBE7D3EDEC62F8B2847A9A560A55CC207A17BC8A2036F5966E0DBA55441BC6DD850544E051034AF255415EBC89D66A3E0F52C2956D9739C9B6096E1F58D5B364318BB6CA102E297ABA42252187EE738E6D69B5B0D47C1F31F59519A6079C0E71BC7C3CC600C0F0B1210BFA0728AC818F03AC72D5847EE341B05AF1729D2275A99409666F58D5A263F18B84C89026297A7A222486876E708E6559953FD46C1F13F5677261826D97CE37790178CDD412902E216524D111324AB73BC42D563A0D778A761D822D551915594AF4DF00A70FB862D94258C5EA86801412C505411338004E79016D4A06B2D47C1F74D9C6F9334DA98A09BE1F58D6D364318D96CA102E29AABA4225E187EE798E6D69B5B0D47C1F3B7A4884CB03CE0F38DE361663086870509885F503945640C789DE316AC23779A053C9F729D9E646840680B37886E77697FEEE3427E64455D94E8140B20C5E4288B8632825512CA02935B2A312FADCE42C55082A3C330FA6871AD72A0176B7FCFB3FDEEB6FB25DE8309D3F3DEABAD08741FA985E473EE8EED159E1EF2C52551C20AC9E8E0D95A41C5395227E45D6837D9539CE24BDD8E57AB6C2F3AB1C8E310DE84D6121BDD88C6E404E0B7557B7AC8959542052C0D8BAB5BD3785567ADCA2888D5B86692E50980DA83BF75925F12A58ECEF5D593FC4A74A04E884770F3EC1995195F22855B52CEBFF68BA1041FBF6D89B49EBF652483AF90D6EAF97BFA96A7854A45362CB6CFDEF24C6CAD42C8F7A99A9969F74BE8750AB9842F55C13101A577AAC0BC444F081105995CCFA8542A150001A575F50C96A872555453502A00C26FE2ED8E787AE5F614CDF284E0E63140B8A6687570CDCD06EA465B9D278763592994FAC086C712B7B27AB357650CB4CAEEF5E431F846EB78977ACA5451AA6737777ACA8C6FAFCA0890FBB7581D6D88D633D0700E00C62A2DC3E08BD040A53E11B963541146B65260042C113BD7953145F078C61699D3619E7C5028914E10DCFED48342653A51680C2C276B750CE3A72CFD6217BF47A874E9ACA76E9150403158ED1A5ABD8DAD1418014AC4EF733CFDD79A8F40CC9EC10666297E5AB42FD7E43B4D51E9549005F13BC6BAA8CEDDAA18A235F4CA1C97252A69892AFC51BA6C2466035B00C8A1D510C479EAAC22B9E98495F4B15E8451C7A9529D58EB331E28E52B4332C6A0C01CF5E241559D422EF9A8D6CE61ACFB10A5A98A218F0E711824A0B440E3309B11A243125542468724C63F8CE8105508A5193D9FC733F0EC67F45E4038FEB45C5E2307332DA78B229A96B3B4BE01A8312D1F7D1EC4D73DD08C9E5F3D939FD153AA733C3A55D870DC147DAC4A1E2D52CC573CB5577759DD18542144A2C0ECB9E1BB0D91781985A618E7D0A8B8B712E7A041A739C5A34D16727AC9549710DF50A67EA79A1CFD34918359FD4C3B3975E74CBF6980FA115B9DFF209FB284D1A0DD642F417753CC29BB756A05D4855FC5EDBF810C4160A4E478A7F1E5E55B260F69F580A64543E9648CD5547A05C48DA52FACB79DC006CA6A62AAE3F78E7DA65EDD6A3A09FC7FCB92F597382AB2D4A201F442C66A018406E226409477C436C0AAAB09AD5E80F756C056AE635D47690767F15D521A2C760EF8D4D05EB1D8C17D98EB04D63B4185C65CF004EBE530573CEBA2486EDD16B30506A6FEAB075E6139D2FB056A5562ABCE8890345985A738030373620BF11CA594A2ECDE56E23935E440A911A16A1508E189080CDEB71C0B9195701AC11019100E371A32709754177518264983B069096C5E7CAF37CCE20D57230D2FD3ED5A0DB73E9C683406207576520D39BC43711A9BA9605D54EADBFD6E2AB802EC75190378C2D37600B177B8691DB89BD0F8CE68AF82070787F5047564A5C11860D4DCDCC7307907E71B38B1A75024A519908FBD81DC0A75A2D1288856DB1DD811FB47F0616E106494574284CB1D824C15596930061679337665ECF0E689069855DF2728CE5A69A7A04AAB716458ADBD827C6EEFDD0098E964B60BCA75D40C4078D82F28AF3CB73A4E23A6A8DC6308058C174F54E935C07C3D87C6792A6A62A8E2F51F0D1C54A23B15A70171938D517C21E3417DD27BA3E4DA6AC2CADFE62879DDBA5575945660B021446F2F881DB8A7B403644A9B3FDEC6BE0FFD2D1F5ABB3D2CA137993D1ED3D9DEF1167676186DEAD0DDCF6109BD89EDE298DA068E37B07743E13A588058023E43D4195E01EB1A6CA3DEE12A30B6BD1AA1A1A5B1A6ACBA9C6C0CAD492C224F64FDF8E0978E55578D15168C4D0175A8CBC4A3AE101FE8E2B0EEBAB0C692B029FCA677D1CBA41672DFC81AAEEAF2ADC2CAAD31D00E73BD76D4A5DA035DA505163995366F437C9A68545F8E0533535A85F5B2006BBCF66AB2EC6ADA86A7BDD83AF175D6B7BEC44A964F2712459A28442C6A502542EC868B47013A6922C27D4C0AA817273A8D0E4EFDE344C62789DC41F5CD9D1F9AFCD1A137796A08289BC936018B1D02EE9AC4A4F7051CC6968037BF1B80F864744F8ACD1D29EE903EF1BB510EE55E94C3BE1305F9FCA826114F8938E2BC5D8E8ECA087F8B5F102856FBA2CCB6519A6665C5FEF7AF457CBAA99E8D2C7E7B5FE67BB68FC732977139DC52811C883A05DA9DC2607828021B1FE2C7DFA5CC48DF1C62C6DFA5CC956956E5457A9FE5DBA876E019491091542E7E5A36014BD5BDCBAB24E3F562211072B1908A191C97E555519D2A9584CB7E993DC075DD24CAC53CE471BC8D399021122572DA782924A64D931BB95E59E2D454B738279172BE8D92CD32CE9F612812C9CAA5AAA3E7A05A431265898BEBA55C5A15AE97094CD605C3058AE6102AC887A5C9ABB35983066BB35BC5D72E1F5C0D20994476FDC02F28AF4E5294B088F212F5E89749FAC497451049C536B490B02649DE8DC745BCD9C032DA34A9106866CA4883886495DAEE7C01ABB2DF4EA42AE6535C39AE42693D8DAA544EE74AA62B0A6BF69CA1318B2FB0A751EE4444C5664824324F92CD26491F4E5F57F0F04FA62B2BD8990A397DAB789125E201022057CE0A7B186AB90C29E52364BACE91AB88B4FA865A13EC94003432734755F7C29A3952E97616D12B1E96799D2B912C157576877E97A866214164A24C4EB3059595D124C8EA312ACA38AF89C1DA23D21595E199A74F5514F469937D170B6B291405B650AC07038ECC219192DCC5262AB15FCC57B4A5501578BD14EAD8A54B7DB2E7E36DFC82062FF467C973CC86342A42CF926295214F4E2698A15311FE47569432C1031A15A11768AA97472B995C9A4C45F43F567732B124898AC8BAD5AD90B326930C50AA647013E7DB248D3632F10C9D8AF06F4911C9040F68E442F33CCB397E419B2615F2471CADE31C3961BB2C2DA02E932690C8C39BA3D2FF8C57650CBAEA44B254D465943EEC23D8CF6BD39487E4EBF4244378F812FFB98F8B523022D384BAF2B986E4529AE5C0E90A85D40AD39BDFF36C0F7BC26DA2BA18DE3035205077816FB2A7981764608854A63542AFA54D55F57F887889C00DD289AAA0F18C2BAD4E928BC8B3E724859D18224D3A7DABDA42BC1605561822C5F9EAF1E28257CA0181FAF497DFC55224BA22C57A0EC8E4B3D9E88527AE49928B88B7BBE52B32F916CD7A57F1E7FDF60E97899507D329482766D3A0A231F596B54464BD0D9911537F569FFD9FB7FB55240180F37EB790BAF0C195F422E903428DD08554B0AE4462455224962053974D2E0189849374DAB66E8E792B18BBA154CF4141B4B6CC6E13A28AC63A822512754429B40EB06110AB45CC3AC16DBDE842900C570AEA747AE9AA5F9524C9BA35CFAE0CE47211B3FEC511D22E5F12429A05237ADFC9B05C0A65EE56516EBB4519B6E02C115F7186163201B9B02330022B0BB004B1D0E4C01AF43A97D82C5C6A5199784CB0A1640B6FAAC23D5B0EC7F76E216D59B3F148F9C5E27040066BD6420536E209030D245DF6D4B754DBB2EBD5597E2F53A5CB3B88AA38B6BD4C25043000684D8332B7AEF22DB9B8C9161D22E32B0F504386207C79812520618041048BB31656516B3722728582A9B51F4D6B8DD88E7A1DBA356A91CDC0D3B09C42D187612DEC431F7C25AD422CBCBBB346B5D25E342BED028390740A0521C81D988594065866B899C0996DEA256ACDD6C667929793CB2B32216F215D3387D0CDB0D1A40F8A484DDA912A17B3C38F63F3757201A371916D6D2B7C3892DEC822331AC0A35A4A96556446689B8A9E70812DB97B450C4C0A6EB0B9653E81A655E51558415104686AA5BD4166D9098C0F6E4032327C7F7B096C5CCEED267489D87B4D2823C94CC2DE5C42767AFD7E28376516791B64BA4465918FA154E6609E05CA4CD8AB93E9629D85FDB74AA18375D4C0C63695BE5A814DA77133DC6ADD886CF053C867ACCE1B6B823739AADA963EA0AF5050E2ACBE7B2B12C7F525D8B66C89E480216E9304A55AE3220616DB764A88E298456173ACBEA19AED9A8A9EBE809A5F4A3E1364B47E6BA9C0700291A19CF956857A55B8DBF5CAB7D990505EB601BDB5A586D20023F1A41998A6DDBEAB082A1139BF60022EC858C4766381B544424301ABD38118A50546EBA9148AD5113B30512FCBB387D1E55863B8DF1C2EB00A45AA509C218703FB5002A185299E404B23F17A228646AD14BCFE47DF1E01FA1EE62C83C8207C627E79B83C9089A0E317025BF16587365A7BBC44C5660DAD4EB1EA2F5E2CD688060CD69FA67164B05B48599EC5206259B9001EBECD244E385F22D754A25A30F4C9F9B82252C50E331F3B4AFE76107C54931BE0D8115C6CD9D5214C1904B785E89A43700308214AE1089593106FA5B1526017536A455C1183EB202E161972AE06ABA038CD55E5D5B580E254D8CECC234F92FBDBB3D582E5027241C9F95CA04589238A227B0AA406887CD3871BC531613EB542011926A1D578E70E55657B0F25F7F5C6AA2C841C43AE040EA6A44E20C7488520273D006B61BDCBEC8178BE4064B821A542E9060C4273D5E713542C36942932960F8C91277CE51023A8D5B1D03339041821946BB2C10967775B48D838B8601F0943ACB0FD83899BDBEF286144065AC16844767DC1F0C436DF6E3083BCA0209FC87E6A6382503A604AEAE0BA7B3BAA6CEAD21A51793CEE6D177ECF17ABAF428355194A793C0E1BAC701495D48395CD24D332D5D9987412A66A0FC1C4CB61B981DB1E94F66C09F9E4C513B18B8C07DF65A1604F618681B67935D17215F3D2A40A8019723840202530A09178A16C8646AD14BCC0B5BE3D0284A9DBEC8617A428B5491E8BBC6C1C4E91D198AB5E144CC8CBC633B4AAEB5C8416A428F825191242F601EF8E11C9085378D1F4902691AA2E9A102A973FD80CB0CE6F701D0FCF062491AC0004AD8D1D483160CF425E3264DFBB1017F8DC9257F000BD0A8754D0CC610EB017195E3424EA3B3842015B0D2E1C726BAAFE6A2189A13A42C512B5F40E8DD489E49A88B84AC9DA4C4DA6C39BA158234164FCF200D49081E08BA8A492A00E47646A63ABB4FD78B5662BB30D442C2B17C0C3B71349AC6033483617517E4C261AB245E45A45130DE6F6860B35CC0F6E02536C8D20B5AC741013DF749CABC954E5066BA95DEE2A4B5F3216E5A2AA2C8119DB718495B0E19559B7EC5D72AC2D652CFCE24A38215B8AAFF4D2920E9E5264AED7F36051E0123D15ABB26C3A6567B89D5B97CD01B63074D7A0072B53370AAA5878C8A253F601A773CB0EA5C356A52F59F46051F62E4515A3325C3A25A7999D9B96C900B62E70DBA407030F6F955431EE8043A7DC24A373A30E84C306A5EED8F4604CF0224D159B428C3AA507F89D5B18CA0336347CF3A8077B03378BAA589B65D3B103C3EDDCD26C0EB09DA10B583D5899BA6655C5C243169DB20F389D5B76281DB62A7DF3ACB3D83C70C7E82D93D65CA3CA0FD76B489187D6D5858982FA0A97A7DAE62E9885486EA0353B4F5ADDB77A4BDDBC0A6E51064905658739789B94DBFB6525474821899C5560F2AA5977DBA4DA7B646FBBA78A04BBA4186285FD4C348F708F1471F5ADCA5E294636746513E7222C274653DB860790EB144E6D439EA9E9C22DCC7417F18A2ED56389F8856168C113A5BCEB7F25A23C5F9847DDA57B4BDDAACB3B71C363E0974CC8273CADC4194835A48B4EE1F4642ECC495DD12BBC9A844BCB2F268F05B21F7BA1B0C07E5CC19EAF3261F215ECB7E3D26A144BB0DBCECE5EA136DBD119577739CB6D85C9D44B83A89D5B08CB048C035E466D6F1622C2AC621E925CBD480497737391B2FD47D099DCE1CB4F20328D22C1579ED899C9EB4D27746EF065E7723B817CEA8584D89D5B12CC447C5CF19CB818DEDED6CC5DDEF86276D17440C22128BA9011B42CC821B6AF3813BF33044EE6C269829447BBACC209833B93869B3950AF0688BD151EAD52F9062C12F3E9D86D28D7BBB342652C1B9505E4EAA5938DCA16860B3C28D3B9730665804CA3489C41D9C64A7EC7642A375E3F2237148753BDA09CFEC8B13939B9888766D16B294E6D2EF68354F88C2C21F683DC587B4A6E105324E5F19BE6D41E5E690B791DCB99EA10DF97E11CDE3C5DF8978CA8335B19837F15891FE307BEAF44AE11F19E924925F4EC3666E9A404AB883E477155F09F9BF25119E4FB5326B541F0DB18A71713AC3E882CC5152278A2CB458D54CF68E9CD8E872C3A461870AA991A7CE64B2B9BD033E43A77FEE66509877E41F99B985D9A33C0167A5ED69AE1069A49BFAC9A010773A38E1272186860EA9C70B9F58D60EA9E989B7D1A0ECAA09B5298FAB0F44A2567D824D6D5322B2B9CDFDD822F173AB0A062F48C22D6289E62FCCCC0706344D0EA9C79EB7D109D4681782B7E5656F2BCE44767A71E5D64E9354AA51E5F3430D96811C6267B6988B1A7D3299534C86862AA50614666EC198E630AE6E2F16A949637243A372B77EC558937C28FFBBAB5BE6AC451C068660ED598A39DD127127564F45241BB98D7CADF5341BB4BFF5205F41ED0CE534733FA25E0B7B28A66FCCB494D4C2302A6176A318AB2E8055824BB3BF8D243C755B4422A26D114AD408AB6D946899FE8864E0CA326BA01136DE38D1227116FBA85E894CA24DE6CDBD3291B29DC265B324FB549B7E67C5B75AAAD619B1166D80A936BF579B5C2945ADD1AC166D21A9368FDF9B3C6D459DD34A3CD981526CBEAF3648529B2864942CD8C81C9A22C0C0DB2A8150BE074663248B6D28CD7A3455527BA16735C93E9AD91512732AB255552F4AC480E2DDF87B4841FEF6A606BA159FD5B536B11CA76FDC978E9C9C6C4E36F89B10808D8C7022CC20036569FC8EC9FF8A4BBF7C5C1BE179B3D2F36C6F7BBD9E5D70FB5B0EAA5CF248DF32EEDD70FCBD563BC8D9A0FBF7E4024AB7857EEA3CD55B68E37459B7015ED7649FA50F49CCD9777CB5DB442E53BFDEFC88D7DD96ED2E2B7F78F65B9FBFB870F4525BAF8699BACF2ACC8EECB9F56D9F643B4CE3E7CFCF9E7BF7D383AFAB0AD657C580D7AA55F296DBB9CCA2C8F1E622A159FD25DC79F92BC28CFA232BA8BF05513A7EB2D438653F19FF10B152DFAB533759B538DCE2E58C3D62AA6C78367CB807F37C77CCF7E3F3BBEFC891396EAEDF60915055FFC5A952A6643442C27E25DAEA24D942FF26C17E7E5EB50CD0BE4239F669BFD36653ED3B0E4CBC21738A7E8D75054FF555DD2222A8AEF594E29D57FD59074F19912823FE86882AF39A7CDD37FD59584EB1A96D6A6A84BC47D032DABFDA62EE5A238469DC333556BFD571D4967F1262EE3352DAAFBAC2EEB327BB8A9CFCE93A2FAAF1A5A2D8ED7EB3C2E2851C46775594DB3387E8E924D74B78951AF10830D87A2D028775494A8940FF1FA22A5CA3E48D16F4BE72FBB24AFFA77566B1E8D7A2E98FE348F23A6F607097A7D098DEDF69BBA94CF71BC2EBEEED64C810709ACBC5F3F501D2DDD897F607A716A38A54705A53103375EA30103331A8C16309BDFEE06737C660689FEAB7FC88D54B918BD46958B190D2A1766F3DBDECEB7A8DB03BBDB618A46EFF59895D9D77C43F557DD5775499593C7228FF8AC2EEB2A59AF211893DFF5FA7C5656FF555D52E5B0AE4AFCC65BBEADFA71BA4E610A8DFAC8ABC5B44165E4E035EBA33534B68826AD4E2E45A10DAA0819AF2EC1866AD0464F517129CDAA2FEA129625EAACDB4B9586A2A8241DA466CF174586E777345E89049DB655DDF4C45642F7595DD67F243B56AFEEA3566BDC46F96B0384CFFBED1DBEB9916A9C008546AF19BD50FD25FEA0D15356ABC79066C39437EBD3F5B780197542E0A5664A3D0F873304BC1B26764C1B2468CB639BCC20E1CD7A8E6D1D2C6C3074B1304711C42BC3D18287A48511962E169FF26CCB91582719C8BCC938127182DFA161242C89CFB7A846262176F5E824CC2D89B94081332A493B8EC37650838409551A76EEAADB3A8DEAACE136AA312E2FDFB60D0B5B5B44827A5DD54B1AB4B0FEABAEA41B1C96838435093A28AA17A8D9426A0D467189A6C69417DE7DD48A983EC4EB935768264FA7E90D9535373B52B6DFA7D34E1EF238C6D999B5132EB74A43113073F1D3F1304D6590A281F038DF16D7F7CB387F66A6E9749A06069262B589922DEDC193DF75A21DE9C33E7A88596F6E98A2B3EE70BA2F4ADA3BE8BF6ACC569A6D68747D90DF2783F6FE813003B0F35E4353803A9FD5AD5965D280F0DB20455F6215A484453649530AC5D1EF9DF3AC4BA66BE8DFECEC65B426BE1FFA22A3EB45ABD328ADB996FB3BE2F9BE81F96012DD65645E8B6253355666E222DE6C58E781FCAEA5277EF41A50B0FDACB9AC80072E561E95A417463A5EAF6FB2D31B368CD425E8C9AB834FB048326D324308B145DF24B2C0397CA01258E0B286E98F5A2E20A43448D1970804BD06293A3D1D10BC36085C5F14E7299EDA303D66F7793278245AB31122097E03500AB943F64D04230B272651532E204D47067E0C7028A2FEA23132643925A1FE72D81BCD50EECBE525A351F54D2718701FED37257211D608A14944D516903C9EB7334D9FCEA5DF7151346C8DDD81F6C321994C8F3A7CE8D76C9C1FCAB09831F204C86638351F6F7ED3A7EACF9D6A5EFE4C924C9FD2DC6F9A6DCF750F7390E198EA08A3752B034E6B6A37345086CCD6888967EE2649BFFE1023BF89758933FA5DF77E63B584645D3048346B134A92541A86A2202E4E6880E8D564B266AAAFFE648A06412B1D10CC6D8A9557476200795DC2A45A9261BB316B241AE3850B4C276B605C683F1E3A76EBB70B6923F55F75E2CAF52D238C17497C1F2BB28F218A77002360017820933496319983BACCF23F48E1DBB3E4D6745C5E264F314F2890AC2DBB7D039C239B4CD69A2B47DBDB2366A65C7FD495F31192F3515FCE2F909C5FF4E5FC3324E79FF5E5FC0592F3177D39FF02C9F9177D39FF0AC9F9577D397F85E4FC555FCEDF20397F33C0E1CF20107F36900443DA00D34720A88F0C507D04C2FAC800D74720B08F0C907D0442FBC800DB4720B88F0CD07D04C2FBC800DF4720C08F0C107E0442FCC800E31F418C7F34C0F84710E31F4DFA6DB8E336C0F84710E31F0D30FE11C4F8472D8C370BB927FBCD13B8C25B2798ACF0C222C9343DA9B573094B25D3F457B71BB7005EE1EE1275B63CFEB94F72E4D09D24CCC6473225E4E1A1EE797860EFF584E672FD5581264BED9C3B105596DAB9AC61A61B6D592807BEFBEA6F417C32E12FC36517053946A12F48CCA104BE5C2E024D336CF08387BCEAF888598BA9790DDA088FD17F68A7E661FB47F2BBAE34B69324BF8FD33E9A63C55F62EC25B0ED8449F6DF5E46C577B369E032499F2C904E483106BD50861867042BDC1428823142A76EB7831CCC46BAA67C46D86A780D10C5E50C513BA7D97617A5E0A1632241EB3C157BF148F7515B2F60EFD03045DF23C2E76FF034E70B73141EA69837B0C864F93C10D03549EE79809E42F73800BF1D0D1327D33FB5670ACC6EBB6A984D6EBCE2B2F20CECF65444CBC5F652C3147D8940FF4225CDCD5F26CB67F3EF11CF6DFF04C9380E4EDD5FF010CFA64EA63B51BAA85ADAAFC8A528F4302A4294D7BDA5CBDE9A87B9B90E83B1A310E5EBCF59B94093B7382D69680FD3D4A5C6C833DAC6397D8299F8AC81E93C3EDE978F1494DB8F3A25DDD5F7560F8BB8832EB316EAB3C7AE5941F730C4679DD302AB4D92329D55F7556714B9DFA7CC08577F539782EF971ECAA8BFE858B9783C89564FB499DBAF93E9718857914C42BBBC77A054E2B97C5EEE84A16561A2A764C274F622D66AD50F77611703567B98AE2B1D159A7B701A22D05923433E4CE513B181383A6DDCB070A30D249148529729BA62D8FC6AE13A46BD88D3356A1643A154D2A1FB9F37D7CBDBE3D5539A7DDFC46BE6CA1536555DF2EF8BE565448DCFED373D29195D07DDC7E9F5CD6DFBB5EAA2794274BA6ABE8CB13AA9E56B51C6DB2B76A580F83ED69652F737C4208C82A15BF2BB86EDAE6069E4770DDD92FBF278B3C9BE33CA91091A9D7075D332247098A229F132D9262520AFF9AE8195C778F5F429ABA31267FBDD2659A13E9C9E16F388D4F3394936C80D7E387D5D6DE2A292078448B8443A138C24852B7098A22911B037F95DDFDE15B7CCDE0C918DBD2B61327B37445A539518772AE0A5B24CA2869DF2789D94C4034503FBD0893A93B43BAE583A4DAB56FFDCC73C6DE944BD09184FEA3049D7B28337DD58DB0E9235ADCB15CDA6EA5A98AF359BAC6765BE643A51A30DA26F299E13E165746A471C9DA6D7B28B45F4BA65423CC3149D1E2ECBEE99F862FB51BB8EAAC71781CAA9BE6B62098A840C12B4DC7978BFC320412F7C57DFB7078B85D235B44D76F0FAF62041631ABCC501456A9772F75157CE47488ED616E59AE517488EDE06651C814C4BE485D23BB0C9041379F43E6C32C1441EBD1B9B4C98DE64CDF47A5B5280CD1C4DEB925B928B3B33D3BEEAD6759CCEE5E36EA830E04DB5E4778DFDE919E382369FF42258FA57D3CE111220806AD1F07A21C68D4F24629C10F162139578659BEA40BBAF1A921EB3149CA90C1234BA8828DDDF23C710B9E0F47B1A83149D4E671DD3979ED69FD465E057E84901F86F8D10EAD5F905153DADBE68485850FC8B37BC8394BED7D5E6B2199BE0A654846CF9C8CFC5B5648881964CA7BD9D08E7F0766FD88F87494CE61FA07C20596391A7889B1BD4889BD4876B3D20C5645AE520B265D224490106CD51CC1EB8B160166056DE7F9E4CB5B57D4DE76157AFD92DB2C4F25E75409E451FAB244D104E6E78D8880B99A215A06EF89665C45CE649279AC83DA743305492A64C4096C97E8783BA870E5F87EA08CC4351163896099A213C43180728D2759E256BD4DF7D8BF3C2F0014FA91005DC2AC8E0C65E6856260A031168C476763BFC8392D97ED4D9F3BB7A42FE262B6B90A0B107B02E0C2B6F90A02DEF649F6CE8BD8583146D89ACEF3D48D05936285679B283569F8884F1767AE9EF751ACB858E8043B84AAE33C4A8E233C37CFCD535E05062F75167950E1FED05624283049D265C2DC04157365049BEC78B5081EDEEF908702EAE356FCECA6853AF0251B365324153DE2947DCA9AEB40A10EC5E4AE2F3641A2F8134C353A71DBFD1C9530177C876E3EEB693F18701BE24FD2DAF23A1F2EC0E7D2AD1DF4698E4732B4052C4CCADF38E8706E4304563AF62CD055C09412668CBFB166DF6B0C026455B22D78B82D2DF6C6CBF5EB032C22AC8A982530E23DFDDADD6D4982D32ED579DB5ACA2C40F6740F2E8341D3CE549B481DCAB61CA8FDA13F3659D6CD094EF3229007983247599FF16BF5EA4FF19AF101B5DC154D2645A20093CB3A01921C0245426640FDB946A9E4F9BEC3BE423B1A9BA92F9524D25B2AE17F97DCC65F67F8FF21CCD9BA8294BFF55E79455891FBADAA0DE27A2CF5891293F6A0737EAD06D3CFFE9D98DC770BDD98FFB26A83FF719B5AADA9ECBA2BA5A11C655C61710BE4FEE741948ECBF4EACFADA48147473997A0D8AA428D7A25888B826495EB836698AB18278EE1CFFE3A2481ED278CD46B686296F789C68F7125A743EDD764453D8F2058821D0F2C1502053C7D871393B33EE407ABDB4EB5D3902D411CAE39700B466E3E0B34F1C3B54E1E3CDBD1FBA6F3DDF3E1F6FE397EAB500A49AE923D24321462F49CB4470277714278D0B285D6BD34075B217EF363863B60E90495A32970808D559C58B748D656439231B22D1CAA339B4D2C961726009B4E4B7DB1380B8219BAAB72F173FB2705CE9D46C6065F7E50224E679A4CCBBD13085690ED729BE5F4A94434BA1D3C1BEDCE00BCD4ABA6FED3E6B681B3D3467DD281D89EF1AD21EF3B878CC366B60CD834ED358C1AE2F50DCC445D19A0E048788CE323716260232ABBC20C008C8F471F925299EAEA2347A88D933CA3C1AF55CF09587F973BCFEB4C751BFAF05707327953C867779602B5D6820AB2E1D7C8E73EBE19A116436642B88110CDB0C3730748334F3F03D0FDFF3F03D0FDF9AB9CDC3F73C7CBF1B73F8FE232B4AEBA17B20C46CD89688100CD9034E60B866D235A08D2FF041D0DA656911DFE0439114AC81749D939855578DF9E833984482D609936D94BFF20FAB03E93AA7B39BE77DE843DADD679DDD3DAB2C5D8B7485290C72E0A80DA5EB87FCF05F70C4AF4ED197082ACB24EACBC5CF31A2A46758709F6AE08E20BF32BB2FBF4779FC8D691F1C1AF55C70E3C57D27DB4886295A1B2FF2E84BBC8A931D7E5965B789A8DD1C2081DEF96A3C3B38BD3C7DDE02F7FF41E91AC75E9222E249A6D37403E3F8AA779E6C98421F87D582363C41E190682C7D57633F759F53F751BF06CFBF5D7DDA64592EA8448A44A31E2F96C7E757DF78F2816493DA14E4C025D298B06DE3977F9C9E08328129746BF42354A35AB7695D9C2DBE54D71B2DF777ABEAC9056A1003D2B5A4E3DBFEEEA2D5135F3E40A1E1CE9F0AD5079275644B940709741CF7816A004A600A8D12D0EA0179F068F47ADEFAA6CFFD6E97D1878DD954ED76843AD66F57FC4644266B603345AD3C5A9D5E3603DA97F84F200B3E958EF719AF91DFDA6CF4A73DCF619AC9682AEB8A8714BA232AB7236652CD465579376C9A4B070F49276C2ABFB7EFCD4B2AB47F9FAE6B7D58329D66667958364C616075583C90ACEF5957B3CBEA4235D0A92692F5655F57677096257D591E94AE3B5EFF028DD75AB7738EBFFB67BC804CD3175BC7646839666119B9144164866606823310C9BC94322FA5CC4B29F3528A666EF352CABC94F26ECC91FB1FAB3BEB519B946136628B2508466B921118A9E9E479949E47E979949E4769CDDCE6517A1EA5DF8D394AD7919F5594AFAD076B4094D998AD2448307403FCC008CEA19A07F279209F07F27920D7CC6D1EC8E781FCDD9803795B33D6C33823C86C1057102318C2196E60000769F4E17E1AEDA2BB64939409FD7A134CA13110AED7B5884D548980F2E0D11894030FD8F92B7B2B1E4860B0FEC65CA3314CD1DC478A2F3605741D2619C83C7FD96529DB6FB1C9DAFB5EEF052A0F52CD240B1467284C754F9F9B2BBF0525E869F43B75B82F9FD73AD5BA70BCC26FDD7D0F849875DD1211826E7BC00974D94CFA3CDB9A675BF36C6B9E6D69E636CFB6E6D9D6BBF186EA3CCF72D3A77D5B6693A199CBCA1D921B0E662826BEEB9C5A281F33E64E9FFA9BC680058D81DAC31E2AC14DFC5232C5AA3FAACB59A296FC841C60FA3122F2BBCEFEE234466EFA2A06EEFCA6D34CA5F234E653BDD986F8471CADF1C3F0F5894393D62896A0D026650278961EF2D1ED934DD51F586899E4778D76E6F5446998484FB82521BF5320BFF76885D8A14A6F0B926CACD17C1722CC8525A8552217F60A397F117D36884AD23A15B4CF81D7BBC8EF3AE374A5C0E926628FCE92291AA364FCE73E4E579CB3C9C334CDD8141056EC3F6B9CB7B95E9EE3E8E61563C3618AB615BF4111333A4D4BCF1A256C6553495A32F1D1044E04834D9DA3717D32C7B3205E7230F2F2097E03A742C8CDB3ADCDBB14029967D571C4234660F7DD40DA478E34AD53AC35FCD82B41C9EF93C153FB04BA11985A660324F15979666D39680C91DFD52BC9FDA3F02D1FFE0B9658A7BCD9494F7B31C0757A92214FF50B1E7E0BE6DD619DA790C5921450A62A883FD442FCBCEB83592A1D47AB62FA946C000031891ACE02F229D97844FF75CC21F7EB6ECD4A233E4F17D9E6F37A455126D8D69DE9730448D16D32F76FB978F8A653C70AF65607EB99F5E261CAD461697A8BBC86387378EADD302F10A208D3B1DFF10837821CD0CDF157C9BAF83DCFF63BB337DF5A6E1354F279F92368C3C2541A99A05E4F150BDB07129F7546F74605E8792226718C4EF5105169FC4ED540820D3AF55EA9F285825E1516A9137BA5AA7BDAFC267B8A53D32550468A491DCA6570EB9166E53EE04E12E82C7056BB7968A9C4678D9EC7F123F3CD9057150C9CFC3429E3CD57CE5F76495EEDE562433B74DA64DAC522CA63AB77E25B76A367E2F9CCDC8871C7C33E124FA668C4A03B3E76CCA5D3DE6C686611BDE2CC9A966E01075A8C112CE442F89549F3B2308128B4FB4876D41B244CA762AF97E6B559F39A54218F53B010045656FF596371EE501E3E457A3F27A9E1C3D65C6695EAE1F3722BA861616A88F8AED1E7365C408F3B48D19946646CA4BEFBA8B391B23A6A421793F83C19F8347E4FBC6E8303463862A418E0494186C4ADEB5969CB8304FAE1151669B6377C9F6FA364038B6C92747D10D8A3D17C3638C90BA0B0C467ADE0C11A8AC692DF7596BF20C5FAAF5AA7D3D8BBD50DEE54C7B7A553CD3CD17BFC1D6F4F88DB2E8BD9864926F9ED8178B2FE23D9B1BD62F751A7BFAE5E1C68B68F0BDE24A02834701BBD5088C51F74A79B9066C314FD31CADD88078E5183844309C0859A525FD107A6AEF48E49E17B6121BF92FCAE216D9FA30E7945F719DD579D5D69AB7D1EFF11158FF48EB4FEBBD69404F57C2CB4C8EFEAD2BE16719E32FD74FF5567542B8AEF59CE8C6AED57EDE0CFE78C8EAB0C1226E3992DBF27E5EAF17871613CF51A4830F0C824FC5C5C926C74A361123510BAB86087FEEEE31BDC8557DBCAEE482C25C318050607622946180986BB906B6658A476E40ED6CF50B38E0D0C04924906323921CB61E254F16BD98F0DE4D8E3D8A44F13D60387C4B09605753CA11ABE895E8C6BB5E135A8492E2737985733304782FACF3A47945F38B14132613A35146F77CBD7A28CB7D569B8660663525D2A82542A4F4D0E77F0A667187A954764491FBA3039C271600B34D8F6D5F192A83A706AD66C87320C11209420A8BD968969CAC324FD40DCE27AC9DB2ED524691EECA99539C7E116B0EFE1D08C362D455C50C320BFAB4BC3FF5EA79877288DFCAEB526FE3FE38852ACFBA825E72A4BCB474650F355C35ACB53CA4CF8835E3FF474C476414F5A074A2A8E8F8014AD832415C72F8014AD475AF0E4990AA6369F7426E8F7086ACB68434B1A24E8CA3BDEED2069D5677559D779F250DD92B2C521D4A1403A4DCB998004129F7582F61CF53E99E9D686A498FE85F8AE75F8093A96A5D75727CCD1AEE6D39B1DB16F91914C4669884F616886D97CD629CE9096A015CCF946F983DF26547BD0206FE67401824C3C2F2531E3BA35BD62C351BAFF3CC5EA3D2ED1846657E287252DEB572049AF86858214EA98E017D4324565E4244B80643047EF9925EAEBEB3DE5EDAD64C1AA774D991B26600ABDE17319D31E46FF35C43928AEC478971549C9EE9B1D2468D434E04BE9BB516EEFB638DE978FE04587C4774DBC70C2217D82BABC93A85C81EA0D12745638FFE45CB96110AAA91F6F6FDEB4A50042A5694B6D6FE801C5F68961A753BF2F9697F48B9EED373D2919FD846FF7516B3216E37903D30CBACF46FDDC327948237C7B9FA0A71BD068C4E3B3E29C73A7CA20C548EF6F59B2FE1247853094352432CE875DF10609A6E8573970A8EC3D296317CAB9F371B8D1CA393E34C78784C8FEF1623BD00865D9D771E5E875780231230FC9111DBDAFBFE8B4026833B2C99510986799FC1720A9FEAA270941346225D55FA788D8DE35B1842C5F901E664572C6F6C708DD06A2C9EF53ACE426B87316DF2546372C298A328A6B7124A907B62A01F2C8564736B9D096AC00069ABB0C6E9DC5CF09BBF1BDFF3A87C95425CD61B2D1C3644E770DA132E549FC1C6DEA990623164A1F2F0877913E67095C7E2A699CC05E1741824D4925CE0143BE8C69050CDDCE28C677A65C78510EDC2773BFC983BF71B8F12CC1E510C6B742CC7BF0B4242D4E2821F8C31C919C2392F38EB54ADAE8235E3383B6DDF1D488711235B00918C86305930C1338DB2A3F0707E6E0C01C1C98F7D0CC53E2790FCDBC87E670F6D0D8FA5F768ED77CBC6F3EDE279636BDD0C27CBC6F0E96CCC192395842A6FB1EA55D9C0DB33F16667C22CC790403FFCB8E2E63D4DD715164ABA4BA779D73C5CD6D77F74B99C6397E0C0675649C6B6C405AEE5D352D0D008C353DD1E389BF5D66FB7C0501039E300AAE56BAE5DFB084ADD8A964A3ED0D0EA940D13E81B68400EEB53C248196F2A759BA4E70EDBFBB40F3F9CDE6B7F7F7D1A6A06FEA14DBE7D70F208AD481D67AAB27A84DA1E25C2F6FF1931B00CA7884D02B200C2DD401D1350767A0093205BF9C902FF6D43591C7D15F0F762AD7D0D8834AD3149E4006A0441576102BEF09AF8EC618836C6E96B5CA11EA0D84405ECE9B9549235203ABAAB19CC1F476985D2CC225402B06A2060A19D986B5C6ABB25B92C011F0589DF59ACA905DDC87FBED1C05D6718834F160CB1F63B560643E8E86858EFE78A93EB73B0C6050A7F86E39F72E480F267219CD2E76006A4D2D434DD8E99D71BC05C82DB1A8582A3D90BABEFCC21ECADA867409EC6A3F1F60D97A07A17C9F229FD5F074871807C23CCDF1ADB4EBF1166270077171D16C416E7B10C629CC55CDE912E88065A1C15ECAA1BFFD485CED502EE64016D4F02D45E60EB960116C016BB611CB294CC5169383B38D39564F9D24699CD3245D50B3F9D2FD5DB41F30DAA28718AF9F6F8A9E6F899CF96D54D9AED8452B1CD14714D5D33DF834E95D54C435C9FB770BFC08C93ACE11C2AB0B797FC2043F2DFFDC9C6E926A27524B7015A5C97DFB3AE66FEF3FFE7CF4F1FDBBE34D12157823C9E6FEFDBB97ED262DFEBEDA1765B68DD2342BABA2FFF6FEB12C777FFFF0A1A8722C7EDA26AB3C2BB2FBF2A755B6FD10ADB30F48D62F1F8E8E3EC4EBED079ABD11AB24E5E7BFB5528A62BD21B145C4D0871BD98600FCF5DF6206152D5ABEC4F7EC1E38BACA69FE5F291C13AC5897DFDE27693B14FE1E2304E0A5894584F09BA7381211575ABF7F87D118DD6DE20E911F84B9F44F6CD499A4CF117682F3F7EFAEA297CB387D281F7F7BFF979FB5E5F60F6E38967BF1D9BDAAED8B57849D8D64B4AB16E672BE641B5A028DF2BF5FA4EBF8E5B7F7FF57C5F2F77717FFE76DCDF54FEFAE73D4F8FEFEEEE777FFB776C6FD731675D695DFA52DA37BBDC75CC865F67093276D6CC6CC8A178BEE61AE2154FEB76DF4F2BF93E2CA7C2F95D634C4E3E728A9B8EA3D8DB5E035FA5D26B8F9E8C9C42F90A1923EC4EB8BD45656DBD4E8E77EF952958C385801B594857B197D58D75C56B0FE1CC7EBE2EB6E4D18440E4A7201503830E07667322AB4AD5C7748007A07D7E301CEE2B36C3CF8A8DFC99AA349B936305C4C6AA305A76E6D00A0765D1BD59B8E9CBE6C5023473F1B8C7B8F59997DCD3792F1F467DDFE8878F6D1B1C6E41B900E3AF6FE1948C77A360F125EA4F759BE8D5A9F5FAFF383645875858B3C59F5A342BC4AB6787BF42247BF8ACAF73EFA2B02309A51A1E48FAEDA245B0A93160ADB53B7BD4A6BC575EBF5D570EB07431D0BA59E0E75D0BC16E423930EE4118F91EA36A586D1AAFD742F980A0D6FD22C81374C0579FCA29F47F5ACA9B24895BA18BE6FEA54B45F97ADC182592FC4790C57A5EB81A0EBBABF6972910E697FD1AE9346B214FF7FD1151CC02D6C6DBFB0A9F3854DAD2FC2D4FBC5E2539E6D6DE6CD9DA09BCC8598201DB5320E886B162C2279F0C654E5681E102172EE75D439197AC91AE6C49578993D9819B361363325C1ECD190F5B2835D3CAF967183395C872D6B33B8967A169768CAE926705607B84E5E8189B241F4A096E66B94387EC8E3186F062F8CF0DC711B017AC0ED13D171BE2DAEEF9771FE9C3872F2CF9262B5895045306EA0E1943C7DD8470FB1DCD3D0F6612E8AD36A04B2094E933B84F486B69E9337B659F4C5AD7013E88A363DC980CB31876BD8B6D9F8892935C2AB50DB8F11096A8BFC29AE4E68D3BD8E069A7B09EAA8068C441C21D3344DC76993FF64D6BE5C2EB99C46692D6AB9BF6BABABB051AE5EE234EF01697E9B1AFB1217F16663B2A6D473DAE4DF1C0F323242CD68D580ABBE0A8FE2262A0C986D8C800335C7EBF54D767A63832B2CA60EF7E849520F00349D84D1FC9F7BBA5561FA0F776BCE67FF4D363E8293AD6C2F23AFCBD8F045719E626A8DEE57193F4483318110D5587551C46FEB5E161BEB9CBCD4B71F0FEB8FAC28DD2C57647939B0AFA39D5466CA70B64F1909439A2D9797568E497C1FED372572282A70459BA978399371DD608F405B93464C63EF418B71D8A3894EAEE9CD1A7907B6D4E78EBD840033C83AB31F67CD7F32CDC3659BF71984D152E0D65E93307121CE5169BDA6CE3942AEDACE1BF6008D1CE5A4B9043AB725B017D6C071CB6FD7AEF827C235B5E209F2D1C614EEC5506B6826ADCB7793AACFDC6A5BDF5107DBD9D31698958879248564D5A117D73D0970F78865FD0DC5F969C846C323742C5D6158645A9687B6FBA687C1FEEA243D64B57C56DD0179ABA6A68FDF71DA29E071CD4479B0C65B8A9394DE87A1BB280D5E8FAE57264886931168B439C6322E2F93A798D5C3D16A7823BFBD27DBB9FC459447DBDB232F523F7A91FA8BA3C01D96F5CF5E34FC8B17A9FFE245EABF7A91FA572F52FFE607FB3FFB11EBA74D1DF96954474CAB7223D64F033BF2D3C28EFC34B1233F6DECC84F233BF2D3CA3EFA69651F3D8D5C7E5AD9473FADECA3EB56D6EC1838D96F9EDCEC187021A99E8AB89084374374EF8E988B3A5EFDB94F72E4579F24CCF65EB38353C079319B13584E641D4C10BD9DB0196D1FE13EBEA0B07D049E623ADF13FCBA931FF80EBBBDC322F64A5F893E475E0F783D6032A1A669C45C959B451DE1326908FCEBC865CD018CC6B96E1475265E7AAB5AB4C21EBAB122A6CD61F22F31760EC682B32604B90F12E8A0517815BF2A302921DE313A52587AE47DC906BB307906BCB5B3A3D49C165E47632B134413F5A38B63B86ADD1F25DEEEA2D4CF7EDFCBEC01BA6EC6702652E9E9674B69EBC4E0B36D78C6F385BC4ECBEEE214A824F3BE31BE105F27629AD6E4E0408C4D974BB27BE8A8DAB32E46579411276C74BB2ACEE91CE7579535D978E9AC5AE13FD0B1C037DFE8DB3AB56DF563BA5875AF617EFE8DE6F7D0EDB08BF1261D1023C5A427028578EC92C2780CF80DD6AC44E28AEAD6654FD9C4C8C7DAC6B9B33BE3002CE7317E43DDA3A176F874B23FFDF7783428885ECA750E67F16A93A41E3340ED6D9FFA138F1FBAF658BDC5E349B47A7294817AD4B9BAFB0C3F186BD0AD75CC469B0649669FC1E7E9EDBFAD8B7E16E38B4BB0CB63A01123C1E6F0735715169728B0226C34AA9CB72A2F6914D3E00A41D721F0465BB13495623BBEF5BA0ECD2FE2749DA40F9611DB6978D337D7CBDBE3D5539A7DDFC4EB073B59BF2F9697913F97038BCF7AB307EFD0DB8668D5B1931D827107CFE9559C07ABAB773CAEDC5FB634FAF6648F17492194BA08735FB990F27B725F1E6F36D9773B31F53DE38EE45C26DBA41C54BBAE63F718AF9E3E6575ECE56CBFDB242B0477AB59FB49B2418EF4C3E9EB6A131795FC4160C7E8C98F1C7139B05925C799CD2A69BE6C5609B7D1127597316EDDD4CDC126DAE1BB089272993CA41139C933BBDDE0CE912464A53FF7B11B516876E3465065282212626D2A47B26A63B91286CCE548D449B44173163447C02BEDAFB62DA85844AF5B224E63D64564D9FD85552FD3181BEF63B3ADFE61CCC0D02575B28FA1BB7BD285B49B64E762F9FA788B0380DDF661E73E722DFFA367F9BF7893BFC021C5B444AE993F1BF579F8B3539F872B5BE9CF570C6F5F26F9AD6629FEEF60E645BC74C22B963309F81D3EFDBD0F9C9B956D5FDBF99C116E9E2BA1C6F72DFF5821833E9868DE0C87214DB3A6C80D8BBA6E8E8B4D54E2E566D7C74D1EB354E531117DC95751BABF472E21F2A19D8BC60FA93A9679BD742CF0E2EAFCC2B5C885638101B6A3D2F715DBDC836415DF13DFBCECBAB9923378FD0174C87DD8B715780C070E2FD5773083E8A7350E847D2DE2E63241E285036579CA0D8CC48A49E3A291AADBB0044877BE4516E7414EA4CD6E56D1EEBB3A7FB67AC86F912566F7FDF7DC46BB9007DC3E3D8D2EA36519D95DCCDA8B3A4FAD9C78972F28F2C78640578E68E30FDF6F3B436F865E40E81DA7EB3C4BD6A8CFFB16E785E1E3A18C10A3277A20211E318832C23F9C9C955844AB27E4FA3893D758C0B5BC937DB27173DF752390F4F58C1E9D8A8B559EECA057B6CC9E4A1AF5DE6B75572E323C3C5A311A396F91FFA3A3551E7088C1B05155AB3A5892C9266C82F9B0EF299B4080B67BDFC345BDDE207D37F51A88B7E1B6CAE394CCC2A47BAA106DB0E74DE39C638752B3B38E8316A2EDEE711BD8283760983CA578F8EF1C2863E5EC6E1997259A811B85D67A6E13A40CB97DEE96ABB371F6684D23EF5BB4D93B15381DC7451D3FD50A861176EAB50F13DC749C3E1730A3A2C4EF790059A90C91036ECB0B45F324DA282CB2989C677C035D1D24E46483A64E974961290801F122FDCF7885C4E8AC22AB4768089418AD6C5018D55ED5E063DCF901BB2A934F9BECBB99E74DF35B35A95A988D22CE9490FA2F46F720A82E9E9A08FFF728CFD1F483F1DA6D0FAA9CBF6088451BD425456F60DA1ED88DEB50693E149B3AFCDCD6348ABF2F459EA64DDB2EC7DCAE64A767665B4EB7E9DABE5D51C3AC8FD5656BC319A6774DB152CCED4C4BF1E95E8E1DFF31F56C9DF8B4C745913CA4F1DAC9C93FFFFD6AB77BCA189FAD04736C92127CAEA1E96F14FB51261ABAA0B95EDAF5698D000BC8F4027E8489B293678BEC0FBC1F4CDF76BE7D3EDEC62FD595E1280BC337928732CC1E4A6665F85DA1ADCED8E1B5E03327130D2471896AA93ABF84908585676E16AB90E466177B97812BB9ED62B0BB95B5F60AF5E34ACF66F39A17C929F190B143C1D729BEB3C589603458DEE06B8B1C953F7A58D4A756DC887BCCE3E231DBACDD05B09B3BCD367151B43675090050BC331400D21D42A115F925299EAEA2347A88B7AE2A12DF06963FC7EB4F7B1CEAF95AB851D8857B1560E5010D1BD5AD5DCF716E3B8431720C873150CE3C94CD43D93C94CD43D93C94CD4359FD191ACAFEC88AD276181BC8301CC218193EE3F9F8DA0754F1BB2C2DE21B7C6EC6CDBA4F52F5823789AB6DB879B28DF257C15947336C2E9A97211CED205965E9DA839A9D60B7FAB6F116683DC74AA00F25F13B5C28E9D9ED408D5CADECBEFC1EE5F1374775851B2FEEAE9C41FFFCA5CCA32FF12A4E76F852FDDD2672B359B375944F2F4F9FB7832B94ECF68F2745E458641D9FC437F7BA16DC60AB5AD773EA69D74328F30EA255459D7FBBFAB4C9B2DC615D5D2C8FCFAFBE3996DA57977BD93848F98FD313F782EB0A635E98341B55CE165FAAFB2A96FBBB557573B62BB1F8A2A6BB68F5E456F0E9A90F754F4FFD684BE9EA0E028CBA6E3BC5FAA2B4FD6E97E54EDB02EA13BF5DB953F522458D375A9D5E3643CE97F84F77C217F83AB568D36CB5753C8CB9EE1CAB81CC5BDFE85C740707C7727B03DFBCA4AECDEB5424695CA7823BD33A95DA3AA1D514ACBA07C7A5D4EB6A0BFBB28C5C8E8FCC53C907B829502722D07486B641015A8C615C00123347B6E7C8F61CD99E23DB73647B8E6CD79FA171EC1FAB3BDB318C1461387ED122E6B16B1EBBE6B16B1EBBE6B16B1EBBEACFD0D855CFEA5768566F3B8401920C47328EA479409B07B479409B07B479409B07B4FA3334A0B5A6B51DCE183986831928C7E796A326BFD36817DD259BA44CD83BF8CDC687F5BA96BD892AD90E45773AE3512D7F75768B4DB706F0CA1EF93513881713F0A55EEE546C259EBFECB2D4596F5EEF3BBB77AF6A23D4A3B6E97373A1A6CBDEF0075B59C1CB7FB61DE0408661E7C7C8987DF8D9879F7DF8D9879F7DF8D987AF3FB3C3579E67B9E1F3612DAFD17045F07A1CA6AEE2F23163FC11D19D554A1D3F3886D84A4526B9895FDC607A899AE213F2F4568EC09CA21A397F59C5EEEE921C8A74AC6F8086F3471CADF17B93F5691393E6339460D288580901A6BA176EDC7BC9D920852B365809BC7B3614F591050FD47402A458EA258FD0AB6906CAB1D44D34E750D38A9260A9CFB1E0E60F357D280996FA48F6B9A9A9C40AB1D48ADCBD405F96ACA61225C1521FD94177359D0029367AA17E1579DE57C89F8DDC6DF7DFE7A9B31051A3DAE9267276E8EFCF3DFB28BB7DB4C959447071BD3CC731CB2BC726FCE63008B6C06F5A63E438AB672411EF7D761B143990F01A7165B2898F45DDB8ACEB60F12F6C76ED5DA19CEA7781997376B637E57592990361B6926BF8F87C79A27DADD1A4EA5B5E937A27793D56BAE163945AB2F15FAE65077CA8F53A3DC990C3FA058F4C85D16373B0249B475B59491E31D2E4F229D9B87BB8EB0A394ACE420D6E0792AFBB75E4E77A42A6F6CC67EE1C514E3015642EDF66E31455AE229ED561CDE7805D4B6D0BD3CB5505E21C0262942BA49D5C4820EC2D356E52A5E5585DAAEAF31ED4AB645DFC9E67FB9D119A5A6623EC90CC1E9152E5E170346AB4C6B20CF03164B784C5B46F821E94D50A5EADAD8D21065496179841B1035F0F6CB7EF10DE644F716AB828C60831B23224C4E76E8E7A7B847EE3EB180FFDEDC86658A9EC3D39B7F8FC6597E49505FC3EF3985BBDF2D8729B3DF248727B447A9F91B3F12BC06CB87903B3696DE655444BB1788F9392E2BF730A38102CAE97C636AE598D0CDBB3FA5CC6E5407F60CA8FEE4C9967CF49BA8A8D9E036D998DAC49F0FAEC4F9A6CA42635791AEDA2C89C2D2234BBBDF547D88E517D2D4D191DCDA81BAFDB499E094A18212670018504986C7BC14D2BFC7C1B25CCE63E6BE9F510AAF380252BE35392179ECA9EACD70E835A9791273D39F78A5ACB3D4D244F299A08C5CB8F71DBD579EC8E34D5FA8F64275D3A393200787D416FB3A155E1A55C83D7293F452F1A2295C228D54C485B5BB5ABEEC0B1D460A87439A24DE67D609753BD2B47A729F0CD718037A9DB99EC73D4D7AFDC5CD2BB8C57FB3CFE232A1EDD9CBE5821AFB27086A6AF057E2DD6D515DF51517CCF723781F4C635F99C9526CA29FB62CBEF49B97A3C5E5C984E7D06024C7C304680CF49E5E262C4852FCD4AB13A5C468930AF985047CBEADC2CC71A8EAEA662BCB8809D74B3850F8A5D23FC690A3FBB9E8129AE250CC3440B89927BEC7C6FA21753E336AC260625587D469CEA5CEC3722A95B33DEEE96AF6836BFAD8E94343EB1816D4D8CEAFF9113A71B740344AF717554FB80A3FADC9409C87B7623A00FD903C475467E34962870B561DA6C8481A55829C69997A8C4213B4E6B051C361FFCEF758A85BA3915F5B2FB9F71E4463524EB2A4B4B37B3ADD3E5A9ABF300AB27376F8954A2DCBC725189727321389EA6B9398D817A3F04D465B4712AEF78B77322ED3A4F1EAA43FF5B1CC9EB868E78956CF181F2458E7EE13315C835FE2BEA66571116FCD1C479F09CC3A700A568E32806BD5EC769D5EBE1830CDA79574C761B3B12FADC84DA862ECB6C03F834D838A607520C0FA2783F802259123659C0FC2693E9CCC7045C154B4773E03859789C3C07CC750DF6E5D60854D8CC59FB221E23DDB7BB12BF3A6469744292A5D929497EA7646DA6F621774218A0BA9ED35E0BB0EA4AE79D81A43D1FF16D440607A62119D623DC92B800C778BDC9E2C005202CDE6545521A6C520480E7D723E31FC516C4A49496A5F6E5A3C222ACBEE00A435E249F44E5CA8FCECBF84F2F729BB7439B67DDFC086FEFF1702C1D9C1F5232B585FEBE585E46FE1A0B169FF56FDC79688CF7319EE5B8598E233ADB65F29046F8562FAB2E9B9062739FC4222B98CB0B280F41B378DFB264FD258E0A26AAA9573E528C4D012991860BD6529788F5702C5D247597C98DB7ECC04D76E01F07708CE760F81C2B9C638587162B7CBB413B684CB7EC88071E86456FCCF354DCEF6870B420E4F4EA002C6C99FC17CF2D526C9B1B7C174DE4759B06E434592268E8C4594088EB0DBAC61051F430B66E4279D5ABE36EE28B95283701C64ED401461821DDF55CA146C221C718CFE2E784D9DBAE907BCB370738E700E71CE07412E0E4ED6CB3948C6C9127F173B4A9A735871441BD489FB3C493597C4567BB70DE1CFB9D63BF871AADA57D1CC0E9B1759B34FC28377EB40B07DA85E71CC2659E638F4A4EB7993E03E6793B6EF8EDB8E78B9379B7EA1C817EA311E879B7AABFC07713B57213467313413BD0E099D5303E87CCE690D91C329B4366F39EC0392E34C785C0C638EF099CF704AAC8BD75E593A9F9676E4271F321F81F38F6361F823FCCA8DB7C08DEC6A199C38A7358710E2BBED1B022FE6AE9D3B423A2855F030CAAAE7D1BFCAF70AA667CC0FAB828B25552016218E5BBFD926D68E39EA7EB77F833EAD17062A3D032DEDCFF547FB8DA6FCA64B749562827A4D47BBA2AAED3FAE6D377F563F6D8272856D19A2D3FD276CDCBBB5170907DF76DA8C17F63043733BC328936A7595A9479842A8C454C92AE925DB4214B4B1181C0C2842C9A70793A9174CA59BC8B538C0BAA7476D9755229B3CA8AFFEB070210629CB497292FDACBDAB96069D3C90AEBBE05814CA72BA403FEEA0536A05D3855495C4D6D019EBE44D67906415075D7F5457A9FE5DB2AE120A0442B4D29C326BF197031453B0494E13B966F65BABBACE023A6827BE1589981B8FA83278418D617CD66899BAA882E730E009AD6096275D7ADCC37E304295723260CE804F1B30B8093366E787BFC90C7317EA28A7E73A8AFB2EEB919B2CEFA8FC34AFBF9A79F44FD0891DD0001C4672F20809FCCE1D40CFF691C3D30704C6B946B484C4C65D0D102DE1B1B7CB4003BA101A80751FB1A06173AD473193560BA8F5ADDCA2848010BC8AB21229C170C15C23C4362A17A5A0B0D77CF02A7A4A259D634642D0EBE4F1E14A4B62A7544D087848634DB80E838C9A374F538B941A7560BC44E9BF4430C404D610F74186AC0C57931D1B0AF98327CC670740DA0321967B7D67971BDBC1595C245057B9A50330511A854A57A859D01046A16A710C4E574A542082C26EB822940712B29933B0CF8422658ACA16A1C920018D5C548B33DCB06A37061F581CA55250C56BB7D557C07AD2518B865DDC750E8A3A1E6CB6303CDC1A94972379B1D961C64170A2F4E430142EF2B5CA58F31FB57ADF4D127FEA8D203BADCC12A7D14E75AB1D2C7F7A899E12DA67D98C371AA81B2481C97388CDB32826B0D15F5A0BD6B8C4BBA55E9F5273F02EC54FB9D7AC3E62800E3651D7888E339F313C5D4C8B3B01171A53EDF1A11598B282FD3096DFD69F41948ECBEBDE910785B4A9799070410FA3F120816C2AC7A25EB6D0D179610102C3A95D4D0BA8145554EC55CFB7A180D155FE222DE6C26D4AFB40A0DB7E3771FDF74CFD215F3C0BA960E44EDB8CAF778B4AA77822BF95A55D412875CC317E719120C7507071BCC13225A3666B0F1DC818C818A4159D5079C49E1C3950BE2699234C65814DE71D102EFD8FECAF27B52A2195BADC565923EB98511BF6B61321E080452DF083AD8921D264CEA2F5C9434C96C9D1E1E46A09272AAA9261D0521A2AC8301E496D19DEFB92A2344E2A3CC20D105C9EDC858C1415D77DD87AF706EA0C5CED0C0510D0C4F0023EC6D945CBC00A464F541C96F0C47326371AA99610B042FF8B6D2A9EC514DD6F2737A86BB00653198A9AD23BDA1DD7CB752B546439BDE1AA71B8428460347DEF16C5EFFE3EC758637198F07B7F676E9C3DFE7DC9664A84DFFF56DF67F5DF98CBABCF171F729AEAEA42D9C6F4FF50BB2566D186C7DEADBD9F5CA94CD3EF3B0FD9BB800EE2A59366E8ED34F19551FC315B6B752CB3D248A2EB3875B4E21CCABD8775F85948615A912FCC24DAFA25DC10B17CC41C623F44FCD6B23E97D26411741C8D62D9978309D13AFE8C2BAEB9946E89A649987BCDC41BC75D7D33D42F3316BAD9C6FC52A04844BE7759F249B4D923E9CBEAE04D7B50E88C84A1E26043D680DFB664CA217D8F18DC60100C9E0087E5A7E9C3CFF31B017F040E2044033E64545073667A3BB4BF98CCDA67E27B8EBD1A8DE68A690BB20D5F20E8A1D95351A273ED07CEE75922E4EE7D0A3B2AEE2459610CAB93A0DEDD5C3010A00AA05D2BD1D000A4A69AF46C81EA9DE0FEC16828A6317B33F3BD0F835065034F7674F0A1F13DF9F3D862B147E07AE1678C7DE78DB2A8BAFB13ED4816EA83BA8114DF2E67A2DAA800731B255EFED1E06D42A5587A1AB08D82D7AC040AACB7348B871BF8CC6778402D7FF28AB5F1A08187DD9AB86C0227AC517F30B2FB12369869E0FF1FD0DF6215CDB70EA94A00F8623699E211CA0A828E3BC5E89BBADFF27C41341425625F95923AE4C643EECA406095E00C22B2BA7AE7A725B8F852C9A8B9C4383E4D326FBAE00918E8C85499F74485081CB2DACB496651CC888730F009B06307CF56DEBD7D3480568E1155FDAF54B325862CB69AEC130D5FAE0D5515909B2946BD32B9A488D015D86C91E7B318DFEC309B20605B3CD7B1C7C1DC40C7D2A281B63EE6E88B4D127F2B5DE8BEB650DB3C31C261BF501B475296F74D06CCB7760636787B7033FB03432FC463EB86400C1099CCEFC2346B59C7F898B1D1214DF9E6F9F8FB7F1CB22CA516A297C568D221CBEAD46276A2DF20E751A48A6933C3DB6263202A73269264B34510575A7C128983A4B8A55F61CE74AB86289296C010487862F8941F835CC308E8233452D46C1DA1F59512AE16C4848618C4A3C347C098CC0AFD501D328B852D060144C5DA4489F68A5042B869642169B7E68E0125B835FBB34DF28105353621494FD6375A784B0011D85AE61DAA1218B6F017E85923CA3208A5460BC7DE22C9CEA19EE2ACAD74AA882C829708124878631A959F8350DB08E8238653D46C1DD4D9C6F9334DA28A18E25A63007101C1AE22406E1D733C3380ADA14B518056BDF922252C2D99090C218957868F81218815FAB03A65170A5A041C0FDC6D7E949863AD4B61083806C97F6E73E2EE4AB4E1439149AA549826E4F068A2A5692A4F31ABB15199A032498D5D12666C802AE950AF454DDEF79B6DFDD76BF84FB858654F4457B444AB097C6AA3C614D3CDEC3C7B114AFD2490E07B740D6A5739473C89B5F6EB2A738C5170B1DAF56D95EB04CDFA693F5DA7D0B7BA550AB33BC33BB4FF58235D04E9CBA6E685D5D0ED395CC3AEF5110767057578D8AB3912FB1D2C2DAF85BFA914ECFA888F85E0F8CFA5C70AD47933E7CC4ACF9A6F7D65D9BE7F06840FFD5D3F36540F938F5D2D0DABE75D795C83ACF908F81D47395DBEE97D08BA2B8185F0A4C0FD21B51398BB4F2FA0880C08E1C24007CAEDE95D0983E2BE41F009537F17647DC2A7F7B8A661A42407604837EAAFB18047A94D2C3470BE8343F9D1D68264E4DB7C49620A34B669FF518F80A76BBD5044032C2DD56462019FB6A2B1A24FF16F3F151259275597F38245CB0E5E3540CA21B010EDC5C474002B1BBF1C0F6B18E0FB3B1F7B29A606F0ABB59190CD2977DE95D7336EDBE48F926B3643D061C78B98E0003E2F7399E480ABD649098F3B61641F083F44C725372F000318E004B753542E0B457E6B82C51694A048A47B76173410F06E6CE83FA80E48D44C139C5B356603CE804BD83666A001A27BC6D01A2F163DC84F2157E02CEF18739735FAFF4079751E6F854C90E638E4F8124C8CC6ECAC8197D7A6680A2494CCF6820D94ECFFC38D9E3E32BDCCCCE044963CEEC2804A90CBE6EAA56B2703B355FC8A462299EF1FC22412DF3151B177DC1E20A531E1727151CB06B00930D0E08CBE5060D733F374E3F3765A8D54FD62BA1AD21E501AE4D0EEDCB4D0EAE9049158052B14D03B37509260CDB65F29056EFF7A800B727E6C08320782BBE21C740F2BAEF18278144893693C0E2B72C597F89A3825EA88081425073704252BC1538F26C244740CF390940CAD419059167F15D528EB7BA54652F815943F366D797EAF2596B30227CDCDDED7A48D8097D55AB0D7246BED390079C919726C746D0541627D57134A9D5C90194DEE4C4746C804E766AAA8ED869CE4D076E9730444CD200EE9745405808AE004E97722498A07707246DAF6B2A81DF1A3AA36CCA180D31236FCB50C7CA94F665D4407170F8C2F760371AAC429EDC3041D3A8473768201DF0D98D09406D42DB8334BCFD69ED0F6A7038F50D42E3616C8C2D421A689AC61E2166BAC89F52B8AA5FEDE0FB84E67D4633AD8A691A933EA6BA05BA8D0CC469ED171AAD1B935B445EEB1E760C19A16F8A5B86E016E1A7E79BBB3D0BC459777B5306DD54B7614C088313DC886184C529EEC4186FC95B82AD37BCD07DA06BDC232D6F8F8393F117B50F733D7BECA5EC71D0329505ECC35BBB767E339B12587828794BD7AA11A5B2CF363428C658D50B0D8A71D7F20E6E19EF1056F00243689C75BB435AB29BD079FCC0D898D042DB21AEB14D7F792D349EC658543BA4F53460294D69EBA56A3D6A47F0C6990AE9D61E413FCE748853855383D4B416C502773E135D0A7B0BAB60B7DCF2D8D5FCDC572982C8B0AF9A2884DECE9983D12139AD9306077CC80080E954575D4707DD04D75ADFC4322BF169C287DD4787DF240FB94FFE803BF22B517D209E1271C4791BBCCDD6F1A7242FCAB3A88CEE22FAF1D9866B1997C315E7F7EFEA0468BD7FB97A8CB7D16FEFD77719824A7447ACA81700D086F2B17901E1F56748324E918BFD5AE0979818B1F567482C4E918BADACB92A2FD2FB2CDF46B587CB64021141594274D2FC9B37D2804C9B1438279C9828D8AD7D386CC1CF02A7F13341C9F25C06C7F378C0AA5305E0AA09A45961C3562F040219756970364DB242811EF238DEC615E0D96C8844301F225D924F1B0F0572E993A03CDA54050474EB652C00BA24B0FEEB5415949D6FA364B38CF367B0910E52A19C08027593D5216F81E15A0291F96A1A851202EB03928C2B1A79DE884C21F7645D406CAC0A1C42500F0EAD5C1738637E2E728C760BFD2C46BB2410A34DAA8909633503C6CAE653D0A27E6B0FC8B74D8072AAD394A52FA2BC44FE41FD583927A3010D3FCF019924F38616C8B24B81326A12151C8BB888371B30833E0974309A547916505C82C90C2282B285E86418ED7747B1C0ECD34034B6C91AB97C8AAB0914D80C581A61AE3D996AE6F0083E4C1666A9348ED7D4CDFE39E49771B32449F8D992548A0386C8C80C8968B85036F149B2D924E9C3E9EB0AF4C487C9508E2485FAC8D8550C9A83ADE2459608BD1A905A547C80415D373C1950528B26146934A455F029D3758EE68CA808DF5067044F35001AD0C364C964A088E071A7F90EC2A04E9276FBAFD8D5E58CDE8354B8FBEF08E4163CBB43BF4B044DA8359189504E64BA2C9F6643339B479300CAAFD214301915659C73B3182683F8232854CBC2A91E32915F26C5CAA9683F6DB2EFC2CC7A027E862D8D6AA66D8BE4B83C10113FF3219D52FE8B4D54E2393F37EF9E809F6F4BA35A68E4AC0BCBDBA50BB26C49A4D3BBE7E36DFC423C890DCCF1681278A237A45299593E9F25C52A43534349F6001947058652498D3FB2A294A8409170B21F5029657D9122CA6825C99DA5E22840132AE9F08FD59D24FF2105276F924829DFBAB75BA1799E247B9090A30540ABA4CC4D9C6F9334DA485401C8388A30944A6A7C4B8A48A20245C2C97E40A590759E6739ECB8F74960564DAA3C8B3FE2681DE768CAB6CBD2021A456802283B9A469227DEC59AFE67BC2A6328B2314885721B1048B2BA8CD2877DF40015AC4F82326953D57DDEEBF4244300FF12FFB98F0B91CF4B138A7C5E9A5657176EB5722995B451AC680E1F671815526BA8A534C4E268D2EF79B607A7FE7D1A2F105525AB05C12A528E8B46A50B73530CBBB5D3B69BEC29E684ED011AE1CCBF2593E78EFA36D1DCA44FE444A6946726CD3446BC0E021109E644049D8202D74B5EAE6D0A98559DA8203FCF9E937405C6318834308F3E591AE2ABBA95782D58160168E0A01F45A61A3D3D5E5C700C49A5F3A3A60D8946BC963B923314C258ADEA384E3108CB4B5129E4AF56F69BE885936F97024655EB4405F9F176B77C4508D8DE205F37FEBCDFDE8116E6D08179C3A40A8A0C7650822A0C28789913447203D467A398CCEACF1C3F462B447EDEEF2E1605C909324998BCA3D45263F03C83489101A1449501ADC6AA815409A5DC75B32576A689F226C8240A1094EA5A903B94446A9074123D48526D4434F7962840A2A154C34443ACAE8E821E6A0A68E7DC6DE852B0816AF1B5DAA6386769965A79C97B22A54E08EE7F882D59CCAE97DB7A6F134132DCF752A7D3FBC3FA0D7AE48E2924BCFDC0EC28036E6B2198BA6FF476E8A1EA0AC5EAF6FDDC767B90D8B2B1447C75A94D4E95BADD37413199BD4B2427FEEAA0A8F48E2D7199B9D43E0ACFDB99D68890EE35D337075ED1BA8524B3B6E0917A2CD0401AB91FB0E2AF3F589BA06DB3B5387E9BAED22DD50BD8A6DB59D02DB9378D2D1D44C657999EA5553AF71F458565B6D7D5E5E56F9CB328B21AA045E4A140AD65502B93741BFF4486E05C5D015ED04116BAFB3885A2567B0B8B66F3A1A0B4241D5F6D60A363A5F9E0FB048A5D6F48D3843E9F29740318EEED1C18A94D726DAABE12A406EA487D748DE3996071BDBCA5B390D902E0F15034811462272E20A84AB53713B851F616C80A30972AAFB7020F65093712D702D5B6051B99B1BF070C3615E79EB0618F43ED15AEFB99EEA3B8F074495D154B349E92E95E86526FC512F68964BA978ED057B1985DDB2ABD9E029BF78E4FB0839DD3728148862B93E1DA50B5137BA98C5E3D4FC50C6CB70DCFA2604A9F2608D78B37FBF5155D4B0175289F923A975089E8BE393347BD58DCC9E55B6248E8446D880FE7E9A9C8ED790A450888C84361803E3B528759BB8FEE4C42AC360B2CD153395239E81CB32B460DE55E27418929521FC56E8918E4FBA86741436768DCB771DF70660E7F89CACB27765F70EE01B74A82FCCC9A0B53B427F5542CD1D0CA0A0494629266B8654483FD1C9FF8F00D817D3B3E028854D745F537CB830E2AC245935F5B4428CC3F4C59E92F3F1E19A2F86C30AB3AAEAC14F6C2943F40D4062CB8E24448953740D1C78E21B69B5715C38602F24098A3CF22D732FAAFEE4CD29E6C1587D1F8D45E826ABC43D24333700FE55A2184952E440843EEAD586321E4327B209E1C11596348E94C799897D8CE3E64FFFFCB3BB71DA761200CBF0AE205FA021512B08090804554705B65B7A6AAD4A6550F887D7B52274E7C98194F529FD8BD5AB59E7132DF3AB13DB6FFCA82E02D423F7DEE6F109AB52F14F77CBC160E7EE63D756B504BB56E728F58D7758C9FCBA25DFF1E340FF4E32860073C2848474086641630B0C02F1B9FAE4100349C7D0E89FA934C28966EDD8C8785D393DC1250963D11DEB13D77485FCC5A567769401A83B59D81F48BF99A24A4428CCA40BB70EDA14D4A7248D9A631E138F9CB188F0291CF736CC2A7F1523DF1A6420BEB99C05C62FEC761751AA31EDBE4664452DC85246259C4006088D2B4430B5073667278D494C136893357881DA221B08305A91B514FB3A3E4D33DD1DAF75942D5156F96BA660DF01C23A6F86DBB0239F2DEF5AFA9671850F3699B87511016C1A0A1E301D01BFAC2B7657B340443510118BAB8CC5A5D089059E09B072002F08285ACDEA77259C91738641CE0E6012FFD4A400D66710C14540F4699C7E8CEF221E9849C98CF05681DFFF1B004AB3426980ED5CD3838A9769F4B9A747B3A44A638CED215EE7211F95CF0B030E5B0457B68C22E2400C1C240B21E8FDC4F084480B8180793EB46A24295CE142EC0A0586496181A0797E942A202D5D81426ABB05844AE621B8792E34582C2B4E3142BB7BC585CA6C01C0795E1416282E4ED1422B3AC583CA0001E8712E448C22234F91433D0A4587480601F079CEB466243D5031534C0A0586496C0200797E942A202150E1526ABB010446ABC07A8BF2D9DB24E3E0FCF438EA8C53F4085A5008D512A2DEC47D7EAAAE8C155137278D34EA5481DBAA5255A076E5D034DC9F51A57274FADDCE8259E4D5B86A29FE91F727F8012C45BF68201C4F600C7180F618426019C30B545FFCCC4E9501A01056FBB08601E79CF483224BDEC1E2569E21A118BDF13954C1CE9C036AB8CAA02DEB049BBED129696661DB6431973C0432184F316C68E63BD9CB37F19E8E89CB29B31591272E4615CD496681F930FE6221279B202A72C3C066293086A1B638F485E0C5217D04FE06A8687A08B0ECAFB6EBF283668354641728394799ABC60663CF0295FC88C1C088D3ADD9A376458B9D2CF80F3E3F5F0E9880FB68EE6C23A1FA11994830DD4CAA406A01E8FF0A3505A23D4864C297E06C3430E4ABD3E7146A6856092B1925D31661BA52B362F869E5C8A83C1D72B11E6897AA5BC78905E09300BD82B650D197B02FD1410CF288116FB5E6177EA1CBFA49D7AAE56A7F740CE85D87D97EDF9625A1D06013F5FCB77E641BCFDC86DD9283589F22930077716004761DD0E1F554CFF8F90EA6AEB53986AFE2C04AE60BC4D00D77F2F18AB94691F3713325D524C850C517A8410AC371F0E11BE4FCDE381073A6D2F639978464E166DA754B3C512504DED6251EF7C7D6C269CC61B88313C76EDBD6F2D67346C7CCF8344D28986859996B08C23E725F2A0C0560920BBA0CB04B903E7E7645CFBE449994C88BC7999C12E4A62264FD8E8FB998102F38D146EC97DCF841C0DE198294993BA0562E3184E0BA47D5F540BC4508C4C3210FE79B30C79B18E9B0B279D067B98C4C13166DE9B6ECA9B05C5C8396E86E96D522CF4962CC88E98644CDF8E355C088B3E74D8BC0956B2B955A2B0FD93A998F3A84441F2274E79E74CA970782749F1E647894274DF92BE741CE81234A4A2DEF213A63A45CD72D2B423D5E71B17608D12748F67DB8A806047E5BECB4A7BE7463772A657E2242F37C2B1CBC7852E1D87C3389FB535C91F2BD9D4E2D897CD67EDAF50775F341FCFFB63B5165FF72BB13DC96FE7B31FCD7476B313EDA73B716A9A455FC5BCA9B316F2BE864A95CD55ABF5FB717F104719857E47CA4415F76717CFD5AA3A576F8FE7CDEF26DCA6F8519C4E9B7AFDFAD5AF6A7B11D773970F62F5B9BEBF9C0F977313B2D83D6C8DA1E67C465F7F3E73EE797E7FB87E3A8508A1B9CD4D1382B8AFDF5D36DB557FDF1FABAD7DBC13ABE27D43FF9368BE6FFF97E7E6AF583FF5357DDBD7CC8A3A7C77E220EA95A8CFD7EDD7DBA6B2D37DBDA8FE8829F7F6F324BE8875F5F824CF4DADAEE781B04AFCFF0813FBFC6E53AD8FD5EED4D531F8371F9B36BCDAFD7DF30F9DAC6620CD350800, N'6.1.3-40302')
GO
SET IDENTITY_INSERT [dbo].[Accounts] ON 
GO
INSERT [dbo].[Accounts] ([AccountId], [Username], [Password], [PIN], [ParentId], [ParentTypeId], [RoleId], [IsActive], [IsDeleted], [LogTries], [IPAddress], [AccountAvailableDate], [LastLoggedIn], [PasswordExpirationDate], [DateCreated], [UserId], [NeedsUpdate]) VALUES (1, N'SDGAdmin', N'fKWinhnYqA12KvAikmAV9IR3FzDWgidTXbkSjqMnAmA=', N'None', 1, 1, 1, 1, 0, 0, N'::1', NULL, CAST(N'2019-07-30T21:30:21.770' AS DateTime), CAST(N'2020-07-21T21:41:15.007' AS DateTime), CAST(N'2019-07-22T21:41:15.007' AS DateTime), 1, 0)
GO
INSERT [dbo].[Accounts] ([AccountId], [Username], [Password], [PIN], [ParentId], [ParentTypeId], [RoleId], [IsActive], [IsDeleted], [LogTries], [IPAddress], [AccountAvailableDate], [LastLoggedIn], [PasswordExpirationDate], [DateCreated], [UserId], [NeedsUpdate]) VALUES (2, N'SubPartner', N'spJrATCPY0lWTgObOwjhGg==', N'None', 2, 1, 1, 1, 0, 0, N'::1', NULL, CAST(N'2019-07-31T07:37:09.413' AS DateTime), CAST(N'2020-07-29T20:55:18.900' AS DateTime), CAST(N'2019-07-30T20:55:18.900' AS DateTime), 2, 0)
GO
INSERT [dbo].[Accounts] ([AccountId], [Username], [Password], [PIN], [ParentId], [ParentTypeId], [RoleId], [IsActive], [IsDeleted], [LogTries], [IPAddress], [AccountAvailableDate], [LastLoggedIn], [PasswordExpirationDate], [DateCreated], [UserId], [NeedsUpdate]) VALUES (3, N'Reseller', N'mVQHYWS6/sCzwYIqyIi72kuNfuqTRConNi7ZZgp5nI8=', N'None', 1, 2, 1, 1, 0, 0, NULL, NULL, NULL, CAST(N'2020-07-29T20:56:14.300' AS DateTime), CAST(N'2019-07-30T20:56:14.300' AS DateTime), 3, 0)
GO
INSERT [dbo].[Accounts] ([AccountId], [Username], [Password], [PIN], [ParentId], [ParentTypeId], [RoleId], [IsActive], [IsDeleted], [LogTries], [IPAddress], [AccountAvailableDate], [LastLoggedIn], [PasswordExpirationDate], [DateCreated], [UserId], [NeedsUpdate]) VALUES (4, N'Merchant', N'E6saGIoFMW8wxy11tT4j489e4PQXHPNAOVoI3bqSDcw=', N'WLDBAny24pVSoJq2HEjCeg==', 1, 3, 1, 1, 0, 0, NULL, NULL, NULL, CAST(N'2020-07-29T21:43:42.650' AS DateTime), CAST(N'2019-07-30T21:43:42.650' AS DateTime), 4, 0)
GO
INSERT [dbo].[Accounts] ([AccountId], [Username], [Password], [PIN], [ParentId], [ParentTypeId], [RoleId], [IsActive], [IsDeleted], [LogTries], [IPAddress], [AccountAvailableDate], [LastLoggedIn], [PasswordExpirationDate], [DateCreated], [UserId], [NeedsUpdate]) VALUES (5, N'Branch', N'X2OQSAOowK7RS6shIJ6nWbmDIYS9PkxBaV74AyAX5Wo=', N'+46mu6fiOwxgYYZvpY1wyQ==', 1, 4, 1, 1, 0, 0, NULL, NULL, NULL, CAST(N'2020-07-30T06:01:02.787' AS DateTime), CAST(N'2019-07-31T06:01:02.787' AS DateTime), 5, 0)
GO
INSERT [dbo].[Accounts] ([AccountId], [Username], [Password], [PIN], [ParentId], [ParentTypeId], [RoleId], [IsActive], [IsDeleted], [LogTries], [IPAddress], [AccountAvailableDate], [LastLoggedIn], [PasswordExpirationDate], [DateCreated], [UserId], [NeedsUpdate]) VALUES (6, N'Admin', N'fKWinhnYqA12KvAikmAV9IR3FzDWgidTXbkSjqMnAmA=', N'None', 1, 1, 5, 1, 0, 0, N'::1', NULL, CAST(N'2019-07-31T06:54:46.053' AS DateTime), CAST(N'2020-07-21T21:41:15.007' AS DateTime), CAST(N'2019-07-22T21:41:15.007' AS DateTime), 6, 0)
GO
SET IDENTITY_INSERT [dbo].[Accounts] OFF
GO
SET IDENTITY_INSERT [dbo].[BillingCycles] ON 
GO
INSERT [dbo].[BillingCycles] ([BillingCycleId], [CycleType]) VALUES (1, N'Weekly')
GO
INSERT [dbo].[BillingCycles] ([BillingCycleId], [CycleType]) VALUES (2, N'Bi-Weekly')
GO
INSERT [dbo].[BillingCycles] ([BillingCycleId], [CycleType]) VALUES (3, N'Monthly')
GO
INSERT [dbo].[BillingCycles] ([BillingCycleId], [CycleType]) VALUES (4, N'Quarterly')
GO
INSERT [dbo].[BillingCycles] ([BillingCycleId], [CycleType]) VALUES (5, N'Annually')
GO
SET IDENTITY_INSERT [dbo].[BillingCycles] OFF
GO
SET IDENTITY_INSERT [dbo].[CardTypes] ON 
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (1, N'MasterCard', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (2, N'Visa', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (3, N'AmericanExpress', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (4, N'Diners', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (5, N'Discover', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (6, N'JCB', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (7, N'Chinabank', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (8, N'Citibank', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (9, N'Barclaycard', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (10, N'Debit', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (11, N'Cheque', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (12, N'Cash', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (13, N'EBT', NULL)
GO
INSERT [dbo].[CardTypes] ([CardTypeId], [TypeName], [IsoCode]) VALUES (14, N'Offline', NULL)
GO
SET IDENTITY_INSERT [dbo].[CardTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[ContactInformation] ON 
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (1, N'Unit 1902, 139 Corporate Center, 139 Valero St.', N'Makati City', N'Metro Manila', NULL, 1, N'1201', N'+639333491863', NULL, NULL, 0)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (2, N'Unit 1902, 139 Corporate Center, 139 Valero St.', N'Makati City', N'Metro Manila', NULL, 1, N'1201', N'+639333491863', NULL, NULL, 0)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (3, N'N/S', N'N/S', NULL, NULL, 1, N'N/S', N'N/S', NULL, NULL, 1)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (4, N'N/S', N'N/S', NULL, NULL, 1, N'N/S', N'N/S', NULL, NULL, 1)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (5, N'N/S', N'N/S', NULL, NULL, 1, N'N/S', N'N/S', NULL, NULL, 1)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (6, N'N/S', N'N/S', NULL, NULL, 1, N'N/S', N'N/S', NULL, NULL, 1)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (12, N'N/S', N'N/S', NULL, N'000', 1, N'N/S', N'N/S', NULL, NULL, 1)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (13, N'N/S', N'N/S', N'Metro Manila', N'000', 1, N'N/S', N'N/S', NULL, NULL, 1)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (14, N'Street', N'City', N'State', NULL, 1, N'1000', N'20193801982312980', N'120398102398', N'03918039812039', 0)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (15, N'Street', N'City', N'State', NULL, 1, N'1000', N'20193801982312980', N'120398102398', N'03918039812039', 0)
GO
INSERT [dbo].[ContactInformation] ([ContactInformationId], [Address], [City], [StateProvince], [ProvIsoCode], [CountryId], [ZipCode], [PrimaryContactNumber], [Fax], [MobileNumber], [NeedsUpdate]) VALUES (16, N'Unit 1902, 139 Corporate Center, 139 Valero St.', N'Makati City', N'Metro Manila', NULL, 1, N'1201', N'+639333491863', NULL, NULL, 0)
GO
SET IDENTITY_INSERT [dbo].[ContactInformation] OFF
GO
SET IDENTITY_INSERT [dbo].[Countries] ON 
GO
INSERT [dbo].[Countries] ([CountryId], [CountryName], [CountryCode], [DateCreated]) VALUES (1, N'Philippines', N'PH', CAST(N'2019-07-22T21:41:14.353' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Countries] OFF
GO
SET IDENTITY_INSERT [dbo].[Currencies] ON 
GO
INSERT [dbo].[Currencies] ([CurrencyId], [CurrencyCode], [CurrencyName], [IsoCode], [IsEnabled]) VALUES (1, N'Php', N'Philippine Pese', N'PHP', 1)
GO
SET IDENTITY_INSERT [dbo].[Currencies] OFF
GO
SET IDENTITY_INSERT [dbo].[DeviceFlowTypes] ON 
GO
INSERT [dbo].[DeviceFlowTypes] ([DeviceFlowTypeId], [FlowType]) VALUES (1, N'BackOffice')
GO
INSERT [dbo].[DeviceFlowTypes] ([DeviceFlowTypeId], [FlowType]) VALUES (2, N'EMV')
GO
SET IDENTITY_INSERT [dbo].[DeviceFlowTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[DeviceMerchantLink] ON 
GO
INSERT [dbo].[DeviceMerchantLink] ([DeviceMerchantLinkId], [MerchantId], [DeviceId], [AssignedDate], [IsDeleted]) VALUES (2, 1, 1, CAST(N'2019-07-31T06:46:35.457' AS DateTime), 0)
GO
SET IDENTITY_INSERT [dbo].[DeviceMerchantLink] OFF
GO
SET IDENTITY_INSERT [dbo].[DevicePlatforms] ON 
GO
INSERT [dbo].[DevicePlatforms] ([DevicePlatformId], [Platform], [DateCreated], [IsActive], [IsDeleted]) VALUES (1, N'Android', CAST(N'2019-07-22T21:41:15.020' AS DateTime), 1, 0)
GO
INSERT [dbo].[DevicePlatforms] ([DevicePlatformId], [Platform], [DateCreated], [IsActive], [IsDeleted]) VALUES (2, N'Blackberry', CAST(N'2019-07-22T21:41:15.020' AS DateTime), 1, 0)
GO
INSERT [dbo].[DevicePlatforms] ([DevicePlatformId], [Platform], [DateCreated], [IsActive], [IsDeleted]) VALUES (3, N'iOS', CAST(N'2019-07-22T21:41:15.020' AS DateTime), 1, 0)
GO
SET IDENTITY_INSERT [dbo].[DevicePlatforms] OFF
GO
SET IDENTITY_INSERT [dbo].[Devices] ON 
GO
INSERT [dbo].[Devices] ([DeviceId], [MasterDeviceId], [SerialNumber], [DateCreated], [IsActive], [IsDeleted], [IsBlackListed], [KeyInjectedId]) VALUES (1, 1, N'1234-2345-3456-4567-5678', CAST(N'2019-07-31T06:40:08.267' AS DateTime), 1, 0, 0, 0)
GO
SET IDENTITY_INSERT [dbo].[Devices] OFF
GO
SET IDENTITY_INSERT [dbo].[DeviceTypes] ON 
GO
INSERT [dbo].[DeviceTypes] ([DeviceTypeId], [TypeName]) VALUES (1, N'PaymentDevice')
GO
INSERT [dbo].[DeviceTypes] ([DeviceTypeId], [TypeName]) VALUES (2, N'Printer')
GO
INSERT [dbo].[DeviceTypes] ([DeviceTypeId], [TypeName]) VALUES (3, N'Miscellaneous')
GO
SET IDENTITY_INSERT [dbo].[DeviceTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[ErrorLogs] ON 
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (1, N'Submit', N'Merchants Registration', N'Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index', N'   at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
   at System.Collections.Generic.List`1.get_Item(Int32 index)
   at SDGBackOffice.Controllers.MerchantsController.Registration(MerchantModel merchant) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 296', NULL, NULL, CAST(N'2019-07-30T21:12:07.417' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (2, N'Submit', N'Merchants Registration', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Internal.InternalContext.SaveChanges()
   at System.Data.Entity.Internal.LazyInternalContext.SaveChanges()
   at System.Data.Entity.DbContext.SaveChanges()
   at SDGDAL.Repositories.MerchantRepository.CreateMerchantWithUser(Merchant merchant, Account account) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Repositories\MerchantRepository.cs:line 47
   at SDGBackOffice.Controllers.MerchantsController.Registration(MerchantModel merchant) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 392', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Core.Mapping.Update.Internal.UpdateTranslator.Update()
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.<Update>b__2(UpdateTranslator ut)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update[T](T noChangesResult, Func`2 updateFunction)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update()
   at System.Data.Entity.Core.Objects.ObjectContext.<SaveChangesToStore>b__35()
   at System.Data.Entity.Core.Objects.ObjectContext.ExecuteInTransaction[T](Func`1 func, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction, Boolean releaseConnectionOnSuccess)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesToStore(SaveOptions options, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.<>c__DisplayClass2a.<SaveChangesInternal>b__27()
   at System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy.Execute[TResult](Func`1 operation)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesInternal(SaveOptions options, Boolean executeInExistingTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChanges(SaveOptions options)
   at System.Data.Entity.Internal.InternalContext.SaveChanges()', CAST(N'2019-07-30T21:34:19.000' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (3, N'Submit', N'Merchants Registration', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Internal.InternalContext.SaveChanges()
   at System.Data.Entity.Internal.LazyInternalContext.SaveChanges()
   at System.Data.Entity.DbContext.SaveChanges()
   at SDGDAL.Repositories.MerchantRepository.CreateMerchantWithUser(Merchant merchant, Account account) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Repositories\MerchantRepository.cs:line 47
   at SDGBackOffice.Controllers.MerchantsController.Registration(MerchantModel merchant) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 392', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Core.Mapping.Update.Internal.UpdateTranslator.Update()
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.<Update>b__2(UpdateTranslator ut)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update[T](T noChangesResult, Func`2 updateFunction)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update()
   at System.Data.Entity.Core.Objects.ObjectContext.<SaveChangesToStore>b__35()
   at System.Data.Entity.Core.Objects.ObjectContext.ExecuteInTransaction[T](Func`1 func, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction, Boolean releaseConnectionOnSuccess)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesToStore(SaveOptions options, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.<>c__DisplayClass2a.<SaveChangesInternal>b__27()
   at System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy.Execute[TResult](Func`1 operation)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesInternal(SaveOptions options, Boolean executeInExistingTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChanges(SaveOptions options)
   at System.Data.Entity.Internal.InternalContext.SaveChanges()', CAST(N'2019-07-30T21:34:51.873' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (4, N'Submit', N'Merchants Registration', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Internal.InternalContext.SaveChanges()
   at System.Data.Entity.Internal.LazyInternalContext.SaveChanges()
   at System.Data.Entity.DbContext.SaveChanges()
   at SDGDAL.Repositories.MerchantRepository.CreateMerchantWithUser(Merchant merchant, Account account) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Repositories\MerchantRepository.cs:line 47
   at SDGBackOffice.Controllers.MerchantsController.Registration(MerchantModel merchant) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 392', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Core.Mapping.Update.Internal.UpdateTranslator.Update()
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.<Update>b__2(UpdateTranslator ut)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update[T](T noChangesResult, Func`2 updateFunction)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update()
   at System.Data.Entity.Core.Objects.ObjectContext.<SaveChangesToStore>b__35()
   at System.Data.Entity.Core.Objects.ObjectContext.ExecuteInTransaction[T](Func`1 func, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction, Boolean releaseConnectionOnSuccess)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesToStore(SaveOptions options, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.<>c__DisplayClass2a.<SaveChangesInternal>b__27()
   at System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy.Execute[TResult](Func`1 operation)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesInternal(SaveOptions options, Boolean executeInExistingTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChanges(SaveOptions options)
   at System.Data.Entity.Internal.InternalContext.SaveChanges()', CAST(N'2019-07-30T21:38:26.050' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (5, N'Submit', N'Merchants Registration', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Internal.InternalContext.SaveChanges()
   at System.Data.Entity.Internal.LazyInternalContext.SaveChanges()
   at System.Data.Entity.DbContext.SaveChanges()
   at SDGDAL.Repositories.MerchantRepository.CreateMerchantWithUser(Merchant merchant, Account account) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Repositories\MerchantRepository.cs:line 47
   at SDGBackOffice.Controllers.MerchantsController.Registration(MerchantModel merchant) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 392', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Core.Mapping.Update.Internal.UpdateTranslator.Update()
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.<Update>b__2(UpdateTranslator ut)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update[T](T noChangesResult, Func`2 updateFunction)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update()
   at System.Data.Entity.Core.Objects.ObjectContext.<SaveChangesToStore>b__35()
   at System.Data.Entity.Core.Objects.ObjectContext.ExecuteInTransaction[T](Func`1 func, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction, Boolean releaseConnectionOnSuccess)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesToStore(SaveOptions options, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.<>c__DisplayClass2a.<SaveChangesInternal>b__27()
   at System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy.Execute[TResult](Func`1 operation)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesInternal(SaveOptions options, Boolean executeInExistingTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChanges(SaveOptions options)
   at System.Data.Entity.Internal.InternalContext.SaveChanges()', CAST(N'2019-07-30T21:38:51.793' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (6, N'Submit', N'Merchants Registration', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Internal.InternalContext.SaveChanges()
   at System.Data.Entity.Internal.LazyInternalContext.SaveChanges()
   at System.Data.Entity.DbContext.SaveChanges()
   at SDGDAL.Repositories.MerchantRepository.CreateMerchantWithUser(Merchant merchant, Account account) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Repositories\MerchantRepository.cs:line 47
   at SDGBackOffice.Controllers.MerchantsController.Registration(MerchantModel merchant) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 392', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Core.Mapping.Update.Internal.UpdateTranslator.Update()
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.<Update>b__2(UpdateTranslator ut)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update[T](T noChangesResult, Func`2 updateFunction)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update()
   at System.Data.Entity.Core.Objects.ObjectContext.<SaveChangesToStore>b__35()
   at System.Data.Entity.Core.Objects.ObjectContext.ExecuteInTransaction[T](Func`1 func, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction, Boolean releaseConnectionOnSuccess)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesToStore(SaveOptions options, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.<>c__DisplayClass2a.<SaveChangesInternal>b__27()
   at System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy.Execute[TResult](Func`1 operation)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesInternal(SaveOptions options, Boolean executeInExistingTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChanges(SaveOptions options)
   at System.Data.Entity.Internal.InternalContext.SaveChanges()', CAST(N'2019-07-30T21:40:24.090' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (7, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2086', NULL, NULL, CAST(N'2019-07-31T05:59:23.537' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (8, N'View', N'Merchants: UpdateInfo', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.UpdateInfo(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 1257', NULL, NULL, CAST(N'2019-07-31T06:01:53.590' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (9, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T06:04:39.790' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (10, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T06:05:40.153' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (11, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T06:32:12.207' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (12, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T06:41:07.843' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (13, N'Assign', N'Merchant: Assign Device', N'An error occurred while updating the entries. See the inner exception for details.', N'   at SDGDAL.Repositories.MerchantRepository.AssignMerchantDevice(DeviceMerchantLink mDevice) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Repositories\MerchantRepository.cs:line 302
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(MerchantDeviceModel md) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2189', N'An error occurred while updating the entries. See the inner exception for details.', N'   at System.Data.Entity.Core.Mapping.Update.Internal.UpdateTranslator.Update()
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.<Update>b__2(UpdateTranslator ut)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update[T](T noChangesResult, Func`2 updateFunction)
   at System.Data.Entity.Core.EntityClient.Internal.EntityAdapter.Update()
   at System.Data.Entity.Core.Objects.ObjectContext.<SaveChangesToStore>b__35()
   at System.Data.Entity.Core.Objects.ObjectContext.ExecuteInTransaction[T](Func`1 func, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction, Boolean releaseConnectionOnSuccess)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesToStore(SaveOptions options, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.<>c__DisplayClass2a.<SaveChangesInternal>b__27()
   at System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy.Execute[TResult](Func`1 operation)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChangesInternal(SaveOptions options, Boolean executeInExistingTransaction)
   at System.Data.Entity.Core.Objects.ObjectContext.SaveChanges(SaveOptions options)
   at System.Data.Entity.Internal.InternalContext.SaveChanges()', CAST(N'2019-07-31T06:41:15.050' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (14, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T06:41:31.533' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (15, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T06:42:52.317' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (16, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T06:45:18.323' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (17, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T06:46:15.520' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (18, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T07:13:06.060' AS DateTime))
GO
INSERT [dbo].[ErrorLogs] ([ErrorLogId], [Method], [Action], [ErrText], [StackTrace], [InnerException], [InnerExceptionStackTrace], [DateCreated]) VALUES (19, N'View', N'ActionResult Merchant AssignDevice', N'Invalid length for a Base-64 char array or string.', N'   at System.Convert.FromBase64_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
   at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
   at System.Convert.FromBase64String(String s)
   at Crypto.CryptoManager.CryptoProvider.Decrypt(String cipherText)
   at SDGDAL.Utility.Decrypt(String t) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGDAL\Utility.cs:line 122
   at SDGBackOffice.Controllers.MerchantsController.AssignDevice(String id) in E:\Users\Gabby Peneyra\source\repos\FromGitHub\VPI-BackOffice\SDGBackOffice\Controllers\MerchantsController.cs:line 2087', NULL, NULL, CAST(N'2019-07-31T07:33:54.643' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[ErrorLogs] OFF
GO
SET IDENTITY_INSERT [dbo].[MasterDevices] ON 
GO
INSERT [dbo].[MasterDevices] ([MasterDeviceId], [DeviceFlowTypeId], [DeviceTypeId], [DeviceName], [Manufacturer], [Warranty], [ExternalData], [DateCreated], [IsActive], [IsDeleted]) VALUES (1, 2, 1, N'Device 1', N'None', N'1 year', NULL, CAST(N'2019-07-31T06:31:10.007' AS DateTime), 1, 0)
GO
SET IDENTITY_INSERT [dbo].[MasterDevices] OFF
GO
SET IDENTITY_INSERT [dbo].[MerchantBranches] ON 
GO
INSERT [dbo].[MerchantBranches] ([MerchantBranchId], [MerchantBranchName], [ContactInformationId], [IsActive], [IsDeleted], [DateCreated], [MerchantId], [Merchant_MerchantId]) VALUES (1, N'Branch', 14, 1, 0, CAST(N'2019-07-31T06:01:02.653' AS DateTime), 1, NULL)
GO
SET IDENTITY_INSERT [dbo].[MerchantBranches] OFF
GO
SET IDENTITY_INSERT [dbo].[MerchantBranchPOSs] ON 
GO
INSERT [dbo].[MerchantBranchPOSs] ([MerchantPOSId], [MerchantPOSName], [IsActive], [IsDeleted], [DateCreated], [MerchantBranchId], [MidsMerchantBranchPOSs_Id]) VALUES (1, N'POS 4IVP', 1, 0, CAST(N'2019-07-31T06:01:29.880' AS DateTime), 1, NULL)
GO
INSERT [dbo].[MerchantBranchPOSs] ([MerchantPOSId], [MerchantPOSName], [IsActive], [IsDeleted], [DateCreated], [MerchantBranchId], [MidsMerchantBranchPOSs_Id]) VALUES (2, N'POS JECT', 1, 0, CAST(N'2019-07-31T06:01:29.890' AS DateTime), 1, NULL)
GO
INSERT [dbo].[MerchantBranchPOSs] ([MerchantPOSId], [MerchantPOSName], [IsActive], [IsDeleted], [DateCreated], [MerchantBranchId], [MidsMerchantBranchPOSs_Id]) VALUES (3, N'POS E5V6', 1, 0, CAST(N'2019-07-31T06:01:29.893' AS DateTime), 1, NULL)
GO
SET IDENTITY_INSERT [dbo].[MerchantBranchPOSs] OFF
GO
SET IDENTITY_INSERT [dbo].[MerchantFeatures] ON 
GO
INSERT [dbo].[MerchantFeatures] ([MerchantFeaturesId], [BillingCycleId], [CurrencyId], [LanguageCode], [TermsOfServiceEnabled], [DisclaimerEnabled], [UseDefaultAgreements]) VALUES (1, 1, 1, N'EN-CA', 0, 0, 1)
GO
SET IDENTITY_INSERT [dbo].[MerchantFeatures] OFF
GO
SET IDENTITY_INSERT [dbo].[Merchants] ON 
GO
INSERT [dbo].[Merchants] ([MerchantId], [MerchantName], [MerchantEmail], [ContactInformationId], [MerchantFeaturesId], [CurrencyId], [IsActive], [IsDeleted], [DateCreated], [CanCreateSubMerchants], [ParentMerchantId], [ResellerId], [PartnerId], [EmailServerId], [NeedAddToCT], [NeedUpdateToCT]) VALUES (1, N'Merchant', N'merchant@mailinator.com', 12, 1, 1, 1, 0, CAST(N'2019-07-30T21:43:37.593' AS DateTime), 0, NULL, 1, 2, NULL, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[Merchants] OFF
GO
SET IDENTITY_INSERT [dbo].[Mids] ON 
GO
INSERT [dbo].[Mids] ([MidId], [MidName], [IsActive], [IsDeleted], [SwitchId], [CardTypeId], [CurrencyId], [MidsPricingId], [TransactionChargesId], [MerchantId], [SetLikeMerchantId], [SetLikeTerminalId], [Param_1], [Param_2], [Param_3], [Param_4], [Param_5], [Param_6], [Param_7], [Param_8], [Param_9], [Param_10], [Param_11], [Param_12], [Param_13], [Param_14], [Param_15], [Param_16], [Param_17], [Param_18], [Param_19], [Param_20], [Param_21], [Param_22], [Param_23], [Param_24], [NeedAddBulk], [NeedUpdateBulk], [NeedDeleteBulk], [NeedAddTerminal], [AcquiringBin], [City], [Country], [Merchant_MerchantId]) VALUES (1, N'Mid Name', 1, 0, 1, 14, 1, 0, 1, 1, NULL, NULL, NULL, N'alksadjakjsdh', NULL, NULL, NULL, N'kjahsdkjaldskh', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 0, 1, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Mids] OFF
GO
SET IDENTITY_INSERT [dbo].[MidsGroupTypes] ON 
GO
INSERT [dbo].[MidsGroupTypes] ([MidsGroupTypeId], [GroupType]) VALUES (1, N'Ordered')
GO
INSERT [dbo].[MidsGroupTypes] ([MidsGroupTypeId], [GroupType]) VALUES (2, N'Balanced')
GO
SET IDENTITY_INSERT [dbo].[MidsGroupTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[MidsMerchantBranches] ON 
GO
INSERT [dbo].[MidsMerchantBranches] ([Id], [MidId], [MerchantBranchId], [IsActive], [IsDeleted], [DateCreated], [DateUpdated]) VALUES (1, 1, 1, 1, 0, CAST(N'2019-07-31T07:38:02.530' AS DateTime), CAST(N'2019-07-31T07:38:02.530' AS DateTime))
GO
INSERT [dbo].[MidsMerchantBranches] ([Id], [MidId], [MerchantBranchId], [IsActive], [IsDeleted], [DateCreated], [DateUpdated]) VALUES (2, 1, 1, 1, 0, CAST(N'2019-07-31T07:38:02.587' AS DateTime), CAST(N'2019-07-31T07:38:02.587' AS DateTime))
GO
INSERT [dbo].[MidsMerchantBranches] ([Id], [MidId], [MerchantBranchId], [IsActive], [IsDeleted], [DateCreated], [DateUpdated]) VALUES (3, 1, 1, 1, 0, CAST(N'2019-07-31T07:38:02.593' AS DateTime), CAST(N'2019-07-31T07:38:02.593' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[MidsMerchantBranches] OFF
GO
SET IDENTITY_INSERT [dbo].[MidsMerchantBranchPOSs] ON 
GO
INSERT [dbo].[MidsMerchantBranchPOSs] ([Id], [MidId], [MerchantBranchPOSId], [IsActive], [IsDeleted], [DateCreated], [DateUpdated], [MerchantBranchPOS_MerchantPOSId]) VALUES (1, 1, 1, 1, 0, CAST(N'2019-07-31T07:38:02.530' AS DateTime), CAST(N'2019-07-31T07:38:02.530' AS DateTime), NULL)
GO
INSERT [dbo].[MidsMerchantBranchPOSs] ([Id], [MidId], [MerchantBranchPOSId], [IsActive], [IsDeleted], [DateCreated], [DateUpdated], [MerchantBranchPOS_MerchantPOSId]) VALUES (2, 1, 2, 1, 0, CAST(N'2019-07-31T07:38:02.587' AS DateTime), CAST(N'2019-07-31T07:38:02.587' AS DateTime), NULL)
GO
INSERT [dbo].[MidsMerchantBranchPOSs] ([Id], [MidId], [MerchantBranchPOSId], [IsActive], [IsDeleted], [DateCreated], [DateUpdated], [MerchantBranchPOS_MerchantPOSId]) VALUES (3, 1, 3, 1, 0, CAST(N'2019-07-31T07:38:02.593' AS DateTime), CAST(N'2019-07-31T07:38:02.593' AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[MidsMerchantBranchPOSs] OFF
GO
SET IDENTITY_INSERT [dbo].[MobileAppFeatures] ON 
GO
INSERT [dbo].[MobileAppFeatures] ([MobileAppFeaturesId], [SystemMode], [CurrencyId], [LanguageCode], [GPSEnabled], [SMSEnabled], [GiftAllowed], [EmailAllowed], [EmailLimit], [CheckForEmailDuplicates], [BillingCyclesCheckEmail], [PrintAllowed], [PrintLimit], [CheckForPrintDuplicates], [BillingCyclesCheckPrint], [ReferenceNumber], [CreditSignature], [DebitSignature], [ChequeSignature], [CashSignature], [CreditTransaction], [DebitTransaction], [ChequeTransaction], [CashTransaction], [BalanceInquiry], [BillsPayment], [ProofId], [ChequeType], [DebitRefund], [TOSRequired], [DisclaimerRequired], [TipsEnabled], [Amount1], [Amount2], [Amount3], [Percentage1], [Percentage2], [Percentage3]) VALUES (1, N'LIVE', 1, N'EN-CA', 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[MobileAppFeatures] ([MobileAppFeaturesId], [SystemMode], [CurrencyId], [LanguageCode], [GPSEnabled], [SMSEnabled], [GiftAllowed], [EmailAllowed], [EmailLimit], [CheckForEmailDuplicates], [BillingCyclesCheckEmail], [PrintAllowed], [PrintLimit], [CheckForPrintDuplicates], [BillingCyclesCheckPrint], [ReferenceNumber], [CreditSignature], [DebitSignature], [ChequeSignature], [CashSignature], [CreditTransaction], [DebitTransaction], [ChequeTransaction], [CashTransaction], [BalanceInquiry], [BillsPayment], [ProofId], [ChequeType], [DebitRefund], [TOSRequired], [DisclaimerRequired], [TipsEnabled], [Amount1], [Amount2], [Amount3], [Percentage1], [Percentage2], [Percentage3]) VALUES (2, N'LIVE', 1, N'EN-CA', 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[MobileAppFeatures] ([MobileAppFeaturesId], [SystemMode], [CurrencyId], [LanguageCode], [GPSEnabled], [SMSEnabled], [GiftAllowed], [EmailAllowed], [EmailLimit], [CheckForEmailDuplicates], [BillingCyclesCheckEmail], [PrintAllowed], [PrintLimit], [CheckForPrintDuplicates], [BillingCyclesCheckPrint], [ReferenceNumber], [CreditSignature], [DebitSignature], [ChequeSignature], [CashSignature], [CreditTransaction], [DebitTransaction], [ChequeTransaction], [CashTransaction], [BalanceInquiry], [BillsPayment], [ProofId], [ChequeType], [DebitRefund], [TOSRequired], [DisclaimerRequired], [TipsEnabled], [Amount1], [Amount2], [Amount3], [Percentage1], [Percentage2], [Percentage3]) VALUES (3, N'LIVE', 1, N'EN-CA', 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[MobileAppFeatures] OFF
GO
SET IDENTITY_INSERT [dbo].[MobileApps] ON 
GO
INSERT [dbo].[MobileApps] ([MobileAppId], [MerchantBranchPOSId], [MobileDeviceInfoId], [MobileAppFeaturesId], [ActivationCode], [DateCreated], [DateActivated], [ExpirationDate], [UpdatePending], [IsActive], [IsDeleted], [TOS_Acknowledged], [GPSLat], [GPSLong]) VALUES (1, 1, NULL, 1, N'B6HS-HGDP-APIT-379X', CAST(N'2019-07-31T06:01:29.877' AS DateTime), NULL, CAST(N'2020-07-31T06:01:29.877' AS DateTime), 1, 0, 0, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[MobileApps] ([MobileAppId], [MerchantBranchPOSId], [MobileDeviceInfoId], [MobileAppFeaturesId], [ActivationCode], [DateCreated], [DateActivated], [ExpirationDate], [UpdatePending], [IsActive], [IsDeleted], [TOS_Acknowledged], [GPSLat], [GPSLong]) VALUES (2, 2, NULL, 2, N'CK1I-3J3H-Z1SR-2TSB', CAST(N'2019-07-31T06:01:29.890' AS DateTime), NULL, CAST(N'2020-07-31T06:01:29.890' AS DateTime), 1, 0, 0, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[MobileApps] ([MobileAppId], [MerchantBranchPOSId], [MobileDeviceInfoId], [MobileAppFeaturesId], [ActivationCode], [DateCreated], [DateActivated], [ExpirationDate], [UpdatePending], [IsActive], [IsDeleted], [TOS_Acknowledged], [GPSLat], [GPSLong]) VALUES (3, 3, NULL, 3, N'7CRR-CV3D-PG81-NTP1', CAST(N'2019-07-31T06:01:29.893' AS DateTime), NULL, CAST(N'2020-07-31T06:01:29.893' AS DateTime), 1, 0, 0, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[MobileApps] OFF
GO
SET IDENTITY_INSERT [dbo].[Partners] ON 
GO
INSERT [dbo].[Partners] ([PartnerId], [CompanyName], [LogoUrl], [CompanyEmail], [MerchantDiscountRate], [ContactInformationId], [IsActive], [IsDeleted], [DateCreated], [CanCreateSubPartners], [ParentPartnerId]) VALUES (1, N'SD Group', NULL, N'gerald.toco@gmail.com', CAST(0.00 AS Decimal(18, 2)), 1, 0, 0, CAST(N'2019-07-22T21:41:14.913' AS DateTime), 0, NULL)
GO
INSERT [dbo].[Partners] ([PartnerId], [CompanyName], [LogoUrl], [CompanyEmail], [MerchantDiscountRate], [ContactInformationId], [IsActive], [IsDeleted], [DateCreated], [CanCreateSubPartners], [ParentPartnerId]) VALUES (2, N'Sub Partner', NULL, N'subpartner@mailinator.com', CAST(0.00 AS Decimal(18, 2)), 3, 1, 0, CAST(N'2019-07-30T20:55:18.770' AS DateTime), 0, 1)
GO
SET IDENTITY_INSERT [dbo].[Partners] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentAccountTypes] ON 
GO
INSERT [dbo].[PaymentAccountTypes] ([PaymentAccountTypeId], [AccountType]) VALUES (1, N'Chequing')
GO
INSERT [dbo].[PaymentAccountTypes] ([PaymentAccountTypeId], [AccountType]) VALUES (2, N'Savings')
GO
INSERT [dbo].[PaymentAccountTypes] ([PaymentAccountTypeId], [AccountType]) VALUES (3, N'Credit')
GO
INSERT [dbo].[PaymentAccountTypes] ([PaymentAccountTypeId], [AccountType]) VALUES (4, N'Checking')
GO
SET IDENTITY_INSERT [dbo].[PaymentAccountTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentTypes] ON 
GO
INSERT [dbo].[PaymentTypes] ([PaymentTypeId], [TypeName], [DateCreated], [IsActive], [IsDeleted]) VALUES (1, N'ACH', CAST(N'2019-07-22T21:41:15.023' AS DateTime), 1, 0)
GO
INSERT [dbo].[PaymentTypes] ([PaymentTypeId], [TypeName], [DateCreated], [IsActive], [IsDeleted]) VALUES (2, N'Cash', CAST(N'2019-07-22T21:41:15.023' AS DateTime), 1, 0)
GO
INSERT [dbo].[PaymentTypes] ([PaymentTypeId], [TypeName], [DateCreated], [IsActive], [IsDeleted]) VALUES (3, N'Credit', CAST(N'2019-07-22T21:41:15.023' AS DateTime), 1, 0)
GO
INSERT [dbo].[PaymentTypes] ([PaymentTypeId], [TypeName], [DateCreated], [IsActive], [IsDeleted]) VALUES (4, N'Debit', CAST(N'2019-07-22T21:41:15.023' AS DateTime), 1, 0)
GO
INSERT [dbo].[PaymentTypes] ([PaymentTypeId], [TypeName], [DateCreated], [IsActive], [IsDeleted]) VALUES (5, N'EMV', CAST(N'2019-07-22T21:41:15.023' AS DateTime), 1, 0)
GO
INSERT [dbo].[PaymentTypes] ([PaymentTypeId], [TypeName], [DateCreated], [IsActive], [IsDeleted]) VALUES (6, N'All', CAST(N'2019-07-22T21:41:15.023' AS DateTime), 1, 0)
GO
SET IDENTITY_INSERT [dbo].[PaymentTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[POSTypes] ON 
GO
INSERT [dbo].[POSTypes] ([POSTypeId], [TypeName]) VALUES (1, N'CC')
GO
INSERT [dbo].[POSTypes] ([POSTypeId], [TypeName]) VALUES (2, N'PIN')
GO
INSERT [dbo].[POSTypes] ([POSTypeId], [TypeName]) VALUES (3, N'EMV')
GO
INSERT [dbo].[POSTypes] ([POSTypeId], [TypeName]) VALUES (4, N'4')
GO
SET IDENTITY_INSERT [dbo].[POSTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Resellers] ON 
GO
INSERT [dbo].[Resellers] ([ResellerId], [ResellerName], [ResellerEmail], [ContactInformationId], [IsActive], [IsDeleted], [DateCreated], [CanCreateSubResellers], [PartnerId], [ParentResellerId]) VALUES (1, N'Reseller', N'reseller@mailinator.com', 5, 1, 0, CAST(N'2019-07-30T20:56:14.263' AS DateTime), 0, 2, NULL)
GO
SET IDENTITY_INSERT [dbo].[Resellers] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 
GO
INSERT [dbo].[Roles] ([RoleId], [RoleName], [DateCreated]) VALUES (1, N'Administrator', CAST(N'2019-07-22T21:41:14.907' AS DateTime))
GO
INSERT [dbo].[Roles] ([RoleId], [RoleName], [DateCreated]) VALUES (2, N'Manager', CAST(N'2019-07-22T21:41:14.910' AS DateTime))
GO
INSERT [dbo].[Roles] ([RoleId], [RoleName], [DateCreated]) VALUES (3, N'Employee', CAST(N'2019-07-22T21:41:14.910' AS DateTime))
GO
INSERT [dbo].[Roles] ([RoleId], [RoleName], [DateCreated]) VALUES (4, N'Developer', CAST(N'2019-07-22T21:41:14.910' AS DateTime))
GO
INSERT [dbo].[Roles] ([RoleId], [RoleName], [DateCreated]) VALUES (5, N'Super Admin', CAST(N'2019-07-22T21:41:14.910' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[SwitchAPITypes] ON 
GO
INSERT [dbo].[SwitchAPITypes] ([SwitchAPITypeId], [APIName], [IsActive]) VALUES (1, N'CT Payments', 1)
GO
INSERT [dbo].[SwitchAPITypes] ([SwitchAPITypeId], [APIName], [IsActive]) VALUES (2, N'Moneris', 0)
GO
INSERT [dbo].[SwitchAPITypes] ([SwitchAPITypeId], [APIName], [IsActive]) VALUES (3, N'Data Collection', 1)
GO
INSERT [dbo].[SwitchAPITypes] ([SwitchAPITypeId], [APIName], [IsActive]) VALUES (4, N'PaymentData Usd', 1)
GO
INSERT [dbo].[SwitchAPITypes] ([SwitchAPITypeId], [APIName], [IsActive]) VALUES (5, N'TransaxPayment', 0)
GO
SET IDENTITY_INSERT [dbo].[SwitchAPITypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Switches] ON 
GO
INSERT [dbo].[Switches] ([SwitchId], [SwitchName], [SwitchCode], [IsActive], [IsAddressRequired], [DateCreated]) VALUES (1, N'Offline', N'000', 1, 0, CAST(N'2019-07-22T21:41:14.913' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Switches] OFF
GO
SET IDENTITY_INSERT [dbo].[SwitchPartnerLink] ON 
GO
INSERT [dbo].[SwitchPartnerLink] ([SwitchPartnerLinkId], [SwitchId], [PartnerId], [IsEnabled], [Switch_SwitchId]) VALUES (1, 1, 2, 1, 1)
GO
SET IDENTITY_INSERT [dbo].[SwitchPartnerLink] OFF
GO
SET IDENTITY_INSERT [dbo].[TransactionCharges] ON 
GO
INSERT [dbo].[TransactionCharges] ([TransactionChargeId], [DiscountRate], [CardNotPresent], [eCommerce], [PreAuth], [Capture], [Purchased], [Declined], [Refund], [Void], [CashBack]) VALUES (1, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[TransactionCharges] OFF
GO
SET IDENTITY_INSERT [dbo].[TransactionEntryTypes] ON 
GO
INSERT [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId], [EntryType]) VALUES (1, N'Credit PIN')
GO
INSERT [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId], [EntryType]) VALUES (2, N'Credit Signature')
GO
INSERT [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId], [EntryType]) VALUES (3, N'Credit Manual')
GO
INSERT [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId], [EntryType]) VALUES (4, N'Debit PIN')
GO
INSERT [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId], [EntryType]) VALUES (5, N'Credit Swipe')
GO
INSERT [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId], [EntryType]) VALUES (6, N'Cheque')
GO
INSERT [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId], [EntryType]) VALUES (7, N'EBTSwipePIN')
GO
SET IDENTITY_INSERT [dbo].[TransactionEntryTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[TransactionTypes] ON 
GO
INSERT [dbo].[TransactionTypes] ([TransactionTypeId], [Name]) VALUES (1, N'Capture')
GO
INSERT [dbo].[TransactionTypes] ([TransactionTypeId], [Name]) VALUES (2, N'Pre Auth')
GO
INSERT [dbo].[TransactionTypes] ([TransactionTypeId], [Name]) VALUES (3, N'Purchased')
GO
INSERT [dbo].[TransactionTypes] ([TransactionTypeId], [Name]) VALUES (4, N'Void')
GO
INSERT [dbo].[TransactionTypes] ([TransactionTypeId], [Name]) VALUES (5, N'Refund')
GO
INSERT [dbo].[TransactionTypes] ([TransactionTypeId], [Name]) VALUES (6, N'Declined')
GO
SET IDENTITY_INSERT [dbo].[TransactionTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([UserId], [EmailAddress], [PhotoUrl], [FirstName], [MiddleName], [LastName], [ContactInformationId], [Price]) VALUES (1, N'gerald.toco@gmail.com', NULL, N'Ralph Gerald', NULL, N'Toco', 2, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Users] ([UserId], [EmailAddress], [PhotoUrl], [FirstName], [MiddleName], [LastName], [ContactInformationId], [Price]) VALUES (2, N'subpartner@mailinator.com', NULL, N'Sub', NULL, N'Partner', 4, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Users] ([UserId], [EmailAddress], [PhotoUrl], [FirstName], [MiddleName], [LastName], [ContactInformationId], [Price]) VALUES (3, N'reseller@mailinator.com', NULL, N'Reseller', NULL, N'Reseller', 6, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Users] ([UserId], [EmailAddress], [PhotoUrl], [FirstName], [MiddleName], [LastName], [ContactInformationId], [Price]) VALUES (4, N'merchant@mailinator.com', NULL, N'Merchant', NULL, N'Merchant', 13, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Users] ([UserId], [EmailAddress], [PhotoUrl], [FirstName], [MiddleName], [LastName], [ContactInformationId], [Price]) VALUES (5, N'branch@mailinator.com', NULL, N'Branch', NULL, N'Branch', 15, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Users] ([UserId], [EmailAddress], [PhotoUrl], [FirstName], [MiddleName], [LastName], [ContactInformationId], [Price]) VALUES (6, N'gerald.toco@gmail.com', NULL, N'Ralph Gerald', NULL, N'Toco', 16, CAST(0.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Accounts_dbo.Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_dbo.Accounts_dbo.Roles_RoleId]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Accounts_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_dbo.Accounts_dbo.Users_UserId]
GO
ALTER TABLE [dbo].[Agreements]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Agreements_dbo.Merchants_MerchantId] FOREIGN KEY([MerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
GO
ALTER TABLE [dbo].[Agreements] CHECK CONSTRAINT [FK_dbo.Agreements_dbo.Merchants_MerchantId]
GO
ALTER TABLE [dbo].[Batch]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Batch_dbo.Merchants_MerchantId] FOREIGN KEY([MerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Batch] CHECK CONSTRAINT [FK_dbo.Batch_dbo.Merchants_MerchantId]
GO
ALTER TABLE [dbo].[Batch]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Batch_dbo.MobileApps_MobileAppId] FOREIGN KEY([MobileAppId])
REFERENCES [dbo].[MobileApps] ([MobileAppId])
GO
ALTER TABLE [dbo].[Batch] CHECK CONSTRAINT [FK_dbo.Batch_dbo.MobileApps_MobileAppId]
GO
ALTER TABLE [dbo].[Batch]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Batch_dbo.PaymentTypes_PaymentTypeId] FOREIGN KEY([PaymentTypeId])
REFERENCES [dbo].[PaymentTypes] ([PaymentTypeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Batch] CHECK CONSTRAINT [FK_dbo.Batch_dbo.PaymentTypes_PaymentTypeId]
GO
ALTER TABLE [dbo].[ContactInformation]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ContactInformation_dbo.Countries_CountryId] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Countries] ([CountryId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ContactInformation] CHECK CONSTRAINT [FK_dbo.ContactInformation_dbo.Countries_CountryId]
GO
ALTER TABLE [dbo].[CountryIPs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CountryIPs_dbo.Countries_CountryId] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Countries] ([CountryId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CountryIPs] CHECK CONSTRAINT [FK_dbo.CountryIPs_dbo.Countries_CountryId]
GO
ALTER TABLE [dbo].[DeviceMerchantLink]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DeviceMerchantLink_dbo.Devices_DeviceId] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Devices] ([DeviceId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DeviceMerchantLink] CHECK CONSTRAINT [FK_dbo.DeviceMerchantLink_dbo.Devices_DeviceId]
GO
ALTER TABLE [dbo].[DeviceMerchantLink]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DeviceMerchantLink_dbo.Merchants_MerchantId] FOREIGN KEY([MerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DeviceMerchantLink] CHECK CONSTRAINT [FK_dbo.DeviceMerchantLink_dbo.Merchants_MerchantId]
GO
ALTER TABLE [dbo].[DevicePOSLink]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DevicePOSLink_dbo.MasterDevices_MasterDeviceId] FOREIGN KEY([MasterDeviceId])
REFERENCES [dbo].[MasterDevices] ([MasterDeviceId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DevicePOSLink] CHECK CONSTRAINT [FK_dbo.DevicePOSLink_dbo.MasterDevices_MasterDeviceId]
GO
ALTER TABLE [dbo].[DevicePOSLink]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DevicePOSLink_dbo.MerchantBranchPOSs_MerchantPOSId] FOREIGN KEY([MerchantPOSId])
REFERENCES [dbo].[MerchantBranchPOSs] ([MerchantPOSId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DevicePOSLink] CHECK CONSTRAINT [FK_dbo.DevicePOSLink_dbo.MerchantBranchPOSs_MerchantPOSId]
GO
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Devices_dbo.MasterDevices_MasterDeviceId] FOREIGN KEY([MasterDeviceId])
REFERENCES [dbo].[MasterDevices] ([MasterDeviceId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_dbo.Devices_dbo.MasterDevices_MasterDeviceId]
GO
ALTER TABLE [dbo].[HeaderResponse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvAmexParameters_EmvAmexParameterId] FOREIGN KEY([EmvAmexParameterId])
REFERENCES [dbo].[EmvAmexParameters] ([EmvAmexParameterId])
GO
ALTER TABLE [dbo].[HeaderResponse] CHECK CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvAmexParameters_EmvAmexParameterId]
GO
ALTER TABLE [dbo].[HeaderResponse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvDiscoverParameters_EmvDiscoverParameterId] FOREIGN KEY([EmvDiscoverParameterId])
REFERENCES [dbo].[EmvDiscoverParameters] ([EmvDiscoverParameterId])
GO
ALTER TABLE [dbo].[HeaderResponse] CHECK CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvDiscoverParameters_EmvDiscoverParameterId]
GO
ALTER TABLE [dbo].[HeaderResponse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvHostParameters_EmvHostParameterId] FOREIGN KEY([EmvHostParameterId])
REFERENCES [dbo].[EmvHostParameters] ([EmvHostParameterId])
GO
ALTER TABLE [dbo].[HeaderResponse] CHECK CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvHostParameters_EmvHostParameterId]
GO
ALTER TABLE [dbo].[HeaderResponse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvInteracParameters_EmvInteracParameterId] FOREIGN KEY([EmvInteracParameterId])
REFERENCES [dbo].[EmvInteracParameters] ([EmvInteracParameterId])
GO
ALTER TABLE [dbo].[HeaderResponse] CHECK CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvInteracParameters_EmvInteracParameterId]
GO
ALTER TABLE [dbo].[HeaderResponse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvJcbParameters_EmvJcbParametersId] FOREIGN KEY([EmvJcbParametersId])
REFERENCES [dbo].[EmvJcbParameters] ([EmvJcbParameterId])
GO
ALTER TABLE [dbo].[HeaderResponse] CHECK CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvJcbParameters_EmvJcbParametersId]
GO
ALTER TABLE [dbo].[HeaderResponse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvMastercardParameters_EmvMastercardParameterId] FOREIGN KEY([EmvMastercardParameterId])
REFERENCES [dbo].[EmvMastercardParameters] ([EmvMastercardParameterId])
GO
ALTER TABLE [dbo].[HeaderResponse] CHECK CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvMastercardParameters_EmvMastercardParameterId]
GO
ALTER TABLE [dbo].[HeaderResponse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvTerminalParameters_EmvTerminalParameterId] FOREIGN KEY([EmvTerminalParameterId])
REFERENCES [dbo].[EmvTerminalParameters] ([EmvTerminalParameterId])
GO
ALTER TABLE [dbo].[HeaderResponse] CHECK CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvTerminalParameters_EmvTerminalParameterId]
GO
ALTER TABLE [dbo].[HeaderResponse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvVisaParameters_EmvVisaParameterId] FOREIGN KEY([EmvVisaParameterId])
REFERENCES [dbo].[EmvVisaParameters] ([EmvVisaParameterId])
GO
ALTER TABLE [dbo].[HeaderResponse] CHECK CONSTRAINT [FK_dbo.HeaderResponse_dbo.EmvVisaParameters_EmvVisaParameterId]
GO
ALTER TABLE [dbo].[MasterDevices]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MasterDevices_dbo.DeviceFlowTypes_DeviceFlowTypeId] FOREIGN KEY([DeviceFlowTypeId])
REFERENCES [dbo].[DeviceFlowTypes] ([DeviceFlowTypeId])
GO
ALTER TABLE [dbo].[MasterDevices] CHECK CONSTRAINT [FK_dbo.MasterDevices_dbo.DeviceFlowTypes_DeviceFlowTypeId]
GO
ALTER TABLE [dbo].[MasterDevices]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MasterDevices_dbo.DeviceTypes_DeviceTypeId] FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceTypes] ([DeviceTypeId])
GO
ALTER TABLE [dbo].[MasterDevices] CHECK CONSTRAINT [FK_dbo.MasterDevices_dbo.DeviceTypes_DeviceTypeId]
GO
ALTER TABLE [dbo].[MerchantBranches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantBranches_dbo.ContactInformation_ContactInformationId] FOREIGN KEY([ContactInformationId])
REFERENCES [dbo].[ContactInformation] ([ContactInformationId])
GO
ALTER TABLE [dbo].[MerchantBranches] CHECK CONSTRAINT [FK_dbo.MerchantBranches_dbo.ContactInformation_ContactInformationId]
GO
ALTER TABLE [dbo].[MerchantBranches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantBranches_dbo.Merchants_Merchant_MerchantId] FOREIGN KEY([Merchant_MerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
GO
ALTER TABLE [dbo].[MerchantBranches] CHECK CONSTRAINT [FK_dbo.MerchantBranches_dbo.Merchants_Merchant_MerchantId]
GO
ALTER TABLE [dbo].[MerchantBranches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantBranches_dbo.Merchants_MerchantId] FOREIGN KEY([MerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
GO
ALTER TABLE [dbo].[MerchantBranches] CHECK CONSTRAINT [FK_dbo.MerchantBranches_dbo.Merchants_MerchantId]
GO
ALTER TABLE [dbo].[MerchantBranchPOSs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantBranchPOSs_dbo.MerchantBranches_MerchantBranchId] FOREIGN KEY([MerchantBranchId])
REFERENCES [dbo].[MerchantBranches] ([MerchantBranchId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MerchantBranchPOSs] CHECK CONSTRAINT [FK_dbo.MerchantBranchPOSs_dbo.MerchantBranches_MerchantBranchId]
GO
ALTER TABLE [dbo].[MerchantBranchPOSs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantBranchPOSs_dbo.MidsMerchantBranchPOSs_MidsMerchantBranchPOSs_Id] FOREIGN KEY([MidsMerchantBranchPOSs_Id])
REFERENCES [dbo].[MidsMerchantBranchPOSs] ([Id])
GO
ALTER TABLE [dbo].[MerchantBranchPOSs] CHECK CONSTRAINT [FK_dbo.MerchantBranchPOSs_dbo.MidsMerchantBranchPOSs_MidsMerchantBranchPOSs_Id]
GO
ALTER TABLE [dbo].[MerchantFeatures]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantFeatures_dbo.BillingCycles_BillingCycleId] FOREIGN KEY([BillingCycleId])
REFERENCES [dbo].[BillingCycles] ([BillingCycleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MerchantFeatures] CHECK CONSTRAINT [FK_dbo.MerchantFeatures_dbo.BillingCycles_BillingCycleId]
GO
ALTER TABLE [dbo].[MerchantFeatures]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantFeatures_dbo.Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currencies] ([CurrencyId])
GO
ALTER TABLE [dbo].[MerchantFeatures] CHECK CONSTRAINT [FK_dbo.MerchantFeatures_dbo.Currencies_CurrencyId]
GO
ALTER TABLE [dbo].[MerchantMobileAppPricePoints]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantMobileAppPricePoints_dbo.Merchants_MerchantId] FOREIGN KEY([MerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MerchantMobileAppPricePoints] CHECK CONSTRAINT [FK_dbo.MerchantMobileAppPricePoints_dbo.Merchants_MerchantId]
GO
ALTER TABLE [dbo].[MerchantOnBoardResponseLink]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantOnBoardResponseLink_dbo.MerchantOnBoardRequest_MerchantOnBoardRequestId] FOREIGN KEY([MerchantOnBoardRequestId])
REFERENCES [dbo].[MerchantOnBoardRequest] ([MerchantOnBoardRequestId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MerchantOnBoardResponseLink] CHECK CONSTRAINT [FK_dbo.MerchantOnBoardResponseLink_dbo.MerchantOnBoardRequest_MerchantOnBoardRequestId]
GO
ALTER TABLE [dbo].[Merchants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Merchants_dbo.ContactInformation_ContactInformationId] FOREIGN KEY([ContactInformationId])
REFERENCES [dbo].[ContactInformation] ([ContactInformationId])
GO
ALTER TABLE [dbo].[Merchants] CHECK CONSTRAINT [FK_dbo.Merchants_dbo.ContactInformation_ContactInformationId]
GO
ALTER TABLE [dbo].[Merchants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Merchants_dbo.Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currencies] ([CurrencyId])
GO
ALTER TABLE [dbo].[Merchants] CHECK CONSTRAINT [FK_dbo.Merchants_dbo.Currencies_CurrencyId]
GO
ALTER TABLE [dbo].[Merchants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Merchants_dbo.EmailServers_EmailServerId] FOREIGN KEY([EmailServerId])
REFERENCES [dbo].[EmailServers] ([EmailServerId])
GO
ALTER TABLE [dbo].[Merchants] CHECK CONSTRAINT [FK_dbo.Merchants_dbo.EmailServers_EmailServerId]
GO
ALTER TABLE [dbo].[Merchants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Merchants_dbo.MerchantFeatures_MerchantFeaturesId] FOREIGN KEY([MerchantFeaturesId])
REFERENCES [dbo].[MerchantFeatures] ([MerchantFeaturesId])
GO
ALTER TABLE [dbo].[Merchants] CHECK CONSTRAINT [FK_dbo.Merchants_dbo.MerchantFeatures_MerchantFeaturesId]
GO
ALTER TABLE [dbo].[Merchants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Merchants_dbo.Merchants_ParentMerchantId] FOREIGN KEY([ParentMerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
GO
ALTER TABLE [dbo].[Merchants] CHECK CONSTRAINT [FK_dbo.Merchants_dbo.Merchants_ParentMerchantId]
GO
ALTER TABLE [dbo].[Merchants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Merchants_dbo.Partners_PartnerId] FOREIGN KEY([PartnerId])
REFERENCES [dbo].[Partners] ([PartnerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Merchants] CHECK CONSTRAINT [FK_dbo.Merchants_dbo.Partners_PartnerId]
GO
ALTER TABLE [dbo].[Merchants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Merchants_dbo.Resellers_ResellerId] FOREIGN KEY([ResellerId])
REFERENCES [dbo].[Resellers] ([ResellerId])
GO
ALTER TABLE [dbo].[Merchants] CHECK CONSTRAINT [FK_dbo.Merchants_dbo.Resellers_ResellerId]
GO
ALTER TABLE [dbo].[MerchantUserPricePoints]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MerchantUserPricePoints_dbo.Merchants_MerchantId] FOREIGN KEY([MerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MerchantUserPricePoints] CHECK CONSTRAINT [FK_dbo.MerchantUserPricePoints_dbo.Merchants_MerchantId]
GO
ALTER TABLE [dbo].[Mids]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Mids_dbo.CardTypes_CardTypeId] FOREIGN KEY([CardTypeId])
REFERENCES [dbo].[CardTypes] ([CardTypeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Mids] CHECK CONSTRAINT [FK_dbo.Mids_dbo.CardTypes_CardTypeId]
GO
ALTER TABLE [dbo].[Mids]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Mids_dbo.Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currencies] ([CurrencyId])
GO
ALTER TABLE [dbo].[Mids] CHECK CONSTRAINT [FK_dbo.Mids_dbo.Currencies_CurrencyId]
GO
ALTER TABLE [dbo].[Mids]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Mids_dbo.Merchants_Merchant_MerchantId] FOREIGN KEY([Merchant_MerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
GO
ALTER TABLE [dbo].[Mids] CHECK CONSTRAINT [FK_dbo.Mids_dbo.Merchants_Merchant_MerchantId]
GO
ALTER TABLE [dbo].[Mids]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Mids_dbo.Merchants_MerchantId] FOREIGN KEY([MerchantId])
REFERENCES [dbo].[Merchants] ([MerchantId])
GO
ALTER TABLE [dbo].[Mids] CHECK CONSTRAINT [FK_dbo.Mids_dbo.Merchants_MerchantId]
GO
ALTER TABLE [dbo].[Mids]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Mids_dbo.Switches_SwitchId] FOREIGN KEY([SwitchId])
REFERENCES [dbo].[Switches] ([SwitchId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Mids] CHECK CONSTRAINT [FK_dbo.Mids_dbo.Switches_SwitchId]
GO
ALTER TABLE [dbo].[Mids]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Mids_dbo.TransactionCharges_TransactionChargesId] FOREIGN KEY([TransactionChargesId])
REFERENCES [dbo].[TransactionCharges] ([TransactionChargeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Mids] CHECK CONSTRAINT [FK_dbo.Mids_dbo.TransactionCharges_TransactionChargesId]
GO
ALTER TABLE [dbo].[MidsGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MidsGroups_dbo.MidsGroupTypes_MidsGroupTypeId] FOREIGN KEY([MidsGroupTypeId])
REFERENCES [dbo].[MidsGroupTypes] ([MidsGroupTypeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MidsGroups] CHECK CONSTRAINT [FK_dbo.MidsGroups_dbo.MidsGroupTypes_MidsGroupTypeId]
GO
ALTER TABLE [dbo].[MidsMerchantBranches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MidsMerchantBranches_dbo.MerchantBranches_MerchantBranchId] FOREIGN KEY([MerchantBranchId])
REFERENCES [dbo].[MerchantBranches] ([MerchantBranchId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MidsMerchantBranches] CHECK CONSTRAINT [FK_dbo.MidsMerchantBranches_dbo.MerchantBranches_MerchantBranchId]
GO
ALTER TABLE [dbo].[MidsMerchantBranches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MidsMerchantBranches_dbo.Mids_MidId] FOREIGN KEY([MidId])
REFERENCES [dbo].[Mids] ([MidId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MidsMerchantBranches] CHECK CONSTRAINT [FK_dbo.MidsMerchantBranches_dbo.Mids_MidId]
GO
ALTER TABLE [dbo].[MidsMerchantBranchPOSs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MidsMerchantBranchPOSs_dbo.MerchantBranchPOSs_MerchantBranchPOS_MerchantPOSId] FOREIGN KEY([MerchantBranchPOS_MerchantPOSId])
REFERENCES [dbo].[MerchantBranchPOSs] ([MerchantPOSId])
GO
ALTER TABLE [dbo].[MidsMerchantBranchPOSs] CHECK CONSTRAINT [FK_dbo.MidsMerchantBranchPOSs_dbo.MerchantBranchPOSs_MerchantBranchPOS_MerchantPOSId]
GO
ALTER TABLE [dbo].[MidsMerchantBranchPOSs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MidsMerchantBranchPOSs_dbo.MerchantBranchPOSs_MerchantBranchPOSId] FOREIGN KEY([MerchantBranchPOSId])
REFERENCES [dbo].[MerchantBranchPOSs] ([MerchantPOSId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MidsMerchantBranchPOSs] CHECK CONSTRAINT [FK_dbo.MidsMerchantBranchPOSs_dbo.MerchantBranchPOSs_MerchantBranchPOSId]
GO
ALTER TABLE [dbo].[MidsMerchantBranchPOSs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MidsMerchantBranchPOSs_dbo.Mids_MidId] FOREIGN KEY([MidId])
REFERENCES [dbo].[Mids] ([MidId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MidsMerchantBranchPOSs] CHECK CONSTRAINT [FK_dbo.MidsMerchantBranchPOSs_dbo.Mids_MidId]
GO
ALTER TABLE [dbo].[MobileAppFeatures]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MobileAppFeatures_dbo.Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currencies] ([CurrencyId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MobileAppFeatures] CHECK CONSTRAINT [FK_dbo.MobileAppFeatures_dbo.Currencies_CurrencyId]
GO
ALTER TABLE [dbo].[MobileAppLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MobileAppLogs_dbo.MobileApps_MobileAppId] FOREIGN KEY([MobileAppId])
REFERENCES [dbo].[MobileApps] ([MobileAppId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MobileAppLogs] CHECK CONSTRAINT [FK_dbo.MobileAppLogs_dbo.MobileApps_MobileAppId]
GO
ALTER TABLE [dbo].[MobileApps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MobileApps_dbo.MerchantBranchPOSs_MerchantBranchPOSId] FOREIGN KEY([MerchantBranchPOSId])
REFERENCES [dbo].[MerchantBranchPOSs] ([MerchantPOSId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MobileApps] CHECK CONSTRAINT [FK_dbo.MobileApps_dbo.MerchantBranchPOSs_MerchantBranchPOSId]
GO
ALTER TABLE [dbo].[MobileApps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MobileApps_dbo.MobileAppFeatures_MobileAppFeaturesId] FOREIGN KEY([MobileAppFeaturesId])
REFERENCES [dbo].[MobileAppFeatures] ([MobileAppFeaturesId])
GO
ALTER TABLE [dbo].[MobileApps] CHECK CONSTRAINT [FK_dbo.MobileApps_dbo.MobileAppFeatures_MobileAppFeaturesId]
GO
ALTER TABLE [dbo].[MobileApps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MobileApps_dbo.MobileDeviceInfo_MobileDeviceInfoId] FOREIGN KEY([MobileDeviceInfoId])
REFERENCES [dbo].[MobileDeviceInfo] ([MobileDeviceInfoId])
GO
ALTER TABLE [dbo].[MobileApps] CHECK CONSTRAINT [FK_dbo.MobileApps_dbo.MobileDeviceInfo_MobileDeviceInfoId]
GO
ALTER TABLE [dbo].[MobileAppTokenLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MobileAppTokenLogs_dbo.Accounts_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([AccountId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MobileAppTokenLogs] CHECK CONSTRAINT [FK_dbo.MobileAppTokenLogs_dbo.Accounts_AccountId]
GO
ALTER TABLE [dbo].[MobileAppTokenLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MobileAppTokenLogs_dbo.MobileApps_MobileAppId] FOREIGN KEY([MobileAppId])
REFERENCES [dbo].[MobileApps] ([MobileAppId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MobileAppTokenLogs] CHECK CONSTRAINT [FK_dbo.MobileAppTokenLogs_dbo.MobileApps_MobileAppId]
GO
ALTER TABLE [dbo].[Partners]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Partners_dbo.ContactInformation_ContactInformationId] FOREIGN KEY([ContactInformationId])
REFERENCES [dbo].[ContactInformation] ([ContactInformationId])
GO
ALTER TABLE [dbo].[Partners] CHECK CONSTRAINT [FK_dbo.Partners_dbo.ContactInformation_ContactInformationId]
GO
ALTER TABLE [dbo].[Partners]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Partners_dbo.Partners_ParentPartnerId] FOREIGN KEY([ParentPartnerId])
REFERENCES [dbo].[Partners] ([PartnerId])
GO
ALTER TABLE [dbo].[Partners] CHECK CONSTRAINT [FK_dbo.Partners_dbo.Partners_ParentPartnerId]
GO
ALTER TABLE [dbo].[Provinces]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Provinces_dbo.Countries_CountryId] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Countries] ([CountryId])
GO
ALTER TABLE [dbo].[Provinces] CHECK CONSTRAINT [FK_dbo.Provinces_dbo.Countries_CountryId]
GO
ALTER TABLE [dbo].[Resellers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Resellers_dbo.ContactInformation_ContactInformationId] FOREIGN KEY([ContactInformationId])
REFERENCES [dbo].[ContactInformation] ([ContactInformationId])
GO
ALTER TABLE [dbo].[Resellers] CHECK CONSTRAINT [FK_dbo.Resellers_dbo.ContactInformation_ContactInformationId]
GO
ALTER TABLE [dbo].[Resellers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Resellers_dbo.Partners_PartnerId] FOREIGN KEY([PartnerId])
REFERENCES [dbo].[Partners] ([PartnerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Resellers] CHECK CONSTRAINT [FK_dbo.Resellers_dbo.Partners_PartnerId]
GO
ALTER TABLE [dbo].[Resellers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Resellers_dbo.Resellers_ParentResellerId] FOREIGN KEY([ParentResellerId])
REFERENCES [dbo].[Resellers] ([ResellerId])
GO
ALTER TABLE [dbo].[Resellers] CHECK CONSTRAINT [FK_dbo.Resellers_dbo.Resellers_ParentResellerId]
GO
ALTER TABLE [dbo].[SwitchParameters]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SwitchParameters_dbo.SwitchParameterTypes_ParameterTypeId] FOREIGN KEY([ParameterTypeId])
REFERENCES [dbo].[SwitchParameterTypes] ([SwitchParameterTypeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SwitchParameters] CHECK CONSTRAINT [FK_dbo.SwitchParameters_dbo.SwitchParameterTypes_ParameterTypeId]
GO
ALTER TABLE [dbo].[SwitchPartnerLink]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SwitchPartnerLink_dbo.Partners_PartnerId] FOREIGN KEY([PartnerId])
REFERENCES [dbo].[Partners] ([PartnerId])
GO
ALTER TABLE [dbo].[SwitchPartnerLink] CHECK CONSTRAINT [FK_dbo.SwitchPartnerLink_dbo.Partners_PartnerId]
GO
ALTER TABLE [dbo].[SwitchPartnerLink]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SwitchPartnerLink_dbo.Switches_Switch_SwitchId] FOREIGN KEY([Switch_SwitchId])
REFERENCES [dbo].[Switches] ([SwitchId])
GO
ALTER TABLE [dbo].[SwitchPartnerLink] CHECK CONSTRAINT [FK_dbo.SwitchPartnerLink_dbo.Switches_Switch_SwitchId]
GO
ALTER TABLE [dbo].[SwitchPartnerLink]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SwitchPartnerLink_dbo.Switches_SwitchId] FOREIGN KEY([SwitchId])
REFERENCES [dbo].[Switches] ([SwitchId])
GO
ALTER TABLE [dbo].[SwitchPartnerLink] CHECK CONSTRAINT [FK_dbo.SwitchPartnerLink_dbo.Switches_SwitchId]
GO
ALTER TABLE [dbo].[TempTransactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TempTransactions_dbo.CardTypes_CardTypeId] FOREIGN KEY([CardTypeId])
REFERENCES [dbo].[CardTypes] ([CardTypeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TempTransactions] CHECK CONSTRAINT [FK_dbo.TempTransactions_dbo.CardTypes_CardTypeId]
GO
ALTER TABLE [dbo].[TempTransactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TempTransactions_dbo.Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currencies] ([CurrencyId])
GO
ALTER TABLE [dbo].[TempTransactions] CHECK CONSTRAINT [FK_dbo.TempTransactions_dbo.Currencies_CurrencyId]
GO
ALTER TABLE [dbo].[TempTransactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TempTransactions_dbo.Keys_KeyId] FOREIGN KEY([KeyId])
REFERENCES [dbo].[Keys] ([KeyId])
GO
ALTER TABLE [dbo].[TempTransactions] CHECK CONSTRAINT [FK_dbo.TempTransactions_dbo.Keys_KeyId]
GO
ALTER TABLE [dbo].[TempTransactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TempTransactions_dbo.MerchantBranchPOSs_MerchantPOSId] FOREIGN KEY([MerchantPOSId])
REFERENCES [dbo].[MerchantBranchPOSs] ([MerchantPOSId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TempTransactions] CHECK CONSTRAINT [FK_dbo.TempTransactions_dbo.MerchantBranchPOSs_MerchantPOSId]
GO
ALTER TABLE [dbo].[TempTransactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TempTransactions_dbo.Mids_MidId] FOREIGN KEY([MidId])
REFERENCES [dbo].[Mids] ([MidId])
GO
ALTER TABLE [dbo].[TempTransactions] CHECK CONSTRAINT [FK_dbo.TempTransactions_dbo.Mids_MidId]
GO
ALTER TABLE [dbo].[TempTransactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TempTransactions_dbo.TransactionEntryTypes_TransactionEntryTypeId] FOREIGN KEY([TransactionEntryTypeId])
REFERENCES [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TempTransactions] CHECK CONSTRAINT [FK_dbo.TempTransactions_dbo.TransactionEntryTypes_TransactionEntryTypeId]
GO
ALTER TABLE [dbo].[TransactionAttemptCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.Accounts_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([AccountId])
GO
ALTER TABLE [dbo].[TransactionAttemptCash] CHECK CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.Accounts_AccountId]
GO
ALTER TABLE [dbo].[TransactionAttemptCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.MobileApps_MobileAppId] FOREIGN KEY([MobileAppId])
REFERENCES [dbo].[MobileApps] ([MobileAppId])
GO
ALTER TABLE [dbo].[TransactionAttemptCash] CHECK CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.MobileApps_MobileAppId]
GO
ALTER TABLE [dbo].[TransactionAttemptCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionCash_TransactionCash_TransactionCashId] FOREIGN KEY([TransactionCash_TransactionCashId])
REFERENCES [dbo].[TransactionCash] ([TransactionCashId])
GO
ALTER TABLE [dbo].[TransactionAttemptCash] CHECK CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionCash_TransactionCash_TransactionCashId]
GO
ALTER TABLE [dbo].[TransactionAttemptCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionCash_TransactionCashId] FOREIGN KEY([TransactionCashId])
REFERENCES [dbo].[TransactionCash] ([TransactionCashId])
GO
ALTER TABLE [dbo].[TransactionAttemptCash] CHECK CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionCash_TransactionCashId]
GO
ALTER TABLE [dbo].[TransactionAttemptCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionCharges_TransactionChargesId] FOREIGN KEY([TransactionChargesId])
REFERENCES [dbo].[TransactionCharges] ([TransactionChargeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransactionAttemptCash] CHECK CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionCharges_TransactionChargesId]
GO
ALTER TABLE [dbo].[TransactionAttemptCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionSignature_TransactionSignatureId] FOREIGN KEY([TransactionSignatureId])
REFERENCES [dbo].[TransactionSignature] ([TransactionSignatureId])
GO
ALTER TABLE [dbo].[TransactionAttemptCash] CHECK CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionSignature_TransactionSignatureId]
GO
ALTER TABLE [dbo].[TransactionAttemptCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionVoidReason_TransactionVoidReasonId] FOREIGN KEY([TransactionVoidReasonId])
REFERENCES [dbo].[TransactionVoidReason] ([TransactionVoidReasonId])
GO
ALTER TABLE [dbo].[TransactionAttemptCash] CHECK CONSTRAINT [FK_dbo.TransactionAttemptCash_dbo.TransactionVoidReason_TransactionVoidReasonId]
GO
ALTER TABLE [dbo].[TransactionAttemptDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.Accounts_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([AccountId])
GO
ALTER TABLE [dbo].[TransactionAttemptDebit] CHECK CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.Accounts_AccountId]
GO
ALTER TABLE [dbo].[TransactionAttemptDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.Devices_DeviceId] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Devices] ([DeviceId])
GO
ALTER TABLE [dbo].[TransactionAttemptDebit] CHECK CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.Devices_DeviceId]
GO
ALTER TABLE [dbo].[TransactionAttemptDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.MobileApps_MobileAppId] FOREIGN KEY([MobileAppId])
REFERENCES [dbo].[MobileApps] ([MobileAppId])
GO
ALTER TABLE [dbo].[TransactionAttemptDebit] CHECK CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.MobileApps_MobileAppId]
GO
ALTER TABLE [dbo].[TransactionAttemptDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.TransactionCharges_TransactionChargesId] FOREIGN KEY([TransactionChargesId])
REFERENCES [dbo].[TransactionCharges] ([TransactionChargeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransactionAttemptDebit] CHECK CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.TransactionCharges_TransactionChargesId]
GO
ALTER TABLE [dbo].[TransactionAttemptDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.TransactionDebit_TransactionDebit_TransactionDebitId] FOREIGN KEY([TransactionDebit_TransactionDebitId])
REFERENCES [dbo].[TransactionDebit] ([TransactionDebitId])
GO
ALTER TABLE [dbo].[TransactionAttemptDebit] CHECK CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.TransactionDebit_TransactionDebit_TransactionDebitId]
GO
ALTER TABLE [dbo].[TransactionAttemptDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.TransactionDebit_TransactionDebitId] FOREIGN KEY([TransactionDebitId])
REFERENCES [dbo].[TransactionDebit] ([TransactionDebitId])
GO
ALTER TABLE [dbo].[TransactionAttemptDebit] CHECK CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.TransactionDebit_TransactionDebitId]
GO
ALTER TABLE [dbo].[TransactionAttemptDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.TransactionSignature_TransactionSignatureId] FOREIGN KEY([TransactionSignatureId])
REFERENCES [dbo].[TransactionSignature] ([TransactionSignatureId])
GO
ALTER TABLE [dbo].[TransactionAttemptDebit] CHECK CONSTRAINT [FK_dbo.TransactionAttemptDebit_dbo.TransactionSignature_TransactionSignatureId]
GO
ALTER TABLE [dbo].[TransactionAttempts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttempts_dbo.Accounts_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([AccountId])
GO
ALTER TABLE [dbo].[TransactionAttempts] CHECK CONSTRAINT [FK_dbo.TransactionAttempts_dbo.Accounts_AccountId]
GO
ALTER TABLE [dbo].[TransactionAttempts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttempts_dbo.Devices_DeviceId] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Devices] ([DeviceId])
GO
ALTER TABLE [dbo].[TransactionAttempts] CHECK CONSTRAINT [FK_dbo.TransactionAttempts_dbo.Devices_DeviceId]
GO
ALTER TABLE [dbo].[TransactionAttempts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttempts_dbo.MobileApps_MobileAppId] FOREIGN KEY([MobileAppId])
REFERENCES [dbo].[MobileApps] ([MobileAppId])
GO
ALTER TABLE [dbo].[TransactionAttempts] CHECK CONSTRAINT [FK_dbo.TransactionAttempts_dbo.MobileApps_MobileAppId]
GO
ALTER TABLE [dbo].[TransactionAttempts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttempts_dbo.TransactionCharges_TransactionChargesId] FOREIGN KEY([TransactionChargesId])
REFERENCES [dbo].[TransactionCharges] ([TransactionChargeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransactionAttempts] CHECK CONSTRAINT [FK_dbo.TransactionAttempts_dbo.TransactionCharges_TransactionChargesId]
GO
ALTER TABLE [dbo].[TransactionAttempts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttempts_dbo.Transactions_Transaction_TransactionId] FOREIGN KEY([Transaction_TransactionId])
REFERENCES [dbo].[Transactions] ([TransactionId])
GO
ALTER TABLE [dbo].[TransactionAttempts] CHECK CONSTRAINT [FK_dbo.TransactionAttempts_dbo.Transactions_Transaction_TransactionId]
GO
ALTER TABLE [dbo].[TransactionAttempts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttempts_dbo.Transactions_TransactionId] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transactions] ([TransactionId])
GO
ALTER TABLE [dbo].[TransactionAttempts] CHECK CONSTRAINT [FK_dbo.TransactionAttempts_dbo.Transactions_TransactionId]
GO
ALTER TABLE [dbo].[TransactionAttempts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttempts_dbo.TransactionSignature_TransactionSignatureId] FOREIGN KEY([TransactionSignatureId])
REFERENCES [dbo].[TransactionSignature] ([TransactionSignatureId])
GO
ALTER TABLE [dbo].[TransactionAttempts] CHECK CONSTRAINT [FK_dbo.TransactionAttempts_dbo.TransactionSignature_TransactionSignatureId]
GO
ALTER TABLE [dbo].[TransactionAttempts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionAttempts_dbo.TransactionVoidReason_TransactionVoidReasonId] FOREIGN KEY([TransactionVoidReasonId])
REFERENCES [dbo].[TransactionVoidReason] ([TransactionVoidReasonId])
GO
ALTER TABLE [dbo].[TransactionAttempts] CHECK CONSTRAINT [FK_dbo.TransactionAttempts_dbo.TransactionVoidReason_TransactionVoidReasonId]
GO
ALTER TABLE [dbo].[TransactionCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionCash_dbo.Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currencies] ([CurrencyId])
GO
ALTER TABLE [dbo].[TransactionCash] CHECK CONSTRAINT [FK_dbo.TransactionCash_dbo.Currencies_CurrencyId]
GO
ALTER TABLE [dbo].[TransactionCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionCash_dbo.MerchantBranchPOSs_MerchantPOSId] FOREIGN KEY([MerchantPOSId])
REFERENCES [dbo].[MerchantBranchPOSs] ([MerchantPOSId])
GO
ALTER TABLE [dbo].[TransactionCash] CHECK CONSTRAINT [FK_dbo.TransactionCash_dbo.MerchantBranchPOSs_MerchantPOSId]
GO
ALTER TABLE [dbo].[TransactionCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionCash_dbo.Mids_MidId] FOREIGN KEY([MidId])
REFERENCES [dbo].[Mids] ([MidId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransactionCash] CHECK CONSTRAINT [FK_dbo.TransactionCash_dbo.Mids_MidId]
GO
ALTER TABLE [dbo].[TransactionCash]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionCash_dbo.TransactionEntryTypes_TransactionEntryTypeId] FOREIGN KEY([TransactionEntryTypeId])
REFERENCES [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId])
GO
ALTER TABLE [dbo].[TransactionCash] CHECK CONSTRAINT [FK_dbo.TransactionCash_dbo.TransactionEntryTypes_TransactionEntryTypeId]
GO
ALTER TABLE [dbo].[TransactionDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionDebit_dbo.AccountType_AccountTypeId] FOREIGN KEY([AccountTypeId])
REFERENCES [dbo].[AccountType] ([AccountTypeId])
GO
ALTER TABLE [dbo].[TransactionDebit] CHECK CONSTRAINT [FK_dbo.TransactionDebit_dbo.AccountType_AccountTypeId]
GO
ALTER TABLE [dbo].[TransactionDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionDebit_dbo.Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currencies] ([CurrencyId])
GO
ALTER TABLE [dbo].[TransactionDebit] CHECK CONSTRAINT [FK_dbo.TransactionDebit_dbo.Currencies_CurrencyId]
GO
ALTER TABLE [dbo].[TransactionDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionDebit_dbo.Keys_KeyId] FOREIGN KEY([KeyId])
REFERENCES [dbo].[Keys] ([KeyId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransactionDebit] CHECK CONSTRAINT [FK_dbo.TransactionDebit_dbo.Keys_KeyId]
GO
ALTER TABLE [dbo].[TransactionDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionDebit_dbo.MerchantBranchPOSs_MerchantPOSId] FOREIGN KEY([MerchantPOSId])
REFERENCES [dbo].[MerchantBranchPOSs] ([MerchantPOSId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransactionDebit] CHECK CONSTRAINT [FK_dbo.TransactionDebit_dbo.MerchantBranchPOSs_MerchantPOSId]
GO
ALTER TABLE [dbo].[TransactionDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionDebit_dbo.Mids_MidId] FOREIGN KEY([MidId])
REFERENCES [dbo].[Mids] ([MidId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransactionDebit] CHECK CONSTRAINT [FK_dbo.TransactionDebit_dbo.Mids_MidId]
GO
ALTER TABLE [dbo].[TransactionDebit]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TransactionDebit_dbo.TransactionEntryTypes_TransactionEntryTypeId] FOREIGN KEY([TransactionEntryTypeId])
REFERENCES [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId])
GO
ALTER TABLE [dbo].[TransactionDebit] CHECK CONSTRAINT [FK_dbo.TransactionDebit_dbo.TransactionEntryTypes_TransactionEntryTypeId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.CardTypes_CardTypeId] FOREIGN KEY([CardTypeId])
REFERENCES [dbo].[CardTypes] ([CardTypeId])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.CardTypes_CardTypeId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currencies] ([CurrencyId])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.Currencies_CurrencyId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.Keys_KeyId] FOREIGN KEY([KeyId])
REFERENCES [dbo].[Keys] ([KeyId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.Keys_KeyId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.MerchantBranchPOSs_MerchantPOSId] FOREIGN KEY([MerchantPOSId])
REFERENCES [dbo].[MerchantBranchPOSs] ([MerchantPOSId])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.MerchantBranchPOSs_MerchantPOSId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.Mids_MidId] FOREIGN KEY([MidId])
REFERENCES [dbo].[Mids] ([MidId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.Mids_MidId]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Transactions_dbo.TransactionEntryTypes_TransactionEntryTypeId] FOREIGN KEY([TransactionEntryTypeId])
REFERENCES [dbo].[TransactionEntryTypes] ([TransactionEntryTypeId])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_dbo.Transactions_dbo.TransactionEntryTypes_TransactionEntryTypeId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Users_dbo.ContactInformation_ContactInformationId] FOREIGN KEY([ContactInformationId])
REFERENCES [dbo].[ContactInformation] ([ContactInformationId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_dbo.Users_dbo.ContactInformation_ContactInformationId]
GO
