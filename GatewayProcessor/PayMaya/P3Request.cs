using System.Collections.Generic;

namespace GatewayProcessor.PayMaya {
     public class P3Request {
          public P3Merchant merchant { get; set; }
          public P3Payer payer { get; set; }
          public P3Transaction transaction { get; set; }
     }
     public class P3Merchant { 
          public string id { get; set; }
          public P3Metadata metadata { get; set; }
          public P3PaymentFacilitator paymentFacilitator { get; set; }
     }
     public class P3Metadata { 
          public string refNo { get; set; }
     }
     public class P3PaymentFacilitator { 
          public List<P3SubMerchant> subMerchants { get; set; }
     }
     public class P3SubMerchant { 
          public string id { get; set; }
          public string name { get; set; }
          public P3Address address { get; set; }
          public string mcc { get; set; }
          public string contactNo { get; set; }
     }
     public class P3Address {
          public string alphaCountryCode { get; set; }
          public string city { get; set; }
          public string line1 { get; set; }
          public string postalCode { get; set; }
          public string state { get; set; }
     }
     public class P3Payer { 
          public P3FundingInstrument fundingInstrument { get; set; }
     }
     public class P3FundingInstrument { 
          public P3Card card { get; set; }
     }
     public class P3Card {
          public string cardNumber { get; set; }
          public string expiryMonth { get; set; }
          public string expiryYear { get; set; }
          public string firstName { get; set; }
          public string lastName { get; set; }
          public string csc { get; set; }
          public string type { get; set; }
     }
     public class P3Transaction {
          public P3Amount amount { get; set; }
     }
     public class P3Amount { 
          public P3Total total { get; set; }
     }
     public class P3Total {
          public string currency { get; set; }
          public decimal value { get; set; }
     }
}
