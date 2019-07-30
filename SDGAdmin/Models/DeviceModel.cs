using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class DeviceModel
    {
        public int DeviceId { get; set; }
        public int MasterDeviceId { get; set; }
        [Required(), StringLength(50)]
        public string SerialNumber { get; set; }
        public Boolean IsKeyInjected { get; set; }
    }
}