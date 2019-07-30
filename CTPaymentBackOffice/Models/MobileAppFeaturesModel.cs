using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTPaymentBackOffice.Models
{
    public class MobileAppFeaturesModel
    {
        public int MobileAppFeaturesId { get; set; }
        public int MerchantPosId { get; set; }
        public string SystemMode { get; set; }
        public int CurrencyId { get; set; }
        public bool GPSEnabled { get; set; }
        public bool SMSEnabled { get; set; }

        public bool EmailAllowed { get; set; }
        public int EmailLimit { get; set; }

        public bool CheckForEmailDuplicates { get; set; }
        public int BillingCyclesCheckEmail { get; set; }

        public bool PrintAllowed { get; set; }
        public int PrintLimit { get; set; }

        public bool CheckForPrintDuplicates { get; set; }
        public int BillingCyclesCheckPrint { get; set; }

        public bool ReferenceNumber { get; set; }
        public bool CreditSignature { get; set; }
        public bool DebitSignature { get; set; }
        public bool ChequeSignature { get; set; }
        public bool CashSignature { get; set; }
        public bool CreditTransaction { get; set; }
        public bool DebitTransaction { get; set; }
        public bool ChequeTransaction { get; set; }
        public bool CashTransaction { get; set; }
        public bool ProofId { get; set; }
        public bool ChequeType { get; set; }
        public bool DebitRefund { get; set; }
        public bool TOSRequired { get; set; }
        public bool DisclaimerRequired { get; set; }
        public decimal Percentage1 { get; set; }
        public decimal Percentage2 { get; set; }
        public decimal Percentage3 { get; set; }
        public bool TipsEnabled { get; set; }
        public decimal Amount1 { get; set; }
        public decimal Amount2 { get; set; }
        public decimal Amount3 { get; set; }
        public string LanguageCode { get; set; }
    }
}