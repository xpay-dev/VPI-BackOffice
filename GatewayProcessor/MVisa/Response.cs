using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.MVisa
{
    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string transactionIdentifier { get; set; }
        public string actionCode { get; set; }
        public string approvalCode { get; set; }
        public string responseCode { get; set; }
        public string transmissionDateTime { get; set; }
        public string retrievalReferenceNumber { get; set; }
        public string feeProgramIndicator { get; set; }
        public string merchantCategoryCode { get; set; }
        public string merchantVerificationValue { get; set; }
        public CardAcceptor cardAcceptor { get; set; }
        public PurchaseIdentifier purchaseIdentifier { get; set; }
    }
}
