namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            BoardMeals = new HashSet<BoardMeal>();
            OrderDetails = new HashSet<OrderDetail>();
            PageButtons = new HashSet<PageButton>();
            PricelistDetails = new HashSet<PricelistDetail>();
            ProductBarcodes = new HashSet<ProductBarcode>();
            ProductExtras = new HashSet<ProductExtra>();
            ProductForBarcodeEods = new HashSet<ProductForBarcodeEod>();
            ProductRecipes = new HashSet<ProductRecipe>();
            RegionLockerProducts = new HashSet<RegionLockerProduct>();
            TransferMappings = new HashSet<TransferMapping>();
        }

        public long Id { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [StringLength(500)]
        public string ExtraDescription { get; set; }

        [StringLength(500)]
        public string SalesDescription { get; set; }

        public double? Qty { get; set; }

        public long? UnitId { get; set; }

        public int? PreparationTime { get; set; }

        public long? KdsId { get; set; }

        public long? KitchenId { get; set; }

        [StringLength(500)]
        public string ImageUri { get; set; }

        public long? ProductCategoryId { get; set; }

        [StringLength(150)]
        public string Code { get; set; }

        public bool? IsCustom { get; set; }

        public long? KitchenRegionId { get; set; }

        public bool? IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BoardMeal> BoardMeals { get; set; }

        public virtual Kd Kd { get; set; }

        public virtual Kitchen Kitchen { get; set; }

        public virtual KitchenRegion KitchenRegion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PageButton> PageButtons { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PricelistDetail> PricelistDetails { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }

        public virtual Unit Unit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductBarcode> ProductBarcodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductExtra> ProductExtras { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductForBarcodeEod> ProductForBarcodeEods { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductRecipe> ProductRecipes { get; set; }

        public virtual ProductsForEOD ProductsForEOD { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegionLockerProduct> RegionLockerProducts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransferMapping> TransferMappings { get; set; }
    }
}
