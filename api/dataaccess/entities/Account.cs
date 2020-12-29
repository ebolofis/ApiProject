namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            EndOfDayPaymentAnalysis = new HashSet<EndOfDayPaymentAnalysi>();
            EndOfDayVoidsAnalysis = new HashSet<EndOfDayVoidsAnalysi>();
            EODAccountToPmsTransfers = new HashSet<EODAccountToPmsTransfer>();
            PosInfoes = new HashSet<PosInfo>();
            PosInfoDetail_Excluded_Accounts = new HashSet<PosInfoDetail_Excluded_Accounts>();
            TablePaySuggestions = new HashSet<TablePaySuggestion>();
            Transactions = new HashSet<Transaction>();
        }

        public long Id { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public short? Type { get; set; }

        public bool? IsDefault { get; set; }

        public bool? SendsTransfer { get; set; }

        public bool? IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EndOfDayPaymentAnalysi> EndOfDayPaymentAnalysis { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EndOfDayVoidsAnalysi> EndOfDayVoidsAnalysis { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EODAccountToPmsTransfer> EODAccountToPmsTransfers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfo> PosInfoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfoDetail_Excluded_Accounts> PosInfoDetail_Excluded_Accounts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TablePaySuggestion> TablePaySuggestions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
