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
    [Table("RegionLockerProduct")]
    [DisplayName("RegionLockerProduct")]
    public class RegionLockerProductDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_RegionLockerProduct")]
        public long Id { get; set; }

        [Column("ProductId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_RegionLockerProduct_Product")]
        [Association("Product", "ProductId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> ProductId { get; set; }

        [Column("PriceListId", Order = 3, TypeName = "BIGINT")]
        [ForeignKey("FK_RegionLockerProduct_Pricelist")]
        [Association("Pricelist", "PriceListId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PriceListId { get; set; }

        [Column("Price", Order = 4, TypeName = "MONEY")]
        public Nullable<decimal> Price { get; set; }

        [Column("Discount", Order = 5, TypeName = "MONEY")]
        public Nullable<decimal> Discount { get; set; }

        [Column("SalesDescription", Order = 6, TypeName = "NVARCHAR(150)")]
        public string SalesDescription { get; set; }

        [Column("ReturnPaymentpId", Order = 7, TypeName = "BIGINT")]
        public Nullable<long> ReturnPaymentpId { get; set; }

        [Column("PaymentId", Order = 8, TypeName = "BIGINT")]
        public Nullable<long> PaymentId { get; set; }

        [Column("SaleId", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> SaleId { get; set; }

        [Column("PosInfoId", Order = 10, TypeName = "BIGINT")]
        [ForeignKey("FK_RegionLockerProduct_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> PosInfoId { get; set; }
    }
}
