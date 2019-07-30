using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGBackOffice.Models
{
    public class SDMasterModel
    {
        public ResellerModel Reseller { get; set; }
        public MerchantModel Merchant { get; set; }
        public MerchantBranchModel MerchantBranch { get; set; }
        public MerchantPOSModel MerchantPOS { get; set; }
        public MerchantMidModel MerchantMID { get; set; }
    }
}