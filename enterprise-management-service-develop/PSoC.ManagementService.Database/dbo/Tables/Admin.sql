CREATE TABLE [dbo].[Admin] (
    [UserID]           UNIQUEIDENTIFIER NOT NULL,
    [DistrictID]        UNIQUEIDENTIFIER NULL,
    [SchoolID]          UNIQUEIDENTIFIER NULL,
	[Active] BIT NOT NULL DEFAULT 1 ,
    [AdminEmail] NVARCHAR (50) NULL,
    [LastLoginDateTime] DATETIME      NULL,
    [Created]         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    PRIMARY KEY NONCLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_Admin_ToUser] FOREIGN KEY ([UserID]) REFERENCES [User]([UserID]),
    CONSTRAINT [FK_Admin_ToDistrict] FOREIGN KEY ([DistrictID]) REFERENCES [dbo].[District] ([DistrictID]), 
    CONSTRAINT [FK_Admin_ToSchool] FOREIGN KEY ([SchoolID]) REFERENCES [School]([SchoolID]), 
);
GO

CREATE INDEX [IX_Admin_UserID] ON [dbo].[Admin] ([UserID])
GO

CREATE NONCLUSTERED INDEX [IX_Admin_DistrictID]
    ON [dbo].[Admin]([DistrictID] ASC);
GO

CREATE CLUSTERED INDEX [IX_DistrictAdministrator_Created] ON [dbo].[Admin] ([Created])
