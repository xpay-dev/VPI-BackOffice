using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class BranchMIDModel
    {
        #region Branch Model
        public int MerchantBranchId { get; set; }
        [Required(), StringLength(100)]
        public string BranchName { get; set; }
        public string LogoUrl { get; set; }

        [StringLength(100), Required()]
        public string Address { get; set; }
        [StringLength(100), Required()]
        public string City { get; set; }
        public string StateProvince { get; set; }
        [Required()]
        public int CountryId { get; set; }
        [Required(), StringLength(10)]
        public string ZipCode { get; set; }

        [StringLength(30), Required()]
        public string PrimaryContactNumber { get; set; } // Office?
        [StringLength(30)]
        public string Fax { get; set; }
        [StringLength(30)]
        public string MobileNumber { get; set; }

        public int MerchantId { get; set; }

        public bool IsActive { get; set; }

        public bool NeedsUpdate { get; set; }

        public List<MerchantPOSModel> POS { get; set; }

        public MerchantPOSModel BranchPOS { get; set; }

        public MerchantModel ParentMerchant { get; set; }

        public UserModel User { get; set; }

        #endregion

        #region MIDModel
        [Required(), StringLength(100)]
        public string MIDName { get; set; }

        public string MerchantName { get; set; }

        [Required()]
        public int MidMerchantId { get; set; }

        public bool MidIsActive { get; set; }
        public bool MidIsDeleted { get; set; }
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
        public string Param_11 { get; set; }
        public string Param_12 { get; set; }

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

        public bool NeedAddBulk { get; set; }
        public bool NeedUpdateBulk { get; set; }
        public bool NeedDeleteBulk { get; set; }
        #endregion

    }
}