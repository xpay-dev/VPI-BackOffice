using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTPayment.Classes
{
    public class DebitCardRequest
    {
        public string MiscText { get; set; }
        public List<DebitCard> DebitCard { get; set; }
    }
}
