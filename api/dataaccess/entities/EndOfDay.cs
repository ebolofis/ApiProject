namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EndOfDay")]
    public partial class EndOfDay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EndOfDay()
        {
            CreditAccounts = new HashSet<CreditAccount>();
            CreditTransactions = new HashSet<CreditTransaction>();
            OrderDetailInvoices = new HashSet<OrderDetailInvoice>();
            Transactions = new HashSet<Transaction>();
            TransferToPms = new HashSet<TransferToPm>();
            EndOfDayDetails = new HashSet<EndOfDayDetail>();
            EndOfDayPaymentAnalysis = new HashSet<EndOfDayPaymentAnalysi>();
            EndOfDayVoidsAnalysis = new HashSet<EndOfDayVoidsAnalysi>();
            Invoices = new HashSet<Invoice>();
            KitchenInstructionLoggers = new HashSet<KitchenInstructionLogger>();
            MealConsumptions = new HashSet<MealConsumption>();
            Orders = new HashSet<Order>();
        }

        public long Id { get; set; }

        public DateTime? FODay { get; set; }

        public long? PosInfoId { get; set; }

        public int? CloseId { get; set; }

        public decimal? Gross { get; set; }

        public decimal? Net { get; set; }

        public int? TicketsCount { get; set; }

        public int? ItemCount { get; set; }

        public decimal? TicketAverage { get; set; }

        public long? StaffId { get; set; }

        public decimal? Discount { get; set; }

        public DateTime? Datetime { get; set; }

        public decimal? Barcodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditAccount> CreditAccounts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailInvoice> OrderDetailInvoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransferToPm> TransferToPms { get; set; }

        public virtual PosInfo PosInfo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EndOfDayDetail> EndOfDayDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EndOfDayPaymentAnalysi> EndOfDayPaymentAnalysis { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EndOfDayVoidsAnalysi> EndOfDayVoidsAnalysis { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitchenInstructionLogger> KitchenInstructionLoggers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MealConsumption> MealConsumptions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
