using CTPayment.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTPayment
{
    public class Request
    {
        public string MiscText { get; set; }
        public MerchantRequest MerchantRequest { get; set; }
    }
}
