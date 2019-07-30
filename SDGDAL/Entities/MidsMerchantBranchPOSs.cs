using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MidsMerchantBranchPOSs")]
    public class MidsMerchantBranchPOSs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MidId { get; set; }

        [ForeignKey("MidId")]
        public virtual Mid Mid { get; set; }

        public int MerchantBranchPOSId { get; set; }

        [ForeignKey("MerchantBranchPOSId")]
        public virtual MerchantBranchPOS MerchantBranchPOS { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}