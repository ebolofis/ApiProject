using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [Table("ProductSalesHistoryPerDay")]
    [DisplayName("ProductSalesHistoryPerDay")]
    public class ProductSalesHistoryPerDayDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_ProductSalesHistoryPerDay")]
        public long Id { get; set; }

        [Column("PosInfoId", Order = 2, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("DepartmentId", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> DepartmentId { get; set; }

        [Column("ProductId", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> ProductId { get; set; }

        [Column("ProductCategoryId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> ProductCategoryId { get; set; }

        [Column("CategoryId", Order = 6, TypeName = "BIGINT")]
        public Nullable<long> CategoryId { get; set; }

        [Column("SalesTypeId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> SalesTypeId { get; set; }

        [Column("PriceListId", Order = 8, TypeName = "BIGINT")]
        public Nullable<long> PriceListId { get; set; }

        [Column("StaffId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> StaffId { get; set; }

        [Column("InvoiceTypeId", Order = 10, TypeName = "BIGINT")]
        public Nullable<long> InvoiceTypeId { get; set; }

        [Column("VatId", Order = 11, TypeName = "BIGINT")]
        public Nullable<long> VatId { get; set; }

        [Column("EodId", Order = 12, TypeName = "BIGINT")]
        public Nullable<long> EodId { get; set; }

        [Column("ProductDescription", Order = 13, TypeName = "NVARCHAR(150)")]
        public string ProductDescription { get; set; }

        [Column("PosInfoDescription", Order = 14, TypeName = "NVARCHAR(150)")]
        public string PosInfoDescription { get; set; }

        [Column("DepartmentDescription", Order = 15, TypeName = "NVARCHAR(150)")]
        public string DepartmentDescription { get; set; }

        [Column("ProductCategoryDescription", Order = 16, TypeName = "NVARCHAR(150)")]
        public string ProductCategoryDescription { get; set; }

        [Column("CategoryDescription", Order = 17, TypeName = "NVARCHAR(150)")]
        public string CategoryDescription { get; set; }

        [Column("SalesTypeDescription", Order = 18, TypeName = "NVARCHAR(150)")]
        public string SalesTypeDescription { get; set; }

        [Column("PriceListDescription", Order = 19, TypeName = "NVARCHAR(150)")]
        public string PriceListDescription { get; set; }

        [Column("InvoiceTypeDescription", Order = 20, TypeName = "NVARCHAR(150)")]
        public string InvoiceTypeDescription { get; set; }

        [Column("StaffName", Order = 21, TypeName = "NVARCHAR(150)")]
        public string StaffName { get; set; }

        [Column("ProductCode", Order = 22, TypeName = "NVARCHAR(50)")]
        public string ProductCode { get; set; }

        [Column("PriceListCode", Order = 23, TypeName = "NVARCHAR(50)")]
        public string PriceListCode { get; set; }

        [Column("IsVoided", Order = 24, TypeName = "BIT")]
        public Nullable<bool> IsVoided { get; set; }

        [Column("Covers", Order = 25, TypeName = "INT")]
        public Nullable<int> Covers { get; set; }

        [Column("Qty", Order = 26, TypeName = "MONEY")]
        public Nullable<decimal> Qty { get; set; }

        [Column("Total", Order = 27, TypeName = "MONEY")]
        public Nullable<decimal> Total { get; set; }

        [Column("VatAmount", Order = 28, TypeName = "MONEY")]
        public Nullable<decimal> VatAmount { get; set; }

        [Column("Net", Order = 29, TypeName = "MONEY")]
        public Nullable<decimal> Net { get; set; }

        [Column("VatRate", Order = 30, TypeName = "MONEY")]
        public Nullable<decimal> VatRate { get; set; }

        [Column("InvoiceCount", Order = 31, TypeName = "INT")]
        public Nullable<int> InvoiceCount { get; set; }

        [Column("UnitPrice", Order = 32, TypeName = "MONEY")]
        public Nullable<decimal> UnitPrice { get; set; }
    }
}
