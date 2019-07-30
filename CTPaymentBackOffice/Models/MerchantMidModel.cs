using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CTPaymentBackOffice.Models
{
    public class MerchantMidModel
    {
        public MerchantModel Merchant { get; set; }
        public MerchantBranchModel MerchantBranch { get; set; }
        public MerchantPOSModel MerchantBranchPOS { get; set; }
        public int MIDId { get; set; }

        [Required(), StringLength(100)]
        public string MIDName { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Switch { get; set; }
        public string CardType { get; set; }
        public string Currency { get; set; }
        public int SwitchId { get; set; }
        public int CardTypeId { get; set; }
        public int CurrencyId { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal DailyLimit { get; set; }

        //MID Rates
        public decimal MR_BatchRate { get; set; }
        public string MR_BatchCloseMethod { get; set; }
        public int MR_BatchTimeHour { get; set; }
        public int MR_BatchTimeMin { get; set; }
        public int MR_BatchTimeSec { get; set; }
        public int MR_BatchTimeMil { get; set; }
        public decimal MR_Capture { get; set; }
        public decimal MR_CardNotPresent { get; set; }
        public decimal MR_ChargeBack { get; set; }
        public decimal MR_Void { get; set; }
        public decimal MR_Discount { get; set; }
        public decimal MR_eCommerce { get; set; }
        public decimal MR_PreAuth { get; set; }
        public decimal MR_Refund { get; set; }
        public decimal MR_Settlement { get; set; }
        public decimal MR_Purchased { get; set; }
        public decimal MR_CashBack { get; set; }
        public decimal MR_BalanceInquiry { get; set; }

        public string Param_1 { get; set; }
        public string Param_2 { get; set; }
        public string Param_3 { get; set; }
        public string Param_4 { get; set; }
        public string Param_5 { get; set; }
        public string Param_6 { get; set; }
        public string Param_7 { get; set; }
        public string Param_8 { get; set; }
        public string Param_9 { get; set; }
        public string Param_10 { get; set; }

        //Transaction Charges
        public int TransactionChargeId { get; set; }
        public decimal TC_Capture { get; set; }
        public decimal TC_CardNotPresent { get; set; }
        public decimal TC_Discount { get; set; }
        public decimal TC_Void { get; set; }
        public decimal TC_Decline { get; set; }
        public decimal TC_eCommerce { get; set; }
        public decimal TC_PreAuth { get; set; }
        public decimal TC_Refund { get; set; }
        public decimal TC_Purchased { get; set; }
        public decimal TC_CashBack { get; set; }
        public decimal TC_BalanceInquiry { get; set; }

        public bool NeedsUpdate { get; set; }
    }
}