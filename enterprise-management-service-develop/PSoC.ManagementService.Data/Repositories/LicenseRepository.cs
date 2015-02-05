using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.DataMapper;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.QueryFactory;

namespace PSoC.ManagementService.Data.Repositories
{
    public class LicenseRepository : Repository<LicenseDto, LicenseQuery, LicenseDataMapper, Guid>, ILicenseRepository
    {
        public new async Task<LicenseDto> GetByIdAsync(Guid key)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@LicenseRequestID", SqlDbType.UniqueIdentifier) { Value = key}   
            };

            var query = GetSelectQuery("WHERE LicenseRequestID = @LicenseRequestID", loadNestedTypes: true);
            var result = await GetQueryResultAsync(query.QueryString, paramList, loadNestedTypes: true).ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        public async Task<IList<LicenseDto>> GetExpiredLicensesAsync()
        {
            var query = GetSelectQuery("WHERE LicenseExpiryDateTime <= GETUTCDATE()", loadNestedTypes: true);
            var result = await GetQueryResultAsync(query.QueryString, loadNestedTypes: true).ConfigureAwait(false);

            return result;
        }

        public new async Task<LicenseDto> InsertAsync(LicenseDto entity)
        {
            var query = GetInsertQuery(entity);
            query.QueryString += Environment.NewLine
                + GetSelectQuery("WHERE LicenseRequestID = @LicenseRequestID", loadNestedTypes: true).QueryString;
                                
            var result = await GetQueryResultAsync(query.QueryString, query.SqlParameters, loadNestedTypes: true).ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        public new async Task<LicenseDto> UpdateAsync(LicenseDto entity)
        {
            var query = GetUpdateQuery(entity);
            query.QueryString += Environment.NewLine
                + GetSelectQuery("WHERE LicenseRequestID = @LicenseRequestID", loadNestedTypes: true).QueryString;

            var result = await GetQueryResultAsync(query.QueryString, query.SqlParameters, loadNestedTypes: true).ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        public async Task<LicenseDto> GetLicenseForDeviceAsync(Guid deviceId)
        {
            LicenseDto license = null;

            const string query = @"
                    SET NOCOUNT ON;

                        SELECT      l.[LicenseRequestID],
                                    l.[DistrictID],
                                    l.[SchoolID],
                                    l.[ConfigCode],
                                    l.[WifiBSSID],
                                    l.[LicenseIssueDateTime],
                                    l.[LicenseExpiryDateTime],
	                                lr.DeviceID,
	                                lr.UserID,
                                    lr.LocationId,
                                    lr.LocationName,
                                    lr.LicenseRequestTypeID,
                                    ap.[WifiSSID],
                                    ap.[AccessPointMaxDownloadLicenses],
                                    ap.[AccessPointExpiryTimeSeconds],
                                    s.[SchoolName],
								    s.[SchoolMaxDownloadLicenses],
								    s.[SchoolLicenseExpirySeconds],
                                    d.[DistrictName],
                                    d.[DistrictMaxDownloadLicenses],
                                    d.[DistrictLicenseExpirySeconds],
                                    d.[OAuthApplicationId],
                                    d.[OAuthClientId],
								    d.[OAuthURL],    
                                    lr.[PrevLicenseRequestID]
                        FROM        [dbo].[License] l
                        INNER JOIN  [dbo].[AccessPoint] ap 
                        ON          l.[WifiBSSID] = ap.[WifiBSSID]
                        INNER JOIN  [dbo].[LicenseRequest] lr
                        ON          l.[LicenseRequestID] = lr.[LicenseRequestID]
                        LEFT JOIN   dbo.School s 
                        ON          l.[SchoolID] = ap.[SchoolID] AND ap.[SchoolID] = s.[SchoolID]
                        LEFT JOIN   dbo.[District] d
                        ON          l.[DistrictID] = ap.[DistrictID] AND ap.[DistrictID] = d.[DistrictID] AND s.[DistrictID] = d.[DistrictID]
                        WHERE       lr.[DeviceID] = @DeviceId AND l.[LicenseExpiryDateTime] > @ValidityDateTime";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@DeviceId", SqlDbType.UniqueIdentifier) { Value = deviceId},
                new SqlParameter("@ValidityDateTime", SqlDbType.DateTime) {Value = DateTime.UtcNow}
            };

            using (SqlDataReader dr = await DataAccessHelper.GetDataReaderAsync(query, paramList).ConfigureAwait(false))
            {
                if (dr.HasRows)
                {
                    while (await dr.ReadAsync().ConfigureAwait(false))
                    {
                        var device = dr.IsDBNull(7) ? null : new DeviceDto()
                        {
                            DeviceID = dr.GetGuid(7)
                        };
                        var user = dr.IsDBNull(8) ? null : new UserDto()
                        {
                            UserID = dr.GetGuid(8)
                        };
                        var district = dr.IsDBNull(1) ? null : new DistrictDto
                        {
                            DistrictId = dr.GetGuid(1),
                            DistrictName = dr.IsDBNull(18) ? null : dr.GetString(18),
                            DistrictMaxDownloadLicenses = dr.IsDBNull(19) ? 0 : dr.GetInt32(19),
                            DistrictLicenseExpirySeconds = dr.IsDBNull(20) ? 0 : dr.GetInt32(20),
                            OAuthApplicationId = dr.IsDBNull(21) ? null : dr.GetString(21),
                            OAuthClientId = dr.IsDBNull(22) ? null : dr.GetString(22),
                            OAuthURL = dr.IsDBNull(23) ? null : dr.GetString(23)

                        };
                        var school = dr.IsDBNull(2) ? null : new SchoolDto
                        {
                            SchoolID = dr.GetGuid(2),
                            SchoolName = dr.IsDBNull(15) ? null : dr.GetString(15),
                            SchoolMaxDownloadLicenses = dr.GetInt32(16),
                            SchoolLicenseExpirySeconds = dr.IsDBNull(17) ? (int?)null : dr.GetInt32(17)
                        };
                        var accessPoint = dr.IsDBNull(4) ? null : new AccessPointDto()
                        {
                            WifiBSSID = dr.IsDBNull(4) ? null : dr.GetString(4),
                            WifiSSID = dr.IsDBNull(12) ? null : dr.GetString(12),
                            AccessPointMaxDownloadLicenses = dr.GetInt32(13),
                            AccessPointExpiryTimeSeconds = dr.IsDBNull(14) ? (int?)null : dr.GetInt32(14),
                            District = district
                        };
                        var licenseRequest = dr.IsDBNull(0) ? null : new LicenseRequestDto()
                        {
                            LicenseRequestID = dr.GetGuid(0),
                            PrevLicenseRequestID = dr.IsDBNull(24) ? default(Guid?) : dr.GetGuid(24),
                            Device = device,
                            User = user,
                            LocationId = dr.IsDBNull(9) ? null : dr.GetString(9),
                            LocationName = dr.IsDBNull(10) ? null : dr.GetString(10),
                            LicenseRequestType = dr.IsDBNull(11) ? LicenseRequestType.RequestLicense : (LicenseRequestType)dr.GetInt32(11)
                        };

                        license = new LicenseDto()
                        {
                            School = school,
                            AccessPoint = accessPoint,
                            LicenseRequest = licenseRequest,
                            ConfigCode = dr.IsDBNull(3) ? null : dr.GetString(3),
                            LicenseIssueDateTime = dr.GetDateTime(5),
                            LicenseExpiryDateTime = dr.GetDateTime(6)
                        };
                    }
                }
            }

            return license;
        }

