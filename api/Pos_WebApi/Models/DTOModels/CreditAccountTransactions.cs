using System;

namespace Pos_WebApi.Models.DTOModels
{
    public class CreditAccountTransactions
    {
        public long? AccountId { get; set; }
        public long? TransactionId { get; set; }
        public long? CreditTransactionId { get; set; }
        public string CreditAccountTransactionsDescription { get; set; }
        public Nullable<byte> Type { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreationTS { get; set;
        }
    }
}
