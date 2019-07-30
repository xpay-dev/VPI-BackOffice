using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class MerchantPOSModel
    {
        public int POSId { get; set; }
        [Required(), StringLength(100)]
        public string POSName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CurrencyId { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ActivationCode { get; set; }
        public int MerchantBranchId { get; set; }

        public string POSStatus { get; set; }

        public MerchantBranchModel ParentBranch { get; set; }
        public List<MerchantMidModel> MIDs { get; set; }
    }
}