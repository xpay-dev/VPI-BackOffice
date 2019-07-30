using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Partners")]
    public class Partner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PartnerId { get; set; }

        [StringLength(100), Required()]
        public string CompanyName { get; set; }

        public string LogoUrl { get; set; }

        [StringLength(100), Required(), EmailAddress]
        public string CompanyEmail { get; set; }

        public decimal MerchantDiscountRate { get; set; }

        public int ContactInformationId { get; set; }

        [ForeignKey("ContactInformationId")]
        public virtual ContactInformation ContactInformation { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public bool CanCreateSubPartners { get; set; }

        public Nullable<int> ParentPartnerId { get; set; }

        [ForeignKey("ParentPartnerId")]
        public virtual Partner ParentPartner { get; set; }

        public virtual ICollection<Partner> SubPartners { get; set; }
        public virtual ICollection<Reseller> Resellers { get; set; }
    }
}