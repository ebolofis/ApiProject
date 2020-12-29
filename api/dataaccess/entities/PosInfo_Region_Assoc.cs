namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PosInfo_Region_Assoc
    {
        public long Id { get; set; }

        public long? PosInfoId { get; set; }

        public long? RegionId { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        public virtual Region Region { get; set; }
    }
}
