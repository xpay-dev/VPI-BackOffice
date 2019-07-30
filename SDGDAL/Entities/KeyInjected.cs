using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("KeyInjected")]
    public class KeyInjected
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KeyInjectedId { get; set; }

        [StringLength(50), Required()]
        public string KeyDetail1 { get; set; }

        [StringLength(50), Required()]
        public string KeyDetail2 { get; set; }

        public DateTime CreateDate { get; set; }
    }
}