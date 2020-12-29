namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KitchenInstructionLogger")]
    public partial class KitchenInstructionLogger
    {
        public long Id { get; set; }

        public long? KicthcenInstuctionId { get; set; }

        public long? StaffId { get; set; }

        public long? PdaModulId { get; set; }

        public long? ClientId { get; set; }

        public long? PosInfoId { get; set; }

        public DateTime? SendTS { get; set; }

        public DateTime? ReceivedTS { get; set; }

        public long? EndOfDayId { get; set; }

        public short? Status { get; set; }

        public long? TableId { get; set; }

        [StringLength(200)]
        public string ExtecrName { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        public virtual EndOfDay EndOfDay { get; set; }

        public virtual KitchenInstruction KitchenInstruction { get; set; }
    }
}
