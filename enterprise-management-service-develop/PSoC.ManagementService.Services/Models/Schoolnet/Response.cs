using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PSoC.ManagementService.Services.Models.Schoolnet
{
    public class DistrictResponse : Response
    {
        public DistrictResponse() : base("District")
        {
        }
    }

    public class SchoolsResponse : Response
    {
        public SchoolsResponse() : base("School")
        {
        }
    }

    public abstract class Response
    {
        private readonly string _validInstitutionType;

        [JsonProperty("code")]
        public int Code { private get; set; }

        [JsonProperty("requestId")]
        public Guid RequestId { get; set; }

        [JsonProperty("status")]
        public string Status { private get; set; }

        [JsonProperty("data")]
        public Institution[] Institutions { get; set; }

        protected Response(string validInstitutionType)
        {
            _validInstitutionType = validInstitutionType;
        }

        public List<string> Validate()
        {
            List<string> validationErrors = new List<string>();

            const int ok = 200;
            const int partialContent = 206;
            int[] validCodes = { ok, partialContent };
            bool isValidCode = validCodes.Any(validCode => Code == validCode);
            if (!isValidCode)
            {
                string validCodesString = string.Empty;
                validCodesString = validCodes.Aggregate(validCodesString, (current, validCode) => current + (validCode + ", "));
                char[] endChars = {',', ' '};
                validCodesString = validCodesString.TrimEnd(endChars);
                validationErrors.Add(string.Format("Code value {0} is not valid. Allowed values: {1}.", Code, validCodesString));
            }

            const string validStatus = "Success";
            if (Status != validStatus)
            {
                validationErrors.Add(string.Format("Status value {0} is not valid. Allowed value: {1}.", Status, validStatus));
            }

            validationErrors.AddRange(from institution in Institutions
                                      where institution.InstitutionType != _validInstitutionType
                                      select string.Format("Institution id {0} type value {1} is not valid. Allowed value: {2}.", institution.InstitutionId, institution.InstitutionType, _validInstitutionType));

            return validationErrors;
        }
    }
}
