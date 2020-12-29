namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EODAccountToPmsTransfer")]
    public partial class EODAccountToPmsTransfer
    {
        public long Id { get; set; }

        public long? PosInfoId { get; set; }

        public long? AccountId { get; set; }

        public long? PmsRoom { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        public bool? Status { get; set; }

        public virtual Account Account { get; set; }

        public virtual PosInfo PosInfo { get; set; }
    }
}
