
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
    
public partial class TR_ReservationCustomers
{

    public long Id { get; set; }

    public int ProtelId { get; set; }

    public byte[] ProtelName { get; set; }

    public byte[] ReservationName { get; set; }

    public string RoomNum { get; set; }

    public byte[] Email { get; set; }

    public long ReservationId { get; set; }

    public Nullable<long> HotelId { get; set; }



    public virtual HotelInfo HotelInfo { get; set; }

    public virtual TR_Reservations TR_Reservations { get; set; }

}

}