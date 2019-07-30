using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class MerchantOnBoardModel
    {
        public int MidId { get; set; }
        public int MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string ResellerName { get; set; }
        public string PartnerName { get; set; }
        public string MerchantMids { get; set; }
        public string TerminalId { get; set; }
        public string SetLikeTerminalId { get; set; }
        public string MidCardType { get; set; }
        public string MerchantAddress { get; set; }
        public string Command { get; set; }
        public int? ActionId { get; set; }
        public bool IsActive { get; set; }
        public bool NeedMerchInfoUpdateToCT { get; set; }
        public bool NeedMidsUpdateToCT { get; set; }
    }
}