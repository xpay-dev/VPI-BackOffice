using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MasterDevices")]
    public class MasterDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MasterDeviceId { get; set; }

        public int DeviceFlowTypeId { get; set; }

        [ForeignKey("DeviceFlowTypeId")]
        public virtual DeviceFlowType FlowType { get; set; }

        public int DeviceTypeId { get; set; }

        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType DeviceType { get; set; }

        [StringLength(100), Required()]
        public string DeviceName { get; set; }

        [StringLength(100), Required()]
        public string Manufacturer { get; set; }

        [StringLength(30), Required()]
        public string Warranty { get; set; }

        public string ExternalData { get; set; }

        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}