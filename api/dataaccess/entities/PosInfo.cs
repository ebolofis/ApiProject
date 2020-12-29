namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PosInfo")]
    public partial class PosInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PosInfo()
        {
            ClientPos = new HashSet<ClientPos>();
            CreditTransactions = new HashSet<CreditTransaction>();
            EndOfDays = new HashSet<EndOfDay>();
            EODAccountToPmsTransfers = new HashSet<EODAccountToPmsTransfer>();
            Invoices = new HashSet<Invoice>();
            Kds = new HashSet<Kd>();
            MealConsumptions = new HashSet<MealConsumption>();
            Notifications = new HashSet<Notification>();
            Orders = new HashSet<Order>();
            PagePosAssocs = new HashSet<PagePosAssoc>();
            PdaModules = new HashSet<PdaModule>();
            PosInfo_KitchenInstruction_Assoc = new HashSet<PosInfo_KitchenInstruction_Assoc>();
            PosInfo_Pricelist_Assoc = new HashSet<PosInfo_Pricelist_Assoc>();
            PosInfo_Region_Assoc = new HashSet<PosInfo_Region_Assoc>();
            PosInfo_StaffPositin_Assoc = new HashSet<PosInfo_StaffPositin_Assoc>();
            PosInfoDetails = new HashSet<PosInfoDetail>();
            PosInfoKdsAssocs = new HashSet<PosInfoKdsAssoc>();
            Regions = new HashSet<Region>();
            Transactions = new HashSet<Transaction>();
        }

        public long Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public DateTime? FODay { get; set; }

        public long? CloseId { get; set; }

        [StringLength(50)]
        public string IPAddress { get; set; }

        public byte? Type { get; set; }

        [StringLength(50)]
        public string wsIP { get; set; }

        [StringLength(50)]
        public string wsPort { get; set; }

        public long? DepartmentId { get; set; }

        [StringLength(200)]
        public string FiscalName { get; set; }

        public byte? FiscalType { get; set; }

        public bool? IsOpen { get; set; }

        public long? ReceiptCount { get; set; }

        public bool? ResetsReceiptCounter { get; set; }

        [StringLength(50)]
        public string Theme { get; set; }

        public long? AccountId { get; set; }

        public bool? LogInToOrder { get; set; }

        public bool? ClearTableManually { get; set; }

        public bool? ViewOnly { get; set; }

        public bool? IsDeleted { get; set; }

        public int? InvoiceSumType { get; set; }

        public short? LoginToOrderMode { get; set; }

        public short? KeyboardType { get; set; }

        public virtual Account Account { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientPos> ClientPos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EndOfDay> EndOfDays { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EODAccountToPmsTransfer> EODAccountToPmsTransfers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kd> Kds { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MealConsumption> MealConsumptions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PagePosAssoc> PagePosAssocs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PdaModule> PdaModules { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfo_KitchenInstruction_Assoc> PosInfo_KitchenInstruction_Assoc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfo_Pricelist_Assoc> PosInfo_Pricelist_Assoc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfo_Region_Assoc> PosInfo_Region_Assoc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfo_StaffPositin_Assoc> PosInfo_StaffPositin_Assoc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfoDetail> PosInfoDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfoKdsAssoc> PosInfoKdsAssocs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Region> Regions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
