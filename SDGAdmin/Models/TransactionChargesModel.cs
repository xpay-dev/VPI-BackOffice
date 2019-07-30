using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class TransactionChargesModel
    {
        public int TransactionChargeId { get; set; }

        public decimal DiscountRate { get; set; }
        public decimal CardNotPresent { get; set; }
        public decimal eCommerce { get; set; }
        public decimal PreAuth { get; set; }
        public decimal Capture { get; set; }
        public decimal Purchased { get; set; }
        public decimal Declined { get; set; }
        public decimal Refund { get; set; }
        public decimal Void { get; set; }

        public decimal CashBack { get; set; }
    }
}