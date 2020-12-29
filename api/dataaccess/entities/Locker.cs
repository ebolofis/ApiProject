namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Locker
    {
        public long Id { get; set; }

        public bool HasLockers { get; set; }

        public double TotalLockers { get; set; }

        public double TotalLockersAmount { get; set; }

        public double Paidlockers { get; set; }

        public decimal PaidlockersAmount { get; set; }

        public decimal OccLockers { get; set; }

        public decimal OccLockersAmount { get; set; }

        public long? EndOfDayId { get; set; }
    }
}
