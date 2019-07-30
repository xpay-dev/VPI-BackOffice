using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Languages")]
    public class Language
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LanguageId { get; set; }

        [StringLength(50), Required()]
        public string LanguageCode { get; set; }

        [StringLength(50), Required()]
        public string LanguageName { get; set; }

        public DateTime DateCreated { get; set; }
    }
}