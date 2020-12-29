namespace Pos_WebApi.Models.DTOModels
{
    public class PosInfo_KitchenInstruction_AssocDTO : IDTOModel<PosInfo_KitchenInstruction_Assoc>
    {
        public long Id { get; set; }
        public long? PosInfoId { get; set; }
        public long? KitchenInstructionId { get; set; }
        public string PosInfoDescription { get; set; }
        public string KitchenInstructionDescription { get; set; }
        public bool IsDeleted { get; set; }
        public string KitchenId { get; set; }

        public PosInfo_KitchenInstruction_Assoc ToModel()
        {
            var model = new PosInfo_KitchenInstruction_Assoc()
            {
                Id = this.Id,
                PosInfoId = this.PosInfoId,
                KitchenInstructionId = this.KitchenInstructionId,
            };

            return model;
        }

        public PosInfo_KitchenInstruction_Assoc UpdateModel(PosInfo_KitchenInstruction_Assoc model)
        {
            model.PosInfoId = this.PosInfoId;
            model.KitchenInstructionId = this.KitchenInstructionId;

            return model;
        }
    }
}
