namespace Pos_WebApi.Models.DTOModels
{
    public class ProductReceipeDTO : IDTOModel<ProductRecipe>
    {
        public long Id { get; set; }
        public long? ProductId { get; set; }
        public long? UnitId { get; set; }
        public double? Qty { get; set; }
        public decimal? Price { get; set; }
        public byte? IsProduct { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
        public long? IngredientId { get; set; }
        public long? ItemsId { get; set; }
        public long? ProductAsIngridientId { get; set; }
        public double? DefaultQty { get; set; }
        public int? Sort { get; set; }
        public bool IsDeleted { get; set; }


        public ProductRecipe ToModel()
        {
            var model = new ProductRecipe()
            {
                Id = this.Id,
                ProductId = this.ProductId,
                UnitId = this.UnitId,
                Qty = this.Qty,
                Price = this.Price,
                IsProduct = this.IsProduct,
                MinQty = this.MinQty,
                MaxQty = this.MaxQty,
                IngredientId = this.IngredientId,
                ItemsId = this.ItemsId,
                ProductAsIngridientId = this.ProductAsIngridientId,
                DefaultQty = this.DefaultQty,
                Sort = this.Sort

            };
            return model;
        }

        public ProductRecipe UpdateModel(ProductRecipe model)
        {
            model.ProductId = this.ProductId;
            model.UnitId = this.UnitId;
            model.Qty = this.Qty;
            model.Price = this.Price;
            model.IsProduct = this.IsProduct;
            model.MinQty = this.MinQty;
            model.MaxQty = this.MaxQty;
            model.IngredientId = this.IngredientId;
            model.ItemsId = this.ItemsId;
            model.ProductAsIngridientId = this.ProductAsIngridientId;
            model.DefaultQty = this.DefaultQty;
            model.Sort = this.Sort;
            return model;
        }
    }
}
