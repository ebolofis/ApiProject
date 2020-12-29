namespace Pos_WebApi.Models.DTOModels
{
    public class CreditTransactionDTO
    {
        public long CreditAccountId { get; set; }
        public string CreditAccountDescription { get; set; }
        public long CreditCodeId { get; set; }
        public long AccountId { get; set; }
        public long StaffId { get; set; }
        public long PosInfoId { get; set; }
        public decimal Amount { get; set; }
        public string Code { get; set; }
        public decimal? Balance { get; set; }
    }
}
