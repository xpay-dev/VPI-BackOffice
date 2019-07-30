using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class MerchantBranchModel
    {
        public int MerchantBranchId { get; set; }
        [Required(), StringLength(100)]
        public string BranchName { get; set; }
        public string LogoUrl { get; set; }

        [StringLength(100), Required()]
        public string Address { get; set; }
        [StringLength(100), Required()]
        public string City { get; set; }
        public string StateProvince { get; set; }
        [Required()]
        public int CountryId { get; set; }
        [Required(), StringLength(10)]
        public string ZipCode { get; set; }

        [StringLength(30), Required()]
        public string PrimaryContactNumber { get; set; } // Office?
        [StringLength(30)]
        public string Fax { get; set; }
        [StringLength(30)]
        public string MobileNumber { get; set; }

        public int MerchantId { get; set; }

        public bool IsActive { get; set; }

        public bool NeedsUpdate { get; set; }

        public List<MerchantPOSModel> POS { get; set; }

        public MerchantPOSModel BranchPOS { get; set; }

        public MerchantModel ParentMerchant { get; set; }

        public UserModel User { get; set; }
    }
}