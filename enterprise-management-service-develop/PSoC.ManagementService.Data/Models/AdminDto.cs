using System;
using System.ComponentModel.DataAnnotations;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Interfaces;

namespace PSoC.ManagementService.Data.Models
{
    public class AdminDto : IEntity
    {
        public bool Active
        {
            get; set;
        }

        public string AdminEmail
        {
            get; set;
        }

        public AdminType AdminType
        {
            get
            {
                if (School != null)
                {
                    return AdminType.SchoolAdmin;
                }

                if (District != null)
                {
                    return AdminType.DistrictAdmin;
                }

                return AdminType.GlobalAdmin;
            }
        }

        public DateTime Created
        {
            get; set;
        }

        public DistrictDto District
        {
            get; set;
        }

        public DateTime? LastLoginDateTime
        {
            get; set;
        }

        public SchoolDto School
        {
            get; set;
        }

        [Key]
        [Required]
        public UserDto User
        {
            get; set;
        }
    }
}