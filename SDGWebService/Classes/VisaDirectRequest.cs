using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGWebService.Classes
{
    public class VisaDirectRequest
    {
        public string merchantCategoryCode;
        public int acquirerCountryCode { get; set; }
        public int acquiringBin { get; set; }
        public decimal amount { get; set; }
        public string businessApplicationId { get; set; }
        public CardAcceptor cardAcceptor { get; set; }
        public string feeProgramIndicator { get; set; }
        public string localTransactionDateTime { get; set; }
        public PurchaseIdentifier purchaseIdentifier { get; set; }
        public string recipientName { get; set; }
        public string recipientPrimaryAccountNumber { get; set; }
        public string retrievalReferenceNumber { get; set; }
        public string secondaryId { get; set; }
        public string senderAccountNumber { get; set; }
        public string senderName { get; set; }
        public string senderReference { get; set; }
        public string systemsTraceAuditNumber { get; set; }
        public string transactionCurrencyCode { get; set; }
        public string transactionIdentifier { get; set; }
    }
}