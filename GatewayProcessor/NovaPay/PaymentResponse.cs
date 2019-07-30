using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.NovaPay
{
    public class PaymentResponse
    {
        public string errorCode { get; set; }
        public string errorMsg { get; set; }
        public string inputCharset { get; set; }
        public string merchantId { get; set; }
        public string merchantTradeId { get; set; }
        public string paymentId { get; set; }
        public string sign { get; set; }
    }
}
