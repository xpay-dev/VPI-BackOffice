using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class CTPaymentUser
    {
        public string CompanyNumber { get; set; }
        public string MerchantNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string LanguageCode { get; set; }
        public string OperatorId { get; set; }
    }
}