using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.TrustPay
{
    public class Transaction
    {
        public string MerchantID { get; set; }
        public string TransactionID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string cardNo { get; set; }
        public string cardExpireMonth { get; set; }
        public string cardExpireYear { get; set; }
        public string cardSecurityCode { get; set; }
        public string issuingBank { get; set; }
        public string email { get; set; }
        public string ip { get; set; }
        public string phone { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string zip { get; set; }
        public string signInfo { get; set; }
        public string returnUrl { get; set; }
        public string remark { get; set; }
        public string csid { get; set; }
        public string orderNo { get; set; }
        public string orderAmount { get; set; }
        public string orderCurrency { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string MerNo { get; set; }
        public string Gatewayno { get; set; }
        public string signKey { get; set; }
        public string ResponseStatus { get; set; }

        public string tradeAmount { get; set; }
        public string refundType { get; set; }
        public string refundReason { get; set; }
        public string batchNo { get; set; }
        public string tradeNo { get; set; }
    }
}
