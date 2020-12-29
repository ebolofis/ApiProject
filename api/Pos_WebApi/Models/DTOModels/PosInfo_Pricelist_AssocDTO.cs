namespace Pos_WebApi.Models.DTOModels
{


    public class PosInfo_Pricelist_AssocDTO : IDTOModel<PosInfo_Pricelist_Assoc>
    {
        public long Id { get; set; }
        public long? PosInfoId { get; set; }
        public long? PricelistId { get; set; }
        public string PosInfoDescription { get; set; }
        public string PricelistDescription { get; set; }
        public bool IsDeleted { get; set; }


        public PosInfo_Pricelist_Assoc ToModel()
        {
            var model = new PosInfo_Pricelist_Assoc()
            {
                Id = this.Id,
                PosInfoId = this.PosInfoId,
                PricelistId = this.PricelistId,
            };

            return model;
        }

        public PosInfo_Pricelist_Assoc UpdateModel(PosInfo_Pricelist_Assoc model)
        {
            model.PosInfoId = this.PosInfoId;
            model.PricelistId = this.PricelistId;

            return model;
        }
    }

}
