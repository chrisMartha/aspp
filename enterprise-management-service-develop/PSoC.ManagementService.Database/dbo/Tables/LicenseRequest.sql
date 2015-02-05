CREATE TABLE [dbo].[LicenseRequest] (
    [LicenseRequestID]     UNIQUEIDENTIFIER NOT NULL,
	[PrevLicenseRequestID] UNIQUEIDENTIFIER NULL,
    [DistrictID]            UNIQUEIDENTIFIER NULL,
    [SchoolID]              UNIQUEIDENTIFIER NULL,
    [ConfigCode]          NVARCHAR (50) NOT NULL,
    [WifiBSSID]              CHAR (17) NOT NULL,
    [LicenseRequestTypeID] INT NOT NULL,
    [DeviceID]              UNIQUEIDENTIFIER NOT NULL,
	[UserID] UNIQUEIDENTIFIER NOT NULL,
    [RequestDateTime]       DATETIME      NOT NULL,
    [Response]              NVARCHAR (50) NULL,
    [ResponseDateTime]      DATETIME      NULL,
    [LocationID]            NVARCHAR (50) NULL,
    [LocationName]          NVARCHAR (50) NULL,
    [LearningContentQueued] INT           NULL,
    [Created] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(), 
    PRIMARY KEY NONCLUSTERED ([LicenseRequestID] ASC),
    CONSTRAINT [FK_LicenseRequest_ToDistrict] FOREIGN KEY ([DistrictID]) REFERENCES [dbo].[District] ([DistrictID]),
    CONSTRAINT [FK_LicenseRequest_ToSchool] FOREIGN KEY ([SchoolID]) REFERENCES [dbo].[School] ([SchoolID]), 
    CONSTRAINT [FK_LicenseRequest_ToAccessPoint] FOREIGN KEY ([WifiBSSID]) REFERENCES [dbo].[AccessPoint] ([WifiBSSID]),
    CONSTRAINT [FK_LicenseRequest_ToLicenseRequestType] FOREIGN KEY ([LicenseRequestTypeID]) REFERENCES [LicenseRequestType]([LicenseRequestTypeID]),
    CONSTRAINT [FK_LicenseRequest_ToDevice] FOREIGN KEY ([DeviceID]) REFERENCES [dbo].[Device] ([DeviceID]),
    CONSTRAINT [FK_LicenseRequest_ToUser] FOREIGN KEY ([UserID]) REFERENCES [User]([UserID])
);
GO

CREATE INDEX [IX_LicenseRequest_LicenseID] ON [dbo].[LicenseRequest] ([LicenseRequestID])
GO

CREATE CLUSTERED INDEX [IX_LicenseRequest_Created] ON [dbo].[LicenseRequest] ([Created])
GO

CREATE NONCLUSTERED INDEX [IX_LicenseRequest_DeviceID]
    ON [dbo].[LicenseRequest]([DeviceID] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_LicenseRequest_DeviceID_Created_LicenseRequestTypeID] 
	ON [dbo].[LicenseRequest]
(
	[DeviceID] ASC,
	[Created] DESC,
	[LicenseRequestTypeID] ASC
)
INCLUDE ([LicenseRequestID])
GO