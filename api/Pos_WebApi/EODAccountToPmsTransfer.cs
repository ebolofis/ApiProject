
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
    
public partial class EODAccountToPmsTransfer
{

    public long Id { get; set; }

    public Nullable<long> PosInfoId { get; set; }

    public Nullable<long> AccountId { get; set; }

    public Nullable<long> PmsRoom { get; set; }

    public string Description { get; set; }

    public Nullable<bool> Status { get; set; }

    public string PmsDepartmentId { get; set; }

    public string PmsDepDescription { get; set; }



    public virtual Accounts Accounts { get; set; }

    public virtual PosInfo PosInfo { get; set; }

}

}
