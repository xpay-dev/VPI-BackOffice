using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.MasterCard
{
    public class ServiceResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }

        // http response from server
        public string Command { get; set; }
        public string MerchTxnRef { get; set; }
        public string MerchantId { get; set; }
        public string OrderInfo { get; set; }
        public decimal Amount { get; set; }
        public string Locale { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionResponseCode { get; set; }
        public string ReceiptNumber { get; set; }
        public string AcqResponseCode { get; set; }
        public string BatchNumber { get; set; }
        public string AuthorizeId { get; set; }
        public string Card { get; set; }
        public int PosEntryMode { get; set; }

        public string AVSResultCode { get; set; }
        public string AcqAVSResponseCode { get; set; }
        public string AcqCSCResponseCode { get; set; }
        public string CSCResultCode { get; set; }
    }
}
