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
    using System.Collections.Generic;
    using Microsoft.Azure.Documents;

    public class ComplianceResponse : Resource
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ComplianceResponse()
        {
            this.ComplianceEntries = new HashSet<ComplianceEntry>();
        }

        public string ComplianceInformation { get; set; }

        public int RegimenNumber { get; set; }

        public int WeekNumber { get; set; }

        public System.DateTime StartDateTime { get; set; }

        public int DeviceId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComplianceEntry> ComplianceEntries { get; set; }

        public virtual Device Device { get; set; }
    }
}
