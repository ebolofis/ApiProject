namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AllowedMealsPerBoardDetail
    {
        public long Id { get; set; }

        public long? ProductCategoryId { get; set; }

        public long? AllowedMealsPerBoardId { get; set; }

        public virtual AllowedMealsPerBoard AllowedMealsPerBoard { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
    }
}
