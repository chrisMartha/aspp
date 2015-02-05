using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Interfaces;

namespace PSoC.ManagementService.Data.Models
{
    public class ClassroomDto : IEntity
    {
        #region Constructors

        public ClassroomDto()
        {
            AccessPoints = new HashSet<AccessPointDto>();
        }

        #endregion Constructors

        public ICollection<AccessPointDto> AccessPoints
        {
            get; set;
        }

        [StringLength(50)]
        public string BuildingName
        {
            get; set;
        }

        [StringLength(200)]
        public string ClassroomAnnotation
        {
            get; set;
        }

        [Key]
        [Required]
        public Guid ClassroomID
        {
            get; set;
        }

        [Required]
        [StringLength(50)]
        public string ClassroomName
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime Created
        {
            get; set;
        }

        public SchoolDto School
        {
            get; set;
        }
    }
}
