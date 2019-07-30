using CTPayment.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTPayment
{
    public class MerchantBulkResponse
    {
        public HeaderResponse HEADER { get; set; }
        public List<StatsResponse> STAT { get; set; }
        public List<MerchantResponse> MERCHANT { get; set; }
        public List<CardResponse> CCARD { get; set; }
        public List<CardResponse> DCARD { get; set; }
        public List<TerminalResponse> TERMINAL { get; set; }
        public List<PinPadResponse> PINPAD { get; set; }
    }
}
