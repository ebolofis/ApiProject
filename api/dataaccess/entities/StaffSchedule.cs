namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StaffSchedule")]
    public partial class StaffSchedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StaffSchedule()
        {
            StaffSecheduleDetails = new HashSet<StaffSecheduleDetail>();
        }

        public long Id { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public DateTime? Day { get; set; }

        public long? DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffSecheduleDetail> StaffSecheduleDetails { get; set; }
    }
}
