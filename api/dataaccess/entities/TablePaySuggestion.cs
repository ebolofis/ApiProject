namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TablePaySuggestion")]
    public partial class TablePaySuggestion
    {
        public long Id { get; set; }

        public long? AccountId { get; set; }

        public long? GuestId { get; set; }

        public decimal? Amount { get; set; }

        public long? OrderDetailId { get; set; }

        public bool? IsUsed { get; set; }

        public long? CreditCodeId { get; set; }

        public virtual Account Account { get; set; }

        public virtual CreditCode CreditCode { get; set; }

        public virtual Guest Guest { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }
    }
}
