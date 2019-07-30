using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.TrustPay
{
    public class TransactionResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string TransactionID { get; set; }
        public string merNo { get; set; }
        public string gatewayNo { get; set; }
        public string orderAmount { get; set; }
        public string orderCurrency { get; set; }
        public string orderInfo { get; set; }
        public string orderNo { get; set; }
        public string orderStatus { get; set; }
        public string remark { get; set; }
        public string signInfo { get; set; }
        public string tradeNo { get; set; }
        public string responseCode { get; set; }
        public string description { get; set; }
        public string batchNo { get; set; }
    }
}
