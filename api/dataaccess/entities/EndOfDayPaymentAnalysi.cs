namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EndOfDayPaymentAnalysi
    {
        public long Id { get; set; }

        public long? EndOfDayId { get; set; }

        public long? AccountId { get; set; }

        public decimal? Total { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public short? AccountType { get; set; }

        public virtual Account Account { get; set; }

        public virtual EndOfDay EndOfDay { get; set; }
    }
}
