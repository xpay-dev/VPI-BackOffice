using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Keys")]
    public class _Key
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KeyId { get; set; }

        [Required(), StringLength(2000)]
        public string Key { get; set; }

        [StringLength(200), Required()]
        public string IV { get; set; }
    }
}