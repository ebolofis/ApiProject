namespace Pos_WebApi.Models.DTOModels
{
    public class LockerDBO
    {
        public int Quantity { get; set; }
        public long AccountId { get; set; }
        public long CreditAccountId { get; set; }
        public string CreditAccountDescription { get; set; }
        public long CreditCodeId { get; set; }
        public decimal UnitPrice { get; set; }
        public long PosInfoId { get; set; }
        public long StaffId { get; set; }
        public long RegionLockerProductId { get; set; }
        public long ExtecrName { get; set; }
        //public long RegionId { get; set; }
    }
}
