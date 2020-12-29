namespace Pos_WebApi.Models.DTOModels
{
    public class PosInfoKdsAssocDTO : IDTOModel<PosInfoKdsAssoc>
    {
        public long Id { get; set; }
        public long? PosInfoId { get; set; }
        public long? KdsId { get; set; }
        public string PosInfoDescription { get; set; }
        public string KdsDescription { get; set; }
        public bool IsDeleted { get; set; }



        public PosInfoKdsAssoc ToModel()
        {
            var model = new PosInfoKdsAssoc()
            {
                Id = this.Id,
                PosInfoId = this.PosInfoId,
                KdsId = this.KdsId,
            };

            return model;
        }

        public PosInfoKdsAssoc UpdateModel(PosInfoKdsAssoc model)
        {
            model.PosInfoId = this.PosInfoId;
            model.KdsId = this.KdsId;

            return model;
        }
    }

}
