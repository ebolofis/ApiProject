
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
    
public partial class ReportList
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public ReportList()
    {

        this.StatisticsMenus = new HashSet<StatisticsMenus>();

    }


    public long Id { get; set; }

    public string ReportName { get; set; }

    public Nullable<int> ReportType { get; set; }

    public string ReportJson { get; set; }

    public Nullable<bool> AppearsInMenu { get; set; }

    public Nullable<System.DateTime> DateCreated { get; set; }

    public Nullable<System.DateTime> Datemodified { get; set; }

    public string Version { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<StatisticsMenus> StatisticsMenus { get; set; }

}

}
