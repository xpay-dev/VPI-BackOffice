using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("SwitchAPITypes")]
    public class SwitchAPIType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SwitchAPITypeId { get; set; }

        public string APIName { get; set; }

        public bool IsActive { get; set; }
    }
}