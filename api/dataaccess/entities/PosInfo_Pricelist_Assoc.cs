namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PosInfo_Pricelist_Assoc
    {
        public long Id { get; set; }

        public long? PosInfoId { get; set; }

        public long? PricelistId { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        public virtual Pricelist Pricelist { get; set; }
    }
}
