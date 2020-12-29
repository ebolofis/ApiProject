namespace Pos_WebApi.Models.DTOModels
{
    public class ProductExtrasDTO : IDTOModel<ProductExtras>
    {

        public long Id { get; set; }
        public long? ProductId { get; set; }
        public bool? IsRequired { get; set; }
        public long? IngredientId { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
        public long? UnitId { get; set; }
        public long? ItemsId { get; set; }
        public decimal? Price { get; set; }
        public long? ProductAsIngridientId { get; set; }
        public int? Sort { get; set; }
        public bool IsDeleted { get; set; }


        public ProductExtras ToModel()
        {
            var model = new ProductExtras()
            {
                Id = this.Id,
                ProductId = this.ProductId,
                IsRequired = this.IsRequired,
                IngredientId = this.IngredientId,
                MinQty = this.MinQty,
                MaxQty = this.MaxQty,
                UnitId = this.UnitId,
                ItemsId = this.ItemsId,
                Price = this.Price,
                ProductAsIngridientId = this.ProductAsIngridientId,
                Sort = this.Sort
            };
            return model;
        }

        public ProductExtras UpdateModel(ProductExtras model)
        {
            model.ProductId = this.ProductId;
            model.IsRequired = this.IsRequired;
            model.IngredientId = this.IngredientId;
            model.MinQty = this.MinQty;
            model.MaxQty = this.MaxQty;
            model.UnitId = this.UnitId;
            model.ItemsId = this.ItemsId;
            model.Price = this.Price;
            model.ProductAsIngridientId = this.ProductAsIngridientId;
            model.Sort = this.Sort;
            return model;
        }
    }
}
