namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ValidModule
    {
        public long Id { get; set; }

        public short? ModuleType { get; set; }

        public int? MaxInstances { get; set; }
    }
}
