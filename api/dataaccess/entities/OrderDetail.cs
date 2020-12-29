namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderDetail()
        {
            OrderDetailIgredients = new HashSet<OrderDetailIgredient>();
            OrderDetailInvoices = new HashSet<OrderDetailInvoice>();
            OrderDetailVatAnals = new HashSet<OrderDetailVatAnal>();
            TablePaySuggestions = new HashSet<TablePaySuggestion>();
        }

        public long Id { get; set; }

        public long? OrderId { get; set; }

        public long? ProductId { get; set; }

        public long? KitchenId { get; set; }

        public long? KdsId { get; set; }

        public int? PreparationTime { get; set; }

        public long? TableId { get; set; }

        public byte? Status { get; set; }

        public DateTime? StatusTS { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public long? PriceListDetailId { get; set; }

        public double? Qty { get; set; }

        public long? SalesTypeId { get; set; }

        public decimal? Discount { get; set; }

        public byte? PaidStatus { get; set; }

        public long? TransactionId { get; set; }

        public decimal? TotalAfterDiscount { get; set; }

        public long? GuestId { get; set; }

        public int? Couver { get; set; }

        public Guid? Guid { get; set; }

        public bool? IsDeleted { get; set; }

        public virtual Guest Guest { get; set; }

        public virtual Order Order { get; set; }

        public virtual PricelistDetail PricelistDetail { get; set; }

        public virtual Product Product { get; set; }

        public virtual SalesType SalesType { get; set; }

        public virtual Table Table { get; set; }

        public virtual Transaction Transaction { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailIgredient> OrderDetailIgredients { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailInvoice> OrderDetailInvoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailVatAnal> OrderDetailVatAnals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TablePaySuggestion> TablePaySuggestions { get; set; }
    }
}
