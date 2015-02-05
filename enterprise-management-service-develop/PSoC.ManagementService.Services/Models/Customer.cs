using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace PSoC.ManagementService.Services.Models
{
    public class Customer : TableEntity
    {
        public Customer()
        {

        }

        [IgnoreProperty]
        public string EnvironmentId
        {
            get { return this.PartitionKey; }
            set { this.PartitionKey = value; }
        }

        [IgnoreProperty]
        public string CustomerName
        {
            get { return this.RowKey; }
            set { this.RowKey = value; }
        }

        public string OAuthURL { get; set; }
        public Guid OAuthApplicationId { get; set; }
        public Guid OAuthClientId { get; set; }
        public string AssociatedEnvironmentIds { get; set; }
        public int DownloadLicenseTimeout { get; set; }
        public int MaxDownloadsPerClass { get; set; }
		public int MaxDownloadsPerCustomer { get; set; }
		
		public List<Dictionary<string, string>> AuthenticationProviders { get; set; }
		
        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);

            results["AuthenticationProviders"] = new EntityProperty(JsonConvert.SerializeObject(this.AuthenticationProviders));

            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            if (properties.ContainsKey("AuthenticationProviders"))
            {
                this.AuthenticationProviders = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(properties["AuthenticationProviders"].StringValue);
            }
        }
    }
}
