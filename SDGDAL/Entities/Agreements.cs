using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Agreements")]
    public class Agreements
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AgreementsId { get; set; }

        public string TermsOfService { get; set; }
        public string Disclaimer { get; set; }

        [StringLength(50)]
        public string LanguageCode { get; set; }

        public bool IsCustom { get; set; }

        public Nullable<int> MerchantId { get; set; }

        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }
    }
}