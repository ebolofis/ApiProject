namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AuthorizedGroupDetail")]
    public partial class AuthorizedGroupDetail
    {
        public long Id { get; set; }

        public long? AuthorizedGroupId { get; set; }

        public long? ActionId { get; set; }

        public virtual Action Action { get; set; }

        public virtual AuthorizedGroup AuthorizedGroup { get; set; }
    }
}
