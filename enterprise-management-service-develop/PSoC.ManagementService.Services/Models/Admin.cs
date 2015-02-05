using System;
using System.ComponentModel.DataAnnotations;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    /// <summary>
    /// Service level representation of Admin Model
    /// </summary>
    public class Admin
    {
        private string _userType;
        private AdminType _adminType;

        [Display(Name = "PEMS ID")]
        public Guid UserId { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Admin Type")]
        public string UserType { get; set; }

        public bool Active { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLoginDateTime { get; set; }

        [Display(Name = "District ID")]
        public Guid? DistrictId { get; set; }

        [Display(Name = "District")]
        public string DistrictName { get; set; }

        [Display(Name = "School ID")]
        public Guid? SchoolId { get; set; }

        [Display(Name = "School")]
        public string SchoolName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string AdminEmail { get; set; }

        [Required]
        [Display(Name = "Type of Admin")]
        public AdminType AdminType { get; set; }

        public static explicit operator Admin(AdminDto admin)
        {
            if (admin == null) return null;

            return new Admin()
            {
                UserId = admin.User.UserID,
                Username = admin.User.Username,
                UserType = admin.User.UserType,
                Active = admin.Active,
                LastLoginDateTime = admin.LastLoginDateTime != null ? admin.LastLoginDateTime.Value : (DateTime?) null,
                DistrictId = admin.District != null ? admin.District.DistrictId : (Guid?) null,
                DistrictName = admin.District != null ? admin.District.DistrictName : null,
                SchoolId = admin.School != null ? admin.School.SchoolID : (Guid?) null,
                SchoolName = admin.School != null ? admin.School.SchoolName : null,
                AdminEmail = admin.AdminEmail,
                AdminType = admin.AdminType
            };
        }

        public static explicit operator AdminDto(Admin admin)
        {
            if (admin == null) return null;

            return new AdminDto
            {
                Active = admin.Active,
                AdminEmail = admin.AdminEmail,
                // Created wil be added by the database default constraint
                District = (admin.DistrictId == null) ? null : new DistrictDto { DistrictId = admin.DistrictId.Value },
                LastLoginDateTime = admin.LastLoginDateTime,
                School = (admin.SchoolId == null) ? null : new SchoolDto { SchoolID = admin.SchoolId.Value },
                User = new UserDto
                {
                    // Created wil be added by the database default constraint
                   UserID = admin.UserId,
                   Username = admin.Username,
                   UserType = admin.UserType
                }
            };
        }
    }
}
