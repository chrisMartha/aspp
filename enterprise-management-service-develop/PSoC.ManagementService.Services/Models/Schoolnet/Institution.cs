using System;
using Newtonsoft.Json;

namespace PSoC.ManagementService.Services.Models.Schoolnet
{
    public class Institution
    {
        [JsonProperty("institutionId")]
        public Guid InstitutionId { get; set; }

        [JsonProperty("institutionName")]
        public string InstitutionName { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("institutionType")]
        public string InstitutionType { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }
    }
}
