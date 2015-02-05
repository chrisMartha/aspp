using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSoC.ManagementService.Services;
using PSoC.ManagementService.Services.Models;

namespace PSoC.DataGenerator
{
    class Program
    {
        const int NUMBER_OF_DEVICES_TO_ADD = 10;
        const string CONNECTION_STRING = @"DefaultEndpointsProtocol=https;AccountName=psocmanagementservicedev;AccountKey=zvIV/1rPJiVyb53Ntd2OQtB0hYeQ9lvZUZV6iMKGifUKr6LZwO542Ft7ZNhjj9vkAXs/oB+v/0UHhMyYCGyEjw==";
        const string TABLE_NAME = "devices";

        static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
            Console.WriteLine("Done");
        }

        static async Task MainAsync(string[] args)
        {
            var azureClient = new AzureTableService(CONNECTION_STRING);
            var environmentIdentifiers = new string[] { 
                "ccsocdct",
                "ccsocdct1",
                "ccsocdct2",
                "ccsocdct3",
                "ccsocdct4",
                "ccsocdct5"
            };

            var environmentId = environmentIdentifiers[Faker.RandomNumber.Next(0, environmentIdentifiers.Length - 1)];
            var devices = new List<Device>();
            for (int i = 0; i < NUMBER_OF_DEVICES_TO_ADD; i++)
            {
                var d = new Device
                {
                    AppId = Faker.Company.Name(),
                    AppVersion = Faker.RandomNumber.Next().ToString(),
                    CanDownloadLearningContent = true,
                    ConfiguredGrades = null,
                    ConfiguredUnitCount = 0,
                    ContentLastUpdatedAt = DateTime.UtcNow,
                    ContentSize = Faker.RandomNumber.Next(),
                    CoursesInstalled = null,
                    DeviceId = Guid.NewGuid().ToString(),
                    DeviceOSVersion = "7.0",
                    DeviceType = "iPad",
                    EnvironmentId = environmentId,
                    FreeSpaceSize = 0,
                    LearningContentQueued = 0,
                    LocationId = Guid.NewGuid().ToString(),
                    LocationName = Faker.Company.Name(),
                    UserType = "student",
                    WifiBSSID = Faker.Lorem.Words(1).First(),
                    WifiSSID = Faker.Lorem.Words(1).First(),
                };

                devices.Add(d);
            }

            await azureClient.InsertBatchEntitiesAsync<Device>(TABLE_NAME, devices);
            Console.WriteLine("Insert complete");
        }
    }
}
