CREATE TABLE [dbo].[AccessPoint] (
    [WifiBSSID]                      CHAR(17)    NOT NULL,
    [WifiSSID]                       NVARCHAR (32)    NOT NULL,
    [DistrictID]                     UNIQUEIDENTIFIER NULL,
    [SchoolID]                       UNIQUEIDENTIFIER NULL,
	[ClassroomID] UNIQUEIDENTIFIER NULL,
    [AccessPointMaxDownloadLicenses] INT              NOT NULL DEFAULT 30,
    [AccessPointExpiryTimeSeconds]   INT              NULL DEFAULT 3600,
    [AccessPointAnnotation]          NVARCHAR (200)   NULL,
    [AccessPointModel]               NVARCHAR (50)    NULL,
	[Created]                        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    PRIMARY KEY NONCLUSTERED ([WifiBSSID] ASC),
    CONSTRAINT [FK_AccessPoint_ToDistrict] FOREIGN KEY ([DistrictID]) REFERENCES [District] ([DistrictID]),
    CONSTRAINT [FK_AccessPoint_ToSchool] FOREIGN KEY ([SchoolID]) REFERENCES [School] ([SchoolID]), 
    CONSTRAINT [FK_AccessPoint_ToClassroom] FOREIGN KEY ([ClassroomID]) REFERENCES [Classroom]([ClassroomID])
);
GO

CREATE NONCLUSTERED INDEX [IX_AccessPoint_WifiBSSID]
    ON [dbo].[AccessPoint]([WifiBSSID] ASC);
GO

CREATE CLUSTERED INDEX [IX_AccessPoint_Created]
    ON [dbo].[AccessPoint] ([Created] ASC);

GO

CREATE INDEX [IX_AccessPoint_DistrictID] ON [dbo].[AccessPoint] ([DistrictID])

GO

CREATE INDEX [IX_AccessPoint_SchoolID] ON [dbo].[AccessPoint] ([SchoolID])
