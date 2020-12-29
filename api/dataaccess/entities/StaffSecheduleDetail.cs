namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StaffSecheduleDetail
    {
        public long Id { get; set; }

        public long? StaffSceduleId { get; set; }

        public long? StaffId { get; set; }

        public long? StaffPositionId { get; set; }

        public DateTime? Hour { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual StaffSchedule StaffSchedule { get; set; }
    }
}
