using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("ContactInformation")]
    public class ContactInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactInformationId { get; set; }

        [StringLength(100), Required()]
        public string Address { get; set; }

        [StringLength(100), Required()]
        public string City { get; set; }

        public string StateProvince { get; set; }
        //[Required()]

        public string ProvIsoCode { get; set; }

        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

        [StringLength(10), Required()]
        public string ZipCode { get; set; }

        [StringLength(30), Required()]
        public string PrimaryContactNumber { get; set; } // Office?

        [StringLength(30)]
        public string Fax { get; set; }

        [StringLength(30)]
        public string MobileNumber { get; set; }

        public bool NeedsUpdate { get; set; }
    }
}