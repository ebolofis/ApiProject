namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SalesType_PricelistMaster_Assoc
    {
        public long Id { get; set; }

        public long? PricelistMasterId { get; set; }

        public long? SalesTypeId { get; set; }

        public virtual PricelistMaster PricelistMaster { get; set; }

        public virtual SalesType SalesType { get; set; }
    }
}
