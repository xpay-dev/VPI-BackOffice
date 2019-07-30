using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.CTPaymentGateway
{
    public class PinPad
    {
        public string SequenceNumber { get; set; }
        public string Command { get; set; }
        public string InstitutionNumber { get; set; }
        public string PinpadId { get; set; }
        public string MerchantId { get; set; }
        public string TerminalID { get; set; }
    }
}
