namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InvoiceShippingDetail
    {
        public long Id { get; set; }

        public long? InvoicesId { get; set; }

        public long? ShippingAddressId { get; set; }

        [StringLength(300)]
        public string ShippingAddress { get; set; }

        [StringLength(100)]
        public string ShippingCity { get; set; }

        [StringLength(50)]
        public string ShippingZipCode { get; set; }

        public long? BillingAddressId { get; set; }

        [StringLength(300)]
        public string BillingAddress { get; set; }

        [StringLength(100)]
        public string BillingCity { get; set; }

        [StringLength(50)]
        public string BillingZipCode { get; set; }

        [StringLength(50)]
        public string Floor { get; set; }

        [StringLength(500)]
        public string CustomerRemarks { get; set; }

        [StringLength(500)]
        public string StoreRemarks { get; set; }

        [StringLength(500)]
        public string CustomerName { get; set; }

        public double? Longtitude { get; set; }

        public double? Latitude { get; set; }

        [StringLength(150)]
        public string Phone { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
