using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGAdmin.Models
{
    public class AndroidAppVersionModel
    {
        public int AndroidAppVersionId { get; set; }
        public string AppName { get; set; }
        public string PackageName { get; set; }
        public string VersionName { get; set; }
        public string VersionBuild { get; set; }
        public int  VersionCode { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
    }
}