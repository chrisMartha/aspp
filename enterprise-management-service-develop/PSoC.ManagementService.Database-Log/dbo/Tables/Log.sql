CREATE TABLE [dbo].[Log](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Logger] [nvarchar](100) NOT NULL,
	[LogDate] [date] NOT NULL DEFAULT GETUTCDATE(),
	[LogTime] [time] NOT NULL DEFAULT SYSUTCDATETIME(),
	[Level] [nvarchar](20) NOT NULL,
	[UserId] [nvarchar](100) NULL,
	[Message] [nvarchar](4000) NULL,
	[Exception] [nvarchar](max) NULL,
	[ThreadId] INT NULL,
	[RequestLength] BIGINT NULL,
	[ResponseLength] BIGINT NULL,
	[Duration] BIGINT NULL,
	[IpAddress] [nvarchar](100) NULL,
	[UserAgent] [nvarchar](max) NULL,
	-- Addtional native fields
	[EventId] INT NULL,
	[Keywords] [nvarchar](100) NULL,
	[Task] [nvarchar](100) NULL,
	[InstanceName] [nvarchar](128) NULL,
	[ProcessId] INT NULL,
	[HttpMethod] [nvarchar](20) NULL,
	[Url] [nvarchar](4000) NULL,
	[HttpStatusCode] SMALLINT NULL,
	[EventSource] [nvarchar](100) NULL,
	[EventDestination] [nvarchar](100) NULL,
	[Event] [nvarchar](100) NULL,
	[EventDescription] [nvarchar](4000) NULL,
	-- Custom fields
	[DistrictId] [nvarchar](100) NULL,
	[SchoolId] [nvarchar](100) NULL,
	[ClassroomId] [nvarchar](100) NULL,
	[AccessPointId] [nvarchar](100) NULL,
	[DeviceId] [nvarchar](100) NULL,
	[AppId] [nvarchar](100) NULL,
	[LicenseRequestId] [nvarchar](100) NULL,
	[ConfigCode] [nvarchar](50) NULL,
	[DownloadRequested] INT NULL,
	[ItemsQueued] INT NULL,
	[GrantDenyDecision] [nvarchar](50) NULL,
	[CountByAccessPoint] INT NULL,
	[CountBySchool] INT NULL,
	[CountByDistrict] INT NULL,
	CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_Log_Logger] ON [dbo].[Log]
(
	[Logger] ASC	
)
GO
CREATE NONCLUSTERED INDEX [IX_Log_LogDate] ON [dbo].[Log]
(
	[LogDate] ASC
)
GO
CREATE NONCLUSTERED INDEX [IX_Log_LogTime] ON [dbo].[Log]
(
	[LogTime] ASC
)
GO
CREATE NONCLUSTERED INDEX [IX_Log_DeviceId] ON [dbo].[Log]
(
	[DeviceId] ASC
)
GO
