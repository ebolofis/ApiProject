namespace Pos_WebApi.Models.DTOModels
{
    public class ExcludedPricelistsDTO : IDTOModel<PosInfoDetail_Pricelist_Assoc>
    {
        public long Id { get; set; }
        public long? PosInfoDetailId { get; set; }
        public long? PricelistId { get; set; }
        public long? PosInfoId { get; set; }
        public long? GroupId { get; set; }
        public bool IsDeleted { get; set; }


        public PosInfoDetail_Pricelist_Assoc ToModel()
        {
            var model = new PosInfoDetail_Pricelist_Assoc()
            {
                PosInfoDetailId = this.PosInfoDetailId,
                PricelistId = this.PricelistId
            };
            return model;
        }

        public PosInfoDetail_Pricelist_Assoc UpdateModel(PosInfoDetail_Pricelist_Assoc model)
        {
            model.PosInfoDetailId = this.PosInfoDetailId;
            model.PricelistId = this.PricelistId;
            return model;
        }
    }
}