namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PagePosAssoc")]
    public partial class PagePosAssoc
    {
        public long Id { get; set; }

        public long? PageSetId { get; set; }

        public long? PosInfoId { get; set; }

        public virtual PageSet PageSet { get; set; }

        public virtual PosInfo PosInfo { get; set; }
    }
}
