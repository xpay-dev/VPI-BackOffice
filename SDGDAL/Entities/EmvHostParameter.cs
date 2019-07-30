using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("EmvHostParameters")]
    public class EmvHostParameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmvHostParameterId { get; set; }

        public string TransResponseTimer { get; set; }
        public string DialActTime { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string IPAddress { get; set; }
        public string SecondaryPhoneNumber { get; set; }
        public string SecondaryIPAddress { get; set; }
        public string MerchantName { get; set; }
        public string MerchantAddress { get; set; }
        public string MerchantCityProv { get; set; }
        public string TerminalAppSoftwareVer { get; set; }
        public string HostDateTime { get; set; }
        public string ExtraReceiptDisplay { get; set; }
        public string DiscoverCLCvmLimit { get; set; }
        public string VisaCLCvmLimit { get; set; }
        public string MasterCardCLCvmLimit { get; set; }
        public string MerchantTypeIndicator { get; set; }
        public string Future1 { get; set; }
        public string DiscoverEVMFloorLimit { get; set; }
        public string VISAEMVFloorLimit { get; set; }
        public string MasterCardEMVFloorLimit { get; set; }
        public string AmexJCBEMVFloorLimit { get; set; }
        public string Future2 { get; set; }
        public string IDPRetailSubcharge { get; set; }
        public string IDPCashbackSubcharge { get; set; }
        public string CCRetailSubcharge { get; set; }
        public string CCCashbackSubcharge { get; set; }
        public string RetailSubchargeLimit { get; set; }
        public string CashbackSubchargeLimit { get; set; }
        public string VisaDebitSupport { get; set; }
        public string AmexJCBCLCVMLimit { get; set; }
        public string InteracCLReceiptReqLimit { get; set; }
        public string PredialSetting { get; set; }
        public string DiscoverCLFloorLimit { get; set; }
        public string VisaCLFloorLimit { get; set; }
        public string MasterCardCLFloorLimit { get; set; }
        public string AmexJCBCLFloorLimit { get; set; }
        public string DiscoverCLTxnLimit { get; set; }
        public string VisaCLTxnLimit { get; set; }
        public string MasterCardCLTxnLimit { get; set; }
        public string AmexJCBCLTxnLimit { get; set; }
        public string TerminalTransInfo { get; set; }
        public string TerminalOptionStat { get; set; }
        public string Future3 { get; set; }

        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
    }
}