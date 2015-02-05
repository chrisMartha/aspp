CREATE TABLE [dbo].[LicenseRequestType]
(
	[LicenseRequestTypeID] INT NOT NULL PRIMARY KEY, 
    [LicenseRequestTypeName] NVARCHAR(50) NOT NULL, 
    [LicenseRequestTypeDescription] NVARCHAR(2000) NULL
)
