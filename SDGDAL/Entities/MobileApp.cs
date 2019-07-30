using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MobileApps")]
    public class MobileApp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MobileAppId { get; set; }

        public int MerchantBranchPOSId { get; set; }

        [ForeignKey("MerchantBranchPOSId")]
        public virtual MerchantBranchPOS MerchantBranchPOS { get; set; }

        public Nullable<int> MobileDeviceInfoId { get; set; }

        [ForeignKey("MobileDeviceInfoId")]
        public virtual MobileDeviceInfo MobileDeviceInfo { get; set; }

        public Nullable<int> MobileAppFeaturesId { get; set; }

        [ForeignKey("MobileAppFeaturesId")]
        public virtual MobileAppFeatures MobileAppFeatures { get; set; }

        [StringLength(30), Required()]
        public string ActivationCode { get; set; }

        public DateTime DateCreated { get; set; }
        public Nullable<DateTime> DateActivated { get; set; }
        public DateTime ExpirationDate { get; set; }

        public bool UpdatePending { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool TOS_Acknowledged { get; set; }

        public decimal GPSLat { get; set; }
        public decimal GPSLong { get; set; }

        public virtual ICollection<MobileAppLog> MobileAppLogs { get; set; }
    }
}