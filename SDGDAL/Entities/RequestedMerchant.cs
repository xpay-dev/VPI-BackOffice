using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("RequestedMerchants")]
    public class RequestedMerchant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestedMerchantId { get; set; }

        [StringLength(100), Required()]
        public string MerchantName { get; set; }

        [StringLength(100), Required()]
        public string MerchantEmail { get; set; }

        public int ParentId { get; set; }

        [StringLength(100), Required()]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [StringLength(100), Required()]
        public string LastName { get; set; }

        [StringLength(100), Required()]
        public string Address { get; set; }

        [StringLength(100), Required()]
        public string City { get; set; }

        public string StateProvince { get; set; }

        public int CountryId { get; set; }

        [StringLength(10), Required()]
        public string ZipCode { get; set; }

        [StringLength(30), Required()]
        public string PrimaryContactNumber { get; set; } // Office?

        [StringLength(30)]
        public string Fax { get; set; }

        [StringLength(30)]
        public string MobileNumber { get; set; }

        public int ProvinceId { get; set; }
        public string ProvIsoCode { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }

        //MID Info
        public string MID { get; set; }

        public int CardTypeId { get; set; }
        public string Currency { get; set; }
        public string SecureHash { get; set; }
        public string AccessCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string RequestNote { get; set; }
    }
}