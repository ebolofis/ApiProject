namespace Pos_WebApi.Models.DTOModels
{
    public class ProductPricesDTO : IDTOModel<PricelistDetail>
    {
        public long Id { get; set; }
        public long? PricelistId { get; set; }
        public long? ProductId { get; set; }
        public decimal? Price { get; set; }
        public long? VatId { get; set; }
        public long? TaxId { get; set; }
        public long? IngredientId { get; set; }
        public decimal? PriceWithout { get; set; }
        public int? MinRequiredExtras { get; set; }
        public bool IsDeleted { get; set; }

        public PricelistDetail ToModel()
        {
            var model = new PricelistDetail()
            {
                PricelistId = this.PricelistId,
                ProductId = this.ProductId,
                Price = this.Price,
                VatId = this.VatId,
                TaxId = this.TaxId
            };
            return model;
        }

        public PricelistDetail UpdateModel(PricelistDetail model)
        {
            model.PricelistId = this.PricelistId;
            model.ProductId = this.ProductId;
            model.Price = this.Price;
            model.VatId = this.VatId;
            model.TaxId = this.TaxId;
            return model;
        }
    }
}