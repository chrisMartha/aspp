// Uncomment the following to provide samples for PageResult<T>. Must also add the Microsoft.AspNet.WebApi.OData
// package to your project.
////#define Handle_PageResultOfT

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
#if Handle_PageResultOfT
using System.Web.Http.OData;
#endif
using PSoC.ManagementService.Services.Models;
using PSoC.ManagementService.Services.Models.Schoolnet;

namespace PSoC.ManagementService.Areas.HelpPage
{
    /// <summary>
    /// Use this class to customize the Help Page.
    /// For example you can set a custom <see cref="System.Web.Http.Description.IDocumentationProvider"/> to supply the documentation
    /// or you can provide the samples for the requests/responses.
    /// </summary>
    public static class HelpPageConfig
    {
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
            MessageId = "PSoC.ManagementService.Areas.HelpPage.TextSample.#ctor(System.String)",
            Justification = "End users may choose to merge this string with existing localized resources.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly",
            MessageId = "bsonspec",
            Justification = "Part of a URI.")]
        public static void Register(HttpConfiguration config)
        {
            // Uncomment the following to use the documentation from XML documentation file.
            config.SetDocumentationProvider(new XmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/App_Data/PSoC.ManagementService.xml")));

            var sampleCourseList = new List<Course>
            {
                new Course
                {
                    Subject = "ELA",
                    Grade = "10",
                    Percent = 100,
                    LearningResourceId = new Guid("0737c4b6-40f2-4c0e-b223-4977b03f9ae7")
                },
                new Course
                {
                    Subject = "Math",
                    Grade = "10",
                    Percent = 70,
                    LearningResourceId = new Guid("bb2f41d7-40f2-4997-aa7d-13424b117dd5")
                },
                new Course
                {
                    Subject = "ELA",
                    Grade = "11",
                    Percent = 100,
                    LearningResourceId = new Guid("aa26af81-40f2-43ad-9683-d0f13890f057")
                },
                new Course
                {
                    Subject = "Math",
                    Grade = "11",
                    Percent = 99,
                    LearningResourceId = new Guid("00ada5dd-40f2-4713-9266-319591b76d6a")
                },
                new Course
                {
                    Subject = "ELA",
                    Grade = "12",
                    Percent = 96,
                    LearningResourceId = new Guid("bbeb7d9b-40f2-438f-b9d6-de3a741b7fca")
                },
            };

            var sampleDeviceLicenseRequest = new DeviceLicenseRequest
            {
                DeviceId = "9D6FC8C4-02CC-437C-9BE5-D95C4D78AC29",
                WifiBSSID = "2a:cf:e9:81:0b:00",
                DownloadLicenseRequested = true,
                LearningContentQueued = 10,
                EnvironmentId = "ccsocdct",
                DeviceName = "iPad Simulator",
                UserId = "14a7b5f9-0cf4-4803-b62f-720eebd6986a",
                LocationId = "d622a7c1-d3b6-4236-9349-474ce92f2232",
                LocationName = "Alliance Collins Family College-Ready High School",
                Username = "John Weber",
                UserType = "teacher",
                ContentLastUpdatedAt = new DateTime(0),
                ConfiguredGrades = new List<int>(),
                ConfiguredUnitCount = 0,
                PSoCAppId = "com.pearson.commoncore.commoncore",
                DeviceOSVersion = "7.1",
                InstalledContentSize = 10,
                WifiSSID = "epic",
                PSoCAppVersion = "1.5.0",
                DeviceFreeSpace = 158157721600,
                CoursesInstalled = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Subject", "ELA"},
                        {"Grade", "10"},
                        {"Percent", 0},
                        {"LearningResourceId", "0737c4b6-40f2-4c0e-b223-4977b03f9ae7"}
                    },
                    new Dictionary<string, object>
                    {
                        {"Subject", "ELA"},
                        {"Grade", "11"},
                        {"Percent", 0},
                        {"LearningResourceId", "aa26af81-40f2-43ad-9683-d0f13890f057"}
                    },
                    new Dictionary<string, object>
                    {
                        {"Subject", "Math"},
                        {"Grade", "11"},
                        {"Percent", 0},
                        {"LearningResourceId", "00ada5dd-40f2-4713-9266-319591b76d6a"}
                    },
                    new Dictionary<string, object>
                    {
                        {"Subject", "ELA"},
                        {"Grade", "6"},
                        {"Percent", 0},
                        {"LearningResourceId", "0aac764c-40f2-4d28-a4f3-2b9953b760d2"}
                    },
                    new Dictionary<string, object>
                    {
                        {"Subject", "Math"},
                        {"Grade", "6"},
                        {"Percent", 0},
                        {"LearningResourceId", "f84ecdfa-40f2-4854-bdb9-a0ee8bcda8cf"}
                    },
                    new Dictionary<string, object>
                    {
                        {"Subject", "ELA"},
                        {"Grade", "8"},
                        {"Percent", 0},
                        {"LearningResourceId", "f54fce7b-40f2-4bf1-9c04-858fbaac5ccf"}
                    },
                    new Dictionary<string, object>
                    {
                        {"Subject", "Math"},
                        {"Grade", "8"},
                        {"Percent", 0},
                        {"LearningResourceId", "33236cc6-40f2-4157-9faf-4aa38d795130"}
                    },
                    new Dictionary<string, object>
                    {
                        {"Subject", "ELA"},
                        {"Grade", "9"},
                        {"Percent", 0},
                        {"LearningResourceId", "104ed877-40f2-41bb-8394-5364e849c6ec"}
                    },
                    new Dictionary<string, object>
                    {
                        {"Subject", "Math"},
                        {"Grade", "9"},
                        {"Percent", 0},
                        {"LearningResourceId", "ff18e8cf-40f2-4450-a233-4cdb5b15d981"}
                    }
                },
                DeviceType = "iPad Simulator"
            };

            var sampleDistrictRequest = new DistrictResponse
            {
                Code = 200,
                RequestId = new Guid("9bbd92a3-9818-44ee-8bac-3c779d4eafef"),
                Status = "Success",
                Institutions = new []
                {
                    new Institution
                    {
                        InstitutionId = new Guid("8141c935-3213-4c38-8342-65348e1cd97b"),
                        InstitutionName = "National School District",
                        ExternalId = "sw_300",
                        InstitutionType = "District",
                        Links = new []
                        {
                            new Link
                            {
                                Relationship = "GetSchools",
                                Url = "https://schoolnet.ccsocdev.net/api/v1/districts/8141c935-3213-4c38-8342-65348e1cd97b/schools"
                            }
                        }
                    }
                }
            };

            var sampleSchoolsRequest = new SchoolsResponse
            {
                Code = 206,
                RequestId = new Guid("e8eb0920-3776-42c5-bf16-fbcb7cffa34a"),
                Status = "Success",
                Institutions = new[]
                {
                    new Institution
                    {
                        InstitutionId = new Guid("008392ec-417c-4bae-8740-ff4891237cac"),
                        InstitutionName = "W. C. BRUNK (VAN)",
                        ExternalId = "965",
                        InstitutionType = "School",
                        Links = new []
                        {
                            new Link
                            {
                                Relationship = "GetSections",
                                Url = "https://schoolnet.ccsocdev.net/api/v1/schools/008392ec-417c-4bae-8740-ff4891237cac/sections"
                            }
                        }
                    },
                    new Institution
                    {
                        InstitutionId = new Guid("02ac9ee0-8a42-4039-82b6-7c663e759ef7"),
                        InstitutionName = "Ohio School For The Deaf",
                        ExternalId = "841",
                        InstitutionType = "School",
                        Links = new []
                        {
                            new Link
                            {
                                Relationship = "GetSections",
                                Url = "https://schoolnet.ccsocdev.net/api/v1/schools/02ac9ee0-8a42-4039-82b6-7c663e759ef7/sections"
                            }
                        }
                    },
                    new Institution
                    {
                        InstitutionId = new Guid("02f456c1-999d-4cde-863a-fc7f7e66c4c9"),
                        InstitutionName = "Columbus Montessori School",
                        ExternalId = "819",
                        InstitutionType = "School",
                        Links = new []
                        {
                            new Link
                            {
                                Relationship = "GetSections",
                                Url = "https://schoolnet.ccsocdev.net/api/v1/schools/02f456c1-999d-4cde-863a-fc7f7e66c4c9/sections"
                            }
                        }
                    }
                }
            };

            // Uncomment the following to use "sample string" as the sample for all actions that have string as the body parameter or return type.
            // Also, the string arrays will be used for IEnumerable<string>. The sample objects will be serialized into different media type 
            // formats by the available formatters.
            config.SetSampleObjects(new Dictionary<Type, object>
            {
                //{typeof(string), "sample string"},
                //{typeof(IEnumerable<string>), new string[]{"sample 1", "sample 2"}}
                { typeof(List<Course>), sampleCourseList },
                { typeof(DeviceLicenseRequest), sampleDeviceLicenseRequest },
                { typeof(DistrictResponse), sampleDistrictRequest },
                { typeof(SchoolsResponse), sampleSchoolsRequest }
            });

            // Extend the following to provide factories for types not handled automatically (those lacking parameterless
            // constructors) or for which you prefer to use non-default property values. Line below provides a fallback
            // since automatic handling will fail and GeneratePageResult handles only a single type.
