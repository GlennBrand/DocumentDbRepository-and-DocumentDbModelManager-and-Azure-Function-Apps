//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComplyNowDocumentDbModelManager.Models
{
    using System;
    using System.Collections.Generic;

    public partial class DeviceDto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeviceDto()
        {
            this.ComplianceResponses = new HashSet<ComplianceResponseDto>();
        }

        public string UniqueId { get; set; }

        public string DeviceType { get; set; }

        public string HardwareVersion { get; set; }

        public string FirmwareVersion { get; set; }

        public int PatientId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComplianceResponseDto> ComplianceResponses { get; set; }
    }
}