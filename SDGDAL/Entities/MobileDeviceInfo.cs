using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MobileDeviceInfo")]
    public class MobileDeviceInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MobileDeviceInfoId { get; set; }

        [StringLength(50)]
        public string Platform { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string Manufacturer { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [StringLength(50)]
        public string OS { get; set; }

        [StringLength(50)]
        public string IMEI { get; set; }

        [StringLength(50)]
        public string IP { get; set; }

        public DateTime DateCreated { get; set; }
    }
}