using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("SwitchParameterTypes")]
    public class SwitchParameterType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SwitchParameterTypeId { get; set; }

        public string ParameterType { get; set; }
    }
}