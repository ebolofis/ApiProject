
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
    
public partial class PdaModuleDetail
{

    public long Id { get; set; }

    public Nullable<long> PdaModuleId { get; set; }

    public Nullable<System.DateTime> LastLoginTS { get; set; }

    public Nullable<System.DateTime> LastLogoutTS { get; set; }

    public Nullable<long> StaffId { get; set; }



    public virtual PdaModule PdaModule { get; set; }

}

}