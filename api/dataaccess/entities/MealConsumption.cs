namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MealConsumption")]
    public partial class MealConsumption
    {
        public long Id { get; set; }

        public long? GuestId { get; set; }

        public int? ConsumedMeals { get; set; }

        public DateTime? ConsumedTS { get; set; }

        public bool? IsDeleted { get; set; }

        [StringLength(150)]
        public string BoardCode { get; set; }

        public int? ReservationId { get; set; }

        public int? ConsumedMealsChild { get; set; }

        public long? DepartmentId { get; set; }

        public long? EndOfDayId { get; set; }

        public long? PosInfoId { get; set; }

        public virtual Department Department { get; set; }

        public virtual EndOfDay EndOfDay { get; set; }

        public virtual Guest Guest { get; set; }

        public virtual PosInfo PosInfo { get; set; }
    }
}
