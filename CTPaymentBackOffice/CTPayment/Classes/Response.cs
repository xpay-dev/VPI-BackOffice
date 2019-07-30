using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTPayment.Classes
{
    public class CardResponse
    {
        public string SequenceNumber { get; set; }
        public string InstitutionNumber { get; set; }
        public string Command { get; set; }
        public string MerchantId { get; set; }
        public string CardType { get; set; }
        public string ResultCode { get; set; }
        public string ResultText { get; set; }
    }
    public class StatsResponse
    {
        public string SubmittedRecordType { get; set; }
        public string RecordsRead { get; set; }
        public string Added { get; set; }
        public string AddFailed { get; set; }
        public string Updated { get; set; }
        public string UpdatedFailed { get; set; }
        public string Deleted { get; set; }
        public string DeletedFailed { get; set; }
    }
    public class HeaderResponse
    {
        public string SequenceNumber { get; set; }
        public string Version { get; set; }
        public string InstitutionNumber { get; set; }
        public string LoadDate { get; set; }
        public string LoadTime { get; set; }
        public string MiscText { get; set; }
        public string ResultCode { get; set; }
        public string ResultText { get; set; }
    }
    public class MerchantResponse
    {
        public string SequenceNumber { get; set; }
        public string InstitutionNumber { get; set; }
        public string Command { get; set; }
        public string MerchantId { get; set; }
        public string GatewayClientNumber { get; set; }
        public string GatewayMerchantNumber { get; set; }
        public string ResultCode { get; set; }
        public string ResultText { get; set; }
    }
    public class TerminalResponse
    {
        public string SequenceNumber { get; set; }
        public string InstitutionNumber { get; set; }
        public string Command { get; set; }
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public string ResultCode { get; set; }
        public string ResultText { get; set; }
        public string SoftwareVersion { get; set; }
    }
    public class PinPadResponse
    {
        public string SequenceNumber { get; set; }
        public string InstitutionNumber { get; set; }
        public string Command { get; set; }
        public string PinpadId { get; set; }
        public string ResultCode { get; set; }
        public string ResultText { get; set; }
        public string TerminalId { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentStatusDesc { get; set; }
        public string UpdateType { get; set; }
        public string UpdateTypeDesc { get; set; }
        public string MaintEmplNo { get; set; }
        public string MaintGroupNo { get; set; }
        public string MaintInstNo { get; set; }
        public string MaintEmplName { get; set; }
        public string MaintLastDate { get; set; }
        public string MaintLastTime { get; set; }
        public string ActivationEmplNo { get; set; }
        public string ActivationGroupNo { get; set; }
        public string ActivationInstNo { get; set; }
        public string ActivationEmplName { get; set; }
        public string ActivationDate { get; set; }
        public string ActivationTime { get; set; }
        public string InjectionDate { get; set; }
        public string InjectionTime { get; set; }
        public string DeviceManufacturer { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceOwnerName { get; set; }
        public string DeviceRawSerial { get; set; }
        public string MerchantName { get; set; }
        public string MerchantAddr { get; set; }
        public string MerchantCity { get; set; }
        public string MerchantProv { get; set; }
        public string MerchantPostal { get; set; }
        public string TerminalOwnInst { get; set; }
        public string PinKeyPresent { get; set; }
        public string MacKeyPresent { get; set; }
        public string MacFieldPresent { get; set; }
        public string SoftwareVersion { get; set; }

    }
}
