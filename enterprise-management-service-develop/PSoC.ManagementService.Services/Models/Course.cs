using System;
using Newtonsoft.Json;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    public class Course
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }

        [JsonProperty("percent")]
        public decimal? Percent { get; set; }

        [JsonProperty("learningResourceId")]
        public Guid LearningResourceId { get; set; }

        public static explicit operator DeviceInstalledCourseDto(Course c)
        {
            if (c == null) return null;
            return new DeviceInstalledCourseDto()
            {
                Course = new CourseDto()
                {
                    CourseLearningResourceID = c.LearningResourceId,
                    Grade = c.Grade,
                    Subject = c.Subject,
                },
                PercentDownloaded = c.Percent
            };
        }
    }   
}
