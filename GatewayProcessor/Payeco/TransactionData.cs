using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.Payeco
{
    public class TransactionData
    {
        public string ProcCode { get; set; }
        public string Amount { get; set; }
        public string Phone { get; set; }
        public string Card { get; set; }
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public string Currency { get; set; }
        public string Beneficiary { get; set; }
        public string MerchantName { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string IdCard { get; set; }
        public string Name { get; set; }
        public string IdCardType { get; set; }
        public string BankAddress { get; set; }
        public string BankName { get; set; }
        public string Version { get; set; }
        public string ProcessCode { get; set; }
        public string MerchantPassword { get; set; }
        public string SynAddress { get; set; }
        public string AsynAddress { get; set; }
        public string MerchantOrderNo { get; set; }
        public string AcqSsn { get; set; }
        public string Datetime { get; set; }
        public string TransData { get; set; }
        public string BeneficiaryMobileNo { get; set; }
        public string DeliveryAddress { get; set; }
        public string IpAddress { get; set; }
        public string OrderNo { get; set; }
        public string RespCode { get; set; }
        public string OrderState { get; set; }
        public string Reference { get; set; }
    }
}
