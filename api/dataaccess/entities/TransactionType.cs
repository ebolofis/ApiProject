namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TransactionType
    {
        public long Id { get; set; }

        public short? Code { get; set; }

        public string Description { get; set; }

        public bool? IsIncome { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
