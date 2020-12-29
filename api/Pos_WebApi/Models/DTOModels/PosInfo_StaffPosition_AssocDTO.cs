namespace Pos_WebApi.Models.DTOModels
{
    public class PosInfo_StaffPosition_AssocDTO : IDTOModel<PosInfo_StaffPositin_Assoc>
    {
        public long Id { get; set; }
        public long? PosInfoId { get; set; }
        public long? StaffPositionId { get; set; }
        public string PosInfoDescription { get; set; }
        public string StaffPositionDescription { get; set; }
        public bool IsDeleted { get; set; }



        public PosInfo_StaffPositin_Assoc ToModel()
        {
            var model = new PosInfo_StaffPositin_Assoc()
            {
                Id = this.Id,
                PosInfoId = this.PosInfoId,
                StaffPositionId = this.StaffPositionId,
            };

            return model;
        }

        public PosInfo_StaffPositin_Assoc UpdateModel(PosInfo_StaffPositin_Assoc model)
        {
            model.PosInfoId = this.PosInfoId;
            model.StaffPositionId = this.StaffPositionId;

            return model;
        }
    }

}
