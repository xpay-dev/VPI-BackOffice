using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.MasterCard
{
    public class TransactionData
    {
        public string AccessCode { get; set; }
        public string MerchTxnRef { get; set; }
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
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
        public string LRC { get; set; }
        public string EmvIccData { get; set; }

        public string PFI { get; set; }
        public string ISO { get; set; }
        public string SMI { get; set; }
        public string PFN { get; set; }
        public string SMN { get; set; }
        public string MSA { get; set; }
        public string MCI { get; set; }
        public string MST { get; set; }
        public string MCO { get; set; }
        public string MPC { get; set; }
        public string MPP { get; set; }
        public string MCC { get; set; }
    }
}
