namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HotelInfo")]
    public partial class HotelInfo
    {
        public long Id { get; set; }

        public long? StoreId { get; set; }

        public int? HotelId { get; set; }

        [StringLength(500)]
        public string HotelUri { get; set; }

        public byte? Type { get; set; }

        [StringLength(1000)]
        public string RedirectToCustomerCard { get; set; }

        public short? PmsType { get; set; }

        [StringLength(100)]
        public string DBUserName { get; set; }

        [StringLength(1000)]
        public string DBPassword { get; set; }

        [StringLength(500)]
        public string HotelName { get; set; }

        [StringLength(500)]
        public string ServerName { get; set; }

        public short? MPEHotel { get; set; }

        public virtual Store Store { get; set; }
    }
}
