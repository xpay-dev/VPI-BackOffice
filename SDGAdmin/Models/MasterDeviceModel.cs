using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class MasterDeviceModel
    {
        public int MasterDeviceId { get; set; }
        [Required(), StringLength(50)]
        public string DeviceName { get; set; }
        public string Manufacturer { get; set; }
        public string Warranty { get; set; }
        public string ExternalData { get; set; }
        public int DeviceTypeId { get; set; }
        public int DeviceFlowTypeId { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}