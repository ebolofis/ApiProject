
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
    
public partial class ProductBarcodes
{

    public long Id { get; set; }

    public string Barcode { get; set; }

    public Nullable<long> ProductId { get; set; }

    public Nullable<byte> Type { get; set; }

    public Nullable<long> DAId { get; set; }



    public virtual Product Product { get; set; }

}

}