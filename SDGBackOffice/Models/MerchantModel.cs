using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGBackOffice.Models
{
    public class MerchantModel
    {
        public int MerchantId { get; set; }
        [Required(), StringLength(100)]
        public string MerchantName { get; set; }
        [StringLength(100)]
        public string MerchantEmail { get; set; }

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

        public int ProvinceId { get; set; }
        public string ProvIsoCode { get; set; }

        [StringLength(10)]
        public string PIN { get; set; }
        public int BillingCycleId { get; set; }
        public bool IsActive { get; set; }

        public bool NeedsUpdate { get; set; }

        public decimal PrinterReceiptsPrice { get; set; }
        public decimal GiftReceiptsPrice { get; set; }
        public decimal EmailReceiptsPrice { get; set; }
        public int CurrencyId { get; set; }
        public int MerchantFeaturesId { get; set; }

        public MerchantModel ParentMerchant { get; set; }
        public ResellerModel Reseller { get; set; }
        public PartnerModel Partner { get; set; }

        public UserModel User { get; set; }
        public List<MerchantModel> SubMerchants { get; set; }

        [Required()]
        public int ResellerId { get; set; }

        public List<MobileAppPricePointModel> MobileAppPricePoints { get; set; }
        public List<UserPricePointModel> UserPricePoints { get; set; }

        public MobileAppPricePointModel InitialMobileAppPricePoint { get; set; }
        public UserPricePointModel InitialUserPricePoint { get; set; }

        public bool NeedAddToCT { get; set; }
        public bool NeedUpdateToCT { get; set; }
    }
}