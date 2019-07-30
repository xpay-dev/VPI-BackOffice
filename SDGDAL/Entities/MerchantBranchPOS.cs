using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MerchantBranchPOSs")]
    public class MerchantBranchPOS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantPOSId { get; set; }

        [StringLength(50), Required()]
        public string MerchantPOSName { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public int MerchantBranchId { get; set; }

        [ForeignKey("MerchantBranchId")]
        public virtual MerchantBranch MerchantBranch { get; set; }

        public virtual MidsMerchantBranchPOSs MidsMerchantBranchPOSs { get; set; }

        public virtual ICollection<MobileApp> MobileApp { get; set; }
        public virtual ICollection<MidsMerchantBranchPOSs> Mids { get; set; }
    }
}