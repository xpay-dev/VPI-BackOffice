using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.NovaPay
{
    public class TransactionData
    {
        public string AmountFee { get; set; }
        public string Currency { get; set; }
        public string GoodsTitle { get; set; }
        public string IssuingBank { get; set; }
        public string MerchantTradeId { get; set; }
        public string NotifyUrl { get; set; }
        public string paymentCode { get; set; }
        public string PayIp { get; set; }

        public string SignType { get; set; }
        public string SecureHash { get; set; }
        public string TerminalId { get; set; }
        public string MerchantId { get; set; }
        public string PrivateKey { get; set; }

        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string Cvv { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
