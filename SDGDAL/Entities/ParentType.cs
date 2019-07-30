using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("ParentTypes")]
    public class ParentType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParentTypeId { get; set; }

        public string ParentTypeName { get; set; }

        public DateTime DateCreated { get; set; }
    }
}