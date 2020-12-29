namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductForBarcodeEod")]
    public partial class ProductForBarcodeEod
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
