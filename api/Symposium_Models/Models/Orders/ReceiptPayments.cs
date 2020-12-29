﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    public class ReceiptPayments
    {
        public long? ReceiptsId { get; set; }
        public long? EndOfDayId { get; set; }
        public string Abbreviation { get; set; }
        public long? ReceiptNo { get; set; }
        public short? InvoiceType { get; set; }
        public long? PosInfoId { get; set; }
        public long? AccountId { get; set; }
        public string AccountDescription { get; set; }
        public short? AccountType { get; set; }
        public long? AccountEODRoom { get; set; }
        public bool? SendsTransfer { get; set; }
        public decimal? Amount { get; set; }
        public short? TransactionType { get; set; }
        public string Room { get; set; }
        public int? RoomId { get; set; }
        public string GuestString { get; set; }
        public long? GuestId { get; set; }
        public int? ProfileNo { get; set; }
        public int? ReservationCode { get; set; }
        public long? CreditAccountId { get; set; }
        public long? CreditCodeId { get; set; }
        public string CreditAccountDescription { get; set; }
        public short CreditTransactionAction { get; set; }
        public long? HotelId { get; set; }
        public long? PMSPaymentId { get; set; }

        public bool isCheckedOut { get; set; }

        public virtual Receipts Receipts { get; set; }

        public ReceiptPayments()
        {
            isCheckedOut = false;
        }
    }

}
