
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
    
public partial class Region
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Region()
    {

        this.PosInfo_Region_Assoc = new HashSet<PosInfo_Region_Assoc>();

        this.Table = new HashSet<Table>();

    }


    public long Id { get; set; }

    public string Description { get; set; }

    public Nullable<System.DateTime> StartTime { get; set; }

    public Nullable<System.DateTime> EndTime { get; set; }

    public string BluePrintPath { get; set; }

    public Nullable<int> MaxCapacity { get; set; }

    public Nullable<long> PosInfoId { get; set; }

    public Nullable<bool> IsLocker { get; set; }



    public virtual PosInfo PosInfo { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PosInfo_Region_Assoc> PosInfo_Region_Assoc { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Table> Table { get; set; }

}

}