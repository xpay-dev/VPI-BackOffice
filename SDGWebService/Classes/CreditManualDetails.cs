using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    public class CreditCardDetails
    {
        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string ExpiryMonth { get; set; }

        [DataMember]
        public string ExpiryYear { get; set; }

        [DataMember]
        public string CVV { get; set; }
    }
}