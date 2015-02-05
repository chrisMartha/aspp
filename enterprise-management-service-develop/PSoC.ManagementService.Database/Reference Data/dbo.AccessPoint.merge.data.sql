--SET IDENTITY_INSERT [AccessPoint] ON 
--GO 
	
-- Reference Data for Access Point
MERGE INTO [AccessPoint] AS Target 
USING (VALUES 
  ('80:cc:68:b9:1d:c0', 'TWCable', '00FCC2A7-A3D5-469C-A790-F2E8BCD44BCA', '82C5E4D1-C449-4F09-8062-2493B5C615C5', NULL, 2, 1200, 'Sample data brought to you by Atul', NULL, '2014-12-15 21:46:35.4535339'),
  ('a2:aa:22:b9:1d:c0', 'epic', NULL, NULL, NULL, 1, 3600, 'Sample data brought to you by Atul', NULL, '2014-12-15 21:47:59.9074987')
) 
AS Source (WifiBSSID, WifiSSID, DistrictID, SchoolID, ClassroomID, AccessPointMaxDownloadLicenses, AccessPointExpiryTimeSeconds, AccessPointAnnotation, AccessPointModel, Created) 
ON Target.WifiBSSID = Source.WifiBSSID

-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET WifiSSID                       = Source.WifiSSID,
           DistrictID                     = Source.DistrictID,
           SchoolID                       = Source.SchoolID,
           ClassroomID                    = Source.ClassroomID,
           AccessPointMaxDownloadLicenses = Source.AccessPointMaxDownloadLicenses,
           AccessPointExpiryTimeSeconds   = Source.AccessPointExpiryTimeSeconds,
           AccessPointAnnotation          = Source.AccessPointAnnotation,
           AccessPointModel               = Source.AccessPointModel,
           Created                        = Source.Created

-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (WifiBSSID,
        WifiSSID,
		DistrictID,
		SchoolID,
		ClassroomID,
		AccessPointMaxDownloadLicenses,
		AccessPointExpiryTimeSeconds,
		AccessPointAnnotation,
		AccessPointModel,
        Created)
VALUES (Source.WifiBSSID,
        Source.WifiSSID,
		Source.DistrictID,
		Source.SchoolID,
		Source.ClassroomID,
		Source.AccessPointMaxDownloadLicenses,
		Source.AccessPointExpiryTimeSeconds,
		Source.AccessPointAnnotation,
		Source.AccessPointModel,
        Source.Created);

-- delete rows that are in the target but not in the source
--WHEN NOT MATCHED BY SOURCE THEN
--DELETE;

--SET IDENTITY_INSERT [AccessPoint] OFF
--GO