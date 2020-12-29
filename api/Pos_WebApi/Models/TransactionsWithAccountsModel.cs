using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos_WebApi.Models
{
    public partial class TransactionsWithAccounts
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public short AccountType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public long? GuestId { get; set; }
        public string Room { get; set; }
        public string GuestName { get; set; }
        public long InvoiceId { get; set; }

    }
}
