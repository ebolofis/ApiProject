
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Pos_WebApi
{

using System;
    using System.Collections.Generic;
    
public partial class Product
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Product()
    {

        this.BoardMeals = new HashSet<BoardMeals>();

        this.Combo = new HashSet<Combo>();

        this.ComboDetail = new HashSet<ComboDetail>();

        this.DA_LoyalRedeemFreeProduct = new HashSet<DA_LoyalRedeemFreeProduct>();

        this.DA_OrderDetails = new HashSet<DA_OrderDetails>();

        this.DA_ShortageProds = new HashSet<DA_ShortageProds>();

        this.ExternalProductMapping = new HashSet<ExternalProductMapping>();

        this.OrderDetail = new HashSet<OrderDetail>();

        this.PageButton = new HashSet<PageButton>();

        this.PricelistDetail = new HashSet<PricelistDetail>();

        this.ProductBarcodes = new HashSet<ProductBarcodes>();

        this.ProductExtras = new HashSet<ProductExtras>();

        this.ProductForBarcodeEod = new HashSet<ProductForBarcodeEod>();

        this.ProductRecipe = new HashSet<ProductRecipe>();

        this.ProductsForEOD = new HashSet<ProductsForEOD>();

        this.RegionLockerProduct = new HashSet<RegionLockerProduct>();

        this.TransferMappings = new HashSet<TransferMappings>();

    }


    public long Id { get; set; }

    public string Description { get; set; }

    public string ExtraDescription { get; set; }

    public Nullable<double> Qty { get; set; }

    public Nullable<long> UnitId { get; set; }

    public string SalesDescription { get; set; }

    public Nullable<int> PreparationTime { get; set; }

    public Nullable<long> KdsId { get; set; }

    public Nullable<long> KitchenId { get; set; }

    public string ImageUri { get; set; }

    public Nullable<long> ProductCategoryId { get; set; }

    public string Code { get; set; }

    public Nullable<bool> IsCustom { get; set; }

    public Nullable<long> KitchenRegionId { get; set; }

    public Nullable<bool> IsDeleted { get; set; }

    public Nullable<bool> IsCombo { get; set; }

    public Nullable<bool> IsComboItem { get; set; }

    public Nullable<bool> IsReturnItem { get; set; }

    public Nullable<long> DAId { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<BoardMeals> BoardMeals { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Combo> Combo { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ComboDetail> ComboDetail { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_LoyalRedeemFreeProduct> DA_LoyalRedeemFreeProduct { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_OrderDetails> DA_OrderDetails { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_ShortageProds> DA_ShortageProds { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ExternalProductMapping> ExternalProductMapping { get; set; }

    public virtual Kds Kds { get; set; }

    public virtual Kitchen Kitchen { get; set; }

    public virtual KitchenRegion KitchenRegion { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<OrderDetail> OrderDetail { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PageButton> PageButton { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PricelistDetail> PricelistDetail { get; set; }

    public virtual ProductCategories ProductCategories { get; set; }

    public virtual Units Units { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ProductBarcodes> ProductBarcodes { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ProductExtras> ProductExtras { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ProductForBarcodeEod> ProductForBarcodeEod { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ProductRecipe> ProductRecipe { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ProductsForEOD> ProductsForEOD { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<RegionLockerProduct> RegionLockerProduct { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<TransferMappings> TransferMappings { get; set; }

}

}