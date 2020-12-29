namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Notification
    {
        public long Id { get; set; }

        public string Message { get; set; }

        public DateTime? MessageTS { get; set; }

        public string UserList { get; set; }

        [StringLength(150)]
        public string Sender { get; set; }

        public long? PosInfoId { get; set; }

        public long? StoreId { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        public virtual Store Store { get; set; }
    }
}
