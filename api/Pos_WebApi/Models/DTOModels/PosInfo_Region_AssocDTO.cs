namespace Pos_WebApi.Models.DTOModels
{
    public class PosInfo_Region_AssocDTO : IDTOModel<PosInfo_Region_Assoc>
    {
        public long Id { get; set; }
        public long? PosInfoId { get; set; }
        public long? RegionId { get; set; }
        public string PosInfoDescription { get; set; }
        public string RegionDescription { get; set; }
        public bool IsDeleted { get; set; }


        public PosInfo_Region_Assoc ToModel()
        {
            var model = new PosInfo_Region_Assoc()
            {
                Id = this.Id,
                PosInfoId = this.PosInfoId,
                RegionId = this.RegionId,
            };

            return model;
        }

        public PosInfo_Region_Assoc UpdateModel(PosInfo_Region_Assoc model)
        {
            model.PosInfoId = this.PosInfoId;
            model.RegionId = this.RegionId;

            return model;
        }
    }
}