        public async Task<bool> GrantLicenseForDeviceAsync(LicenseRequestDto licenseRequest)
        {
            const int requestType = (int)LicenseRequestType.RequestLicense;

            const string query = @"
                                        BEGIN TRANSACTION
                                        
                                        --Variable Declaration and Initialization
                                        DECLARE @ExistingLicenseRequestId UNIQUEIDENTIFIER, @ExistingLicenseAccesspoint CHAR(17)
                                        DECLARE @ValidLicenseExistsWithSameAccessPoint BIT, @LicenseReturnTypeId SMALLINT
                                        DECLARE @DistrictID UNIQUEIDENTIFIER, @SchoolID UNIQUEIDENTIFIER
                                        DECLARE @Response NVARCHAR(50), @ReturnResponse NVARCHAR(50)
                                        DECLARE @GrantLicense BIT
                                        DECLARE @apMaxDowloadLicenses INT, @apExpiryTimeSeconds INT, @apLicensesInUse INT
                                        DECLARE @districtMaxDowloadLicenses INT, @districtExpiryTimeSeconds INT, @districtLicensesInUse INT
                                        DECLARE @schoolMaxDowloadLicenses INT, @schoolExpiryTimeSeconds INT, @schoolLicensesInUse INT
                                        DECLARE @LicenseDuration INT, @DurationLevel VARCHAR(15)

                                        SET @GrantLicense = 1               --Default to true for Grant License
                                        SET @Response = 'License Granted'   --Default Response Message
                                        SET @ValidLicenseExistsWithSameAccessPoint = 0
                                        SET @LicenseReturnTypeId = 2
                                        SET @DurationLevel = ''

                                        /*  Check if valid license exists for device and 
                                            retrive original LicenseRequestId and WifiBSSID
                                        */
                                        SELECT      @ExistingLicenseRequestId = l.[LicenseRequestID],
                                                    @ExistingLicenseAccesspoint = l.[WifiBSSID]
                                        FROM        [dbo].[License] l
										INNER JOIN  [dbo].[LicenseRequest] lr 
                                        ON          l.[LicenseRequestID] = lr.[LicenseRequestID]
                                        WHERE       lr.[DeviceID] = @DeviceId 
                                        AND         l.[LicenseExpiryDateTime] > @ValidityDateTime;
                                        
                                        /*License Request - SAME Device (with a valid license) and
                                                            SAME Accesspoint*/
                                        IF (@ExistingLicenseRequestId IS NOT NULL 
                                            AND (@ExistingLicenseAccesspoint IS NOT NULL
                                            AND @ExistingLicenseAccesspoint = @WifiBSSID))
                                        BEGIN
                                            SET @ValidLicenseExistsWithSameAccessPoint = 1
                                        END
                                        
                                        --Insert/Update USER Record
                                        UPDATE [dbo].[User] 
                                        SET    [Username] = @Username, 
                                               [UsernameHash] = @UsernameHash, 
                                               [UserType] = @UserType,
                                               [UserTypeHash] = @UserTypeHash
                                        WHERE  [UserID] = @UserID;
                                        
                                        IF @@rowcount = 0
                                        BEGIN
                                            INSERT INTO [dbo].[User] 
                                                            ([UserID], 
                                                             [Username],
                                                             [UsernameHash],
                                                             [UserType],
                                                             [UserTypeHash])
                                            VALUES           (@UserID, 
                                                             @Username, 
                                                             @UsernameHash,
                                                             @UserType,
                                                             @UserTypeHash);
                                        END

                                        --Insert/Update ACCESSPOINT Record
                                        UPDATE [dbo].[AccessPoint] 
                                        SET    [WifiSSID] = @WifiSSID
                                        WHERE  [WifiBSSID] = @WifiBSSID;
                                        
                                        IF @@rowcount = 0
                                        BEGIN
                                            INSERT INTO [dbo].[AccessPoint] 
                                                                ([WifiBSSID], 
                                                                [WifiSSID])
                                            VALUES              (@WifiBSSID, 
                                                                @WifiSSID);
                                        END

                                        --Retrieve DistrictID and SchoolID Info from Accesspoint Record
                                        SELECT  @DistrictID = [DistrictID],
                                                @SchoolID = [SchoolID]
                                        FROM    [dbo].[AccessPoint]
                                        WHERE   [WifiBSSID] = @WifiBSSID;


                                        --Insert/Update DEVICE Record
                                        UPDATE [dbo].[Device]
                                           SET [DeviceName] = @DeviceName,
                                               [DeviceNameHash] = @DeviceNameHash,
                                               [DeviceType] = @DeviceType,
                                               [DeviceOS] = @DeviceOS,
                                               [DeviceOSVersion] = @DeviceOSVersion,
                                               [PSoCAppID] = @PSoCAppID,
                                               [PSoCAppVersion] = @PSoCAppVersion,
                                               [DeviceFreeSpace] = @DeviceFreeSpace,
                                               [DistrictID] = @DistrictID,
                                               [SchoolID] = @SchoolID,
                                               [LastUsedConfigCode] = @LastUsedConfigCode,
                                               [DeviceAnnotation] = @DeviceAnnotation,
                                               [ConfiguredGrades] = @ConfiguredGrades,
                                               [ConfiguredUnitCount] = @ConfiguredUnitCount,
                                               [ContentLastUpdatedAt] = @ContentLastUpdatedAt,
                                               [InstalledContentSize] = @InstalledContentSize
                                         WHERE [DeviceID] = @DeviceID;

                                        IF @@rowcount = 0
                                        BEGIN
                                           INSERT INTO [dbo].[Device]
                                                               ([DeviceID],
                                                                [DeviceName],
                                                                [DeviceNameHash],
                                                                [DeviceType],
                                                                [DeviceOS],
                                                                [DeviceOSVersion],
                                                                [PSoCAppID],
                                                                [PSoCAppVersion],
                                                                [DeviceFreeSpace],
                                                                [DistrictID],
                                                                [SchoolID],
                                                                [LastUsedConfigCode], 
                                                                [DeviceAnnotation],
                                                                [ConfiguredGrades],
                                                                [ConfiguredUnitCount],
                                                                [ContentLastUpdatedAt],
                                                                [InstalledContentSize])
                                                VALUES          (@DeviceID, 
                                                                @DeviceName,
                                                                @DeviceNameHash,
                                                                @DeviceType, 
                                                                @DeviceOS,
                                                                @DeviceOSVersion,
                                                                @PSoCAppID,
                                                                @PSoCAppVersion,
                                                                @DeviceFreeSpace, 
                                                                @DistrictID,
                                                                @SchoolID, 
                                                                @LastUsedConfigCode,
                                                                @DeviceAnnotation,
                                                                @ConfiguredGrades, 
                                                                @ConfiguredUnitCount, 
                                                                @ContentLastUpdatedAt,
                                                                @InstalledContentSize);
                                        END

                                        /*
                                          License Request - SAME Device (with a valid license) and DIFFERENT Accesspoint
                                          Delete original license and indicate that as a return request
                                        */
                                        IF (@ExistingLicenseRequestId IS NOT NULL 
                                            AND (@ExistingLicenseAccesspoint IS NOT NULL
                                            AND @ExistingLicenseAccesspoint <> @WifiBSSID))
                                        BEGIN
                                                SET @ReturnResponse = 'Returned as new AP request:' + @WifiBSSID
                                                --Delete original license
                                                DELETE 
                                                FROM    [dbo].[License]
                                                WHERE   [LicenseRequestID] = @ExistingLicenseRequestId
                                                AND     [WifiBSSID] = @ExistingLicenseAccesspoint

                                                --Insert into License Request Table and indicate Return/Revoke from Device
                                                INSERT INTO [dbo].[LicenseRequest]
                                                       ([LicenseRequestID]
                                                       ,[PrevLicenseRequestID]
                                                       ,[DistrictID]
                                                       ,[SchoolID]
                                                       ,[ConfigCode]
                                                       ,[WifiBSSID]
                                                       ,[LicenseRequestTypeID]
                                                       ,[DeviceID]
                                                       ,[UserID]
                                                       ,[RequestDateTime]
                                                       ,[Response]
                                                       ,[ResponseDateTime]
                                                       ,[LocationID]
                                                       ,[LocationName]
                                                       ,[LearningContentQueued])
                                                SELECT
                                                        @LicenseReturnRequestID
                                                       ,@ExistingLicenseRequestId
                                                       ,[DistrictID]
                                                       ,[SchoolID]
                                                       ,[ConfigCode]
                                                       ,[WifiBSSID]
                                                       ,@LicenseReturnTypeId
                                                       ,[DeviceID]
                                                       ,@UserId
                                                       ,@RequestDateTime
                                                       ,@ReturnResponse
                                                       ,GETUTCDATE()
                                                       ,[LocationID]
                                                       ,[LocationName]
                                                       ,[LearningContentQueued]
                                                FROM	[dbo].[LicenseRequest]
                                                WHERE   [LicenseRequestID] = @ExistingLicenseRequestId

                                                --Reset ExistingLicenseRequestId and ExistingLicenseAccesspoint
                                                SET @ExistingLicenseRequestId = NULL
                                                SET @ExistingLicenseAccesspoint = NULL
                                                SET @ValidLicenseExistsWithSameAccessPoint = 0
                                        END

                                        /*  Get Accesspoint, School and District Threshold Params
                                        */
                                        SELECT  @apMaxDowloadLicenses = AP.[AccessPointMaxDownloadLicenses], 
                                                @apExpiryTimeSeconds =  AP.[AccessPointExpiryTimeSeconds],
                                                @apLicensesInUse =      (SELECT COUNT([LicenseRequestId]) 
                                                                         FROM   [dbo].[License] L 
                                                                         WHERE  L.[WifiBSSID] = AP.[WifiBSSID] 
                                                                         AND    L.[LicenseExpiryDateTime] > @ValidityDateTime),
                                                @districtMaxDowloadLicenses = 
                                                                        (SELECT [DistrictMaxDownloadLicenses] 
                                                                         FROM   [dbo].[District] D  
                                                                         WHERE  D.[DistrictID] = AP.[DistrictID]),
                                                @districtExpiryTimeSeconds = 
                                                                        (SELECT [DistrictLicenseExpirySeconds] 
                                                                         FROM   [dbo].[District] D 
                                                                         WHERE  D.[DistrictID] = AP.[DistrictID]),
                                                @districtLicensesInUse = (SELECT COUNT(LicenseRequestId) 
                                                                          FROM   [dbo].[License] L 
                                                                          WHERE  L.[DistrictID] = AP.[DistrictID]
                                                                          AND    L.[LicenseExpiryDateTime] > @ValidityDateTime),
                                                @schoolMaxDowloadLicenses = (SELECT [SchoolMaxDownloadLicenses] 
                                                                            FROM    [dbo].[School] S 
                                                                            WHERE   S.[SchoolID] = AP.[SchoolID]),
                                                @schoolExpiryTimeSeconds =  (SELECT [SchoolLicenseExpirySeconds] 
                                                                            FROM    [dbo].[School] S 
                                                                            WHERE   S.[SchoolID] = AP.[SchoolID]),
                                                @schoolLicensesInUse = (SELECT  COUNT(LicenseRequestId) 
                                                                        FROM    [dbo].[License] L 
                                                                        WHERE   l.[SchoolID] = AP.[SchoolID] 
                                                                        AND     l.[LicenseExpiryDateTime] > @ValidityDateTime)
                                          FROM  AccessPoint AP 
                                          WHERE AP.[WifiBSSID] = @WifiBSSID;
                                      
                                    
                                    /*
                                        AccessPoint level License Expiration Time is available
                                    */
                                     IF (@apExpiryTimeSeconds IS NOT NULL)
                                        BEGIN
                                            SET @LicenseDuration = @apExpiryTimeSeconds
                                            SET @DurationLevel = 'AccessPoint'
                                        END
                                    
                                     /*
                                        AccessPoint Level License Expiration Time is not available so revert to School level
                                    */
                                     IF (@LicenseDuration IS NULL AND @schoolExpiryTimeSeconds IS NOT NULL)
                                        BEGIN
                                             SET @LicenseDuration = @schoolExpiryTimeSeconds
                                             SET @DurationLevel = 'School'
                                        END

                                    /*
                                        Both AccessPoint Level and School Level License Expiration are not available 
                                        so revert to District level
                                    */
                                     IF (@LicenseDuration IS NULL AND @districtExpiryTimeSeconds IS NOT NULL)
                                        BEGIN
                                             SET @LicenseDuration = @districtExpiryTimeSeconds
                                             SET @DurationLevel = 'District'
                                        END

                                    /* Debug Start*/
                                    /*Print 'ExistingLicenseAccesspoint: ' + @ExistingLicenseAccesspoint
									Print 'apExpiryTimeSeconds: ' + Cast(@apExpiryTimeSeconds as varchar(11))
									Print 'apMaxDowloadLicenses: ' + Cast(@apMaxDowloadLicenses as varchar(11))
									Print 'apExpiryTimeSeconds: ' + Cast(@apExpiryTimeSeconds as varchar(11))
									Print 'apLicensesInUse: ' + Cast(@apLicensesInUse as varchar(11))
																		
									IF (@DistrictID IS NOT NULL)
									BEGIN
										Print 'districtMaxDowloadLicenses: ' + Cast(@districtMaxDowloadLicenses as varchar(11))
										Print 'districtExpiryTimeSeconds: ' + Cast(@districtExpiryTimeSeconds as varchar(11))
										Print 'districtLicensesInUse: ' + Cast(@districtLicensesInUse as varchar(11))
									END
																		
									IF (@SchoolID IS NOT NULL)
									BEGIN
										Print 'schoolMaxDowloadLicenses: ' + Cast(@schoolMaxDowloadLicenses as varchar(11))
										Print 'schoolExpiryTimeSeconds: ' + Cast(@schoolExpiryTimeSeconds as varchar(11))
										Print 'schoolLicensesInUse: ' + Cast(@schoolLicensesInUse as varchar(11))
									END*/
                                    /* Debug End*/    

                                IF (@DistrictID IS NULL AND @SchoolID IS NULL)
                                    BEGIN
                                        --AccessPoint level check only for a dynamic accesspoint
                                        IF (@apLicensesInUse >= @apMaxDowloadLicenses)
                                            BEGIN                            
                                                SET @GrantLicense = 0
                                                SET @Response = 'Denied-DynAccessPoint Limit: Max:' + 
                                                                CAST(@apMaxDowloadLicenses as VARCHAR(11)) +
                                                                ', In Use:' + CAST(@apLicensesInUse AS VARCHAR(11))
                                            END
                                    END
                                ELSE
                                    BEGIN
                                            --AccessPoint level check as first step
                                        IF (@apLicensesInUse >= @apMaxDowloadLicenses)
                                            BEGIN                            
                                                SET @GrantLicense = 0
                                                SET @Response = 'Denied-AccessPoint Limit: Max:' + 
                                                                CAST(@apMaxDowloadLicenses as VARCHAR(11)) +
                                                                ', In Use:' + CAST(@apLicensesInUse AS VARCHAR(11))
                                            END
                                            --School level check if accesspoint check is satisfied as second step
                                        IF (@apLicensesInUse < @apMaxDowloadLicenses AND 
                                            @schoolLicensesInUse >= @schoolMaxDowloadLicenses)
                                            BEGIN                            
                                                SET @GrantLicense = 0   
                                                SET @Response = 'Denied-School Limit: Max:' + 
                                                                CAST(@schoolMaxDowloadLicenses as VARCHAR(11)) +
                                                                ', In Use:' + CAST(@schoolLicensesInUse AS VARCHAR(11))
                                            END
                                            --District level check if accesspoint and schol check are satisfied as last step
                                        IF (@apLicensesInUse < @apMaxDowloadLicenses AND 
                                            @schoolLicensesInUse < @schoolMaxDowloadLicenses AND
                                            @districtLicensesInUse >= @districtMaxDowloadLicenses)
                                            BEGIN                            
                                                SET @GrantLicense = 0
                                                SET @Response = 'Denied-District Limit: Max:' + 
                                                                CAST(@districtMaxDowloadLicenses as VARCHAR(11)) +
                                                                ', In Use:' + CAST(@districtLicensesInUse AS VARCHAR(11))
                                            END
                                    END

                                    /* Set Grant to true if Valid License Exists with Same Accesspoint and we are about 
                                        to extend its validity*/
                                    IF @ValidLicenseExistsWithSameAccessPoint = 1
                                    BEGIN
                                        SET @GrantLicense = 1
                                    END
                                    
                                    /*Override to deny license if LicenseDuration is less than 1 sec*/
                                    IF (@GrantLicense = 1 AND @ValidLicenseExistsWithSameAccessPoint = 0 AND (@LicenseDuration IS NULL OR 
                                                                            @LicenseDuration <= 0))
                                    BEGIN
                                        SET @GrantLicense = 0
                                        SET @Response = 'Insufficient ' + @DurationLevel + ' Expiration duration'
                                    END

                                    /*Print 'GrantLicense: ' + cast(@GrantLicense as varchar(11))*/

                                    /*Existing License is Present and Valid with the same accesspoint, bypass all validations
                                    and simply extend the licenseexpiration*/
                                    IF (@GrantLicense = 1 AND @ValidLicenseExistsWithSameAccessPoint = 1)
                                    BEGIN
                                            SET @Response = 'License Granted - Renewed'

                                            /*Print 'Extend License'*/
                                            INSERT INTO [dbo].[LicenseRequest]
                                                            ([LicenseRequestID],[PrevLicenseRequestID], [DistrictID], [SchoolID]
                                                            ,[ConfigCode], [WifiBSSID], [LicenseRequestTypeID]
                                                            ,[DeviceID], [UserID], [RequestDateTime], [Response]
                                                            ,[ResponseDateTime], [LocationID], [LocationName],[LearningContentQueued])
                                            VALUES
                                                        (@LicenseRequestID, @ExistingLicenseRequestId, @DistrictID, @SchoolID,
                                                        @LastUsedConfigCode, @WifiBSSID, @LicenseRequestTypeID,
                                                        @DeviceID, @UserID, @RequestDateTime,  @Response,
                                                        GETUTCDATE(), @LocationID, @LocationName, @LearningContentQueued)

                                            UPDATE [dbo].[License]
                                            SET     [LicenseRequestID] = @LicenseRequestID,
                                                    [LicenseExpiryDateTime] = DATEADD(ss, @LicenseDuration, LicenseExpiryDateTime),
                                                    [ConfigCode] = @LastUsedConfigCode,  
                                                    [DistrictID] = @DistrictID,
                                                    [SchoolID] = @SchoolID
                                            WHERE  [LicenseRequestID] = @ExistingLicenseRequestId
                                    END
                                        
                                    /*
                                    Valid License Does not exist for the device. Grant one if business rules are satisfied
                                    */
                                    IF (@GrantLicense = 1 AND @ValidLicenseExistsWithSameAccessPoint = 0)
                                    BEGIN
                                        /*Print 'Grant License'*/
                                        INSERT INTO [dbo].[LicenseRequest]
                                                            ([LicenseRequestID], [DistrictID], [SchoolID]
                                                            ,[ConfigCode], [WifiBSSID], [LicenseRequestTypeID]
                                                            ,[DeviceID], [UserID], [RequestDateTime], [Response]
                                                            ,[ResponseDateTime], [LocationID], [LocationName],[LearningContentQueued])
                                        VALUES
                                                    (@LicenseRequestID, @DistrictID, @SchoolID,
                                                    @LastUsedConfigCode, @WifiBSSID, @LicenseRequestTypeID,
                                                    @DeviceID, @UserID, @RequestDateTime,  @Response,
                                                    GETUTCDATE(), @LocationID, @LocationName, @LearningContentQueued)

                                        INSERT INTO [dbo].[License]
                                                        ([LicenseRequestID], [DistrictID], [SchoolID]
                                                        ,[ConfigCode], [WifiBSSID]
                                                        ,[LicenseIssueDateTime], [LicenseExpiryDateTime])
                                            VALUES
                                                (@LicenseRequestID, @DistrictID, @SchoolID,
                                                @LastUsedConfigCode, @WifiBSSID,
                                                GETUTCDATE(), DATEADD(ss, @LicenseDuration, GETUTCDATE()))
                                    END

                                    /*
                                    Log into LicenseRequest Table if license can not be granted due to business rules validation
                                    */

                                    IF (@GrantLicense = 0 AND @ValidLicenseExistsWithSameAccessPoint = 0) 
                                        BEGIN
                                            /*PRINT 'Deny License'*/
                                            INSERT INTO [dbo].[LicenseRequest]
                                                            ([LicenseRequestID], [DistrictID], [SchoolID]
                                                            ,[ConfigCode], [WifiBSSID], [LicenseRequestTypeID]
                                                            ,[DeviceID], [UserID], [RequestDateTime], [Response]
                                                            ,[ResponseDateTime], [LocationID], [LocationName],[LearningContentQueued])
                                            VALUES
                                                    (@LicenseRequestID, @DistrictID, @SchoolID,
                                                    @LastUsedConfigCode, @WifiBSSID, @LicenseRequestTypeID,
                                                    @DeviceID, @UserID, @RequestDateTime,  @Response,
                                                    GETUTCDATE(), @LocationID, @LocationName, @LearningContentQueued)
                                        END

                                    /*
                                    SELECT [LicenseRequestID] 
                                    FROM   [dbo].[LicenseRequest] 
                                    WHERE  [DeviceID] = @DeviceID AND [RequestDateTime] = @RequestDateTime
                                    */

                                    SELECT @@rowcount;              
                                    COMMIT TRANSACTION";


            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@ValidityDateTime", SqlDbType.DateTime) {Value = DateTime.UtcNow},

                //User
                new SqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Value = licenseRequest.User.UserID.CreateIfEmpty()},
                new SqlParameter("@Username", SqlDbType.Binary)  { Value =  string.IsNullOrEmpty(licenseRequest.User.Username) ? DBNull.Value : licenseRequest.User.UsernameEnc.EncryptedValue.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@UsernameHash", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(licenseRequest.User.Username) ? DBNull.Value : licenseRequest.User.UsernameEnc.GetHashBytes().NullIfEmpty(), IsNullable =true},
                new SqlParameter("@UserType", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(licenseRequest.User.UserType) ? DBNull.Value : licenseRequest.User.UserTypeEnc.EncryptedValue.NullIfEmpty(), IsNullable = true},
                new SqlParameter("@UserTypeHash", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(licenseRequest.User.UserType) ? DBNull.Value : licenseRequest.User.UserTypeEnc.GetHashBytes().NullIfEmpty(), IsNullable =true},

               //Accesspoint
                new SqlParameter("@WifiBSSID", SqlDbType.Char, 17) { Value = licenseRequest.AccessPoint.WifiBSSID},
                new SqlParameter("@WifiSSID", SqlDbType.NVarChar, 32) { Value = licenseRequest.AccessPoint.WifiSSID},
                
                //Device
                new SqlParameter("@DeviceID", SqlDbType.UniqueIdentifier) { Value = licenseRequest.Device.DeviceID.CreateIfEmpty()},
                new SqlParameter("@DeviceName", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(licenseRequest.Device.DeviceName) ? DBNull.Value : licenseRequest.Device.DeviceNameEnc.EncryptedValue.NullIfEmpty(), IsNullable =true},
                new SqlParameter("@DeviceNameHash", SqlDbType.Binary) { Value =  string.IsNullOrEmpty(licenseRequest.Device.DeviceName) ? DBNull.Value : licenseRequest.Device.DeviceNameEnc.GetHashBytes().NullIfEmpty(), IsNullable =true},

                new SqlParameter("@DeviceType", SqlDbType.NVarChar, 50) { Value = (Object)licenseRequest.Device.DeviceType?? DBNull.Value},
                new SqlParameter("@DeviceOS", SqlDbType.NVarChar, 50) { Value = (Object)licenseRequest.Device.DeviceOS?? DBNull.Value},
                new SqlParameter("@DeviceOSVersion", SqlDbType.NVarChar, 50) { Value = (Object)licenseRequest.Device.DeviceOSVersion?? DBNull.Value},
                new SqlParameter("@PSoCAppID", SqlDbType.NVarChar, 50) { Value = (Object)licenseRequest.Device.PSoCAppID?? DBNull.Value},
                new SqlParameter("@PSoCAppVersion", SqlDbType.NVarChar, 50) { Value = (Object)licenseRequest.Device.PSoCAppVersion?? DBNull.Value},
                new SqlParameter("@DeviceFreeSpace", SqlDbType.BigInt) { Value = (Object)licenseRequest.Device.DeviceFreeSpace ?? DBNull.Value},
                new SqlParameter("@LastUsedConfigCode", SqlDbType.NVarChar, 50) { Value = (Object)licenseRequest.Device.LastUsedConfigCode?? DBNull.Value},
                new SqlParameter("@DeviceAnnotation", SqlDbType.NVarChar, 80) { Value = (Object)licenseRequest.Device.DeviceAnnotation?? DBNull.Value},
                new SqlParameter("@ConfiguredGrades", SqlDbType.NVarChar, 50) { Value = (Object)licenseRequest.Device.ConfiguredGrades?? DBNull.Value},
                new SqlParameter("@ConfiguredUnitCount", SqlDbType.Int) { Value = (Object)licenseRequest.Device.ConfiguredUnitCount ?? DBNull.Value},
                new SqlParameter("@ContentLastUpdatedAt", SqlDbType.DateTime) { Value = (Object)licenseRequest.Device.ContentLastUpdatedAt ?? DBNull.Value},
                new SqlParameter("@InstalledContentSize", SqlDbType.BigInt) { Value = (Object)licenseRequest.Device.InstalledContentSize ?? DBNull.Value},
            
                //LicenseRequest
                 new SqlParameter("@LicenseRequestID", SqlDbType.UniqueIdentifier) { Value = licenseRequest.LicenseRequestID.CreateIfEmpty()},
                 new SqlParameter("@LicenseRequestTypeID", SqlDbType.Int) { Value = requestType},
                 new SqlParameter("@RequestDateTime", SqlDbType.DateTime) { Value = (Object)licenseRequest.RequestDateTime ?? DBNull.Value},
                 new SqlParameter("@LocationID", SqlDbType.NVarChar, 50) { Value = (Object)licenseRequest.LocationId?? DBNull.Value},
                 new SqlParameter("@LocationName", SqlDbType.NVarChar, 50) { Value = (Object)licenseRequest.LocationName?? DBNull.Value},
                 new SqlParameter("@LearningContentQueued", SqlDbType.Int) { Value = (Object)licenseRequest.LearningContentQueued?? DBNull.Value},
                 new SqlParameter("@LicenseReturnRequestID", SqlDbType.UniqueIdentifier) { Value = new Guid().CreateIfEmpty()},
            };

