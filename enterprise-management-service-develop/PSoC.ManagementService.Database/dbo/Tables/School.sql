CREATE TABLE [dbo].[School] (
    [SchoolID]                       UNIQUEIDENTIFIER NOT NULL,
    [DistrictID]                     UNIQUEIDENTIFIER NOT NULL,
    [SchoolName]                     NVARCHAR (50)    NULL,
    [SchoolAddress1]                 NVARCHAR (80)    NULL,
    [SchoolAddress2]                 NVARCHAR (80)    NULL,
    [SchoolTown]                     NVARCHAR (80)    NULL,
    [SchoolState]                    NVARCHAR (80)    NULL,
    [SchoolZipCode]                  NVARCHAR (10)    NULL,
    [SchoolGrades]                   NVARCHAR (50)    NULL,
    [SchoolMaxDownloadLicenses]      INT              NOT NULL DEFAULT 1000,
    [SchoolLicenseExpirySeconds]     INT              NULL DEFAULT 3600,
    [GradeInstructionHoursBegin]     TIME (7)         NULL,
    [GradeInstructionHoursEnd]       TIME (7)         NULL,
    [SchoolOverrideCode]             NVARCHAR (50)    NULL,
    [GradePreloadHoursBegin]         TIME (7)         NULL,
    [GradePreloadHoursEnd]           TIME (7)         NULL,
    [SchoolUseCacheServer]           NVARCHAR (50)    NULL,
    [SchoolUserPolicy]               INT              NULL,
    [SchoolAnnotation]               NVARCHAR (200)   NULL,
	[Created]                        DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    PRIMARY KEY NONCLUSTERED ([SchoolID] ASC),
    CONSTRAINT [FK_School_ToDistrict] FOREIGN KEY ([DistrictID]) REFERENCES [dbo].[District] ([DistrictID])
);
GO

CREATE NONCLUSTERED INDEX [IX_School_SchoolID]
    ON [dbo].[School]([SchoolID] ASC);
GO

CREATE CLUSTERED INDEX [IX_School_Created]
    ON [dbo].[School] ([Created] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_School_SchoolName]
    ON [dbo].[School]([SchoolName] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_School_SchoolTown]
    ON [dbo].[School]([SchoolTown] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_School_SchoolZipCode]
    ON [dbo].[School]([SchoolZipCode] ASC);


GO

CREATE INDEX [IX_School_DistrictID] ON [dbo].[School] ([DistrictID])
