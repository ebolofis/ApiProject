
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
    
public partial class EndOfDayPaymentAnalysis_Hist
{

    public int nYear { get; set; }

    public long Id { get; set; }

    public Nullable<long> EndOfDayId { get; set; }

    public Nullable<long> AccountId { get; set; }

    public Nullable<decimal> Total { get; set; }

    public string Description { get; set; }

    public Nullable<short> AccountType { get; set; }

}

}
