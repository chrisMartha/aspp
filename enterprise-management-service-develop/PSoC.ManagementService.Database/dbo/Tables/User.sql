CREATE TABLE [dbo].[User]
(
	[UserID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED, 
    [Username]  BINARY (256) NULL, 
	[UsernameHash] BINARY(20) NULL, 
	[UserType] BINARY (256) NULL, 
	[UserTypeHash] BINARY(20) NULL, 
    [Created] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
)
GO

CREATE INDEX [IX_User_UserID] ON [dbo].[User] ([UserID])
GO

CREATE CLUSTERED INDEX [IX_User_Created] ON [dbo].[User] ([Created])
GO

CREATE INDEX [IX_User_UsernameHash] ON [dbo].[User] ([UsernameHash])
