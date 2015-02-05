
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Security;

namespace PSoC.ManagementService.Data.Models
{
    public class UserDto : IEntity
    {
        #region Constructors

        public UserDto()
        {
            LicenseRequests = new HashSet<LicenseRequestDto>();
        }

        #endregion Constructors


        [StringLength(80)]
        public string Username
        {
            get
            {
                if (UsernameEnc == null)
                    return null;

                return UsernameEnc.DecryptedValue;
            }
            set
            {
                UsernameEnc = value;
            }
        }

        [StringLength(20)]
        public string UserType
        {
            get
            {
                if (UserTypeEnc == null)
                    return null;

                return UserTypeEnc.DecryptedValue;
            }
            set
            {
                UserTypeEnc = value;
            }
        }

        [Column(TypeName = "datetime2")]
        public DateTime Created
        {
            get;
            set;
        }


        [Key]
        [Required]
        public Guid UserID
        {
            get;
            set;
        }


        internal EncrypedField<string> UsernameEnc { get; set; }
        internal EncrypedField<string> UserTypeEnc { get; set; }

        public ICollection<LicenseRequestDto> LicenseRequests { get; set; }

    }
}
