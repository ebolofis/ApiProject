namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CreditAccount
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CreditAccount()
        {
            CreditCodes = new HashSet<CreditCode>();
            CreditTransactions = new HashSet<CreditTransaction>();
        }

        public long Id { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public long? EndOfDayId { get; set; }

        public DateTime? ActivateTS { get; set; }

        public DateTime? DeactivateTS { get; set; }

        public virtual EndOfDay EndOfDay { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditCode> CreditCodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; }
    }
}
