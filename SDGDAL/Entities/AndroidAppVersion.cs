using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("AndroidAppVersion")]
    public class AndroidAppVersion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AndroidAppVersionId { get; set; }

        public string AppName { get; set; }
        public string PackageName { get; set; }
        public string VersionName { get; set; }
        public string VersionBuild { get; set; }
        public int VersionCode { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
    }
}