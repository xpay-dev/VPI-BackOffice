using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class MidsMerchantBranchesPOSsModel
    {
        public int MidId { get; set; }
        public int MerchantBranchPOSsId { get; set; }
        public string MidName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Currency { get; set; }
    }
}