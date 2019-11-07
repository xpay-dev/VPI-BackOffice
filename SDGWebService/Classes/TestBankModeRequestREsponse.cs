using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGWebService.Classes {

     public class RequeTestBankModeRequest {
          public string CardNumber { get; set; }
          public int ExpMonth { get; set; }
          public int ExpYear { get; set; }
          public string CVV { get; set; }
          public decimal Amount { get; set; }
     }
     public class RequeTestBankModeResponse {
          public string RefNumber { get; set; }
          public decimal Amount { get; set; }
          public string Status { get; set; }
          public string Note { get; set; }
     }
}