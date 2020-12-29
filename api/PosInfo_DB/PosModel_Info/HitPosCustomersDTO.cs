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
    [Table("HitPosCustomers")]
    [DisplayName("HitPosCustomers")]
    public class HitPosCustomersDTO
    {
        [Column("CurId", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [Phone]
        [DisplayName("HitPosCustomers_PK")]
        public long CurId { get; set; }

        [Column("customerid", Order = 2, TypeName = "NVARCHAR(15)")]
        [Required]
        public string customerid { get; set; }

        [Column("name", Order = 3, TypeName = "VARCHAR(50)")]
        public string name { get; set; }

        [Column("fname", Order = 4, TypeName = "VARCHAR(50)")]
        public string fname { get; set; }

        [Column("title", Order = 5, TypeName = "NVARCHAR(50)")]
        public string title { get; set; }

        [Column("profession", Order = 6, TypeName = "VARCHAR(50)")]
        public string profession { get; set; }

        [Column("tel1", Order = 7, TypeName = "NVARCHAR(40)")]
        public string tel1 { get; set; }

        [Column("tel2", Order = 8, TypeName = "NVARCHAR(20)")]
        public string tel2 { get; set; }

        [Column("fax", Order = 9, TypeName = "NVARCHAR(20)")]
        public string fax { get; set; }

        [Column("mobile", Order = 10, TypeName = "NVARCHAR(20)")]
        public string mobile { get; set; }

        [Column("address1", Order = 11, TypeName = "VARCHAR(200)")]
        public string address1 { get; set; }

        [Column("address2", Order = 12, TypeName = "CHAR(50)")]
        public string address2 { get; set; }

        [Column("address_no", Order = 13, TypeName = "VARCHAR(30)")]
        public string address_no { get; set; }

        [Column("orofos1", Order = 14, TypeName = "VARCHAR(30)")]
        public string orofos1 { get; set; }

        [Column("orofos2", Order = 15, TypeName = "NVARCHAR(10)")]
        public string orofos2 { get; set; }

        [Column("city", Order = 16, TypeName = "VARCHAR(50)")]
        public string city { get; set; }

        [Column("zipcode", Order = 17, TypeName = "CHAR(10)")]
        public string zipcode { get; set; }

        [Column("doy", Order = 18, TypeName = "VARCHAR(30)")]
        public string doy { get; set; }

        [Column("afm", Order = 19, TypeName = "VARCHAR(30)")]
        public string afm { get; set; }

        [Column("email", Order = 20, TypeName = "NVARCHAR(200)")]
        public string email { get; set; }

        [Column("contact", Order = 21, TypeName = "CHAR(50)")]
        public string contact { get; set; }

        [Column("vip", Order = 22, TypeName = "NCHAR(5)")]
        public string vip { get; set; }

        [Column("member", Order = 23, TypeName = "NVARCHAR(10)")]
        public string member { get; set; }

        [Column("tomeas", Order = 24, TypeName = "NVARCHAR(20)")]
        public string tomeas { get; set; }

        [Column("store", Order = 25, TypeName = "VARCHAR(30)")]
        public string store { get; set; }

        [Column("sector", Order = 26, TypeName = "NVARCHAR(10)")]
        public string sector { get; set; }

        [Column("diet", Order = 27, TypeName = "NVARCHAR(50)")]
        public string diet { get; set; }

        [Column("entolh", Order = 28, TypeName = "NVARCHAR(50)")]
        public string entolh { get; set; }

        [Column("farsa", Order = 29, TypeName = "NVARCHAR(50)")]
        public string farsa { get; set; }

        [Column("remarks", Order = 30, TypeName = "CHAR(200)")]
        public string remarks { get; set; }

        [Column("amount", Order = 31, TypeName = "FLOAT")]
        public Nullable<double> amount { get; set; }

        [Column("expireddate", Order = 32, TypeName = "DATETIME")]
        public Nullable<System.DateTime> expireddate { get; set; }

        [Column("order_comments", Order = 33, TypeName = "NVARCHAR(200)")]
        public string order_comments { get; set; }

        [Column("first_order", Order = 34, TypeName = "DATETIME")]
        public Nullable<System.DateTime> first_order { get; set; }

        [Column("last_order", Order = 35, TypeName = "DATETIME")]
        public Nullable<System.DateTime> last_order { get; set; }

        [Column("no_of_orders", Order = 36, TypeName = "INT")]
        public Nullable<int> no_of_orders { get; set; }

        [Column("tziros", Order = 37, TypeName = "NUMERIC(18,0)")]
        public Nullable<decimal> tziros { get; set; }

        [Column("bonus", Order = 38, TypeName = "INT")]
        public Nullable<int> bonus { get; set; }

        [Column("epitages", Order = 39, TypeName = "INT")]
        public Nullable<int> epitages { get; set; }

        [Column("zerobonus", Order = 40, TypeName = "INT")]
        public Nullable<int> zerobonus { get; set; }

        [Column("domino_false", Order = 41, TypeName = "INT")]
        public Nullable<int> domino_false { get; set; }

        [Column("lates", Order = 42, TypeName = "INT")]
        public Nullable<int> lates { get; set; }

        [Column("credit", Order = 43, TypeName = "MONEY")]
        public Nullable<decimal> credit { get; set; }

        [Column("max_charge", Order = 44, TypeName = "MONEY")]
        public Nullable<decimal> max_charge { get; set; }

        [Column("company_name", Order = 45, TypeName = "VARCHAR(50)")]
        public string company_name { get; set; }

        [Column("bl_address", Order = 46, TypeName = "VARCHAR(200)")]
        public string bl_address { get; set; }

        [Column("bl_address_no", Order = 47, TypeName = "VARCHAR(30)")]
        public string bl_address_no { get; set; }

        [Column("bl_city", Order = 48, TypeName = "VARCHAR(50)")]
        public string bl_city { get; set; }

        [Column("doycode", Order = 49, TypeName = "INT")]
        public Nullable<int> doycode { get; set; }
    }
}
