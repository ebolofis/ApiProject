namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AllowedMealsPerBoard")]
    public partial class AllowedMealsPerBoard
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AllowedMealsPerBoard()
        {
            AllowedMealsPerBoardDetails = new HashSet<AllowedMealsPerBoardDetail>();
        }

        public long Id { get; set; }

        [StringLength(150)]
        public string BoardId { get; set; }

        [StringLength(350)]
        public string BoardDescription { get; set; }

        public int? AllowedMeals { get; set; }

        [Column(TypeName = "money")]
        public decimal? AllowedDiscountAmount { get; set; }

        public bool? IsDeleted { get; set; }

        public long? PriceListId { get; set; }

        public int? AllowedMealsChild { get; set; }

        [Column(TypeName = "money")]
        public decimal? AllowedDiscountAmountChild { get; set; }

        public virtual Pricelist Pricelist { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AllowedMealsPerBoardDetail> AllowedMealsPerBoardDetails { get; set; }
    }
}
