using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MerchantOnBoardResponseLink")]
    public class MerchantOnBoardResponseLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantOnBoardResponseLinkId { get; set; }

        public string MerchantId { get; set; }

        public int MerchantOnBoardRequestId { get; set; }

        [ForeignKey("MerchantOnBoardRequestId")]
        public virtual MerchantOnBoardRequest MerchantOnBoardRequest { get; set; }

        public bool IsDeleted { get; set; }
    }
}