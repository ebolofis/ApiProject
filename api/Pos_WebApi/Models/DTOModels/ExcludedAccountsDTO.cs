namespace Pos_WebApi.Models.DTOModels
{
    public class ExcludedAccountsDTO : IDTOModel<PosInfoDetail_Excluded_Accounts>
    {
        public long Id { get; set; }
        public long? PosInfoDetailId { get; set; }
        public long? AccountId { get; set; }
        public long? PosInfoId { get; set; }
        public long? GroupId { get; set; }
        public bool IsDeleted { get; set; }

        public PosInfoDetail_Excluded_Accounts ToModel()
        {
            var model = new PosInfoDetail_Excluded_Accounts()
            {
                PosInfoDetailId = this.PosInfoDetailId,
                AccountId = this.AccountId
            };
            return model;
        }

        public PosInfoDetail_Excluded_Accounts UpdateModel(PosInfoDetail_Excluded_Accounts model)
        {
            model.PosInfoDetailId = this.PosInfoDetailId;
            model.AccountId = this.AccountId;
            return model;
        }
    }
}