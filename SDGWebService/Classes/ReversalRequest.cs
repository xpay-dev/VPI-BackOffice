using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGWebService.Classes
{
    public class ReversalRequest
    {
        public string AuthorizationID { get; set; }
        public string Message { get; set; }
        public string SystemTraceNumber { get; set; }
        public string RetrievalReferenceNumber { get; set; }
        public string ReturnCode { get; set; }
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
    }
}