using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class ResellerModel
    {
        public int ResellerId { get; set; }
        public string EncryptedResellerId { get; set; }
        [Required(), StringLength(100)]
        public string ResellerName { get; set; }
        [Required(), StringLength(100), EmailAddress]
        public string ResellerEmail { get; set; }

        [StringLength(100)]
        public string Address { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        public string StateProvince { get; set; }
        public int CountryId { get; set; }
        [StringLength(10)]
        public string ZipCode { get; set; }

        [StringLength(30)]
        public string PrimaryContactNumber { get; set; } // Office?
        [StringLength(30)]
        public string Fax { get; set; }
        [StringLength(30)]
        public string MobileNumber { get; set; }

        public bool NeedsUpdate { get; set; }

        public bool IsActive { get; set; }

        public List<ResellerModel> SubResellers { get; set; }
        public List<MerchantModel> Merchants { get; set; }
        public PartnerModel Partner { get; set; }

        public UserModel User { get; set; }

        [Required()]
        public int PartnerId { get; set; }
    }
}