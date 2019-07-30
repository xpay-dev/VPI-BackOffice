using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("ErrorLogs")]
    public class ErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ErrorLogId { get; set; }

        [StringLength(100)]
        public string Method { get; set; }

        [StringLength(100)]
        public string Action { get; set; }

        public string ErrText { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public string InnerExceptionStackTrace { get; set; }
        public DateTime DateCreated { get; set; }
    }
}