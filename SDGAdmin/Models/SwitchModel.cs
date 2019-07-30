using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class SwitchModel
    {
        public int SwitchId { get; set; }
        public string SwitchName { get; set; }
        public bool IsActive { get; set; }
        public bool IsAddressRequired { get; set; }
        public DateTime DateCreated { get; set; }
    }
}