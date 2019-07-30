using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class TransactionModel
    {
        [StringLength(50), Required()]
        public string Amount { get; set; }
        public int AccountId { get; set; }
        public int MobileAppId { get; set; }
        public int MerchantPosId { get; set; }
        public int CardTypeId { get; set; }
        public int MidId { get; set; }
        public int TransactionChargesId { get; set; }
        public string Currency { get; set; }
        public string ErrNumber { get; set; }
        public string ErrMessage { get; set; }
    }
}