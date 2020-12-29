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
    [Table("PosInfo")]
    [DisplayName("PosInfo")]
    public class PosInfoDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PosInfo")]
        public long Id { get; set; }

        [Column("Code", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("Description", Order = 3, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("FODay", Order = 4, TypeName = "DATETIME")]
        public Nullable<System.DateTime> FODay { get; set; }

        [Column("CloseId", Order = 5, TypeName = "BIGINT")]
        public Nullable<long> CloseId { get; set; }

        [Column("IPAddress", Order = 6, TypeName = "NVARCHAR(50)")]
        public string IPAddress { get; set; }

        [Column("Type", Order = 7, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("wsIP", Order = 8, TypeName = "NVARCHAR(50)")]
        public string wsIP { get; set; }

        [Column("wsPort", Order = 9, TypeName = "NVARCHAR(50)")]
        public string wsPort { get; set; }

        [Column("DepartmentId", Order = 10, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfo_Department")]
        [Association("Department", "DepartmentId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MinLength(-1)]
        public Nullable<long> DepartmentId { get; set; }

        [Column("FiscalName", Order = 11, TypeName = "NVARCHAR(200)")]
        public string FiscalName { get; set; }     

        [Column("FiscalType", Order = 12, TypeName = "TINYINT")]
        public Nullable<Int16> FiscalType { get; set; }

        [Column("IsOpen", Order = 13, TypeName = "BIT")]
        public Nullable<bool> IsOpen { get; set; }

        [Column("ReceiptCount", Order = 14, TypeName = "BIGINT")]
        public Nullable<long> ReceiptCount { get; set; }

        [Column("ResetsReceiptCounter", Order = 15, TypeName = "BIT")]
        public Nullable<bool> ResetsReceiptCounter { get; set; }

        [Column("Theme", Order = 16, TypeName = "NVARCHAR(50)")]
        public string Theme { get; set; }

        [Column("AccountId", Order = 17, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfo_Accounts")]
        [Association("Accounts", "AccountId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> AccountId { get; set; }

        [Column("LogInToOrder", Order = 18, TypeName = "BIT")]
        public Nullable<bool> LogInToOrder { get; set; }

        [Column("ClearTableManually", Order = 19, TypeName = "BIT")]
        public Nullable<bool> ClearTableManually { get; set; }

        [Column("ViewOnly", Order = 20, TypeName = "BIT")]
        public Nullable<bool> ViewOnly { get; set; }

        [Column("IsDeleted", Order = 21, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("InvoiceSumType", Order = 22, TypeName = "INT")]
        public Nullable<int> InvoiceSumType { get; set; }

        [Column("LoginToOrderMode", Order = 23, TypeName = "SMALLINT")]
        public Nullable<short> LoginToOrderMode { get; set; }

        [Column("KeyboardType", Order = 24, TypeName = "SMALLINT")]
        public Nullable<short> KeyboardType { get; set; }

        [Column("CustomerDisplayGif", Order = 25, TypeName = "VARCHAR(200)")]
        public string CustomerDisplayGif { get; set; }

        [Column("DefaultHotelId", Order = 26, TypeName = "INT")]
        public Nullable<int> DefaultHotelId { get; set; }

        [Column("NfcDevice", Order = 27, TypeName = "NVARCHAR(200)")]
        public string NfcDevice { get; set; }

        [Column("Configuration", Order = 28, TypeName = "NVARCHAR(100)")]
        public string Configuration { get; set; }
    }
}
