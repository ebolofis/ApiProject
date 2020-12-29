namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pricelist")]
    public partial class Pricelist
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pricelist()
        {
            AllowedMealsPerBoards = new HashSet<AllowedMealsPerBoard>();
            OrderDetailInvoices = new HashSet<OrderDetailInvoice>();
            PosInfo_Pricelist_Assoc = new HashSet<PosInfo_Pricelist_Assoc>();
            PosInfoDetail_Pricelist_Assoc = new HashSet<PosInfoDetail_Pricelist_Assoc>();
            PriceList_EffectiveHours = new HashSet<PriceList_EffectiveHours>();
            PricelistDetails = new HashSet<PricelistDetail>();
        }

        public long Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public long? LookUpPriceListId { get; set; }

        public double? Percentage { get; set; }

        public byte? Status { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime? DeactivationDate { get; set; }

        public long? SalesTypeId { get; set; }

        public long? PricelistMasterId { get; set; }

        public bool? IsDeleted { get; set; }

        public short? Type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AllowedMealsPerBoard> AllowedMealsPerBoards { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailInvoice> OrderDetailInvoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfo_Pricelist_Assoc> PosInfo_Pricelist_Assoc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfoDetail_Pricelist_Assoc> PosInfoDetail_Pricelist_Assoc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceList_EffectiveHours> PriceList_EffectiveHours { get; set; }

        public virtual PricelistMaster PricelistMaster { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PricelistDetail> PricelistDetails { get; set; }

        public virtual RegionLockerProduct RegionLockerProduct { get; set; }
    }
}
