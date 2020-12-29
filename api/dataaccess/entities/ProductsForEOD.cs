namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductsForEOD")]
    public partial class ProductsForEOD
    {
        public long Id { get; set; }

        public long? ProductId { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public virtual Product Product { get; set; }
    }
}
