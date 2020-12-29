namespace hit.webpos.entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AuthorizedGroup")]
    public partial class AuthorizedGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AuthorizedGroup()
        {
            AuthorizedGroupDetails = new HashSet<AuthorizedGroupDetail>();
            StaffAuthorizations = new HashSet<StaffAuthorization>();
        }

        public long Id { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        [StringLength(500)]
        public string ExtendedDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AuthorizedGroupDetail> AuthorizedGroupDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffAuthorization> StaffAuthorizations { get; set; }
    }
}
