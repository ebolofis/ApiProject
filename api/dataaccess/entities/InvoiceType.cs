namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InvoiceType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InvoiceType()
        {
            Invoices = new HashSet<Invoice>();
            PosInfoDetails = new HashSet<PosInfoDetail>();
            PredefinedCredits = new HashSet<PredefinedCredit>();
        }

        public long Id { get; set; }

        [StringLength(15)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Abbreviation { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public short? Type { get; set; }

        public bool? IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfoDetail> PosInfoDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PredefinedCredit> PredefinedCredits { get; set; }
    }
}
