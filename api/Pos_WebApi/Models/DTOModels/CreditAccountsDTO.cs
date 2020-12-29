using System;
using System.Collections.Generic;

namespace Pos_WebApi.Models.DTOModels
{
    public class CreditAccountsDTO
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long? EndOfDayId { get; set; }
        public DateTime? ActivateTS { get; set; }
        public DateTime? DeactivateTS { get; set; }
        public long CreditCodesId { get; set; }
        public string CreditCodesDescription { get; set; }
        public string CreditCodesCode { get; set; }
        public long? CreditAccountId { get; set; }
        public bool HasConnectedAccounts { get; set; }
        public ICollection<string> ConnectedCreditCodes { get; set; }
        public long? AccountId { get; set; }
        public ICollection<CreditAccountTransactions> CreditAccountTransactions { get; set; }
        public decimal? Balance { get; set; }
    }
}
