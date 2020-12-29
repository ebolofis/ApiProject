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
    [Table("Staff")]
    [DisplayName("Staff")]
    public  class StaffDTO : ITables
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK_Personnel")]
        public long Id { get; set; }

        [Column("Code", Order = 2, TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("FirstName", Order = 3, TypeName = "NVARCHAR(50)")]
        public string FirstName { get; set; }

        [Column("LastName", Order = 4, TypeName = "NVARCHAR(50)")]
        public string LastName { get; set; }

        [Column("ImageUri", Order = 5, TypeName = "NVARCHAR(500)")]
        public string ImageUri { get; set; }

        [Column("Password", Order = 6, TypeName = "NVARCHAR(50)")]
        public string Password { get; set; }

        [Column("IsDeleted", Order = 7, TypeName = "BIT")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("Identification", Order = 8, TypeName = "NVARCHAR(250)")]
        [DisplayFormatAttribute(DataFormatString = "DF_Staff_Identification", NullDisplayText = "''")]
        public string Identification { get; set; }

        [Column("DASTORE", Order = 9, TypeName = "BIGINT")]
        public Nullable<long> DASTORE { get; set; }

        [Column("IsOnRoad", Order = 10, TypeName = "BIT")]
        public Nullable<bool> CurrentOrderStatus { get; set; }

        [Column("StatusTimeChanged", Order = 11, TypeName = "DATETIME")]
        public Nullable<DateTime> StatusTimeChange { get; set; }

        [Column("isAdmin", Order = 12, TypeName = "BIT")]
        public bool isAdmin { get; set; }

        [Column("LogInAfterOrder", Order = 13, TypeName = "BIT")]
        public Nullable<bool> LogInAfterOrder { get; set; }

        [Column("AllowReporting", Order = 14, TypeName = "BIT")]
        public Nullable<bool> AllowReporting { get; set; }

        [Column("isAssignedToRoute", Order = 15, TypeName = "BIT")]
        public Nullable<bool> isAssignedToRoute { get; set; }
    }
}
