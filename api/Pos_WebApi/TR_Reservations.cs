
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Pos_WebApi
{

using System;
    using System.Collections.Generic;
    
public partial class TR_Reservations
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public TR_Reservations()
    {

        this.TR_ReservationCustomers = new HashSet<TR_ReservationCustomers>();

    }


    public long Id { get; set; }

    public long RestId { get; set; }

    public long CapacityId { get; set; }

    public int Couver { get; set; }

    public System.DateTime ReservationDate { get; set; }

    public System.TimeSpan ReservationTime { get; set; }

    public System.DateTime CreateDate { get; set; }

    public int Status { get; set; }

    public string Description { get; set; }



    public virtual TR_Capacities TR_Capacities { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<TR_ReservationCustomers> TR_ReservationCustomers { get; set; }

}

}
