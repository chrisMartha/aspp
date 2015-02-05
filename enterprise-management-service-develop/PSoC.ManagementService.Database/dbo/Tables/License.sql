CREATE TABLE [dbo].[License] (
    [LicenseRequestID]            UNIQUEIDENTIFIER NOT NULL,
    [DistrictID]            UNIQUEIDENTIFIER NULL,
    [SchoolID]              UNIQUEIDENTIFIER NULL,
    [ConfigCode]          NVARCHAR (50) NOT NULL,
    [WifiBSSID]              CHAR (17) NOT NULL,
    [LicenseIssueDateTime] DATETIME      NOT NULL,
    [LicenseExpiryDateTime] DATETIME      NOT NULL,
    [Created]           DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    PRIMARY KEY NONCLUSTERED ([LicenseRequestID] ASC),
    CONSTRAINT [FK_License_ToLicenseRequest] FOREIGN KEY ([LicenseRequestID]) REFERENCES [LicenseRequest]([LicenseRequestID]),
    CONSTRAINT [FK_License_ToDistrict] FOREIGN KEY ([DistrictID]) REFERENCES [dbo].[District] ([DistrictID]),
    CONSTRAINT [FK_License_ToSchool] FOREIGN KEY ([SchoolID]) REFERENCES [dbo].[School] ([SchoolID]),
    CONSTRAINT [FK_License_ToAccessPoint] FOREIGN KEY ([WifiBSSID]) REFERENCES [dbo].[AccessPoint] ([WifiBSSID])
);
GO

CREATE INDEX [IX_License_LicenseID] ON [dbo].[License] ([LicenseRequestID])
GO

CREATE CLUSTERED INDEX [IX_License_Created] ON [dbo].[License] ([Created])
GO

CREATE NONCLUSTERED INDEX [IX_License_LicenseIssueDateTime]
    ON [dbo].[License]([LicenseIssueDateTime] ASC)
GO

CREATE INDEX [IX_License_DistrictID] ON [dbo].[License] ([DistrictID])
GO

CREATE INDEX [IX_License_SchoolID] ON [dbo].[License] ([SchoolID])
GO

CREATE INDEX [IX_License_LicenseRequestID_WifiBSSID] ON [dbo].[License] ([LicenseRequestID], [WifiBSSID])
GO

CREATE INDEX [IX_License_LicenseExpiryDateTime] ON [dbo].[License] ([LicenseExpiryDateTime])
GO

CREATE INDEX [IX_License_DistrictID_LicenseExpiryDateTime] ON [dbo].[License] ([DistrictID], [LicenseExpiryDateTime])
GO

CREATE INDEX [IX_License_SchoolID_LicenseExpiryDateTime] ON [dbo].[License] ([SchoolID], [LicenseExpiryDateTime])
GO

CREATE INDEX [IX_License_WifiBSSID_LicenseExpiryDateTime] ON [dbo].[License] ([WifiBSSID], [LicenseExpiryDateTime])