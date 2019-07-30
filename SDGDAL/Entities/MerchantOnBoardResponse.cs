using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("MerchantOnBoardResponse")]
    public class MerchantOnBoardResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantOnBoardResponseId { get; set; }

        public string ResponseFileName { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateReceived { get; set; }
    }
}