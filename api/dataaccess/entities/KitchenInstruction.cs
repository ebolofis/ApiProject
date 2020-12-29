namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KitchenInstruction")]
    public partial class KitchenInstruction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KitchenInstruction()
        {
            KitchenInstructionLoggers = new HashSet<KitchenInstructionLogger>();
            PosInfo_KitchenInstruction_Assoc = new HashSet<PosInfo_KitchenInstruction_Assoc>();
        }

        public long Id { get; set; }

        public long? KitchenId { get; set; }

        [StringLength(300)]
        public string Message { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitchenInstructionLogger> KitchenInstructionLoggers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfo_KitchenInstruction_Assoc> PosInfo_KitchenInstruction_Assoc { get; set; }
    }
}
