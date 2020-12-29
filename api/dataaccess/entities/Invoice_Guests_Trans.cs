namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Invoice_Guests_Trans
    {
        public long Id { get; set; }

        public long? InvoiceId { get; set; }

        public long? GuestId { get; set; }

        public long? TransactionId { get; set; }

        public virtual Guest Guest { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
