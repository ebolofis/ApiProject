namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductSalesHistoryPerDay")]
    public partial class ProductSalesHistoryPerDay
    {
        public long Id { get; set; }

        public long? PosInfoId { get; set; }

        public long? DepartmentId { get; set; }

        public long? ProductId { get; set; }

        public long? ProductCategoryId { get; set; }

        public long? CategoryId { get; set; }

        public long? SalesTypeId { get; set; }

        public long? PriceListId { get; set; }

        public long? StaffId { get; set; }

        public long? InvoiceTypeId { get; set; }

        public long? VatId { get; set; }

        public long? EodId { get; set; }

        [StringLength(150)]
        public string ProductDescription { get; set; }

        [StringLength(150)]
        public string PosInfoDescription { get; set; }

        [StringLength(150)]
        public string DepartmentDescription { get; set; }

        [StringLength(150)]
        public string ProductCategoryDescription { get; set; }

        [StringLength(150)]
        public string CategoryDescription { get; set; }

        [StringLength(150)]
        public string SalesTypeDescription { get; set; }

        [StringLength(150)]
        public string PriceListDescription { get; set; }

        [StringLength(150)]
        public string InvoiceTypeDescription { get; set; }

        [StringLength(150)]
        public string StaffName { get; set; }

        [StringLength(50)]
        public string ProductCode { get; set; }

        [StringLength(50)]
        public string PriceListCode { get; set; }

        public bool? IsVoided { get; set; }

        public int? Covers { get; set; }

        [Column(TypeName = "money")]
        public decimal? Qty { get; set; }

        [Column(TypeName = "money")]
        public decimal? Total { get; set; }

        [Column(TypeName = "money")]
        public decimal? VatAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal? Net { get; set; }

        [Column(TypeName = "money")]
        public decimal? VatRate { get; set; }

        public int? InvoiceCount { get; set; }

        [Column(TypeName = "money")]
        public decimal? UnitPrice { get; set; }
    }
}
