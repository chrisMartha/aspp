using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace PSoC.ManagementService.Services.Models
{
    [Obsolete("Replaced with DeviceLicenseRequest")]
    public class Device : TableEntity
    {
        public Device()
        {
            this.CanDownloadLearningContent = false;
            //To support 1.4, if parameter is missing default to true
            this.DownloadLicenseRequested = true;
        }

        [IgnoreProperty]
        public string EnvironmentId
        {
            get { return this.PartitionKey; }
            set { this.PartitionKey = value; }
        }

        [IgnoreProperty]
        public string DeviceId
        {
            get { return this.RowKey; }
            set { this.RowKey = value; }
        }

        public string AppId { get; set; }
        public string AppVersion { get; set; }
        public string DeviceType { get; set; }
        public string DeviceOSVersion { get; set; }
        //public string DeviceName { get; set; }
        //public string UserId { get; set; }
        //public string Username { get; set; }
        public string UserType { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public DateTime ContentLastUpdatedAt { get; set; }
        public long FreeSpaceSize { get; set; }
        public long ContentSize { get; set; }
        public Boolean CanDownloadLearningContent { get; set; }
        public List<int> ConfiguredGrades { get; set; }
        public int ConfiguredUnitCount { get; set; }
        public List<Dictionary<string, object>> CoursesInstalled { get; set; }
        public int LearningContentQueued { get; set; }
        public string WifiBSSID { get; set; }
        public string WifiSSID { get; set; }
        public bool DownloadLicenseRequested { get; set; }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            // azure complains about dates earlier than 1601
            if (ContentLastUpdatedAt.Year < 1970)
            {
                ContentLastUpdatedAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            var results = base.WriteEntity(operationContext);

            results["ConfiguredGrades"] = new EntityProperty(JsonConvert.SerializeObject(this.ConfiguredGrades));
            results["CoursesInstalled"] = new EntityProperty(JsonConvert.SerializeObject(this.CoursesInstalled));

            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            if (properties.ContainsKey("ConfiguredGrades"))
            {
                this.ConfiguredGrades = JsonConvert.DeserializeObject<List<int>>(properties["ConfiguredGrades"].StringValue);
            }

            if (properties.ContainsKey("CoursesInstalled"))
            {
                this.CoursesInstalled = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(properties["CoursesInstalled"].StringValue);
            }
        }
    }
}