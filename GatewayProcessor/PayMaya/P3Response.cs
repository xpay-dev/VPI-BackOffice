using System.Collections.Generic;

namespace GatewayProcessor.PayMaya {
     public class P3Response {
          public string paymentTransactionReferenceNo { get; set; }
          public string status { get; set; }
          public string createdAt { get; set; }
          public P3Receipt receipt { get; set; }
          public P3Payer payer { get; set; }
          public P3Amount amount { get; set; }
     }
     public class P3Receipt { 
          public string transactionId { get; set; }
          public string approvalCode { get; set; }
          public string batch { get; set; }
          public string receiptNo { get; set; }
     }
     public class P3ErrorLog {
          public string logref { get; set; }
          public string message { get; set; }
          public string code { get; set; }
          public List<P3ErrorLinks> links { get; set; }
     }
     public class P3ErrorLinks { 
          public string rel { get; set; }
          public string href { get; set; }
     }
}
