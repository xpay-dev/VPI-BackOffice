using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGWebService.Classes
{
    public class Request
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string EncCardDetails { get; set; }
        public string Currency { get; set; }
        public string RefNumApp { get; set; }
        public POSWSRequest POSWSRequest { get; set; }
    }
}