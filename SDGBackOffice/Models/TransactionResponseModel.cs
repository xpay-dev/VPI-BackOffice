using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGBackOffice.Models
{
    public class TransactionResponseModel
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string AuthNumber { get; set; }

        public string OrderNumber { get; set; }

        public string SequenceNumber { get; set; }

        public string BatchNumber { get; set; }

        public string Timestamp { get; set; }

        public string TransactionNumber { get; set; }

        public string CardType { get; set; }

        public string TransactionType { get; set; }

        public string TransactionEntryType { get; set; }

        public string CardNumber { get; set; }

        public string Total { get; set; }

        public string TraceNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string TerminalId { get; set; }

        public string MerchantId { get; set; }
    }
}