--SET IDENTITY_INSERT [District] ON 
--GO 
	
-- Reference Data for District
MERGE INTO [District] AS Target 
USING (VALUES 
  ('00fcc2a7-a3d5-469c-a790-f2e8bcd44bca', NULL, '2014-12-15 21:12:57.4348764', 'National School District', 10, NULL, NULL, 3600, NULL, NULL, NULL, NULL, NULL, 'Sample data brought to you by Atul', 'c23cd56c-0fd0-4de6-8b8f-97e4332d9eea', '42b1233a-2020-4bb2-8f5f-78daff0cb84d', 'https://schoolnet-dct.ccsocdev.net/'),
  ('2055be5c-8d06-407c-8d27-5448da915a08', NULL, '2014-12-15 21:16:07.0454649', 'LA School District', 20, NULL, NULL, 3600, NULL, NULL, NULL, NULL, NULL, 'Sample data brought to you by Atul', 'c23cd56c-0fd0-4de6-8b8f-97e4332d9eea', '42b1233a-2020-4bb2-8f5f-78daff0cb84d', 'https://schoolnet-dct.ccsocdev.net/')
) 
AS Source (DistrictID, CreatedBy, CreationDate, DistrictName, DistrictMaxDownloadLicenses, DistrictInstructionHoursStart, DistrictInstructionHoursEnd, DistrictLicenseExpirySeconds, DistrictPreloadHoursStart, DistrictPreloadHoursEnd, DistrictOverrideCode, DistrictUserPolicy, DistrictUseCacheServer, DistrictAnnotation, OAuthApplicationId, OAuthClientId, OAuthURL) 
ON Target.DistrictID = Source.DistrictID

-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET CreatedBy                     = Source.CreatedBy,
           CreationDate                  = Source.CreationDate,
           DistrictName                  = Source.DistrictName,
           DistrictMaxDownloadLicenses   = Source.DistrictMaxDownloadLicenses,
           DistrictInstructionHoursStart = Source.DistrictInstructionHoursStart,
           DistrictInstructionHoursEnd   = Source.DistrictInstructionHoursEnd,
           DistrictLicenseExpirySeconds  = Source.DistrictLicenseExpirySeconds,
           DistrictPreloadHoursStart     = Source.DistrictPreloadHoursStart,
           DistrictPreloadHoursEnd       = Source.DistrictPreloadHoursEnd,
           DistrictOverrideCode          = Source.DistrictOverrideCode,
           DistrictUserPolicy            = Source.DistrictUserPolicy,
           DistrictUseCacheServer        = Source.DistrictUseCacheServer,
           DistrictAnnotation            = Source.DistrictAnnotation,
           OAuthApplicationId            = Source.OAuthApplicationId,
           OAuthClientId                 = Source.OAuthClientId,
           OAuthURL                      = Source.OAuthURL

-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (DistrictID,
        CreatedBy,
		CreationDate,
		DistrictName,
		DistrictMaxDownloadLicenses,
		DistrictInstructionHoursStart,
		DistrictInstructionHoursEnd,
		DistrictLicenseExpirySeconds,
		DistrictPreloadHoursStart,
		DistrictPreloadHoursEnd,
		DistrictOverrideCode,
		DistrictUserPolicy,
		DistrictUseCacheServer,
		DistrictAnnotation,
		OAuthApplicationId,
		OAuthClientId,
		OAuthURL) 
VALUES (Source.DistrictID,
        Source.CreatedBy,
		Source.CreationDate,
		Source.DistrictName,
		Source.DistrictMaxDownloadLicenses,
		Source.DistrictInstructionHoursStart,
		Source.DistrictInstructionHoursEnd,
		Source.DistrictLicenseExpirySeconds,
		Source.DistrictPreloadHoursStart,
		Source.DistrictPreloadHoursEnd,
		Source.DistrictOverrideCode,
		Source.DistrictUserPolicy,
		Source.DistrictUseCacheServer,
		Source.DistrictAnnotation,
		Source.OAuthApplicationId,
		Source.OAuthClientId,
		Source.OAuthURL);

-- delete rows that are in the target but not in the source
--WHEN NOT MATCHED BY SOURCE THEN
--DELETE;

--SET IDENTITY_INSERT [District] OFF
--GO