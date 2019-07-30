using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Switches")]
    public class Switch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SwitchId { get; set; }

        [StringLength(100), Required()]
        public string SwitchName { get; set; }

        [StringLength(100), Required()]
        public string SwitchCode { get; set; }

        public bool IsActive { get; set; }
        public bool IsAddressRequired { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ICollection<SwitchPartnerLink> SwitchPartnerLinks { get; set; }
    }
}