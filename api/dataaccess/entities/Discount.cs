namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Discount")]
    public partial class Discount
    {
        public long Id { get; set; }

        public byte? Type { get; set; }

        public decimal? Amount { get; set; }

        public bool? Status { get; set; }

        public int? Sort { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
    }
}
