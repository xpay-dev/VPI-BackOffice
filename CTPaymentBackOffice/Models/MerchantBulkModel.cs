using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTPaymentBackOffice.Models
{
    public class MerchantBulkModel
    {
        public int Id { get; set; }
        public string Mid { get; set; }
        public string TerminalId { get; set; }
        public string MerchantName { get; set; }
    }
}