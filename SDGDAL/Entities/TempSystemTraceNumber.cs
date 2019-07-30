using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("TempSystemTraceNumber")]
    public class TempSystemTraceNumber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string TraceNumber { get; set; }
        public DateTime DateCreated { get; set; }
    }
}