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
    [Table("HotelInfo")]
    [DisplayName("HotelInfo")]
    public class HotelInfoDTO
    {
        [Column("Id", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [DisplayName("PK__HotelInf__3214EC076C6E1476")]
        public long Id { get; set; }

        [Column("StoreId", Order = 2, TypeName = "BIGINT")]
        [ForeignKey("FK__HotelInfo__Store__6E565CE8")]
        [Association("Store", "StoreId", "Id")]/*Foreign Table, Table Field, Foreign Field*/
        [MaxLength(1)]//ON UPDATE CASCADE
        [MinLength(1)]//ON DELETE CASCADE
        public Nullable<long> StoreId { get; set; }

        [Column("HotelId", Order = 3, TypeName = "INT")]
        public Nullable<int> HotelId { get; set; }

        [Column("HotelUri", Order = 4, TypeName = "VARCHAR(500)")]
        public string HotelUri { get; set; }

        [Column("Type", Order = 5, TypeName = "TINYINT")]
        public Nullable<byte> Type { get; set; }

        [Column("RedirectToCustomerCard", Order = 6, TypeName = "NVARCHAR(1000)")]
        public string RedirectToCustomerCard { get; set; }

        [Column("PmsType", Order = 7, TypeName = "SMALLINT")]
        public Nullable<short> PmsType { get; set; }

        [Column("DBUserName", Order = 8, TypeName = "NVARCHAR(100)")]
        public string DBUserName { get; set; }

        [Column("DBPassword", Order = 9, TypeName = "NVARCHAR(1000)")]
        public string DBPassword { get; set; }

        [Column("HotelName", Order = 10, TypeName = "NVARCHAR(500)")]
        public string HotelName { get; set; }

        [Column("ServerName", Order = 11, TypeName = "NVARCHAR(500)")]
        public string ServerName { get; set; }

        [Column("MPEHotel", Order = 12, TypeName = "SMALLINT")]
        public Nullable<short> MPEHotel { get; set; }

        [Column("DBName", Order = 13, TypeName = "VARCHAR(50)")]
        public string DBName { get; set; }

        [Column("allHotels", Order = 14, TypeName = "SMALLINT")]
        public Nullable<short> allHotels { get; set; }

        [Column("HotelType", Order = 15, TypeName = "VARCHAR(20)")]
        public string HotelType { get; set; }
    }
}
