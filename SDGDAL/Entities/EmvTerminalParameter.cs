using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("EmvTerminalParameters")]
    public class EmvTerminalParameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmvTerminalParameterId { get; set; }

        public string TerminalCapabilities { get; set; }
        public string AddTerminlaCapablities { get; set; }
        public string TerminalCountryCode { get; set; }
        public string TerminalType { get; set; }
        public string TransCurrCode { get; set; }
        public string TransCurrExponent { get; set; }
        public string TransRefCurrCode { get; set; }
        public string TransRefCurrExponent { get; set; }
        public string TransRefCurrConversion { get; set; }
        public string Reserved { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
    }
}