namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReportList")]
    public partial class ReportList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReportList()
        {
            StatisticsMenus = new HashSet<StatisticsMenu>();
        }

        public long Id { get; set; }

        [StringLength(350)]
        public string ReportName { get; set; }

        public int? ReportType { get; set; }

        public string ReportJson { get; set; }

        public bool? AppearsInMenu { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? Datemodified { get; set; }

        [StringLength(50)]
        public string Version { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StatisticsMenu> StatisticsMenus { get; set; }
    }
}
