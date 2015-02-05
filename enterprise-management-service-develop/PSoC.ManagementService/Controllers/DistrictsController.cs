using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using PSoC.ManagementService.Core;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models;
using PSoC.ManagementService.Services.Models.Schoolnet;

namespace PSoC.ManagementService.Controllers
{
    /// <summary>
    /// Web API 2.0 controller with POST actions to import district and schools data
    /// </summary>
    public class DistrictsController : ControllerBase
    {
        private readonly IDistrictService _districtService;
        private readonly ISchoolService _schoolService;

        public DistrictsController(IDistrictService districtService, ISchoolService schoolService)
        {
            _districtService = districtService;
            _schoolService = schoolService;
        }

        /// <summary>
        /// Endpoint to import district data
        /// </summary>
        /// <param name="response">District data in Schoolnet response format</param>
        /// <returns>200 if successful, 400 if errors validating data or inserting it into the database</returns>
        [HttpPost]
        [Route("api/v1/districts")]
        public async Task<IHttpActionResult> Post([FromBody]DistrictResponse response)
        {
            List<string> validationErrors = response.Validate();
            if (validationErrors != null && validationErrors.Count > 0)
            {
                string message = string.Empty;
                message = validationErrors.Aggregate(message, (current, validationError) => current + (validationError + Environment.NewLine));
                return BadRequest(message);
            }

            List<string> operationErrors = new List<string>();
            foreach (Institution institution in response.Institutions)
            {
                District district = await _districtService.GetByIdAsync(institution.InstitutionId);
                if (district == null)
                {
                    district = new District
                    {
                        DistrictId = institution.InstitutionId,
                        DistrictName = institution.InstitutionName,
                        CreatedBy = "DistrictsController",
                        DistrictAnnotation = string.Format("externalId = {0}", institution.ExternalId),
                        // TODO: The following values are required but shouldn't be the same as in configuraiton settings
                        OAuthApplicationId = GlobalAppSettings.GetValue("OAuthApplicationId"),
                        OAuthClientId = GlobalAppSettings.GetValue("OAuthClientId"),
                        OAuthUrl = GlobalAppSettings.GetValue("OAuthUrl")
                    };
                    try
                    {
                        District newDistrict = await _districtService.CreateAsync(district);
                        if (newDistrict == null)
                        {
                            operationErrors.Add(string.Format("Failed to add district id {0}.", institution.InstitutionId));
                        }
                    }
                    catch (Exception e)
                    {
                        operationErrors.Add(string.Format("Failed to add district id {0}. Exception: {1}", institution.InstitutionId, e));
                    }
                }
                else
                {
                    operationErrors.Add(string.Format("District with id {0} already exists.", institution.InstitutionId));
                }
            }

            if (operationErrors.Count > 0)
            {
                string message = string.Empty;
                message = operationErrors.Aggregate(message, (current, operationError) => current + (operationError + Environment.NewLine));
                return BadRequest(message);
            }

            return Ok();
        }

        /// <summary>
        /// Endpoint to import schools data
        /// </summary>
        /// <param name="districtId">District id as Schoolnet institution id</param>
        /// <param name="response">District data in Schoolnet response format</param>
        /// <returns>200 if successful, 400 if errors validating data or inserting it into the database, 404 if district is not found</returns>
        [HttpPost]
        [Route("api/v1/districts/{districtId}/schools")]
        public async Task<IHttpActionResult> Post(Guid districtId, [FromBody]SchoolsResponse response)
        {
            List<string> validationErrors = response.Validate();
            if (validationErrors != null && validationErrors.Count > 0)
            {
                string message = string.Empty;
                message = validationErrors.Aggregate(message, (current, validationError) => current + (validationError + Environment.NewLine));
                return BadRequest(message);
            }

            District district;
            try
            {
                district = await _districtService.GetByIdAsync(districtId);
                if (district == null)
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            List<string> operationErrors = new List<string>();
            foreach (Institution institution in response.Institutions)
            {
                School school = await _schoolService.GetByIdAsync(institution.InstitutionId);
                if (school == null)
                {
                    school = new School
                    {
                        SchoolId = institution.InstitutionId,
                        District = district,
                        SchoolName = institution.InstitutionName,
                        SchoolAnnotation = string.Format("externalId = {0}", institution.ExternalId),
                    };
                    try
                    {
                        School newSchool = await _schoolService.CreateAsync(school);
                        if (newSchool == null)
                        {
                            operationErrors.Add(string.Format("Failed to add school id {0}.", institution.InstitutionId));
                        }
                    }
                    catch (Exception e)
                    {
                        operationErrors.Add(string.Format("Failed to add school id {0}. Exception: {1}", institution.InstitutionId, e));
                    }
                }
                else
                {
                    operationErrors.Add(string.Format("School with id {0} already exists.", institution.InstitutionId));
                }
            }

            if (operationErrors.Count > 0)
            {
                string message = string.Empty;
                message = operationErrors.Aggregate(message, (current, operationError) => current + (operationError + Environment.NewLine));
                return BadRequest(message);
            }

            return Ok();
        }

    }
}
