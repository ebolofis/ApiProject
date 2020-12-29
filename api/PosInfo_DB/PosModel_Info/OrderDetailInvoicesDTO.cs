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
    [Table("OrderDetailInvoices")]
    [DisplayName("OrderDetailInvoices")]
    public class OrderDetailInvoicesDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_OrderDetailInvoices")]
        public long Id { get; set; }

        [Column("OrderDetailId", Order = 1, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetailInvoices_OrderDetail")]
        [Association("OrderDetail", "OrderDetailId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> OrderDetailId { get; set; }

        [Column("StaffId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetailInvoices_Staff")]
        [Association("Staff", "StaffId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> StaffId { get; set; }

        [Column("PosInfoDetailId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetailInvoices_PosInfoDetail")]
        [Association("PosInfoDetail", "PosInfoDetailId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> PosInfoDetailId { get; set; }

        [Column("Counter", Order = 4, TypeName = "BIGINT")]
        public Nullable<long> Counter { get; set; }

        [Column("CreationTS", Order = 5, TypeName = "DATETIME")]
        public Nullable<System.DateTime> CreationTS { get; set; }

        [Column("IsPrinted", Order = 6, TypeName = "BIT")]
        public Nullable<bool> IsPrinted { get; set; }

        [Column("CustomerId", Order = 7, TypeName = "VARCHAR(50)")]
        public string CustomerId { get; set; }

        [Column("InvoicesId", Order = 8, TypeName = "BIGINT")]
        [ForeignKey("FK_OrderDetailInvoices_Invoices")]
        [Association("Invoices", "InvoicesId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> InvoicesId { get; set; }

        [Column("IsDeleted", Order = 9, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("OrderDetailIgredientsId", Order = 10, TypeName = "BIGINT")]
        [ForeignKey("FK_dbo.OrderDetailInvoices_dbo.OrderDetailIgredients_OrderDetailIgredientsId")]
        [Association("OrderDetailIgredients", "OrderDetailIgredientsId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> OrderDetailIgredientsId { get; set; }

        [Column("Price", Order = 11, TypeName = "MONEY")]
        public Nullable<decimal> Price { get; set; }

        [Column("Discount", Order = 12, TypeName = "MONEY")]
        public Nullable<decimal> Discount { get; set; }

        [Column("Net", Order = 13, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> Net { get; set; }

        [Column("VatRate", Order = 14, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> VatRate { get; set; }

        [Column("VatAmount", Order = 15, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> VatAmount { get; set; }

        [Column("VatId", Order = 16, TypeName = "BIGINT")]
        public Nullable<long> VatId { get; set; }

        [Column("TaxId", Order = 17, TypeName = "BIGINT")]
        public Nullable<long> TaxId { get; set; }

        [Column("VatCode", Order = 18, TypeName = "INT")]
        public Nullable<int> VatCode { get; set; }

        [Column("TaxAmount", Order = 19, TypeName = "DECIMAL(19,2)")]
        public Nullable<decimal> TaxAmount { get; set; }

        [Column("Qty", Order = 20, TypeName = "FLOAT")]
        public Nullable<double> Qty { get; set; }

        [Column("Total", Order = 21, TypeName = "DECIMAL(19,2)")]
        public Nullable<decimal> Total { get; set; }

        [Column("PricelistId", Order = 22, TypeName = "BIGINT")]
        [ForeignKey("FK_dbo.OrderDetailInvoices_dbo.Pricelist_PricelistId")]
        [Association("Pricelist", "PricelistId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PricelistId { get; set; }

        [Column("ProductId", Order = 23, TypeName = "BIGINT")]
        public Nullable<long> ProductId { get; set; }

        [Column("Description", Order = 24, TypeName = "NVARCHAR(150)")]
        public string Description { get; set; }

        [Column("ItemCode", Order = 25, TypeName = "NVARCHAR(150)")]
        public string ItemCode { get; set; }

        [Column("ItemRemark", Order = 26, TypeName = "NVARCHAR(150)")]
        public string ItemRemark { get; set; }

        [Column("InvoiceType", Order = 27, TypeName = "INT")]
        public Nullable<int> InvoiceType { get; set; }

        [Column("TableId", Order = 28, TypeName = "BIGINT")]
        public Nullable<long> TableId { get; set; }

        [Column("TableCode", Order = 29, TypeName = "NVARCHAR(50)")]
        public string TableCode { get; set; }

        [Column("RegionId", Order = 30, TypeName = "BIGINT")]
        public Nullable<long> RegionId { get; set; }

        [Column("OrderNo", Order = 31, TypeName = "BIGINT")]
        public Nullable<long> OrderNo { get; set; }

        [Column("OrderId", Order = 32, TypeName = "BIGINT")]
        public Nullable<long> OrderId { get; set; }

        [Column("IsExtra", Order = 33, TypeName = "BIT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_OrderDetailInvoices_IsExtra", NullDisplayText = "0")]//DefaultValue (Name, Value)
        public bool IsExtra { get; set; }

        [Column("PosInfoId", Order = 34, TypeName = "BIGINT")]
        public Nullable<long> PosInfoId { get; set; }

        [Column("EndOfDayId", Order = 35, TypeName = "BIGINT")]
        [ForeignKey("FK_dbo.OrderDetailInvoices_dbo.EndOfDay_EndOfDayId")]
        [Association("EndOfDay", "EndOfDayId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> EndOfDayId { get; set; }

        [Column("Abbreviation", Order = 36, TypeName = "NVARCHAR(50)")]
        public string Abbreviation { get; set; }

        [Column("DiscountId", Order = 37, TypeName = "BIGINT")]
        public Nullable<long> DiscountId { get; set; }

        [Column("SalesTypeId", Order = 38, TypeName = "BIGINT")]
        public Nullable<long> SalesTypeId { get; set; }

        [Column("ProductCategoryId", Order = 39, TypeName = "BIGINT")]
        public Nullable<long> ProductCategoryId { get; set; }

        [Column("CategoryId", Order = 40, TypeName = "BIGINT")]
        public Nullable<long> CategoryId { get; set; }

        [Column("ItemPosition", Order = 41, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_OrderDetailInvoices_ItemPosition", NullDisplayText = "0")]//DefaultValue (Name, Value)
        public int ItemPosition { get; set; }

        [Column("ItemSort", Order = 42, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_OrderDetailInvoices_ItemSort", NullDisplayText = "0")]//DefaultValue (Name, Value)
        public int ItemSort { get; set; }

        [Column("ItemRegion", Order = 43, TypeName = "NVARCHAR(MAX)")]
        public string ItemRegion { get; set; }

        [Column("RegionPosition", Order = 44, TypeName = "INT")]
        public Nullable<int> RegionPosition { get; set; }

        [Column("ItemBarcode", Order = 45, TypeName = "INT")]
        [Required]
        [DisplayFormatAttribute(DataFormatString = "DF_OrderDetailInvoices_ItemBarcode", NullDisplayText = "0")]//DefaultValue (Name, Value)
        public int ItemBarcode { get; set; }

        [Column("TotalBeforeDiscount", Order = 46, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> TotalBeforeDiscount { get; set; }

        [Column("TotalAfterDiscount", Order = 47, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> TotalAfterDiscount { get; set; }

        [Column("ReceiptSplitedDiscount", Order = 48, TypeName = "DECIMAL(18,2)")]
        public Nullable<decimal> ReceiptSplitedDiscount { get; set; }

        [Column("TableLabel", Order = 49, TypeName = "NVARCHAR(150)")]
        public string TableLabel { get; set; }
    }
}
