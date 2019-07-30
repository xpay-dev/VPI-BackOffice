using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class POSFeaturesResponse
    {
        [DataMember]
        public POSWSResponse POSWSResponse { get; set; }

        [DataMember]
        public string SystemMode { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public bool GPSEnabled { get; set; }

        [DataMember]
        public bool SMSEnabled { get; set; }

        [DataMember]
        public bool GiftAllowed { get; set; }

        [DataMember]
        public bool EmailAllowed { get; set; }

        [DataMember]
        public int EmailLimit { get; set; }

        [DataMember]
        public bool CheckForEmailDuplicates { get; set; }

        [DataMember]
        public int BillingCyclesCheckEmail { get; set; }

        [DataMember]
        public bool PrintAllowed { get; set; }

        [DataMember]
        public int PrintLimit { get; set; }

        [DataMember]
        public bool CheckForPrintDuplicates { get; set; }

        [DataMember]
        public int BillingCyclesCheckPrint { get; set; }

        [DataMember]
        public bool ReferenceNumber { get; set; }

        [DataMember]
        public bool CreditSignature { get; set; }

        [DataMember]
        public bool DebitSignature { get; set; }

        [DataMember]
        public bool ChequeSignature { get; set; }

        [DataMember]
        public bool CashSignature { get; set; }

        [DataMember]
        public bool CreditTransaction { get; set; }

        [DataMember]
        public bool DebitTransaction { get; set; }

        [DataMember]
        public bool ChequeTransaction { get; set; }

        [DataMember]
        public bool CashTransaction { get; set; }

        [DataMember]
        public bool DebitBalanceCheck { get; set; }

        [DataMember]
        public bool BillsPayment { get; set; }

        [DataMember]
        public bool ChequeProofId { get; set; }

        [DataMember]
        public bool ChequeType { get; set; }

        [DataMember]
        public bool DebitRefund { get; set; }

        [DataMember]
        public bool TOSRequired { get; set; }

        [DataMember]
        public bool DisclaimerRequired { get; set; }

        [DataMember]
        public bool TipsEnabled { get; set; }

        [DataMember]
        public decimal Amount1 { get; set; }

        [DataMember]
        public decimal Amount2 { get; set; }

        [DataMember]
        public decimal Amount3 { get; set; }

        [DataMember]
        public decimal Percentage1 { get; set; }

        [DataMember]
        public decimal Percentage2 { get; set; }

        [DataMember]
        public decimal Percentage3 { get; set; }

        [DataMember]
        public string HelpOpenHour { get; set; }

        [DataMember]
        public string HelpClosedHour { get; set; }

        [DataMember]
        public string HelpGMT { get; set; }

        [DataMember]
        public string HelpContactNumber { get; set; }

        [DataMember]
        public string Disclaimer { get; set; }

        [DataMember]
        public string TermsOfService { get; set; }

        [DataMember]
        public MerchantDetails MerchantDetails { get; set; }

        [DataMember]
        public List<Devices> Devices { get; set; }

        [DataMember]
        public List<Countries> Countries { get; set; }
    }
}