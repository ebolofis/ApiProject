
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
    
public partial class PricelistMaster
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public PricelistMaster()
    {

        this.Pricelist = new HashSet<Pricelist>();

        this.SalesType_PricelistMaster_Assoc = new HashSet<SalesType_PricelistMaster_Assoc>();

    }


    public long Id { get; set; }

    public string Description { get; set; }

    public Nullable<byte> Status { get; set; }

    public Nullable<bool> Active { get; set; }

    public Nullable<long> DAId { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Pricelist> Pricelist { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<SalesType_PricelistMaster_Assoc> SalesType_PricelistMaster_Assoc { get; set; }

}

}