CREATE TABLE [dbo].[Device] (
    [DeviceID]             UNIQUEIDENTIFIER  NOT NULL,
	[DeviceName] BINARY (256) NULL, 
	[DeviceNameHash] BINARY(20) NULL,
    [DeviceType]           NVARCHAR (50)  NULL,
    [DeviceOS]             NVARCHAR (50)  NULL,
    [DeviceOSVersion]      NVARCHAR (50)  NULL,
    [PSoCAppID]            NVARCHAR(50)  NULL,
    [PSoCAppVersion]       NVARCHAR (50)  NULL,
    [DeviceFreeSpace]      BIGINT  NULL,
    [DistrictID]           UNIQUEIDENTIFIER  NULL,
    [SchoolID]             UNIQUEIDENTIFIER  NULL,
    [LastUsedConfigCode]         NVARCHAR (50)  NULL,
    [DeviceAnnotation]     NVARCHAR (80)  NULL,
    [ConfiguredGrades]     NVARCHAR (50)  NULL,
    [ConfiguredUnitCount]  INT            NULL,
    [ContentLastUpdatedAt] DATETIME       NULL,
	[InstalledContentSize] BIGINT NULL,
	[GeoLocation] NVARCHAR(50) NULL,
    [Created] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(), 
    PRIMARY KEY NONCLUSTERED ([DeviceID] ASC)
);
GO

CREATE INDEX [IX_Device_DeviceID] ON [dbo].[Device] ([DeviceID])
GO

CREATE CLUSTERED INDEX [IX_Device_Created] ON [dbo].[Device] ([Created])