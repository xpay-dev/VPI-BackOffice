using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MerchantFeatures")]
    public class MerchantFeatures
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantFeaturesId { get; set; }

        [Required()]
        public int BillingCycleId { get; set; }

        [ForeignKey("BillingCycleId")]
        public virtual BillingCycle BillingCycle { get; set; }

        public Nullable<int> CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        [StringLength(50)]
        public string LanguageCode { get; set; }

        public bool TermsOfServiceEnabled { get; set; }
        public bool DisclaimerEnabled { get; set; }
        public bool UseDefaultAgreements { get; set; }
    }
}