#if Handle_PageResultOfT
            config.GetHelpPageSampleGenerator().SampleObjectFactories.Add(GeneratePageResult);
#endif

            // Extend the following to use a preset object directly as the sample for all actions that support a media
            // type, regardless of the body parameter or return type. The lines below avoid display of binary content.
            // The BsonMediaTypeFormatter (if available) is not used to serialize the TextSample object.
            config.SetSampleForMediaType(
                new TextSample("Binary JSON content. See http://bsonspec.org for details."),
                new MediaTypeHeaderValue("application/bson"));

            //// Uncomment the following to use "[0]=foo&[1]=bar" directly as the sample for all actions that support form URL encoded format
            //// and have IEnumerable<string> as the body parameter or return type.
            //config.SetSampleForType("[0]=foo&[1]=bar", new MediaTypeHeaderValue("application/x-www-form-urlencoded"), typeof(IEnumerable<string>));

            //// Uncomment the following to use "1234" directly as the request sample for media type "text/plain" on the controller named "Values"
            //// and action named "Put".
            //config.SetSampleRequest("1234", new MediaTypeHeaderValue("text/plain"), "Values", "Put");

            //// Uncomment the following to use the image on "../images/aspNetHome.png" directly as the response sample for media type "image/png"
            //// on the controller named "Values" and action named "Get" with parameter "id".
            //config.SetSampleResponse(new ImageSample("../images/aspNetHome.png"), new MediaTypeHeaderValue("image/png"), "Values", "Get", "id");

            //// Uncomment the following to correct the sample request when the action expects an HttpRequestMessage with ObjectContent<string>.
            //// The sample will be generated as if the controller named "Values" and action named "Get" were having string as the body parameter.
            //config.SetActualRequestType(typeof(string), "Values", "Get");

            //// Uncomment the following to correct the sample response when the action returns an HttpResponseMessage with ObjectContent<string>.
            //// The sample will be generated as if the controller named "Values" and action named "Post" were returning a string.
            //config.SetActualResponseType(typeof(string), "Values", "Post");
        }

#if Handle_PageResultOfT
        private static object GeneratePageResult(HelpPageSampleGenerator sampleGenerator, Type type)
        {
            if (type.IsGenericType)
            {
                Type openGenericType = type.GetGenericTypeDefinition();
                if (openGenericType == typeof(PageResult<>))
                {
                    // Get the T in PageResult<T>
                    Type[] typeParameters = type.GetGenericArguments();
                    Debug.Assert(typeParameters.Length == 1);

                    // Create an enumeration to pass as the first parameter to the PageResult<T> constuctor
                    Type itemsType = typeof(List<>).MakeGenericType(typeParameters);
                    object items = sampleGenerator.GetSampleObject(itemsType);

                    // Fill in the other information needed to invoke the PageResult<T> constuctor
                    Type[] parameterTypes = new Type[] { itemsType, typeof(Uri), typeof(long?), };
                    object[] parameters = new object[] { items, null, (long)ObjectGenerator.DefaultCollectionSize, };

                    // Call PageResult(IEnumerable<T> items, Uri nextPageLink, long? count) constructor
                    ConstructorInfo constructor = type.GetConstructor(parameterTypes);
                    return constructor.Invoke(parameters);
                }
            }

            return null;
        }
#endif
    }
}