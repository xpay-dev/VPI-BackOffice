using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.Worldnet
{
    public class PaymentResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }

        public string UniqueReference { get; set; }
        public string ResponseCode { get; set; }
        public string ApprovalCode { get; set; }
        public DateTime Datetime { get; set; }
        public string CvvResponse { get; set; }
        public string Hash { get; set; }
    }
}
