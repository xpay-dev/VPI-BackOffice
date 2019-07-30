using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("EmvAmexParameters")]
    public class EmvAmexParameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmvAmexParameterId { get; set; }

        public string ApplicationID { get; set; }
        public string AppSelectionIndicator { get; set; }
        public string AppAccountSelection { get; set; }
        public string AppVersionNumber { get; set; }
        public string TerminalActionDefault { get; set; }
        public string TerminalActionDenial { get; set; }
        public string TerminalActionOnline { get; set; }
        public string MaxTarget { get; set; }
        public string TagPercent { get; set; }
        public string ThresholdValue { get; set; }
        public string ContactlessTerminalDefault { get; set; }
        public string ContactlessTerminalDenial { get; set; }
        public string ContactlessTerminalOnline { get; set; }
        public string TerminalRiskManagement { get; set; }
        public string ReservedFutureUse { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
    }
}