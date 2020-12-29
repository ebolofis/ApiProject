namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StaffPosition")]
    public partial class StaffPosition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StaffPosition()
        {
            AssignedPositions = new HashSet<AssignedPosition>();
            PosInfo_StaffPositin_Assoc = new HashSet<PosInfo_StaffPositin_Assoc>();
        }

        public long Id { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssignedPosition> AssignedPositions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PosInfo_StaffPositin_Assoc> PosInfo_StaffPositin_Assoc { get; set; }
    }
}
