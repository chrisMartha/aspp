CREATE TABLE [dbo].[District] (
    [DistrictID]                    UNIQUEIDENTIFIER  NOT NULL,
    [CreatedBy]                     NVARCHAR (50)     NULL,
    [CreationDate]                  DATETIME2         NOT NULL DEFAULT SYSUTCDATETIME(),
    [DistrictName]                  NVARCHAR (50)     NOT NULL,
    [DistrictMaxDownloadLicenses]   INT               NOT NULL DEFAULT 10000,
    [DistrictInstructionHoursStart] TIME (7)          NULL,
    [DistrictInstructionHoursEnd]   TIME (7)          NULL,
    [DistrictLicenseExpirySeconds]  INT               NOT NULL DEFAULT 3600,
    [DistrictPreloadHoursStart]     TIME (7)          NULL,
    [DistrictPreloadHoursEnd]       TIME (7)          NULL,
    [DistrictOverrideCode]          NVARCHAR (50)     NULL,
    [DistrictUserPolicy]            INT               NULL,
    [DistrictUseCacheServer]        NCHAR (10)        NULL,
    [DistrictAnnotation]            NVARCHAR (200)    NULL,
    [OAuthApplicationId]            NVARCHAR (50)     NOT NULL,
    [OAuthClientId]                 NVARCHAR (50)     NOT NULL,
    [OAuthURL]                      NVARCHAR (200)    NOT NULL,
    PRIMARY KEY NONCLUSTERED ([DistrictID] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_District_DistrictName]
    ON [dbo].[District]([DistrictName] ASC);
GO

CREATE CLUSTERED INDEX [IX_District_CreationDate]
	ON [dbo].[District] ([CreationDate])
