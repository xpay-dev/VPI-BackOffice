using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Mids")]
    public class Mid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MidId { get; set; }

        [StringLength(50), Required()]
        public string MidName { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int SwitchId { get; set; }

        [ForeignKey("SwitchId")]
        public virtual Switch Switch { get; set; }

        public int CardTypeId { get; set; }

        [ForeignKey("CardTypeId")]
        public virtual CardType CardType { get; set; }

        public int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        public int MidsPricingId { get; set; }

        //public virtual MidsPricing MidsPricing { get; set; }
        public int TransactionChargesId { get; set; }

        [ForeignKey("TransactionChargesId")]
        public virtual TransactionCharges TransactionCharges { get; set; }

        public int MerchantId { get; set; }

        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }

        [StringLength(50)]
        public string SetLikeMerchantId { get; set; }

        [StringLength(50)]
        public string SetLikeTerminalId { get; set; }

        [StringLength(50)]
        public string Param_1 { get; set; }

        [StringLength(50)]
        public string Param_2 { get; set; }

        public string Param_3 { get; set; }

        [StringLength(50)]
        public string Param_4 { get; set; }

        [StringLength(50)]
        public string Param_5 { get; set; }

        [StringLength(50)]
        public string Param_6 { get; set; }

        [StringLength(50)]
        public string Param_7 { get; set; }

        [StringLength(50)]
        public string Param_8 { get; set; }

        [StringLength(50)]
        public string Param_9 { get; set; }

        [StringLength(50)]
        public string Param_10 { get; set; }

        [StringLength(50)]
        public string Param_11 { get; set; }

        [StringLength(50)]
        public string Param_12 { get; set; }

        [StringLength(50)]
        public string Param_13 { get; set; }

        [StringLength(50)]
        public string Param_14 { get; set; }

        [StringLength(50)]
        public string Param_15 { get; set; }

        [StringLength(50)]
        public string Param_16 { get; set; }

        [StringLength(50)]
        public string Param_17 { get; set; }

        [StringLength(50)]
        public string Param_18 { get; set; }

        [StringLength(50)]
        public string Param_19 { get; set; }

        [StringLength(50)]
        public string Param_20 { get; set; }

        [StringLength(50)]
        public string Param_21 { get; set; }

        [StringLength(50)]
        public string Param_22 { get; set; }

        [StringLength(50)]
        public string Param_23 { get; set; }

        [StringLength(50)]
        public string Param_24 { get; set; }

        public virtual ICollection<MidsMerchantBranches> MerchantBranches { get; set; }
        public virtual ICollection<MidsMerchantBranchPOSs> MerchantBranchPOSs { get; set; }

        public bool NeedAddBulk { get; set; }
        public bool NeedUpdateBulk { get; set; }
        public bool NeedDeleteBulk { get; set; }
        public bool NeedAddTerminal { get; set; }

        public string AcquiringBin { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}