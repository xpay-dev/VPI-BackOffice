using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    [DataContract]
    public class TransactionVoidRefundRequest
    {
        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string TransactionNumber { get; set; }

        [DataMember]
        public int VoidRefundReasonId { get; set; }

        [DataMember]
        public string VoidRefundNote { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}