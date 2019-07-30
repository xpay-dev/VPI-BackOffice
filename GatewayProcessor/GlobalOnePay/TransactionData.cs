using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.GlobalOnePay
{
    public class TransactionData
    {
        public string OrderId { get; set; }
        public string TerminalId { get; set; }
        public string Amount { get; set; }
        public string Datetime { get; set; }
        public string CardNumber { get; set; }
        public string CardType { get; set; }
        public string CardExpiry { get; set; }
        public string CardHolderName { get; set; }
        public string Hash { get; set; }
        public string Currency { get; set; }
        public string TerminalType { get; set; }
        public string Cvv { get; set; }
        public string TransNumber { get; set; }
        public string ReasonToRefund { get; set; }
        public string Operator { get; set; }
    }
}
