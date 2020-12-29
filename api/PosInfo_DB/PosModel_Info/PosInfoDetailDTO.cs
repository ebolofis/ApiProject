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
    [Table("PosInfoDetail")]
    [DisplayName("PosInfoDetail")]
    public class PosInfoDetailDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_PosInfoDetail")]
        public long Id { get; set; }

        [Column("PosInfoId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfoDetail_PosInfo")]
        [Association("PosInfo", "PosInfoId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]
        [MinLength(1)]
        public Nullable<long> PosInfoId { get; set; }

        [Column("Counter", Order = 3, TypeName = "BIGINT")]
        public Nullable<long> Counter { get; set; }

        [Column("Abbreviation", Order = 4, TypeName = "NVARCHAR(50)")]
        public string Abbreviation { get; set; }

        [Column("Description", Order = 5, TypeName = "NVARCHAR(250)")]
        public string Description { get; set; }

        [Column("ResetsAfterEod", Order = 6, TypeName = "BIT")]
        public Nullable<bool> ResetsAfterEod { get; set; }

        [Column("InvoiceId", Order = 7, TypeName = "SMALLINT")]
        public Nullable<short> InvoiceId { get; set; }

        [Column("ButtonDescription", Order = 8, TypeName = "NVARCHAR(250)")]
        public string ButtonDescription { get; set; }

        [Column("Status", Order = 9, TypeName = "SMALLINT")]
        public Nullable<short> Status { get; set; }

        [Column("CreateTransaction", Order = 10, TypeName = "BIT")]
        public Nullable<bool> CreateTransaction { get; set; }

        [Column("FiscalType", Order = 11, TypeName = "TINYINT")]
        public Nullable<byte> FiscalType { get; set; }

        [Column("IsInvoice", Order = 12, TypeName = "BIT")]
        public Nullable<bool> IsInvoice { get; set; }

        [Column("IsCancel", Order = 13, TypeName = "BIT")]
        public Nullable<bool> IsCancel { get; set; }

        [Column("GroupId", Order = 14, TypeName = "INT")]
        public Nullable<int> GroupId { get; set; }

        [Column("InvoicesTypeId", Order = 15, TypeName = "BIGINT")]
        [ForeignKey("FK_PosInfoDetail_InvoiceTypes")]
        [Association("InvoiceTypes", "InvoicesTypeId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        public Nullable<long> InvoicesTypeId { get; set; }

        [Column("IsPdaHidden", Order = 16, TypeName = "BIT")]
        public Nullable<bool> IsPdaHidden { get; set; }

        [Column("IsDeleted", Order = 17, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("SendsVoidToKitchen", Order = 18, TypeName = "SMALLINT")]
        public Nullable<short> SendsVoidToKitchen { get; set; }

        [Column("PMSInvoiceId", Order = 19, TypeName = "BIGINT")]
        public Nullable<long> PMSInvoiceId { get; set; }

        [Column("Background", Order = 20, TypeName = "NVARCHAR(25)")]
        public string Background { get; set; }

        [Column("Color", Order = 21, TypeName = "NVARCHAR(25)")]
        public string Color { get; set; }
    }
}
