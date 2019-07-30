using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("DevicePOSLink")]
    public class DevicePOSLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DevicePOSLinkId { get; set; }

        public int MasterDeviceId { get; set; }

        [ForeignKey("MasterDeviceId")]
        public virtual MasterDevice MasterDevice { get; set; }

        public int MerchantPOSId { get; set; }

        [ForeignKey("MerchantPOSId")]
        public virtual MerchantBranchPOS MerchantBranchPOS { get; set; }

        public DateTime AssignedDate { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}