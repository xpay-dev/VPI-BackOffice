using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.PayOnline
{
    public class TransactionRequest
    {
        public string MerchantId { get; set; }
        public string SecurityKey { get; set; }
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Ip { get; set; }
        public string Email { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiry { get; set; }
        public string Cvv { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public string Issuer { get; set; }
        public string ContentType { get; set; }
    }
}
