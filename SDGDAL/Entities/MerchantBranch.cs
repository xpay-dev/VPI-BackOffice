using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MerchantBranches")]
    public class MerchantBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantBranchId { get; set; }

        [StringLength(100), Required()]
        public string MerchantBranchName { get; set; }

        public int ContactInformationId { get; set; }

        [ForeignKey("ContactInformationId")]
        public virtual ContactInformation ContactInformation { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public int MerchantId { get; set; }

        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }

        public virtual ICollection<MerchantBranchPOS> MerchantPOSs { get; set; }
        public virtual ICollection<MidsMerchantBranches> Mids { get; set; }
    }
}