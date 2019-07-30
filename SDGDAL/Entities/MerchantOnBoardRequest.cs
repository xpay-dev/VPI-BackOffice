using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MerchantOnBoardRequest")]
    public class MerchantOnBoardRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantOnBoardRequestId { get; set; }

        public string RequestFileName { get; set; }
        public string MiscText { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsUpdated { get; set; }
    }
}