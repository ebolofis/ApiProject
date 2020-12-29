namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BoardMeal
    {
        public long Id { get; set; }

        [StringLength(50)]
        public string BoardId { get; set; }

        [StringLength(100)]
        public string BoardDescription { get; set; }

        public long? ProductId { get; set; }

        public int? MealsQty { get; set; }

        [StringLength(50)]
        public string BoardCode { get; set; }

        public virtual Product Product { get; set; }
    }
}
