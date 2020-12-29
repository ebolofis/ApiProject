namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PdaModuleDetail")]
    public partial class PdaModuleDetail
    {
        public long Id { get; set; }

        public long? PdaModuleId { get; set; }

        public DateTime? LastLoginTS { get; set; }

        public DateTime? LastLogoutTS { get; set; }

        public long? StaffId { get; set; }

        public virtual PdaModule PdaModule { get; set; }
    }
}
