namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RegionLockerProduct")]
    public partial class RegionLockerProduct
    {
        public long Id { get; set; }

        public long? RegionId { get; set; }

        public long? ProductId { get; set; }

        public long? PriceListId { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        public decimal? Discount { get; set; }

        [StringLength(150)]
        public string SalesDescription { get; set; }

        public long? ReturnPaymentpId { get; set; }

        public long? PaymentId { get; set; }

        public long? SaleId { get; set; }

        public virtual Pricelist Pricelist { get; set; }

        public virtual Product Product { get; set; }

        public virtual Region Region { get; set; }
    }
}
