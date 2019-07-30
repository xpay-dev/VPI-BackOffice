using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.PayOnline
{
    public class TransactionResponse
    {
        public string Id { get; set; }
        public string Operation { get; set; }
        public string Result { get; set; }
        public string ErrorCode { get; set; }
        public string Status { get; set; }
    }
}
