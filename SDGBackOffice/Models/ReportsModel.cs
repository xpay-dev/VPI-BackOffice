using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGBackOffice.Models
{
    public class ReportsModel
    {
        public int TransactionId { get; set; }
        public string CardType { get; set; }
        public int TransactionTypeId { get; set; }
        public int TotalTransaction { get; set; }
        public string Currency { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionEntryTypeId { get; set; }
    }
}