using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.CTPaymentGateway
{
    public class DebitCard
    {
        public string SequenceNumber { get; set; }
        public string Command { get; set; }
        public string InstitutionNumber { get; set; }
        public string MerchantId { get; set; }
        public string CardType { get; set; }
        public string MerchantBranchTransitNumber { get; set; }
        public string MerchantBankNumber { get; set; }
        public string MerchantAccountNumber { get; set; }
        public string MerchantCategoryCode { get; set; }
        public string HighRiskDollarAmount { get; set; }
        public string HighRiskTransactionCount { get; set; }
    }
}
