namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CreditTransaction
    {
        public long Id { get; set; }

        public long? CreditAccountId { get; set; }

        public long? CreditCodeId { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? CreationTS { get; set; }

        public byte? Type { get; set; }

        public long? StaffId { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public long? PosInfoId { get; set; }

        public long? EndOfDayId { get; set; }

        public long? InvoiceId { get; set; }

        public long? TransactionId { get; set; }

        public virtual CreditAccount CreditAccount { get; set; }

        public virtual CreditCode CreditCode { get; set; }

        public virtual EndOfDay EndOfDay { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
