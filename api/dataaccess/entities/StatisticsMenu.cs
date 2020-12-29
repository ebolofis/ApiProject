namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StatisticsMenu
    {
        public long Id { get; set; }

        [StringLength(250)]
        public string MenuDescription { get; set; }

        public short? CategoryOrder { get; set; }

        [StringLength(250)]
        public string MenuItemDescription { get; set; }

        public short? MenuItemOrder { get; set; }

        public short? MenuLevel { get; set; }

        public long? ReportListId { get; set; }

        public long? ParentMenuId { get; set; }

        [StringLength(2048)]
        public string Url { get; set; }

        public virtual ReportList ReportList { get; set; }
    }
}
