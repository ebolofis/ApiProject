
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
    
public partial class DA_CustomersTokens
{

    public long Id { get; set; }

    public Nullable<long> CustomerId { get; set; }

    public string Token { get; set; }



    public virtual DA_Customers DA_Customers { get; set; }

}

}