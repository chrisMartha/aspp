--SET IDENTITY_INSERT [LicenseRequestType] ON 
--GO 
	
-- Reference Data for License Request Types
MERGE INTO [LicenseRequestType] AS Target 
USING (VALUES 
  (1, N'Request License', N'Device is requesting a new license or reporting it has a license and continuing download that results in slide of expiry time'),
  (2, N'Return License', N'Device is returning the license'),
  (3, N'Revoke License', N'Administrator has requested to revoke license already granted for a specific device'),
  (4, N'Server Grant', N'This may come up from push (to distinguish pull request from push request)')
) 
AS Source (LicenseRequestTypeID, LicenseRequestTypeName, LicenseRequestTypeDescription) 
ON Target.LicenseRequestTypeID = Source.LicenseRequestTypeID

-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET LicenseRequestTypeName = Source.LicenseRequestTypeName,
           LicenseRequestTypeDescription = Source.LicenseRequestTypeDescription

-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (LicenseRequestTypeID,
		LicenseRequestTypeName, 
        LicenseRequestTypeDescription) 
VALUES (Source.LicenseRequestTypeID,
		Source.LicenseRequestTypeName, 
        Source.LicenseRequestTypeDescription);

-- delete rows that are in the target but not in the source
--WHEN NOT MATCHED BY SOURCE THEN
--DELETE;

--SET IDENTITY_INSERT [LicenseRequestType] OFF
--GO