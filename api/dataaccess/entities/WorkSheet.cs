namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkSheet")]
    public partial class WorkSheet
    {
        public long Id { get; set; }

        public long? StaffId { get; set; }

        public DateTime? Day { get; set; }

        public DateTime? Hour { get; set; }

        public byte? Type { get; set; }

        public long? DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
