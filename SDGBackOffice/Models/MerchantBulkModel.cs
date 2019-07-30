using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGBackOffice.Models
{
    public class MerchantBulkModel
    {
        public string MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string ResellerName { get; set; }
        public string PartnerName { get; set; }
        public string SwitchName { get; set; }
        public string Command { get; set; }
        public string MID { get; set; }
        public string MerchantInfo  { get; set; }
        public string PhoneNumber { get; set; }
        public bool NeedUpdateMerchant { get; set; }
        public string[] CreditCard { get; set; }
        public string[] DebitCard { get; set; }
    }
}