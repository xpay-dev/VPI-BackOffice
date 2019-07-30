using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("DbSettings")]
    public class DbSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DbSettingsId { get; set; }

        public string SettingCode { get; set; }
        public string SettingValue { get; set; }
        public string SettingDescription { get; set; }
        public DateTime DateCreated { get; set; }
    }
}