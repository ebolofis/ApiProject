namespace Pos_WebApi.Models.DTOModels
{
    public class ProductBarcodeDTO : IDTOModel<ProductBarcodes>
    {
        public long Id { get; set; }
        public string Barcode { get; set; }
        public long? ProductId { get; set; }
        public byte? Type { get; set; }
        public bool IsDeleted { get; set; }


        public ProductBarcodes ToModel()
        {
            var model = new ProductBarcodes()
            {
                Id = this.Id,
                Barcode = this.Barcode,
                ProductId = this.ProductId,
                Type = this.Type
            };
            return model;
        }

        public ProductBarcodes UpdateModel(ProductBarcodes model)
        {
            model.Barcode = this.Barcode;
            model.ProductId = this.ProductId;
            model.Type = this.Type;
            return model;
        }
    }
}
