using System.Runtime.Serialization;

namespace SDGWebService.Classes {
     [DataContract]
     public class TransactionRequest {
          [DataMember]
          public string LanguageUser { get; set; }

          [DataMember]
          public int Device { get; set; }

          [DataMember]
          public int DeviceId { get; set; }

          [DataMember]
          public decimal Tips { get; set; }

          [DataMember]
          public int AccountTypeId { get; set; }

          [DataMember]
          public int Action { get; set; }

          [DataMember]
          public POSWSRequest POSWSRequest { get; set; }

          [DataMember]
          public CardDetails CardDetails { get; set; }

          [DataMember]
          public int TransactionStatus { get; set; } //0 Purchase, 1 Void, 2 Refund

          [DataMember]
          public string VoidRefundNote { get; set; }
     }
}