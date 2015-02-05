--SET IDENTITY_INSERT [School] ON 
--GO 
	
-- Reference Data for School
MERGE INTO [School] AS Target 
USING (VALUES 
  ('82c5e4d1-c449-4f09-8062-2493b5c615c5', '00FCC2A7-A3D5-469C-A790-F2E8BCD44BCA', 'Colin Powell High', NULL, NULL, NULL, NULL, NULL, NULL, 5, 2400, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'Sample data brought to you by Atul', '2014-12-15 21:32:14.3042153'),
  ('36bfa22c-ff03-4f08-b5f4-6d2a946887d2', '2055BE5C-8D06-407C-8D27-5448DA915A08', 'Eagleburger High', NULL, NULL, NULL, NULL, NULL, NULL, 4, 2400, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'Sample data brought to you by Atul', '2014-12-15 21:33:51.1020483'),
  ('b388cb21-694d-4d63-a9a3-e9d6d14b8453', '2055BE5C-8D06-407C-8D27-5448DA915A08', 'Henry L. Stimson High', NULL, NULL, NULL, NULL, NULL, NULL, 4, 2400, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'Sample data brought to you by Atul', '2014-12-15 21:34:00.8521731')
) 
AS Source (SchoolID, DistrictID, SchoolName, SchoolAddress1, SchoolAddress2, SchoolTown, SchoolState, SchoolZipCode, SchoolGrades, SchoolMaxDownloadLicenses, SchoolLicenseExpirySeconds, GradeInstructionHoursBegin, GradeInstructionHoursEnd, SchoolOverrideCode, GradePreloadHoursBegin, GradePreloadHoursEnd, SchoolUseCacheServer, SchoolUserPolicy, SchoolAnnotation, Created) 
ON Target.SchoolID = Source.SchoolID

-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET DistrictID                 = Source.DistrictID,
           SchoolName                 = Source.SchoolName,
           SchoolAddress1             = Source.SchoolAddress1,
           SchoolAddress2             = Source.SchoolAddress2,
           SchoolTown                 = Source.SchoolTown,
           SchoolState                = Source.SchoolState,
           SchoolZipCode              = Source.SchoolZipCode,
           SchoolGrades               = Source.SchoolGrades,
           SchoolMaxDownloadLicenses  = Source.SchoolMaxDownloadLicenses,
           SchoolLicenseExpirySeconds = Source.SchoolLicenseExpirySeconds,
           GradeInstructionHoursBegin = Source.GradeInstructionHoursBegin,
           GradeInstructionHoursEnd   = Source.GradeInstructionHoursEnd,
		   SchoolOverrideCode         = Source.SchoolOverrideCode,
           GradePreloadHoursBegin     = Source.GradePreloadHoursBegin,
           GradePreloadHoursEnd       = Source.GradePreloadHoursEnd,
           SchoolUseCacheServer       = Source.SchoolUseCacheServer,
           SchoolUserPolicy           = Source.SchoolUserPolicy,
           SchoolAnnotation           = Source.SchoolAnnotation,
           Created                    = Source.Created

-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (SchoolID,
        DistrictID,
		SchoolName,
		SchoolAddress1,
		SchoolAddress2,
		SchoolTown,
		SchoolState,
		SchoolZipCode,
		SchoolGrades,
		SchoolMaxDownloadLicenses,
		SchoolLicenseExpirySeconds,
		GradeInstructionHoursBegin,
		GradeInstructionHoursEnd,
		SchoolOverrideCode,
        GradePreloadHoursBegin,
        GradePreloadHoursEnd,
        SchoolUseCacheServer,
        SchoolUserPolicy,
        SchoolAnnotation,
        Created)
VALUES (Source.SchoolID,
        Source.DistrictID,
		Source.SchoolName,
		Source.SchoolAddress1,
		Source.SchoolAddress2,
		Source.SchoolTown,
		Source.SchoolState,
		Source.SchoolZipCode,
		Source.SchoolGrades,
		Source.SchoolMaxDownloadLicenses,
		Source.SchoolLicenseExpirySeconds,
		Source.GradeInstructionHoursBegin,
		Source.GradeInstructionHoursEnd,
		Source.SchoolOverrideCode,
        Source.GradePreloadHoursBegin,
        Source.GradePreloadHoursEnd,
        Source.SchoolUseCacheServer,
        Source.SchoolUserPolicy,
        Source.SchoolAnnotation,
        Source.Created);

-- delete rows that are in the target but not in the source
--WHEN NOT MATCHED BY SOURCE THEN
--DELETE;

--SET IDENTITY_INSERT [School] OFF
--GO