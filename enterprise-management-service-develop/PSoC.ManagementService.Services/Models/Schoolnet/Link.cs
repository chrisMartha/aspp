using Newtonsoft.Json;

namespace PSoC.ManagementService.Services.Models.Schoolnet
{
    public class Link
    {
        [JsonProperty("rel")]
        public string Relationship { get; set; }

        [JsonProperty("href")]
        public string Url { get; set; }
    }
}
