namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductBarcode
    {
        public long Id { get; set; }

        [StringLength(250)]
        public string Barcode { get; set; }

        public long? ProductId { get; set; }

        public byte? Type { get; set; }

        public virtual Product Product { get; set; }
    }
}
