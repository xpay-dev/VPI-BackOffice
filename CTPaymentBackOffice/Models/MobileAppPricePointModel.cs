using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CTPaymentBackOffice.Models
{
    public class MobileAppPricePointModel
    {
        public int PricePointId { get; set; }

        [Required()]
        public int PricePointStart { get; set; }
        [Required(), CustomAttributes.GreaterThan(OtherProperty = "PricePointStart")] 
        public int PricePointEnd { get; set; }
        [Required()]
        public decimal Price { get; set; }

        public int MerchantId { get; set; }
        public int CurrencyId { get; set; }
    }
}