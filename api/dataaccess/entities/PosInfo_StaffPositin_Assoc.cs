namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PosInfo_StaffPositin_Assoc
    {
        public long Id { get; set; }

        public long? PosInfoId { get; set; }

        public long? StaffPositionId { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        public virtual StaffPosition StaffPosition { get; set; }
    }
}
