using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Controllers
{
    /// <summary>
    /// Device specific endpoints (including License Service)
    /// </summary>
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ILicenseService _licenseService;
        private readonly IUserService _userService;

        private bool IsAuthenticated
        {
            //TODO - to be refactored or moved to a base controller
            get { return (User != null && User.Identity != null && User.Identity.IsAuthenticated); }
        }

        private bool IsAdmin
        {
            get { return (IsAuthenticated && (User.IsInRole("GlobalAdmin") || User.IsInRole("DistrictAdmin") || User.IsInRole("SchoolAdmin"))); }
        }

        private Guid AdminId
        {
            get
            {
                var adminId = new Guid();
                if (IsAuthenticated && IsAdmin)
                {
                    var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                    .Select(c => c.Value).SingleOrDefault();
                    if (sid != null)
                        adminId = new Guid(sid);
                }
                return adminId;
            }
        }

        public DevicesController(IDeviceService deviceService, ILicenseService licenseService, IUserService userService)
        {
            _deviceService = deviceService;
            _licenseService = licenseService;
            _userService = userService;
        }

        [HttpPost]
        [Route("api/v1/devices/status/{deviceId}")]
        public async Task<IHttpActionResult> ReportDownloadStatus(string deviceId, [FromBody]List<Course> courses)
        {
            try
            {
                Guid deviceGuidId;
                if (!Guid.TryParse(deviceId, out deviceGuidId))
                {
                    PEMSEventSource.Log.DeviceApiReportDownloadStatusFailure(String.Format("Invalid device id {0}.", deviceId), deviceId, LogRequest);
                    return BadRequest("Invalid Device Id");
                }

                if (courses == null || courses.Any(c => c.LearningResourceId == Guid.Empty))
                {
                    PEMSEventSource.Log.DeviceApiReportDownloadStatusFailure(String.Format("Invalid courses for device {0}.", deviceId), deviceId, LogRequest);
                    return BadRequest("Invalid courses. Can not parse requested courses. Please check parameters of courses");
                }

                await _deviceService.SaveDownloadStatusAsync(deviceGuidId, courses, LogRequest).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                LogRequest = LogRequest ?? new LogRequest();
                LogRequest.Exception = ex;
                PEMSEventSource.Log.DeviceApiReportDownloadStatusFailure(ex.Message, deviceId, LogRequest);
                return InternalServerError(new Exception(String.Format("Failed to save download status for device {0}.", deviceId)));
            }
        }

        [HttpPut]
        [Route("api/v1/devices/{deviceId}")]
        public async Task<IHttpActionResult> Put(string deviceId, [FromBody] DeviceLicenseRequest licenseRequest)
        {
            try
            {
                Guid deviceGuid;
                Guid userGuid;

                if (licenseRequest == null)
                {
                    PEMSEventSource.Log.DeviceApiPutFailure(String.Format("Invalid payload {0}.", deviceId), deviceId, LogRequest);
                    return BadRequest("Invalid Payload");
                }

                // Append request to log
                PEMSEventSource.Log.Append(licenseRequest, LogRequest);

                if (!Guid.TryParse(deviceId, out deviceGuid) || deviceId != licenseRequest.DeviceId)
                {
                    PEMSEventSource.Log.DeviceApiPutFailure(String.Format("Invalid device id {0}.", deviceId), deviceId, LogRequest);
                    return BadRequest("Invalid Device Id");
                }

                if (!licenseRequest.IsAdminRequest && string.IsNullOrEmpty(licenseRequest.WifiBSSID))
                {
                    PEMSEventSource.Log.DeviceApiPutFailure(String.Format("Invalid WifiBSSID {0}.", deviceId), deviceId, LogRequest);
                    return BadRequest("Invalid WifiBSSID");
                }

                if (!licenseRequest.IsAdminRequest && string.IsNullOrEmpty(licenseRequest.EnvironmentId))
                {
                    PEMSEventSource.Log.DeviceApiPutFailure(String.Format("Invalid environment id {0}.", deviceId), deviceId, LogRequest);
                    return BadRequest("Invalid Environment Id");
                }

                if (!licenseRequest.IsAdminRequest && !Guid.TryParse(licenseRequest.UserId, out userGuid))
                {
                    PEMSEventSource.Log.DeviceApiPutFailure(String.Format("Invalid user id {0}.", deviceId), deviceId, LogRequest);
                    return BadRequest("Invalid User Id");
                }

                if (!Enum.IsDefined(typeof(LicenseRequestType), (int)licenseRequest.RequestType))
                {
                    PEMSEventSource.Log.DeviceApiPutFailure(String.Format("Invalid request type {0}.", deviceId), deviceId, LogRequest);
                    return BadRequest("Invalid Request Type");
                }

                //Revoke license for a device requested by Admin when initiated through site, this will be explicitly set in the request
                //as RequestType=3. We simply ignore the values DownloadLicenseRequested and LearningContentQueued
                if (licenseRequest.RequestType == LicenseRequestType.RevokeLicense)
                {
                    return await RevokeLicenseFromSite(deviceId, licenseRequest).ConfigureAwait(false);
                }

                //Either download license is not requested, or if it is, there is no content i.e. queued to be downloaded
                //Or if request type is set explicitly to return license
                if (!licenseRequest.DownloadLicenseRequested || licenseRequest.LearningContentQueued <= 0
                    || licenseRequest.RequestType == LicenseRequestType.ReturnLicense)
                {
                    //Revoke/Return License from a device
                    return await ReturnLicenseFromDevice(deviceId, licenseRequest).ConfigureAwait(false);
                }

                //TODO with PUSH Notification Implementation
                if (licenseRequest.RequestType == LicenseRequestType.ServerGrant)
                {
                    PEMSEventSource.Log.DeviceApiPutFailure(
                      String.Format("Attempting to request license for request type ServerGrant - not implemented {0}.", deviceId),
                       deviceId, LogRequest);
                    HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.NotImplemented,
                        "Attempting to request license for request type ServerGrant - not implemented");
                    return new ResponseMessageResult(response);
                }

                //Defaulting to RequestLicense as above conditions are not satisifed
                return await RequestLicenseForDevice(deviceId, licenseRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogRequest = LogRequest ?? new LogRequest();
                LogRequest.Exception = ex;
                PEMSEventSource.Log.DeviceApiPutFailure(ex.Message, deviceId, LogRequest);
                return InternalServerError(new Exception(String.Format("Failed to request/return/revoke a license for device {0}.", deviceId)));
            }
        }

        private async Task<IHttpActionResult> RequestLicenseForDevice(string deviceId, DeviceLicenseRequest licenseRequest)
        {
            try
            {
                PEMSEventSource.Log.DeviceApiLicenseRequested("Attempting to request license", licenseRequest.DeviceId, LogRequest);

                var result = await _licenseService.RequestLicenseForDeviceAsync(licenseRequest, LogRequest).ConfigureAwait(false);
                if (result)
                {
                    PEMSEventSource.Log.DeviceApiLicenseSucceeded("License Request Completed", licenseRequest.DeviceId,
                       LogRequest);
                    var installedCourses = (List<Course>)licenseRequest;
                    var saveCourses = await _licenseService.SaveDownloadStatusAsync(new Guid(deviceId), installedCourses, LogRequest).ConfigureAwait(false);
                    if (!saveCourses)
                    {
                        PEMSEventSource.Log.DeviceApiLicenseFailed("Failed to update installed courses for device", licenseRequest.DeviceId,
                       LogRequest);
                        return
                     InternalServerError(
                         new Exception(String.Format("Failed to update installed courses for device {0}.", deviceId)));
                    }
                }
                else
                {
                    PEMSEventSource.Log.DeviceApiLicenseFailed("License Request could not be completed", licenseRequest.DeviceId,
                       LogRequest);
                }

                var deviceLicense = await _licenseService.GetLicenseForDeviceAsync(new Guid(deviceId), LogRequest).ConfigureAwait(false);
                if (deviceLicense == null)
                {
                    licenseRequest.CanDownloadLearningContent = false;
                    LogRequest.GrantDenyDecision = Boolean.FalseString;
                    PEMSEventSource.Log.DeviceApiLicenseFailed("Failed to retrieve a license", licenseRequest.DeviceId,
                      LogRequest);
                }
                else
                {
                    licenseRequest.CanDownloadLearningContent = true;
                    LogRequest.GrantDenyDecision = Boolean.TrueString;
                    PEMSEventSource.Log.DeviceApiLicenseSucceeded("Succesfully retrieved a license", licenseRequest.DeviceId,
                      LogRequest);
                }
                return Ok(licenseRequest);
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.DeviceApiPutFailure(ex.Message, deviceId, LogRequest);
                return
                    InternalServerError(
                        new Exception(String.Format("Failed to retrieve license for device {0}.", deviceId)));
            }
        }

        private async Task<IHttpActionResult> ReturnLicenseFromDevice(string deviceId, DeviceLicenseRequest licenseRequest)
        {
            try
            {
                PEMSEventSource.Log.DeviceApiLicenseReturned("Attempting to return license", licenseRequest.DeviceId, LogRequest);

                var deviceLicense = await _licenseService.GetLicenseForDeviceAsync(new Guid(deviceId), LogRequest).ConfigureAwait(false);
                if (deviceLicense == null)
                {
                    //Silent failure. Essentially this should be a 404 but avoiding this for backward compatibility.
                    PEMSEventSource.Log.DeviceApiPutFailure(
                        String.Format("Attempting to return license, for a device, that does not exist {0}", deviceId),
                        deviceId, LogRequest);
                }
                else
                {
                    var user = await _userService.GetUserAsync(new Guid(licenseRequest.UserId), LogRequest).ConfigureAwait(false) ??
                               await _userService.InsertUserAsync(new User()
                               {
                                   UserId = new Guid(licenseRequest.UserId),
                                   Username = licenseRequest.Username,
                                   UserType = licenseRequest.UserType
                               }, LogRequest).ConfigureAwait(false);

                    await _licenseService.RevokeLicenseForDeviceAsync(deviceLicense.LicenseRequest.LicenseRequestId,
                        user.UserId, licenseRequest.RequestTime, LogRequest, false).ConfigureAwait(false);
                }
                licenseRequest.CanDownloadLearningContent = false;
                LogRequest.GrantDenyDecision = Boolean.FalseString;
                return Ok(licenseRequest);
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.DeviceApiPutFailure(ex.Message, deviceId, LogRequest);
                return
                    InternalServerError(
                        new Exception(String.Format("Failed to return license for device {0}.", deviceId)));
            }
        }

        private async Task<IHttpActionResult> RevokeLicenseFromSite(string deviceId, DeviceLicenseRequest licenseRequest)
        {
            try
            {
                PEMSEventSource.Log.DeviceApiLicenseRevoked("Attempting to revoke license", licenseRequest.DeviceId, LogRequest);

                if (!(IsAuthenticated && IsAdmin))
                {
                    PEMSEventSource.Log.DeviceApiPutFailure(
                        String.Format("Attempted to revoke license as an admin when not " +
                                      "authorized {0}.", deviceId), deviceId, LogRequest);
                    HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                        "Unauthorized to access this endpoint");
                    return new ResponseMessageResult(response);
                }

                var deviceLicense = await _licenseService.GetLicenseForDeviceAsync(new Guid(deviceId), LogRequest).ConfigureAwait(false);
                if (deviceLicense == null)
                {
                    //Silent failure. Essentially this should be a 404 and displayed as an error message on the site.
                    PEMSEventSource.Log.DeviceApiPutFailure(
                        String.Format("Attempting to revoke device license that does not exist {0} by an admin",
                            deviceId),
                        deviceId, LogRequest);
                }
                else
                {
                    await
                        _licenseService.RevokeLicenseForDeviceAsync(deviceLicense.LicenseRequest.LicenseRequestId,
                            AdminId,
                            licenseRequest.RequestTime, LogRequest).ConfigureAwait(false);
                    licenseRequest.CanDownloadLearningContent = false;
                    LogRequest.GrantDenyDecision = Boolean.FalseString;
                }
                return Ok(licenseRequest);
            }
            catch (Exception ex)
            {
                LogRequest = LogRequest ?? new LogRequest();
                LogRequest.Exception = ex;
                PEMSEventSource.Log.DeviceApiPutFailure(ex.Message, deviceId, LogRequest);
                return
                    InternalServerError(
                        new Exception(String.Format("Failed to revoke license for device {0}.", deviceId)));
            }

            //TODO Future Implementation Lets try and get a license, requested by an admin, if there is anything to download 
            //if (licenseRequest.DownloadLicenseRequested && licenseRequest.LearningContentQueued > 0)
            //{
            //    licenseRequest.CanDownloadLearningContent = false;
            //    PEMSEventSource.Log.DeviceApiPutFailure(
            //        String.Format("Attempting to request license as an admin - not implemented {0}.", deviceId), 
            //        deviceId, LogRequest);
            //    HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.NotImplemented,
            //        "Attempting to request license as an admin - not implemented");
            //    throw new HttpResponseException(response);
            //}
        }
    }
}
