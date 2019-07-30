using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Merchants")]
    public class Merchant
    {
        public Merchant()
        {
            MobileAppPricePoints = new HashSet<MerchantMobileAppPricePoint>();
            UserPricePoints = new HashSet<MerchantUserPricePoint>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantId { get; set; }

        [StringLength(100), Required()]
        public string MerchantName { get; set; }

        [StringLength(100), Required()]
        public string MerchantEmail { get; set; }

        public int ContactInformationId { get; set; }

        [ForeignKey("ContactInformationId")]
        public virtual ContactInformation ContactInformation { get; set; }

        public Nullable<int> MerchantFeaturesId { get; set; }

        [ForeignKey("MerchantFeaturesId")]
        public virtual MerchantFeatures MerchantFeatures { get; set; }

        public Nullable<int> CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public bool CanCreateSubMerchants { get; set; }

        public Nullable<int> ParentMerchantId { get; set; }

        [ForeignKey("ParentMerchantId")]
        public virtual Merchant ParentMerchant { get; set; }

        public Nullable<int> ResellerId { get; set; }

        [ForeignKey("ResellerId")]
        public virtual Reseller Reseller { get; set; }

        public int PartnerId { get; set; }

        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }

        public Nullable<int> EmailServerId { get; set; }

        [ForeignKey("EmailServerId")]
        public virtual EmailServer Emailserver { get; set; }

        public virtual ICollection<Mid> Mids { get; set; }

        public virtual ICollection<Merchant> SubMerchants { get; set; }

        public virtual ICollection<MerchantBranch> MerchantBranches { get; set; }

        public virtual ICollection<MerchantUserPricePoint> UserPricePoints { get; set; }
        public virtual ICollection<MerchantMobileAppPricePoint> MobileAppPricePoints { get; set; }
        public virtual ICollection<Agreements> Agreements { get; set; }

        public bool NeedAddToCT { get; set; }
        public bool NeedUpdateToCT { get; set; }
    }
}