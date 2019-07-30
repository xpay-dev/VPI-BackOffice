using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CTPaymentBackOffice.Models
{
    public class PartnerModel
    {
        public int PartnerId { get; set; }
        [Required(), StringLength(100)]
        public string CompanyName { get; set; }
        [Required(), StringLength(100), EmailAddress]
        public string CompanyEmail { get; set; }
        public string LogoUrl { get; set; }

        public decimal MDR { get; set; }

        [StringLength(100), Required()]
        public string Address { get; set; }
        [StringLength(100), Required()]
        public string City { get; set; }
        public string StateProvince { get; set; }

        public int CountryId { get; set; }
        [StringLength(10), Required()]
        public string ZipCode { get; set; }

        [StringLength(30),Required()]
        public string PrimaryContactNumber { get; set; } // Office?
        [StringLength(30)]
        public string Fax { get; set; }
        [StringLength(30)]
        public string MobileNumber { get; set; }

        public bool NeedsUpdate { get; set; }

        public bool IsActive { get; set; }

        public List<ResellerModel> Resellers { get; set; }

        public List<PartnerModel> SubPartners { get; set; }

        public PartnerModel ParentPartner { get; set; }

        public UserModel User { get; set; }
    }
}