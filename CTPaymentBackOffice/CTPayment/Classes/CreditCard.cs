using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTPayment.Classes
{
    public class CreditCard
    {
        public string SequenceNumber { get; set; }
        public string Command { get; set; }
        public string InstitutionNumber { get; set; }
        public string MerchantId { get; set; }
        public string CardType { get; set; }
        public string OriginatorId { get; set; }
        public string DepositMerchantId { get; set; }
        public string MerchantBranchTransitNumber { get; set; }
        public string MerchantBankNumber { get; set; }
        public string MerchantAccountNumber { get; set; }
        public string AuthorizationMerchantId { get; set; }
        public string MerchantCategoryCode { get; set; }
        public string AllowIssuerDebitCardType { get; set; }
    }
}
