using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTPayment.Classes
{
    public class Terminal
    {
        public string SequenceNumber { get; set; }
        public string Command { get; set; }
        public string InstitutionNumber { get; set; }
        public string MerchantId { get; set; }
        public string TerminalID { get; set; }
        public string SetLikeTerminalID { get; set; }
        public string AddCount { get; set; }
        public string AllowPurchase { get; set; }
        public string AllowVoid { get; set; }
        public string AllowReturns { get; set; }
        public string AllowPreauths { get; set; }
        public string AllowSettle { get; set; }
        public string AllowTotalsandDetails { get; set; }
        public string AutoSettle { get; set; }
        public string SurchargeDebitRetail { get; set; }
        public string SurchargeDebitCashback { get; set; }
        public string SurchargeCreditRetail { get; set; }
        public string SurchargeCreditCashback { get; set; }
        public string RetailSurchargeLimit { get; set; }
        public string CashbackSurchargeLimit { get; set; }
        public string DebitPurchaseLimit { get; set; }
        public string DebitRefundLimit { get; set; }
        public string DebitRefundCount { get; set; }
        public string CreditPurchaseLimit { get; set; }
        public string CreditRefundLimit { get; set; }
        public string CreditRefundCount { get; set; }
        public string EmvFloorVisa { get; set; }
        public string EmvFloorMc { get; set; }
        public string EmvFloorAmex { get; set; }
        public string EmvFloorDisc { get; set; }
        public string ContactlessCvmInterac { get; set; }
        public string ContactlessCvmVisa { get; set; }
        public string ContactlessCvmMc { get; set; }
        public string ContactlessCvmAmex { get; set; }
        public string ContactlessCvmDisc { get; set; }
        public string ContactlessFloorVisa { get; set; }
        public string ContactlessFloorMc { get; set; }
        public string ContactlessFloorAmex { get; set; }
        public string ContactlessFloorDisc { get; set; }
        public string ContactlessTxnInterac { get; set; }
        public string ContactlessTxnVisa { get; set; }
        public string ContactlessTxnMc { get; set; }
        public string ContactlessTxnAmex { get; set; }
        public string ContactlessTxnDisc { get; set; }
        public string ContactlessReceiptInterac { get; set; }
        public string ContactlessReceiptVisa { get; set; }
        public string ContactlessReceiptMc { get; set; }
        public string ContactlessReceiptAmex { get; set; }
        public string ContactlessReceiptDisc { get; set; }
        public string ExtraReceiptDisplay { get; set; }
        public string Ecommerce { get; set; }
    }
}
