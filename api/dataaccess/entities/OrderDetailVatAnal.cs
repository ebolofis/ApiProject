namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderDetailVatAnal")]
    public partial class OrderDetailVatAnal
    {
        public long Id { get; set; }

        public long? OrderDetailId { get; set; }

        public decimal? Gross { get; set; }

        public decimal? Net { get; set; }

        public decimal? VatRate { get; set; }

        public decimal? VatAmount { get; set; }

        public long? VatId { get; set; }

        public long? TaxId { get; set; }

        public decimal? TaxAmount { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }

        public virtual Tax Tax { get; set; }
    }
}