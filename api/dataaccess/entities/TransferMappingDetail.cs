namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TransferMappingDetail
    {
        public long Id { get; set; }

        public long? TransferMappingsId { get; set; }

        public long? ProductCategoryId { get; set; }

        public long? ProductId { get; set; }

        public virtual TransferMapping TransferMapping { get; set; }
    }
}
