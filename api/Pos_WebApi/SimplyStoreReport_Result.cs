
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
    
public partial class SimplyStoreReport_Result
{

    public int ID { get; set; }

    public string WaiterCode { get; set; }

    public string WaiterName { get; set; }

    public string PosDescription { get; set; }

    public Nullable<int> FO_Month { get; set; }

    public Nullable<int> FO_Year { get; set; }

    public Nullable<int> Orders { get; set; }

    public Nullable<decimal> Total { get; set; }

    public Nullable<decimal> DineIn { get; set; }

    public Nullable<decimal> Delivery { get; set; }

    public Nullable<decimal> TakeOut { get; set; }

    public Nullable<int> Cover { get; set; }

}

}