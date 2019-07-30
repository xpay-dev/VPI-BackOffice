using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGBackOffice.Models
{
    public class MerchantDeviceModel
    {
        public int MerchantId { get; set; }
        public string Merchant { get; set; }
        public MasterDeviceModel MasterDevice { get; set; }
    }
}