using log4net;
using System;
using System.Collections.Generic;

namespace Pos_WebApi.Models.DTOModels
{
    public class BOProductDTOModel : Product, IDTOModel<Product>
    {
        //  public Product Product { get; set; }
        public ICollection<ProductRecipeExt> ProductRecipeDTO { get; set; }
        public ICollection<ProductExtrasExt> ProductExtrasDTO { get; set; }
        public ICollection<ProductBarcodesExt> ProductBarcodeDTO { get; set; }
        public int EntityStatus { get; set; }
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Product ToModel()
        {
            try {
                Product pr = new Product
                {
                    Id = this.Id,
                    Description = this.Description,
                    ExtraDescription = this.ExtraDescription,
                    SalesDescription = this.SalesDescription,
                    Qty = this.Qty,
                    UnitId = this.UnitId,
                    PreparationTime = this.PreparationTime,
                    KdsId = this.KdsId,
                    KitchenId = this.KitchenId,
                    ImageUri = this.ImageUri,
                    ProductCategoryId = this.ProductCategoryId,
                    Code = this.Code,
                    IsCustom = this.IsCustom,
                    KitchenRegionId = this.KitchenRegionId,
                    IsDeleted = this.IsDeleted
                };
                foreach (var pe in ProductExtrasDTO)
                {
                    ProductExtras ex = new ProductExtras()
                    {
                        Id = pe.Id,
                        ProductId = pe.ProductId,
                        IsRequired = pe.IsRequired,
                        IngredientId = pe.IngredientId,
                        MinQty = pe.MinQty,
                        MaxQty = pe.MaxQty,
                        UnitId = pe.UnitId,
                        ItemsId = pe.ItemsId,
                        ProductAsIngridientId = pe.ProductAsIngridientId,
                        Price = pe.Price,
                        Sort = pe.Sort

                    };
                    pr.ProductExtras.Add(ex);
                }
                foreach (var pre in ProductRecipeDTO)
                {
                    ProductRecipe ex = new ProductRecipe()
                    {
                        Id = pre.Id,
                        ProductId = pre.ProductId,
                        IngredientId = pre.IngredientId,
                        UnitId = pre.UnitId,
                        Qty = pre.Qty,
                        Price = pre.Price,
                        IsProduct = pre.IsProduct,
                        MinQty = pre.MinQty,
                        MaxQty = pre.MaxQty,
                        ItemsId = pre.ItemsId,
                        ProductAsIngridientId = pre.ProductAsIngridientId,
                        DefaultQty = pre.DefaultQty,
                        Sort = pre.Sort

                    };
                    pr.ProductRecipe.Add(ex);
                }
                foreach (var pb in ProductBarcodeDTO)
                {
                    ProductBarcodes ex = new ProductBarcodes()
                    {
                        Id = pb.Id,
                        Barcode = pb.Barcode,
                        ProductId = pb.ProductId,
                        Type = pb.Type
                    };
                    pr.ProductBarcodes.Add(ex);
                }

                return pr;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
               // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Product ToModel() : " + Id + " | " + ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            }
            return null;
        }


        public Product UpdateModel(Product model)
        {
            throw new NotImplementedException();
        }

    }
    public class ProductBarcodesExt : ProductBarcodes
    {
        public int EntityStatus { get; set; }
    }
    public class ProductExtrasExt : ProductExtras
    {
        public int EntityStatus { get; set; }
    }
    public class ProductRecipeExt : ProductRecipe
    {
        public int EntityStatus { get; set; }
    }

   
}