            var result = await DataAccessHelper.ExecuteAsync(query, paramList).ConfigureAwait(false);
            return (result > 0);
        }

        public async Task<bool> RevokeLicenseForDeviceAsync(Guid licenseRequestId, Guid userId, DateTime requestedDateTime, bool isAdmin)
        {
            int licenseCancelType = isAdmin ? (int)LicenseRequestType.RevokeLicense : (int)LicenseRequestType.ReturnLicense;
            string responsePrefix = isAdmin ? "Revoked:" : "Returned:";

            const string query = @"
                                        BEGIN TRANSACTION
                    
                                        INSERT INTO [dbo].[LicenseRequest]
                                                   ([LicenseRequestID]
                                                   ,[DistrictID]
                                                   ,[SchoolID]
                                                   ,[ConfigCode]
                                                   ,[WifiBSSID]
                                                   ,[LicenseRequestTypeID]
                                                   ,[DeviceID]
                                                   ,[UserID]
                                                   ,[RequestDateTime]
                                                   ,[Response]
                                                   ,[ResponseDateTime]
                                                   ,[LocationID]
                                                   ,[LocationName]
                                                   ,[LearningContentQueued])
                                        SELECT
                                                    @LicenseReturnRequestId
                                                   ,[DistrictID]
                                                   ,[SchoolID]
                                                   ,[ConfigCode]
                                                   ,[WifiBSSID]
                                                   ,@LicenseRequestTypeId
                                                   ,[DeviceID]
                                                   ,@UserId
                                                   ,@RequestDateTime
                                                   ,@Response
                                                   ,GETUTCDATE()
                                                   ,[LocationID]
                                                   ,[LocationName]
                                                   ,[LearningContentQueued]
                                        FROM	[dbo].[LicenseRequest]
                                        WHERE   [LicenseRequestID] = @LicenseRequestId
               
                                        DELETE FROM [dbo].[License]
                                        WHERE       [LicenseRequestID] = @LicenseRequestId
                    
                                        SELECT @@rowcount             
                                        COMMIT TRANSACTION";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@LicenseRequestId", SqlDbType.UniqueIdentifier) { Value = licenseRequestId },
                new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) {Value = userId},
                new SqlParameter("@LicenseRequestTypeId", SqlDbType.Int) {Value = licenseCancelType},
                new SqlParameter("@RequestDateTime", SqlDbType.DateTime) { Value = requestedDateTime },
                new SqlParameter("@Response", SqlDbType.NVarChar) { Value = responsePrefix + licenseRequestId },
                new SqlParameter("@LicenseReturnRequestId", SqlDbType.UniqueIdentifier) { Value = new Guid().CreateIfEmpty() }
            };

            var result = await DataAccessHelper.ExecuteAsync(query, paramList).ConfigureAwait(false);
            return (result > 0);
        }
    }
}
