using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CTPayment;
using CTPayment.Classes;

namespace SDGBackOffice.Models
{
    public class MerchantOnBoardResponseModel
    {
        public string SequenceNumber { get; set; }
        public string Version { get; set; }
        public string InstitutionNumber { get; set; }
        public string LoadDate { get; set; }
        public string LoadTime { get; set; }
        public string MiscText { get; set; }
        public string ResultCode { get; set; }
        public string ResultText { get; set; }

        public HeaderResponse HEADER { get; set; }
        public List<StatsResponse> STAT { get; set; }
        public List<MerchantResponse> MERCHANT { get; set; }
        public List<CardResponse> CCARD { get; set; }
        public List<CardResponse> DCARD { get; set; }
        public List<TerminalResponse> TERMINAL { get; set; }
        public List<PinPadResponse> PINPAD { get; set; }
    }
}