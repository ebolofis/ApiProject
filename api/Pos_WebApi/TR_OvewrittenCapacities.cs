
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
    
public partial class TR_OvewrittenCapacities
{

    public long Id { get; set; }

    public long RestId { get; set; }

    public long CapacityId { get; set; }

    public int Capacity { get; set; }

    public System.DateTime Date { get; set; }



    public virtual TR_Capacities TR_Capacities { get; set; }

    public virtual TR_Restaurants TR_Restaurants { get; set; }

}

}