using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("POSTypes")]
    public class POSType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int POSTypeId { get; set; }

        [StringLength(20), Required()]
        public string TypeName { get; set; }
    }
}