
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
    
public partial class KitchenRegion
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public KitchenRegion()
    {

        this.Product = new HashSet<Product>();

    }


    public long Id { get; set; }

    public string ItemRegion { get; set; }

    public Nullable<int> RegionPosition { get; set; }

    public string Abbr { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Product> Product { get; set; }

}

}