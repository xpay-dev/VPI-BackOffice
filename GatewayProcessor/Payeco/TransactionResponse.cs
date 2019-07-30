using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.Payeco
{
    public class TransactionResponse
    {
        public string AcqSsn { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string MAC { get; set; }
        public string MerchantNo { get; set; }
        public string MerchantOrderNo { get; set; }
        public string OrderNo { get; set; }
        public string OrderState { get; set; }
        public string OrderType { get; set; }
        public string ProcCode { get; set; }
        public string ProcessCode { get; set; }
        public string Remark { get; set; }
        public string ResponseCode { get; set; }
        public string DateTime { get; set; }
        public string Status { get; set; }
    }
}
