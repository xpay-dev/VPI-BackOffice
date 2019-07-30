using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.Debit
{
    public class DebitDetails
    {
        #region Card Details
        public string CardNumber { get; set; }
        public string Amount { get; set; }
        public string NameOnCard { get; set; }
        public string Track2Data { get; set; }
        public string Track1Data { get; set; }
        public string ExpirationDate { get; set; }
        public string MerchantID { get; set; }
        public string TerminalID { get; set; }
        #endregion

        #region Fee
        public string AmountTransactionProcessingFee { get; set; }
        public string AmountSettlementProcessingFee { get; set; }
        public string AmountSettlementFee { get; set; }
        public string AmountTransactionFee { get; set; }
        #endregion

        public string SystemTraceAudit { get; set; }
        public string PrivateAdditionalData { get; set; }
        public string AdditionalData { get; set; }
        public string MessageAuthorizationCode { get; set; }
        public string Invoice { get; set; }
        public string PinBlock { get; set; }
        public string ChipCardData { get; set; }
        public string AuthorizationIDResponse { get; set; }
        public string RetrievalReferenceNumber { get; set; }
        public string ResponseCode { get; set; }
        public string AccountType { get; set; }
        public string ForwardingInstitutionCode { get; set; }
        public string CurrencyCode { get; set; }
    }
}
