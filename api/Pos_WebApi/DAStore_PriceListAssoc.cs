
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
    
public partial class DAStore_PriceListAssoc
{

    public long Id { get; set; }

    public long PriceListId { get; set; }

    public long DAStoreId { get; set; }

    public int PriceListType { get; set; }



    public virtual DA_Stores DA_Stores { get; set; }

    public virtual Pricelist Pricelist { get; set; }

}

}