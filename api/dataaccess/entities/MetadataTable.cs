namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetadataTable")]
    public partial class MetadataTable
    {
        public long Id { get; set; }

        public long? ReportType { get; set; }

        [StringLength(350)]
        public string FieldName { get; set; }

        [StringLength(350)]
        public string Description { get; set; }

        public short? FieldType { get; set; }

        [StringLength(350)]
        public string DefaultStyle { get; set; }

        public bool? Summable { get; set; }

        [StringLength(300)]
        public string Expression { get; set; }
    }
}
