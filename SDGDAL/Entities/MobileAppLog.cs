using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MobileAppLogs")]
    public class MobileAppLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MobileAppLogId { get; set; }

        public int MobileAppId { get; set; }

        [ForeignKey("MobileAppId")]
        public virtual MobileApp MobileApp { get; set; }

        public int AccountId { get; set; }

        [StringLength(500)]
        public string LogDetails { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime DateLogged { get; set; }

        public decimal GPSLat { get; set; }
        public decimal GPSLong { get; set; }
    }
}