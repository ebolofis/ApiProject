
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
    
public partial class EndOfDay_Hist
{

    public int nYear { get; set; }

    public long Id { get; set; }

    public Nullable<System.DateTime> FODay { get; set; }

    public Nullable<long> PosInfoId { get; set; }

    public Nullable<int> CloseId { get; set; }

    public Nullable<decimal> Gross { get; set; }

    public Nullable<decimal> Net { get; set; }

    public Nullable<int> TicketsCount { get; set; }

    public Nullable<int> ItemCount { get; set; }

    public Nullable<decimal> TicketAverage { get; set; }

    public Nullable<long> StaffId { get; set; }

    public Nullable<decimal> Discount { get; set; }

    public Nullable<decimal> Barcodes { get; set; }

    public Nullable<System.DateTime> dtDateTime { get; set; }

    public string zlogger { get; set; }

    public Nullable<System.DateTime> eodPMS { get; set; }

}

}