namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PageButton")]
    public partial class PageButton
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PageButton()
        {
            PageButtonDetails = new HashSet<PageButtonDetail>();
        }

        public long Id { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public short? PreparationTime { get; set; }

        [StringLength(500)]
        public string ImageUri { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public long? PriceListId { get; set; }

        public short? Sort { get; set; }

        public long? NavigateToPage { get; set; }

        public long? SetDefaultPriceListId { get; set; }

        public byte? Type { get; set; }

        public long? PageId { get; set; }

        [StringLength(25)]
        public string Color { get; set; }

        [StringLength(25)]
        public string Background { get; set; }

        public long? ProductId { get; set; }

        public long? SetDefaultSalesType { get; set; }

        public virtual Page Page { get; set; }

        public virtual Product Product { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PageButtonDetail> PageButtonDetails { get; set; }
    }
}
