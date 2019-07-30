using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTPayment.Classes
{
    public class CreditCardRequest
    {
        public string MiscText { get; set; }
        public List<CreditCard> CreditCard { get; set; }
    }
}
