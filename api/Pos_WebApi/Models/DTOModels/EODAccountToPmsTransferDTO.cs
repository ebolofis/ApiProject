namespace Pos_WebApi.Models.DTOModels
{
    public class EODAccountToPmsTransferDTO : IDTOModel<EODAccountToPmsTransfer>
    {
        public long Id { get; set; }
        public long? PosInfoId { get; set; }
        public long? AccountId { get; set; }
        public long? PmsRoom { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }

        public EODAccountToPmsTransfer ToModel()
        {
            var model = new EODAccountToPmsTransfer()
            {
                Id = this.Id,
                PosInfoId = this.PosInfoId,
                AccountId = this.AccountId,
                PmsRoom = this.PmsRoom,
                Description = this.Description,
                Status = this.Status
            };

            return model;
        }

        public EODAccountToPmsTransfer UpdateModel(EODAccountToPmsTransfer model)
        {
            model.PosInfoId = this.PosInfoId;
            model.AccountId = this.AccountId;
            model.PmsRoom = this.PmsRoom;
            model.Description = this.Description;
            model.Status = this.Status;

            return model;
        }
    }

}
