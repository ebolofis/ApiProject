namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Guest")]
    public partial class Guest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Guest()
        {
            Invoice_Guests_Trans = new HashSet<Invoice_Guests_Trans>();
            Invoices = new HashSet<Invoice>();
            MealConsumptions = new HashSet<MealConsumption>();
            OrderDetails = new HashSet<OrderDetail>();
            TablePaySuggestions = new HashSet<TablePaySuggestion>();
        }

        public long Id { get; set; }

        public DateTime? arrivalDT { get; set; }

        public DateTime? departureDT { get; set; }

        public DateTime? birthdayDT { get; set; }

        [StringLength(150)]
        public string Room { get; set; }

        public int? RoomId { get; set; }

        [StringLength(50)]
        public string Arrival { get; set; }

        [StringLength(50)]
        public string Departure { get; set; }

        [StringLength(150)]
        public string ReservationCode { get; set; }

        public int? ProfileNo { get; set; }

        [StringLength(150)]
        public string FirstName { get; set; }

        [StringLength(150)]
        public string LastName { get; set; }

        [StringLength(150)]
        public string Member { get; set; }

        [StringLength(150)]
        public string Password { get; set; }

        [StringLength(550)]
        public string Address { get; set; }

        [StringLength(550)]
        public string City { get; set; }

        [StringLength(150)]
        public string PostalCode { get; set; }

        [StringLength(150)]
        public string Country { get; set; }

        [StringLength(150)]
        public string Birthday { get; set; }

        [StringLength(550)]
        public string Email { get; set; }

        [StringLength(150)]
        public string Telephone { get; set; }

        [StringLength(550)]
        public string VIP { get; set; }

        public string Benefits { get; set; }

        [StringLength(150)]
        public string NationalityCode { get; set; }

        [StringLength(150)]
        public string ConfirmationCode { get; set; }

        public int? Type { get; set; }

        [StringLength(150)]
        public string Title { get; set; }

        public int? Adults { get; set; }

        public int? Children { get; set; }

        [StringLength(150)]
        public string BoardCode { get; set; }

        [StringLength(150)]
        public string BoardName { get; set; }

        public string Note1 { get; set; }

        public string Note2 { get; set; }

        public int? ReservationId { get; set; }

        public bool? IsSharer { get; set; }

        public long? HotelId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice_Guests_Trans> Invoice_Guests_Trans { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MealConsumption> MealConsumptions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TablePaySuggestion> TablePaySuggestions { get; set; }
    }
}
