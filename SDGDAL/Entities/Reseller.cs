using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Resellers")]
    public class Reseller
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResellerId { get; set; }

        [StringLength(100), Required()]
        public string ResellerName { get; set; }

        [StringLength(100), Required(), EmailAddress]
        public string ResellerEmail { get; set; }

        public int ContactInformationId { get; set; }

        [ForeignKey("ContactInformationId")]
        public virtual ContactInformation ContactInformation { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public bool CanCreateSubResellers { get; set; }

        public int PartnerId { get; set; }
        public Nullable<int> ParentResellerId { get; set; }

        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }

        [ForeignKey("ParentResellerId")]
        public virtual Reseller ParentReseller { get; set; }

        public virtual ICollection<Reseller> SubResellers { get; set; }
        public virtual ICollection<Merchant> Merchants { get; set; }
    }
}