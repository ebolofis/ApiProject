namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StoreMessage
    {
        public long Id { get; set; }

        public string Message { get; set; }

        public DateTime? CreationDate { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        public long? StoreId { get; set; }

        public DateTime? ShowFrom { get; set; }

        public DateTime? ShowTo { get; set; }

        [StringLength(250)]
        public string ImageUri { get; set; }

        public byte? Status { get; set; }

        public virtual Store Store { get; set; }
    }
}
