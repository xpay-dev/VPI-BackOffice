using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Devices")]
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceId { get; set; }

        public int MasterDeviceId { get; set; }

        [ForeignKey("MasterDeviceId")]
        public virtual MasterDevice MasterDevice { get; set; }

        [StringLength(100), Required()]
        public string SerialNumber { get; set; }

        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBlackListed { get; set; }
        public int KeyInjectedId { get; set; }
    }
}