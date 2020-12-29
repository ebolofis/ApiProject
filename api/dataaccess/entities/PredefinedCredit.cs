namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PredefinedCredit
    {
        public long Id { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public decimal? Amount { get; set; }

        public long? InvoiceTypeId { get; set; }

        public virtual InvoiceType InvoiceType { get; set; }
    }
}
