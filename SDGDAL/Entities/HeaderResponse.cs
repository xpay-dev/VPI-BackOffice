using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGDAL.Entities
{
    [Table("HeaderResponse")]
    public class HeaderResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HeaderResponseId { get; set; }

        public string TerminalId { get; set; }

        public Nullable<int> EmvHostParameterId { get; set; }

        [ForeignKey("EmvHostParameterId")]
        public virtual EmvHostParameter EmvHostParameter { get; set; }

        public Nullable<int> EmvTerminalParameterId { get; set; }

        [ForeignKey("EmvTerminalParameterId")]
        public virtual EmvTerminalParameter EmvTerminalParameter { get; set; }

        public Nullable<int> EmvMastercardParameterId { get; set; }

        [ForeignKey("EmvMastercardParameterId")]
        public virtual EmvMastercardParameter EmvMastercardParameter { get; set; }

        public Nullable<int> EmvVisaParameterId { get; set; }

        [ForeignKey("EmvVisaParameterId")]
        public virtual EmvVisaParameter EmvVisaParameter { get; set; }

        public Nullable<int> EmvAmexParameterId { get; set; }

        [ForeignKey("EmvAmexParameterId")]
        public virtual EmvAmexParameter EmvAmexParameter { get; set; }

        public Nullable<int> EmvInteracParameterId { get; set; }

        [ForeignKey("EmvInteracParameterId")]
        public virtual EmvInteracParameter EmvInteracParameter { get; set; }

        public Nullable<int> EmvJcbParametersId { get; set; }

        [ForeignKey("EmvJcbParametersId")]
        public virtual EmvJcbParameter EmvJcbParameter { get; set; }

        public Nullable<int> EmvDiscoverParameterId { get; set; }

        [ForeignKey("EmvDiscoverParameterId")]
        public virtual EmvDiscoverParameter EmvDiscoverParameter { get; set; }

        public string ResultMessage { get; set; }
        public string ReturnCode { get; set; }
        public string MessageClass { get; set; }
        public string SequenceNumber { get; set; }
        public string TransType { get; set; }
        public string POSEntryMode { get; set; }
        public string MessageVersion { get; set; }
        public string POSResultCode { get; set; }
        public string POSStatIndicator { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
    }
}