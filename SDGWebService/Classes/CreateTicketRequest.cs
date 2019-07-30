using System.Runtime.Serialization;

namespace SDGWebService.Classes
{
    public class CreateTicketRequest
    {
        [DataMember]
        public string EmailServerName { get; set; }

        [DataMember]
        public string TicketNumber { get; set; }

        [DataMember]
        public EmailDetails EmailDetails { get; set; }

        [DataMember]
        public POSWSRequest POSWSRequest { get; set; }
    }
}