namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PosInfoKdsAssoc")]
    public partial class PosInfoKdsAssoc
    {
        public long Id { get; set; }

        public long? PosInfoId { get; set; }

        public long? KdsId { get; set; }

        public virtual Kd Kd { get; set; }

        public virtual PosInfo PosInfo { get; set; }
    }
}
