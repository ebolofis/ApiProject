namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Store")]
    public partial class Store
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Store()
        {
            HotelInfoes = new HashSet<HotelInfo>();
            Notifications = new HashSet<Notification>();
            StoreMessages = new HashSet<StoreMessage>();
        }

        public long Id { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public string ExtDescription { get; set; }

        [StringLength(150)]
        public string Phone1 { get; set; }

        [StringLength(50)]
        public string Phone2 { get; set; }

        [StringLength(50)]
        public string Phone3 { get; set; }

        [StringLength(250)]
        public string Email { get; set; }

        [StringLength(150)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Latitude { get; set; }

        [StringLength(50)]
        public string Longtitude { get; set; }

        [StringLength(250)]
        public string ImageUri { get; set; }

        [StringLength(250)]
        public string LogoUri { get; set; }

        public byte? Status { get; set; }

        public Guid? StoreMapId { get; set; }

        public short? StoreKitchenInstruction { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelInfo> HotelInfoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreMessage> StoreMessages { get; set; }
    }
}
