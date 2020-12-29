namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PosInfoDetail_Pricelist_Assoc
    {
        public long Id { get; set; }

        public long? PosInfoDetailId { get; set; }

        public long? PricelistId { get; set; }

        public virtual PosInfoDetail PosInfoDetail { get; set; }

        public virtual Pricelist Pricelist { get; set; }
    }
}
