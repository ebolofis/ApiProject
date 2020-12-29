namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PosInfoDetail_Excluded_Accounts
    {
        public long Id { get; set; }

        public long? PosInfoDetailId { get; set; }

        public long? AccountId { get; set; }

        public virtual Account Account { get; set; }

        public virtual PosInfoDetail PosInfoDetail { get; set; }
    }
}
