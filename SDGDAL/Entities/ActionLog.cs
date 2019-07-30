using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("ActionLogs")]
    public class ActionLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionLogId { get; set; }

        public int TargetId { get; set; }

        [StringLength(50), Required()]
        public string TargetTable { get; set; }

        [StringLength(50), Required()]
        public string Action { get; set; } // Create, Update, Deactivate, Activate

        public string Details { get; set; }

        public int LoggedByUserId { get; set; }
        public DateTime DateLogged { get; set; }
    }
}