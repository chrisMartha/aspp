using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;


namespace PSoC.ManagementService.Services.Extentions
{
    public static class Models
    {
        /// <summary>
        /// Cast SchoolDto to School
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public static IList<School> ToSchoolList (this IList<SchoolDto> schools)
        {
            if (schools == null) return null;

            var schoolList = new List<School>();

            foreach(var s in schools)
            {
                schoolList.Add(new School(s));
            }

            return schoolList;
        }
    }
}
