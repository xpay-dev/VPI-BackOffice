using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SDGWebService.Classes
{
    [DataContract]
    public class DebitBalanceInquiryResponse
    {
        [DataMember]
        public string MerchantId { get; set; }

        [DataMember]
        public string TerminalId { get; set; }

        [DataMember]
        public string Date { get; set; }

        [DataMember]
        public string AuthNumber { get; set; }

        [DataMember]
        public string Amount { get; set; }

        [DataMember]
        public string AccountType { get; set; }

        [DataMember]
        public string TransactionType { get; set; }

        [DataMember]
        public string TransactionEntryType { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string CardName { get; set; }

        [DataMember]
        public string TraceNumber { get; set; }

        [DataMember]
        public string RetrievalRefNumber { get; set; }

        [DataMember]
        public string CurrencyCurrentBalance { get; set; }

        [DataMember]
        public string CurrentBalance { get; set; }

        [DataMember]
        public string CurrencyAvailableBalance { get; set; }

        [DataMember]
        public string AvailableBalance { get; set; }

        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }
    }
}