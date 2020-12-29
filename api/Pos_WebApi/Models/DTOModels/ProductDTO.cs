using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos_WebApi.Models.DTOModels
{
    public class ProductDTO : IDTOModel<Product>
    {
        public ProductDTO()
        {
            ProductRecipe = new HashSet<ProductReceipeDTO>();
            ProductExtras = new HashSet<ProductExtrasDTO>();
            ProductBarcodes = new HashSet<ProductBarcodeDTO>();
            ProductPrices = new HashSet<ProductPricesDTO>();
        }

        public long Id { get; set; }
        public string Description { get; set; }
        public string ExtraDescription { get; set; }
        public double? Qty { get; set; }
        public long? UnitId { get; set; }
        public string SalesDescription { get; set; }
        public int? PreparationTime { get; set; }
        public long? KdsId { get; set; }
        public long? KitchenId { get; set; }
        public string ImageUri { get; set; }
        public long? ProductCategoryId { get; set; }
        public string Code { get; set; }
        public bool? IsReturnItem { get; set; }
        public bool? IsCustom { get; set; }
        public long? KitchenRegionId { get; set; }
        public bool? IsDeleted { get; set; }


        public ICollection<ProductReceipeDTO> ProductRecipe { get; set; }
        public ICollection<ProductExtrasDTO> ProductExtras { get; set; }
        public ICollection<ProductBarcodeDTO> ProductBarcodes { get; set; }
        public ICollection<ProductPricesDTO> ProductPrices { get; set; }

        public Product ToModel()
        {
            var model = new Product()
            {
                Id = this.Id,
                Description = this.Description,
                ExtraDescription = this.ExtraDescription,
                Qty = this.Qty,
                UnitId = this.UnitId,
                SalesDescription = this.SalesDescription,
                PreparationTime = this.PreparationTime,
                KdsId = this.KdsId,
                KitchenId = this.KitchenId,
                ImageUri = this.ImageUri,
                ProductCategoryId = this.ProductCategoryId,
                Code = this.Code,
                IsReturnItem = this.IsReturnItem,
                IsCustom = this.IsCustom,
                KitchenRegionId = this.KitchenRegionId,
                IsDeleted = this.IsDeleted
            };

            foreach (var det in this.ProductRecipe)
            {
                model.ProductRecipe.Add(det.ToModel());
            }

            foreach (var det in this.ProductExtras)
            {
                model.ProductExtras.Add(det.ToModel());
            }

            foreach (var det in this.ProductBarcodes)
            {
                model.ProductBarcodes.Add(det.ToModel());
            }

            foreach (var det in this.ProductPrices)
            {
                model.PricelistDetail.Add(det.ToModel());
            }
            return model;
        }

        public Product UpdateModel(Product model)
        {
            model.Description = this.Description;
            model.ExtraDescription = this.ExtraDescription;
            model.Qty = this.Qty;
            model.UnitId = this.UnitId;
            model.SalesDescription = this.SalesDescription;
            model.PreparationTime = this.PreparationTime;
            model.KdsId = this.KdsId;
            model.KitchenId = this.KitchenId;
            model.ImageUri = this.ImageUri;
            model.ProductCategoryId = this.ProductCategoryId;
            model.Code = this.Code;
            model.IsReturnItem = this.IsReturnItem;
            model.IsCustom = this.IsCustom;
            model.KitchenRegionId = this.KitchenRegionId;
            model.IsDeleted = this.IsDeleted;


            foreach (var det in this.ProductExtras.Where(w => w.IsDeleted == false))
            {
                if (det.Id == 0)
                    model.ProductExtras.Add(det.ToModel());
                else
                {
                    var cur = model.ProductExtras.FirstOrDefault(x => x.Id == det.Id);
                    if (cur != null)
                    {
                        det.UpdateModel(det.ToModel());
                    }

                }
            }

            foreach (var det in this.ProductRecipe.Where(w => w.IsDeleted == false))
            {
                if (det.Id == 0)
                    model.ProductRecipe.Add(det.ToModel());
                else
                {
                    var cur = model.ProductRecipe.FirstOrDefault(x => x.Id == det.Id);
                    if (cur != null)
                    {
                        det.UpdateModel(det.ToModel());
                    }

                }
            }

            foreach (var det in this.ProductBarcodes.Where(w => w.IsDeleted == false))
            {
                if (det.Id == 0)
                    model.ProductBarcodes.Add(det.ToModel());
                else
                {
                    var cur = model.ProductBarcodes.FirstOrDefault(x => x.Id == det.Id);
                    if (cur != null)
                    {
                        det.UpdateModel(det.ToModel());
                    }

                }
            }


            foreach (var det in this.ProductPrices.Where(w => w.IsDeleted == false))
            {
                if (det.Id == 0)
                    model.PricelistDetail.Add(det.ToModel());
                else
                {
                    var cur = model.PricelistDetail.FirstOrDefault(x => x.Id == det.Id);
                    if (cur != null)
                    {
                        det.UpdateModel(det.ToModel());
                    }

                }
            }

            return model;
        }

        public ProductDTO ModelToDTO(Product dbmodel) {
            ProductDTO ret = new ProductDTO {
                Id = dbmodel.Id,
                Description = dbmodel.Description,
                ExtraDescription = dbmodel.ExtraDescription,
                Qty = dbmodel.Qty,
                UnitId = dbmodel.UnitId,
                SalesDescription = dbmodel.SalesDescription,
                PreparationTime = dbmodel.PreparationTime,
                KdsId = dbmodel.KdsId,
                KitchenId = dbmodel.KitchenId,
                ImageUri = dbmodel.ImageUri,
                ProductCategoryId = dbmodel.ProductCategoryId,
                Code = dbmodel.Code,
                IsReturnItem = dbmodel.IsReturnItem,
                IsCustom = dbmodel.IsCustom,
                KitchenRegionId = dbmodel.KitchenRegionId,
                IsDeleted = dbmodel.IsDeleted,
                ProductRecipe = dbmodel.ProductRecipe.Select(ss => new ProductReceipeDTO {
                    Id = ss.Id, ProductId = ss.ProductId, UnitId = ss.UnitId, Qty = ss.Qty, Price = ss.Price,
                    IsProduct = ss.IsProduct, MinQty = ss.MinQty, MaxQty = ss.MaxQty,
                    IngredientId = ss.IngredientId, ItemsId = ss.ItemsId, ProductAsIngridientId = ss.ProductAsIngridientId,
                    DefaultQty = ss.DefaultQty, Sort = ss.Sort, IsDeleted = false
                }).OrderBy(ss => ss.Sort).ToList(),
                ProductExtras = dbmodel.ProductExtras.Select(ss => new ProductExtrasDTO {
                    Id = ss.Id, ProductId = ss.ProductId, IsRequired = ss.IsRequired, IngredientId = ss.IngredientId,
                    MinQty = ss.MinQty, MaxQty = ss.MaxQty, UnitId = ss.UnitId, ItemsId = ss.ItemsId,
                    Price = ss.Price, ProductAsIngridientId = ss.ProductAsIngridientId, Sort = ss.Sort, IsDeleted = false
                }).OrderBy(ss => ss.Sort).ToList(),
                ProductBarcodes = dbmodel.ProductBarcodes.Select(ss => new ProductBarcodeDTO {
                    Id = ss.Id, Barcode = ss.Barcode, ProductId = ss.ProductId, Type = ss.Type, IsDeleted = false
                }).ToList(),
                ProductPrices = dbmodel.PricelistDetail.Select(ss => new ProductPricesDTO {
                    Id = ss.Id,
                    PricelistId = ss.PricelistId,
                    ProductId = ss.ProductId,
                    Price = ss.Price,
                    VatId = ss.VatId,
                    TaxId = ss.TaxId
                }).ToList(),
            };
            return ret;
        }
    }
}
