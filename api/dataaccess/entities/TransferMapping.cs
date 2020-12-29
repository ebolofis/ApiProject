namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TransferMapping
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TransferMapping()
        {
            TransferMappingDetails = new HashSet<TransferMappingDetail>();
        }

        public long Id { get; set; }

        [StringLength(100)]
        public string PmsDepartmentId { get; set; }

        [StringLength(250)]
        public string PmsDepDescription { get; set; }

        public long? ProductId { get; set; }

        public long? SalesTypeId { get; set; }

        public double? VatPercentage { get; set; }

        public long? PosDepartmentId { get; set; }

        public long? PriceListId { get; set; }

        public int? VatCode { get; set; }

        public long? ProductCategoryId { get; set; }

        public long? HotelId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Product Product { get; set; }

        public virtual SalesType SalesType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransferMappingDetail> TransferMappingDetails { get; set; }
    }
}
