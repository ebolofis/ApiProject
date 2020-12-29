namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StaffAuthorization")]
    public partial class StaffAuthorization
    {
        public long Id { get; set; }

        public long? AuthorizedGroupId { get; set; }

        public long? StaffId { get; set; }

        public virtual AuthorizedGroup AuthorizedGroup { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
