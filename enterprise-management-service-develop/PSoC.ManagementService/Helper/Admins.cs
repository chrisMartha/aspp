using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using PSoC.ManagementService.Models;
using PSoC.ManagementService.Services;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Helper
{
    public class Admins
    {
        private static IEnumerable<SelectListItem> _availableDistricts = null;
        private static IEnumerable<SelectListItem> _availableSchools = null;

        public static IEnumerable<SelectListItem> AvailableDistricts
        {
            get
            {
                if (_availableDistricts == null)
                {
                    var districts =  ServiceLocator.GetDistrictService().GetAsync().Result;
                    _availableDistricts = districts
                        .Select(x =>
                            new SelectListItem
                            {
                                Value = x.DistrictId.ToString(),
                                Text = x.DistrictName ??  x.DistrictId.ToString()
                            })
                            .OrderBy(x => x.Text);

                    // needed for place text holder to show correctly
                    _availableDistricts = (new List<SelectListItem> { new SelectListItem { } }).Concat(_availableDistricts);
                }

                return _availableDistricts;
            }
        }

        public static IEnumerable<SelectListItem> AvailableSchools
        {
            get
            {
                if (_availableSchools == null)
                {
                    var schools =  ServiceLocator.GetSchoolService().GetAsync().Result;
                    _availableSchools = schools
                        .Select(x =>
                            new SelectListItem
                            {
                                Value = x.SchoolId.ToString(),
                                Text = x.SchoolName ?? x.SchoolId.ToString()
                            })
                            .OrderBy(x => x.Text);

                    // needed for place holder text to show correctly
                    return _availableSchools = (new List<SelectListItem> { new SelectListItem { } }).Concat(_availableSchools); 
                }

                // needed for place holder text to show correctly
                return _availableSchools;
            }
        }
    }
}