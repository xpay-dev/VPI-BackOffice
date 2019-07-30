using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("Countries")]
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }

        [StringLength(50)]
        public string CountryName { get; set; }

        [StringLength(5)]
        public string CountryCode { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual ICollection<CountryIP> CountryIPs { get; set; }
    }
}