using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.CTPaymentGateway
{
    public class MerchantRequest
    {
        public string SequenceNumber { get; set; }
        public string Command { get; set; }
        public string InstitutionNumber { get; set; }
        public string MerchantId { get; set; }
        public string MerchantName { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ContactName { get; set; }

        public string CurrencyCode { get; set; }
        public string AutoSettle { get; set; }
        public string AuthOnly { get; set; }
        public string IsoSurchargeInst { get; set; }
        public string PerVisaTranFee { get; set; }
        public string PerMcTranFee { get; set; }
        public string PerAmexTranFee { get; set; }
        public string PerIdpTranFee { get; set; }
        public string PerAdminTranFee { get; set; }
        public string PerCrDeclineFee { get; set; }
        public string PerDrDeclineFee { get; set; }
        public string FixedTerminalMonthlyFee { get; set; }
        public string FixedMonthlyFee2 { get; set; }
        public string ECommerce { get; set; }
        public string SetLikeMerchantID { get; set; }


        public List<CreditCard> CreditCard { get; set; }
        public List<DebitCard> DebitCard { get; set; }
        public List<Terminal> Terminal { get; set; }
        public List<PinPad> PinPad { get; set; }

    }

    public class MerchRequestandMiscTextRequest
    {
        public Dictionary<string, MerchantRequest> merchRequest { get; set; }
        public string MiscText { get; set; }
    }
}
