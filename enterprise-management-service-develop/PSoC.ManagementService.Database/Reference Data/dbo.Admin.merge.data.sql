--SET IDENTITY_INSERT [Admin] ON 
--GO 
	
-- Reference Data for User
MERGE INTO [Admin] AS Target 
USING (VALUES 
  ('c89ccc4e-edb6-43c9-9dbe-e46cdb845fae', '2055BE5C-8D06-407C-8D27-5448DA915A08', NULL, 1, NULL, NULL),
  ('7c25630e-ffe3-4d84-b42b-17c06a7109cb', '00FCC2A7-A3D5-469C-A790-F2E8BCD44BCA', '82c5e4d1-c449-4f09-8062-2493b5c615c5', 1, NULL, NULL),
  ('2433bcfc-030a-487e-9f78-4f60ef49967e', NULL, NULL, 1, NULL, NULL),
  ('886e726c-0319-4dca-9925-69a28f53594a', '2055BE5C-8D06-407C-8D27-5448DA915A08', NULL, 1, NULL, NULL),
  ('fdc6d6ae-82ff-45d3-a9a9-3061171d9a2d', '00FCC2A7-A3D5-469C-A790-F2E8BCD44BCA', '82c5e4d1-c449-4f09-8062-2493b5c615c5', 1, NULL, NULL)
) 
AS Source (UserId, DistrictID, SchoolID, Active, AdminEmail, LastLoginDateTime) 
ON Target.UserId = Source.UserId

-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET DistrictID        = Source.DistrictID,
           SchoolID          = Source.SchoolID,
		   Active            = Source.Active,
		   AdminEmail        = Source.AdminEmail,
		   LastLoginDateTime = Source.LastLoginDateTime

-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (UserId,
		DistrictID, 
        SchoolID,
		Active,
		AdminEmail,
		LastLoginDateTime) 
VALUES (Source.UserId,
		Source.DistrictID,
        Source.SchoolID,
		Source.Active,
		Source.AdminEmail,
		Source.LastLoginDateTime);

-- delete rows that are in the target but not in the source
--WHEN NOT MATCHED BY SOURCE THEN
--DELETE;

--SET IDENTITY_INSERT [Admin] OFF
--GO