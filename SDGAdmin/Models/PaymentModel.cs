using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class PaymentModel
    {
        public int PaymentType { get; set; }
        public long CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public int CVV { get; set; }
        public string CardHolderName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int PostCode { get; set; }
    }
}