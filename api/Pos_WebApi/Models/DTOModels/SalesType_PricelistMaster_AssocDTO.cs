namespace Pos_WebApi.Models.DTOModels
{
    public class SalesType_PricelistMaster_AssocDTO : IDTOModel<SalesType_PricelistMaster_Assoc>
    {

        public long Id { get; set; }
        public long? PricelistMasterId { get; set; }
        public long? SalesTypeId { get; set; }
        public bool? IsDeleted { get; set; }
        public string SalesTypeDescription { get; set; }

        public SalesType_PricelistMaster_Assoc ToModel()
        {
            var model = new SalesType_PricelistMaster_Assoc()
            {
                Id = this.Id,
                PricelistMasterId = this.PricelistMasterId,
                SalesTypeId = this.SalesTypeId
            };
            return model;

        }

        public SalesType_PricelistMaster_Assoc UpdateModel(SalesType_PricelistMaster_Assoc model)
        {
            model.PricelistMasterId = this.PricelistMasterId;
            model.SalesTypeId = this.SalesTypeId;
            return model;
        }
    }
}