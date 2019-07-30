using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("SwitchPartnerLink")]
    public class SwitchPartnerLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SwitchPartnerLinkId { get; set; }

        public int SwitchId { get; set; }

        [ForeignKey("SwitchId")]
        public virtual Switch Switch { get; set; }

        public int PartnerId { get; set; }

        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }

        public bool IsEnabled { get; set; }
    }
}