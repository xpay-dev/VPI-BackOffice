using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.CTPaymentGateway
{
    public class TransactionEMVData
    {
        public string AccessCode { get; set; }
        public string MerchTxnRef { get; set; }
        public string MerchantId { get; set; }
        public string OrderInfo { get; set; }
        public string Amount { get; set; }
        public string TransNumber { get; set; }

        public string CardNumber { get; set; }
        public string CardExpirationDate { get; set; }
        public string Currency { get; set; }
        public string CSC { get; set; }
        public string SecureHash { get; set; }
        public string Track1 { get; set; }
        public string Track2 { get; set; }
        public string EmvIccData { get; set; }
    }
}
