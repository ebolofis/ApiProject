namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AssignedPosition
    {
        public long Id { get; set; }

        public long? StaffPositionId { get; set; }

        public long? StaffId { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual StaffPosition StaffPosition { get; set; }
    }
}
