namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderStatu
    {
        public long Id { get; set; }

        public long? Status { get; set; }

        public DateTime? TimeChanged { get; set; }

        public long? OrderId { get; set; }

        public long? StaffId { get; set; }

        public virtual Order Order { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
